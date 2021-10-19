#if UNITY_4 || UNITY_5_1
#pragma warning disable 0618
#endif

#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

public static class BitmapFontGenerater {

	private enum TextAssetType {
		Text,
		XML,
		Bin,
	}
	static string DEFAULT_SHADER = "Unlit/Transparent";

	[MenuItem("Assets/Generate Bitmap Font/From Text")]
	public static void GenerateBitmapFontText() {
		TextAsset[] textAssets = Selection.GetFiltered<TextAsset>(SelectionMode.DeepAssets);
		Texture2D[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);

		if (textAssets.Length < 1) {
			Debug.LogError("BitmapFont Create Error -- Fnt(Text) File is not Selected.");
			return;
		}
		if (textures.Length < 1) {
			Debug.LogError("BitmapFont Create Error -- Texture File is not selected.");
			return;
		}

		Generate(textAssets[0], textures[0], TextAssetType.Text);
	}

	[MenuItem("Assets/Generate Bitmap Font/From XML")]
	public static void GenerateBitmapFontXML() {
		TextAsset[] textAssets = Selection.GetFiltered<TextAsset>(SelectionMode.DeepAssets);
		Texture2D[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);

		if (textAssets.Length < 1) {
			Debug.LogError("BitmapFont Create Error -- Fnt(XML) File is not Selected.");
			return;
		}
		if (textures.Length < 1) {
			Debug.LogError("BitmapFont Create Error -- Texture File is not selected.");
			return;
		}

		Generate(textAssets[0], textures[0], TextAssetType.XML);
	}

	private static CharacterInfo _GenerateCharacterInfo(float textureW, float textureH, int id, float x, float y, float width, float height, float xoffset, float yoffset, int xadvance) {
		var info = new CharacterInfo();

		info.index = id;

		Rect vertRect = new Rect();
		vertRect.width = width;
		vertRect.height = -height;
		vertRect.x = xoffset;
		vertRect.y = -yoffset;

#if UNITY_4 || UNITY_5_1
		Rect uvRect = new Rect();
		uvRect.x = x / textureW;
		uvRect.width = width / textureW;
		uvRect.height = height;
		uvRect.y = (textureH - y - uvRect.height) / textureH;
		uvRect.height = uvRect.height / textureH;

		info.width = xadvance;
		info.flipped = false;
		info.uv = uvRect;
		info.vert = vertRect;
#else
		float charX = x / textureW;
		float charWidth = width / textureW;
		float charHeight = height;
		float charY = (textureH - y - charHeight) / textureH;
		charHeight = charHeight / textureH;

		// UnFlipped.
		info.uvBottomLeft = new Vector2(charX, charY);
		info.uvBottomRight = new Vector2(charX + charWidth, charY);
		info.uvTopLeft = new Vector2(charX, charY + charHeight);
		info.uvTopRight = new Vector2(charX + charWidth, charY + charHeight);

		info.minX = (int) vertRect.xMin;
		info.maxX = (int) vertRect.xMax;
		info.minY = (int) vertRect.yMax;
		info.maxY = (int) vertRect.yMin;

		info.advance = xadvance;
#endif

		return info;
	}

	private static int _JumpSpace(string text, int startPos) {
		var pos = startPos;
		var p = -1;
		do {
			p = text.IndexOfAny(new char[] { ' ', '\t' }, pos, 1);
			if (p != -1) {
				pos = p + 1;
			}
		} while (p != -1);
		return pos;
	}

	private static bool _TryGetValueFromText(string text, string key, out float value) {
		var pos = text.IndexOf(key);
		value = 0;
		if (pos == -1) { return false; }
		pos += key.Length;
		pos = _JumpSpace(text, pos);
		if (pos >= text.Length) { return false; }
		if (text[pos] != '=') { return false; }
		pos = _JumpSpace(text, pos + 1);
		var pe = text.IndexOfAny(new char[]{' ', '\t', '\n', '\r'}, pos);
		string valueStr = "";
		if (pe != -1) {
			valueStr = text.Substring(pos, pe - pos).Trim();
		} else {
			valueStr = text.Substring(pos).Trim();
		}
		return float.TryParse(valueStr, out value);
	}

	private static List<CharacterInfo> _GenerateDataListByText(TextAsset textAsset, out float size) {

		var charInfos = new List<CharacterInfo>();
		size = 0;

		var lines = textAsset.text.Split('\n');

		for (int i = 0; i < lines.Length; ++i) {
			var l = lines[i].Trim();
			if (l == "") { continue; }
			if (l.IndexOf("info") != 0) { continue; }
			_TryGetValueFromText(l, "size", out size);
			break;
		}

		float scaleW = 0;
		float scaleH = 0;

		for (int i = 0; i < lines.Length; ++i) {
			var l = lines[i].Trim();
			if (l == "") { continue; }
			if (l.IndexOf("common") != 0) { continue; }
			_TryGetValueFromText(l, "scaleW", out scaleW);
			_TryGetValueFromText(l, "scaleH", out scaleH);
			break;
		}

		for (int i = 0; i < lines.Length; ++i) {
			var l = lines[i].Trim();
			if (l == "") { continue; }
			if (l.IndexOf("char") != 0) { continue; }
			bool bRet = true;
			bRet &= _TryGetValueFromText(l, "id", out var id);
			bRet &= _TryGetValueFromText(l, "x", out var x);
			bRet &= _TryGetValueFromText(l, "y", out var y);
			bRet &= _TryGetValueFromText(l, "width", out var width);
			bRet &= _TryGetValueFromText(l, "height", out var height);
			bRet &= _TryGetValueFromText(l, "xoffset", out var xoffset);
			bRet &= _TryGetValueFromText(l, "yoffset", out var yoffset);
			bRet &= _TryGetValueFromText(l, "xadvance", out var xadvance);
			if (bRet) {
				var retInfo = _GenerateCharacterInfo(scaleW, scaleH, (int) id, x, y, width, height, xoffset, yoffset, (int) xadvance);
				charInfos.Add(retInfo);
			}
		}

		return charInfos;
	}

	private static List<CharacterInfo> _GenerateDataListByXML(TextAsset textAsset, out float size) {
		XmlDocument xml = new XmlDocument();
		xml.LoadXml(textAsset.text);

		XmlNode common = xml.GetElementsByTagName("common") [0];
		XmlNodeList chars = xml.GetElementsByTagName("chars") [0].ChildNodes;

		XmlNode info = xml.GetElementsByTagName("info") [0];
		size = float.Parse(GetValue(info, "size"));

		var charInfos = new List<CharacterInfo>();

		float textureW = float.Parse(GetValue(common, "scaleW"));
		float textureH = float.Parse(GetValue(common, "scaleH"));

		for (int i = 0; i < chars.Count; i++) {
			XmlNode charNode = chars[i];
			if (charNode.Attributes != null) {
				var index = int.Parse(GetValue(charNode, "id"));
				var x = float.Parse(GetValue(charNode, "x"));
				var y = float.Parse(GetValue(charNode, "y"));
				var width = float.Parse(GetValue(charNode, "width"));
				var height = float.Parse(GetValue(charNode, "height"));
				var xoffset = float.Parse(GetValue(charNode, "xoffset"));
				var yoffset = float.Parse(GetValue(charNode, "yoffset"));
				var xadvance = int.Parse(GetValue(charNode, "xadvance"));
				var retInfo = _GenerateCharacterInfo(textureW, textureH, index, x, y, width, height, xoffset, yoffset, xadvance);
				charInfos.Add(retInfo);
			}
		}
		return charInfos;
	}

	static void Generate(TextAsset textAsset, Texture2D texture, TextAssetType taType) {

		List<CharacterInfo> charInfos = null;
		float size;

		switch (taType) {
		case TextAssetType.Text:
			charInfos = _GenerateDataListByText(textAsset, out size);
			break;
		case TextAssetType.XML:
			charInfos = _GenerateDataListByXML(textAsset, out size);
			break;
		default:
			return;
		}

		if (charInfos.Count == 0) {
			Debug.LogError("BitmapFont Create Error -- Fnt File is parsing failed.");
			return;
		}

		string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(textAsset));
		string exportPath = rootPath + "/" + Path.GetFileNameWithoutExtension(textAsset.name);

		Material material = GenerateMaterial(exportPath, texture);
		Font font = GenerateFont(exportPath, textAsset.name, material);

		font.characterInfo = charInfos.ToArray();

		// Save m_LineSpacing.
		SerializedObject serializedFont = new SerializedObject(font);
		SerializedProperty serializedLineSpacing = serializedFont.FindProperty("m_LineSpacing");
		serializedLineSpacing.floatValue = Mathf.Abs(size);
		serializedFont.ApplyModifiedProperties();
	}

	static Material GenerateMaterial(string materialPath, Texture2D texture) {
		Shader shader = Shader.Find(DEFAULT_SHADER);
		Material material = LoadAsset<Material>(materialPath + ".mat", new Material(shader));
		material.shader = shader;
		material.mainTexture = texture;

		SaveAsset(material, materialPath + ".mat");

		return material;
	}

	static Font GenerateFont(string fontPath, string fontName, Material material) {
		Font font = LoadAsset<Font>(fontPath + ".fontsettings", new Font(fontName));
		font.material = material;

		SaveAsset(font, fontPath + ".fontsettings");

		return font;
	}

	static string GetValue(XmlNode node, string name) {
		return node.Attributes.GetNamedItem(name).InnerText;
	}

	static void SaveAsset(Object obj, string path) {
		Object existingAsset = AssetDatabase.LoadMainAssetAtPath(path);
		if (existingAsset != null) {
			EditorUtility.CopySerialized(obj, existingAsset);
			AssetDatabase.SaveAssets();
		} else {
			AssetDatabase.CreateAsset(obj, path);
		}
	}

	static T LoadAsset<T>(string path, T defaultAsset) where T : Object {
		T existingAsset = AssetDatabase.LoadMainAssetAtPath(path) as T;
		if (existingAsset == null) {
			existingAsset = defaultAsset;
		}
		return existingAsset;
	}
}
#endif

#if UNITY_4 || UNITY_5_1
#pragma warning restore 0618
#endif