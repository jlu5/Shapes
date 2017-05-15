using UnityEditor;
using UnityEngine;

public class CreateAssetBundles : MonoBehaviour
{
    [MenuItem("Assets/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/LevelAssetBundles", BuildAssetBundleOptions.None,
                                        BuildTarget.StandaloneWindows);
    }
}
