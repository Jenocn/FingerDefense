using System.Collections.Generic;
using DG.Tweening;
using Game.Managers;
using Game.Systems;
using GCL.Base;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.SceneManagement;

class GameApplication : MonoBehaviour {
	public static bool isFirstOpen { get; private set; } = true;

	/// <summary>
	/// 加载index位置的存档
	/// </summary>
	public static void LoadArchive(uint index) {
		var archive = ArchiveSystem.GetArchive(index);
		ArchiveSystem.SetCurrent(archive);
		archive.Load();
		ManagerCenter.OnArchiveLoaded(archive);
	}

	/// <summary>
	/// 保存当前存档
	/// </summary>
	public static void SaveArchive() {
		SaveCommonArchive();

		if (ArchiveSystem.current) {
			ManagerCenter.OnArchiveSaveBegin(ArchiveSystem.current);
			ArchiveSystem.current.Save();
		}
	}

	/// <summary>
	/// 将当前存档保存到index位置
	/// </summary>
	public static void SaveArchive(uint index) {
		SaveCommonArchive();

		if (ArchiveSystem.current) {
			ManagerCenter.OnArchiveSaveBegin(ArchiveSystem.current);
			ArchiveSystem.GetArchive(index).WriteData(ArchiveSystem.current.GetData());
		}
	}

	/// <summary>
	/// 保存公共配置
	/// </summary>
	public static void SaveCommonArchive() {
		ManagerCenter.OnCommonArchiveSaveBegin();
		ArchiveSystem.common.Save();
	}

	/// <summary>
	/// 删除index位置的存档
	/// </summary>
	public static void DeleteArchive(uint index) {
		ArchiveSystem.GetArchive(index).Delete();
	}

	/// <summary>
	/// 退出游戏
	/// </summary>
	public static void ExitGame() {
		Application.Quit(0);
#if UNITY_EDITOR
		UnityEditor.EditorApplication.ExitPlaymode();
#endif
	}

	//===================================================================================================
	private void Awake() {
		// user folder
		if (Application.isEditor) {
			if (!System.IO.Directory.Exists("user")) {
				System.IO.Directory.CreateDirectory("user");
			}
		}

		// DOTween init
		DOTween.Init();
		DOTween.SetTweensCapacity(1000, 1000);

		// LogSystem init
		LogSystem.Init();
		if (Application.isEditor) {
			// 日志文件
			var logFile = PathTool.Join("user", "log.txt");
			System.IO.File.WriteAllText(logFile, "");
			LogSystem.RegisterDebugLog(this, (string condition, string stackTrace) => {
				var text = "--------------------------------------------\n";
				text = text + condition + "\n";
				var rets = stackTrace.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
				for (var i = 0; i < rets.Length; ++i) {
					if (i >= 5) {
						break;
					}
					text = text + rets[i] + "\n";
				}
				System.IO.File.AppendAllText(logFile, text);
			});
		}

		// ArchiveSystem init
		if (Application.isEditor) {
			ArchiveSystem.Init("user/savedata", ".json");
			ArchiveSystem.SetEncodeEnabled(false, false);
		} else {
			ArchiveSystem.Init(PathTool.Join(Application.persistentDataPath, "savedata"), ".dat");
			ArchiveSystem.SetEncodeEnabled(true, true);
		}
		ArchiveSystem.SetArchiveDefaultNameOfIndex("archive");
		ArchiveSystem.SetCommonName("common");
		ArchiveSystem.common.Load();

		// isFirstOpen
		isFirstOpen = ArchiveSystem.common.GetBool("GameApplication", "isFirstOpen", true);
		ArchiveSystem.common.SetBool("GameApplication", "isFirstOpen", false);

		// AssetSystem init
		if (Application.isEditor) {
			AssetSystem.AddEditorSearchDir("Assets/Res");
			// AssetSystem.SetEditorUseAssetBundle(true);
		}
		AssetSystem.SetRootPath(PathTool.Join(Application.streamingAssetsPath, "assets"));
		AssetSystem.SetExtname(".dat");

		var assetsLoadList = new LinkedList<string>();
		if (!Application.isEditor || AssetSystem.IsEditorUseAssetBundle) {
			// assetsLoadList.AddLast("common");
			// assetsLoadList.AddLast("tables");
			assetsLoadList.AddLast("drive");
			assetsLoadList.AddLast("fonts");
			assetsLoadList.AddLast("strings");
			assetsLoadList.AddLast("textures");
			assetsLoadList.AddLast("tiles");
			assetsLoadList.AddLast("materials");
			assetsLoadList.AddLast("scripts");
			assetsLoadList.AddLast("audio");
			assetsLoadList.AddLast("prefabs");
			assetsLoadList.AddLast("scenes");

			foreach (var item in assetsLoadList) {
				AssetSystem.Preload(item);
			}
		}

		// LocalizationSystem Init
		LocalizationSystem.Init();

		// ManagerCenter init
		ManagerCenter.OnInitManagers();
		ManagerCenter.OnCommonArchiveLoaded();

		SceneManager.sceneLoaded += _OnSceneLoaded;
		SceneManager.sceneUnloaded += _OnSceneUnloaded;

		if (!Application.isEditor || AssetSystem.IsEditorUseAssetBundle) {
			// LoadScene
			SceneManager.LoadScene("HomeScene", LoadSceneMode.Single);
		}
	}

	private void _OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		ManagerCenter.OnSceneLoaded();
	}
	private void _OnSceneUnloaded(Scene scene) {
		ManagerCenter.OnSceneUnloaded();
	}

	private void Start() {
		// ManagerCenter start
		ManagerCenter.OnStartManagers();
	}

	private void Update() {
		InputSystem.Update();
		MessageCenter.OnDispatch();
	}

	private void OnDestroy() {
		// ManagerCenter destroy
		ManagerCenter.OnDestroyManagers();

		SceneManager.sceneLoaded -= _OnSceneLoaded;
		SceneManager.sceneUnloaded -= _OnSceneUnloaded;
	}

	private static bool _bInit = false;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnBeforeSceneLoad() {
		if (!_bInit) {
			var gameObject = new GameObject("GameApplication");
			gameObject.AddComponent<GameApplication>();
			DontDestroyOnLoad(gameObject);
			_bInit = true;
		}
	}
}