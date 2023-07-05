using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorBuildAssetBundle {
    [MenuItem("Assets/AssetBundle/Build for MacOS")]
    private static void BuildMacOS() {
        BuildAssets(BuildTarget.StandaloneOSX);
    }

    [MenuItem("Assets/AssetBundle/Build for Windows")]
    private static void BuildWindows() {
        BuildAssets(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Assets/AssetBundle/Build for Linux")]
    private static void BuildLinux() {
        BuildAssets(BuildTarget.StandaloneLinux64);
    }

    [MenuItem("Assets/AssetBundle/Build for IOS")]
    private static void BuildIOS() {
        BuildAssets(BuildTarget.iOS);
    }

    [MenuItem("Assets/AssetBundle/Build for Android")]
    private static void BuildAndroid() {
        BuildAssets(BuildTarget.Android);
    }

    [MenuItem("Assets/AssetBundle/Build for PS4")]
    private static void BuildPS4() {
        BuildAssets(BuildTarget.PS4);
    }

    [MenuItem("Assets/AssetBundle/Build for Switch")]
    private static void BuildSwitch() {
        BuildAssets(BuildTarget.Switch);
    }

    [MenuItem("Assets/AssetBundle/Build for XBoxOne")]
    private static void BuildXBoxOne() {
        BuildAssets(BuildTarget.XboxOne);
    }
	[MenuItem("Assets/AssetBundle/Build for WebGL")]
	private static void BuildWebGL() {
		BuildAssets(BuildTarget.WebGL);
	}

	private static void BuildAssets(BuildTarget target) {
        var dir = "user/build_assets/" + target.ToString();
        if (!Directory.Exists("user")) {
            Directory.CreateDirectory("user");
        }
        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        var ret = BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, target);
        if (ret) {
            var tdir = Application.streamingAssetsPath + "/assets";
            if (!Directory.Exists(tdir)) {
                Directory.CreateDirectory(tdir);
            }
            var bundles = ret.GetAllAssetBundles();
            foreach (var name in bundles) {
                File.Copy(dir + "/" + name, tdir + "/" + name, true);
            }
        }
    }
}