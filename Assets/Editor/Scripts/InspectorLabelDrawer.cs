using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InspectorLabelAttribute))]
public class InspectorLabelDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		var att = attribute as InspectorLabelAttribute;
		if (!string.IsNullOrEmpty(att.name)) {
			label.text = att.name;
		}
		EditorGUI.PropertyField(position, property, label);
	}
}