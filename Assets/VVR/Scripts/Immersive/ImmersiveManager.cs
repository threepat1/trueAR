using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using VVR.API;
using VVR.Storage;
using UnityEngine.UI;

namespace VVR.ImmersiveContents
{
    public class ImmersiveManager : MonoBehaviour
    {
        public static ImmersiveManager Instance;

        public Transform immersiveCategoryContainer;
        public GameObject immersiveCategoryPrefab;
        //public ScrollRect immersiveCategoryScrollRect;

        public bool hideCategoryIfNoItems = false;

        [Header("Volumetric content download")]
        private UnityWebRequest wwwVolumetric;
        public GameObject volumetricLoaderCanvas;
        public Slider volumetricLoaderProgressBar;
        public Text volumetricLoaderProgressText;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            OpenDownloadVolumetricContentProcess(false);
            StorageManager.SetUpVolumetricContentPath();
            LoadImmersiveCategory();
        }

        void LoadImmersiveCategory()
        {
            MainAPI.Instance.SendRequst(MainAPI.RequestType.GET_IMMERSIVE_CATEGORY, "", OnLoadImmersiveCategorySuccess);
        }

        private void OnLoadImmersiveCategorySuccess(string res, string err)
        {
            ImmersiveCategoryAPIResult result = JsonUtility.FromJson<ImmersiveCategoryAPIResult>(res);
            if (result.status != "success")
            {
                Debug.Log(result.message);
                return;
            }

            foreach (var item in result.result.data)
            {
                GameObject obj = Instantiate(immersiveCategoryPrefab, immersiveCategoryContainer) as GameObject;
                obj.GetComponent<ImmersiveCategory>().Init(item);
            }
        }

        public void DownloadVolumetricContent(ImmersivItems item)
        {
            StartCoroutine(DownloadVolumetricContentProcess(item));
        }

        IEnumerator DownloadVolumetricContentProcess(ImmersivItems item)
        {
            if(StorageManager.HasVolumetricContent(item.itemsData.video))
            {
                OpenDownloadVolumetricContentProcess(false);
                yield break;
            }

            OpenDownloadVolumetricContentProcess(true);
            wwwVolumetric = UnityWebRequest.Get(item.itemsData.video);
            wwwVolumetric.SendWebRequest();

            while (!wwwVolumetric.isDone)
            {
                DisplayDownloadVolumetricContentProcess(wwwVolumetric.downloadProgress);
                yield return null;
            }

            if (wwwVolumetric.isNetworkError || wwwVolumetric.isHttpError || !string.IsNullOrWhiteSpace(wwwVolumetric.error))
            {
                // Download fail
                Debug.Log(wwwVolumetric.error);
            }
            else
            {
                // Download success
                string newPath = Path.Combine(StorageManager.VolumetricContentPath, StorageManager.GetFileName(item.itemsData.video));
                File.WriteAllBytes(newPath, wwwVolumetric.downloadHandler.data);
                item.OnClickItems();
            }

            wwwVolumetric.Dispose();
            OpenDownloadVolumetricContentProcess(false);
        }

        public void AbortDownloadVolumetricContent()
        {
            if(wwwVolumetric != null && !wwwVolumetric.isDone)
            {
                wwwVolumetric.Abort();
                Debug.Log("! Volumetric download has been cancel");
            }
        }

        private void OpenDownloadVolumetricContentProcess(bool val)
        {
            volumetricLoaderProgressBar.value = 0.00f;
            volumetricLoaderProgressText.text = "";
            volumetricLoaderCanvas.gameObject.SetActive(val);
        }

        public void DisplayDownloadVolumetricContentProcess(float progress)
        {
            float percentage = progress * 100;
            string msg = string.Format("Downloading volumetric content {0}%",  percentage.ToString("#.##"));
            float clamp = Mathf.Clamp01(progress / .9f);
            volumetricLoaderProgressBar.value = clamp;
            volumetricLoaderProgressText.text = msg;
        }
    }
}