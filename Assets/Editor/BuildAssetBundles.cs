using UnityEditor;
using UnityEngine;

public class CreateAssetBundles : MonoBehaviour
{
    [MenuItem("Assets/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        // Clear the cache, and then rebuild the asset bundles.
        Caching.ClearCache();
        BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/LevelAssetBundles", BuildAssetBundleOptions.None,
                                        BuildTarget.StandaloneWindows);  // XXX: hardcoding for specific OS
    }
}
