using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VVR.API;
using UnityEngine.UI;
using System;

namespace VVR.ImmersiveContents
{
    public class ImmersiveCategory : MonoBehaviour
    {
        public Transform immersiveItemsContainer;
        public GameObject immersiveItemsPrefab;

        public ImmersiveCategoryResultData categoryData;
        public Text txtTitle;

        public CanvasGroup canvasGroup;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }

        public void Init(ImmersiveCategoryResultData data)
        {
            categoryData = data;
            txtTitle.text = data.name;
            LoadImmersiveContent();
        }

        public void LoadImmersiveContent()
        {
            MainAPI.Instance.SendRequst(MainAPI.RequestType.GET_IMMERSIVE_ITEMS, $"?immersive_id=" + categoryData.id, OnLoadImmersiveItemsSuccess);
        }

        private void OnLoadImmersiveItemsSuccess(string res, string err)
        {
            ImmersiveItemsAPIResult result = JsonUtility.FromJson<ImmersiveItemsAPIResult>(res);
            if (result.status != "success")
            {
                Debug.Log(result.message);
                return;
            }

            if (result.result.data.Length == 0 && ImmersiveManager.Instance.hideCategoryIfNoItems)
            {
                gameObject.SetActive(false);
                return;
            }

            canvasGroup.alpha = 1;
            foreach (var item in result.result.data)
            {
                GameObject obj = Instantiate(immersiveItemsPrefab, immersiveItemsContainer) as GameObject;
                obj.GetComponent<ImmersivItems>().Init(item);
            }
        }
    }
}
