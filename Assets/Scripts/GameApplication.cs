using System.Collections.Generic;
using DG.Tweening;
using Game.Managers;
using Game.Systems;
using GCL.Base;
using GCL.Pattern;
using UnityEngine;
using UnityEngine.SceneManagement;

class GameApplication : MonoBehaviour {

	/// <summary>
	/// 加载index位置的存档
	/// </summary>
	public static void LoadArchive(uint index) {
		var archive = ArchiveSystem.GetArchive(index);
		ArchiveSystem.SetCurrent(archive);
		archive.Load();
		ManagerCenter.OnArchiveLoaded();
	}

	/// <summary>
	/// 保存当前存档
	/// </summary>
	public static void SaveArchive() {
		ManagerCenter.OnArchiveSaveBegin();
		ArchiveSystem.common.Save();
		if (ArchiveSystem.current) {
			ArchiveSystem.current.Save();
		}
	}

	/// <summary>
	/// 将当前存档保存到index位置
	/// </summary>
	public static void SaveArchive(uint index) {
		ManagerCenter.OnArchiveSaveBegin();
		ArchiveSystem.common.Save();
		if (ArchiveSystem.current) {
			ArchiveSystem.GetArchive(index).SaveData(ArchiveSystem.current.GetData());
		}
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
		// DOTween init
		DOTween.Init();
		DOTween.SetTweensCapacity(1000, 1000);

		// LogSystem init
		LogSystem.Init();
		LogSystem.RegisterDebugLog(this, (string condition, string stackTrace) => {
			if (Application.isEditor) {
				System.IO.File.AppendAllText(PathTool.Join("user", "log.txt"), condition + "\n" + stackTrace);
/*
				// 非编辑器条件暂不生成文件
			} else {
				System.IO.File.AppendAllText(PathTool.Join(Application.persistentDataPath, "log.txt"), condition + "\n" + stackTrace);
*/
			}
		});

		// ArchiveSystem init
		if (Application.isEditor) {
			ArchiveSystem.Init("user/savedata", ".dat");
		} else {
			ArchiveSystem.Init(PathTool.Join(Application.persistentDataPath, "savedata"), ".dat");
		}
		ArchiveSystem.SetArchiveDefaultNameOfIndex("archive");
		ArchiveSystem.SetCommonName("common");
		ArchiveSystem.common.Load();

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
			assetsLoadList.AddLast("fonts");
			assetsLoadList.AddLast("strings");
			assetsLoadList.AddLast("textures");
			assetsLoadList.AddLast("tiles");
			assetsLoadList.AddLast("materials");
			assetsLoadList.AddLast("scripts");
			assetsLoadList.AddLast("prefabs");
			assetsLoadList.AddLast("scenes");

			foreach (var item in assetsLoadList) {
				AssetSystem.Preload(item);
			}
		}

		// ManagerCenter init
		ManagerCenter.OnInitManagers();

		if (!Application.isEditor || AssetSystem.IsEditorUseAssetBundle) {
			// LoadScene
			SceneManager.LoadScene("HomeScene", LoadSceneMode.Single);
		}
	}

	private void Update() {
		InputSystem.Update();
		MessageCenter.OnDispatch();
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