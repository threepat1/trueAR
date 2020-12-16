using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using VVR.API;
using VVR.Storage;

namespace VVR.UnityAsset
{
    public class AssetBundleManager : MonoBehaviour
    {
        public static AssetBundleManager Instance;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            StorageManager.SetupCachePath();
        }

        public void LoadAssetBundle(AssetBundleContainer assetBundle, Action<string, AssetBundle> callback)
        {
            MainAPI.Instance.SendRequst(MainAPI.HTTPMethod.GET, assetBundle.manifest, "", (res, err) =>
            {
                AssetBundleManifestData manifestData = new AssetBundleManifestData
                {
                    manifestText = res,
                    assetBundleUrl = assetBundle.asset
                };
                StartCoroutine(LoadActualAsset(manifestData, callback));
            });
        }

        private IEnumerator LoadActualAsset(AssetBundleManifestData manifestData, Action<string, AssetBundle> callback)
        {
            uint crc = 0;
            string[] lines;
            lines = manifestData.manifestText.Split('\n');
  
            if (lines[1].Contains("CRC:") && lines[5].Contains("Hash:"))
            {
                uint.TryParse(lines[1].Substring(5, lines[1].Length - 5), out crc);
                Hash128 hash = Hash128.Parse(lines[5].Split(':')[1].Trim());

                UnityWebRequest request;
                request = UnityWebRequestAssetBundle.GetAssetBundle(manifestData.assetBundleUrl, hash, crc);
                yield return request.SendWebRequest();

                if (request.isHttpError || request.isNetworkError)
                {
                    callback?.Invoke(request.error, null);
                }

                // This one is crucial for debugging. Code 200 means downloaded from web and Code 0 means loaded from cache.
                Debug.Log(request.responseCode);

                // Download the actual content bundle
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

                // Execute action with downloaded asset
                callback?.Invoke("none", bundle);

                // unload the bundle
                bundle.Unload(false); 
            }
        }
    }
}

public class AssetBundleManifestData
{
    public string assetBundleUrl;
    public string manifestText;
}
