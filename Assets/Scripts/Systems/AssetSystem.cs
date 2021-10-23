using System.Collections.Generic;
using System.IO;
using GCL.Base;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Systems {

	/// <summary>
	/// 资源管理系统
	/// </summary>
	public static class AssetSystem {
		// 资源池
		private static Dictionary<string, Dictionary<string, List<Object>>> _editorAssetsPool = new Dictionary<string, Dictionary<string, List<Object>>>();
		// 资源包持有
		private static Dictionary<string, AssetBundle> _assetBundlePool = new Dictionary<string, AssetBundle>();
		// 编辑器下资源根目录
		private static LinkedList<string> _editorSearchDir = new LinkedList<string>();
		private static string _rootPath = "";
		private static string _extname = "";
		// 编辑器环境下是否使用AssetBundle
		private static bool _bEditorUseAssetBundle = false;
		public static bool IsEditorUseAssetBundle { get => _bEditorUseAssetBundle; }

		public static void SetRootPath(string rootPath) {
			_rootPath = rootPath;
		}
		public static void SetExtname(string name) {
			_extname = name;
		}

		/// <summary>
		/// 设置编辑器环境下是否使用AssetBundle
		/// </summary>
		public static void SetEditorUseAssetBundle(bool enable) {
			_bEditorUseAssetBundle = enable;
		}
		/// <summary>
		/// 添加编辑器环境不使用AssetBundle的查找资源根目录
		/// </summary>
		public static void AddEditorSearchDir(params string[] dirs) {
			foreach (var item in dirs) {
				_editorSearchDir.AddLast(PathTool.Normalize(item));
			}
		}

		/// <summary>
		/// 查询某个包是否已经加载
		/// </summary>
		public static bool IsLoaded(string bundle) {
#if UNITY_EDITOR
			return _assetBundlePool.ContainsKey(bundle) || _editorAssetsPool.ContainsKey(bundle);
#else
			return _assetBundlePool.ContainsKey(bundle);
#endif
		}

		/// <summary>
		/// 获取已存在的资源
		/// </summary>
		/// <param name="bundle">包名</param>
		/// <param name="name">资源名</param>
		/// <param name="type">类型</param>
		public static Object Get(string bundle, string name, System.Type type) {
#if UNITY_EDITOR
			if (_editorAssetsPool.TryGetValue(bundle, out var tempDict0)) {
				if (tempDict0.TryGetValue(name, out var tempDict1)) {
					foreach (var obj in tempDict1) {
						if (type.IsInstanceOfType(obj)) {
							return obj;
						}
					}
				}
			}
#endif
			if (_assetBundlePool.TryGetValue(bundle, out var assetBundle)) {
				return assetBundle.LoadAsset(name, type);
			}
			return null;
		}

		/// <summary>
		/// 获取已存在的资源
		/// </summary>
		/// <param name="bundle">包名</param>
		/// <param name="name">资源名</param>
		public static T Get<T>(string bundle, string name) where T : Object {
			return Get(bundle, name, typeof(T)) as T;
		}

		/// <summary>
		/// 从内存数据byte[]中加载包
		/// </summary>
		/// <param name="bundle">包名</param>
		public static bool Preload(string bundle, byte[] data) {
			if (IsLoaded(bundle)) {
				return true;
			}
			var assetBundle = AssetBundle.LoadFromMemory(data);
			if (!assetBundle) { return false; }
			_assetBundlePool.Add(bundle, assetBundle);
			return true;
		}

		/// <summary>
		/// 从指定的url加载包
		/// </summary>
		/// <param name="bundle">包名</param>
		public static void PreloadFromWeb(string bundle, string url, System.Action<bool> complete) {
			if (IsLoaded(bundle)) {
				if (complete != null) {
					complete.Invoke(true);
				}
				return;
			}
			var uwr = UnityWebRequest.Get(url);
			var uwrOperation = uwr.SendWebRequest();
			if (uwrOperation == null) { return; }
			uwrOperation.completed += (AsyncOperation operation) => {
				if ((uwrOperation.webRequest.result == UnityWebRequest.Result.Success) && (uwrOperation.webRequest.downloadHandler != null)) {
					bool bRet = Preload(bundle, uwrOperation.webRequest.downloadHandler.data);
					complete.Invoke(bRet);
				} else {
					complete.Invoke(false);
				}
			};
		}

		/// <summary>
		/// 预加载整个包/文件夹的所有资源
		/// </summary>
		/// <param name="bundle">包名</param>
		public static bool Preload(string bundle) {
			if (IsLoaded(bundle)) {
				return true;
			}

#if UNITY_EDITOR
			if (!_bEditorUseAssetBundle) {
				bool bRet = false;
				foreach (var path in _editorSearchDir) {
					var bundlePath = PathTool.Join(path, bundle);
					if (Directory.Exists(bundlePath)) {
						var items = _GetAllValidFilenameFromPath(bundlePath);
						foreach (var filename in items) {
							var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filename);
							if (obj) {
								_AddToEditorPool(bundle, obj.name, obj);
								bRet = true;
							}
						}
					}
				}
				return bRet;
			}
#endif
			var assetBundle = AssetBundle.LoadFromFile(PathTool.Join(_rootPath, bundle) + _extname);
			if (!assetBundle) { return false; }
			_assetBundlePool.Add(bundle, assetBundle);
			return true;
		}

		/// <summary>
		/// 异步预加载整个包/文件夹的所有资源
		/// </summary>
		/// <param name="bundle">包名</param>
		/// <param name="complete">加载成功回调</param>
		public static void PreloadAsync(string bundle, System.Action<bool> complete) {
			if (IsLoaded(bundle)) {
				if (complete != null) {
					complete.Invoke(true);
				}
				return;
			}

#if UNITY_EDITOR
			if (!_bEditorUseAssetBundle) {
				bool bRet = Preload(bundle);
				if (complete != null) {
					complete.Invoke(bRet);
				}
				return;
			}
#endif
			var request = AssetBundle.LoadFromFileAsync(PathTool.Join(_rootPath, bundle) + _extname);
			if (request == null) {
				if (complete != null) {
					complete.Invoke(false);
				}
				return;
			}

			request.completed += (AsyncOperation operation) => {
				if (request.assetBundle) {
					_assetBundlePool.Add(bundle, request.assetBundle);
				}
				if (complete != null) {
					complete.Invoke(request.assetBundle != null);
				}
			};
		}

		/// <summary>
		/// 加载并获取某个资源
		/// </summary>
		/// <param name="bundle">包名</param>
		/// <param name="name">资源名</param>
		/// <param name="type">类型</param>
		public static Object Load(string bundle, string name, System.Type type) {
			if (Preload(bundle)) {
				return Get(bundle, name, type);
			}
			return null;
		}

		/// <summary>
		/// 加载并获取某个资源
		/// </summary>
		/// <param name="bundle">包名</param>
		/// <param name="name">资源名</param>
		public static T Load<T>(string bundle, string name) where T : Object {
			return Load(bundle, name, typeof(T)) as T;
		}

		/// <summary>
		/// 卸载某个资源包
		/// </summary>
		/// <param name="bundle">包名</param>
		public static void Unload(string bundle) {
#if UNITY_EDITOR
			_editorAssetsPool.Remove(bundle);
#endif

			if (_assetBundlePool.TryGetValue(bundle, out var assetBundle)) {
				assetBundle.Unload(true);
				_assetBundlePool.Remove(bundle);
			}
		}

		/// <summary>
		/// 卸载所有资源
		/// </summary>
		public static void UnloadAll() {
#if UNITY_EDITOR
			_editorAssetsPool.Clear();
#endif
			foreach (var pair in _assetBundlePool) {
				pair.Value.Unload(true);
			}
			_assetBundlePool.Clear();
		}

		private static readonly HashSet<string> AssetIgnoreExtname = new HashSet<string> {
			".meta",
			".Meta",
			".DS_Store",
			".vs",
			".vscode"
		};

		private static LinkedList<string> _GetAllValidFilenameFromPath(string path) {
			var retList = new LinkedList<string>();
			if (!Directory.Exists(path)) {
				return retList;
			}
			var directoryInfo = new DirectoryInfo(path);
			var files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
			foreach (var item in files) {
				if (AssetIgnoreExtname.Contains(item.Extension)) {
					continue;
				}
				var fullname = PathTool.Normalize(item.FullName);
				var pos = fullname.LastIndexOf(path, System.StringComparison.Ordinal);
				if (pos != -1) {
					var filename = fullname.Substring(pos, fullname.Length - pos);
					retList.AddLast(filename);
				}
			}
			return retList;
		}

		private static void _AddToEditorPool(string bundle, string name, Object asset) {
			if (!_editorAssetsPool.TryGetValue(bundle, out var tempDict0)) {
				tempDict0 = new Dictionary<string, List<Object>>();
				_editorAssetsPool.Add(bundle, tempDict0);
			}
			if (!tempDict0.TryGetValue(name, out var tempList)) {
				tempList = new List<Object>();
				tempDict0.Add(name, tempList);
			}
			tempList.Add(asset);
		}
	}
}