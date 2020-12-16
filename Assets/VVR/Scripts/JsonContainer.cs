using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VVR.API
{
    public class JsonContainer
    {      
    }

    [System.Serializable]
    public class APIBaseResult
    {
        public string status;
        public string message;
        public int code;
    }

    [System.Serializable]
    public class AssetBundleContainer
    {
        public string asset;
        public string manifest;
    }

    // IMMERSIVE CATEGORY
    [System.Serializable]
    public class ImmersiveCategoryAPIResult : APIBaseResult
    {
        public ImmersiveCategoryResult result;
    }

    [System.Serializable]
    public class ImmersiveCategoryResult
    {
        public ImmersiveCategoryResultData[] data;
    }

    [System.Serializable]
    public class ImmersiveCategoryResultData
    {
        public int no;
        public string id;
        public string name;
        public string image;
        public string description;
        public string link;
    }
    // IMMERSIVE CATEGORY

    // IMMERSIVE ITEMS
    [System.Serializable]
    public class ImmersiveItemsAPIResult : APIBaseResult
    {
        public ImmersiveItemsResult result;
    }

    [System.Serializable]
    public class ImmersiveItemsResult
    {
        public ImmersiveItemsResultData[] data;
    }

    [System.Serializable]
    public class ImmersiveItemsResultData
    {
        public string id;
        public string name;
        public string image;
        public string marker;
        public string description;
        //public string link;
        //public string universal_link;
        public string video;
        public string price;
        //public string items_type;
        public bool requiredMarker
        {
            get
            {
                return !string.IsNullOrWhiteSpace(marker);
            }
        }

        public ImmersiveItemsResultSponsor sponsor;
    }

    [System.Serializable]
    public class ImmersiveItemsResultSponsor
    {
        public string sponsor_1_link;
        public string sponsor_1_image;
        public string sponsor_2_link;
        public string sponsor_2_image;
    }
    // IMMERSIVE ITEMS

    // IMMERSIVE BANNER
    [System.Serializable]
    public class ImmersiveBannerAPIResult : APIBaseResult
    {
        public ImmersiveBannerResult result;
    }

    [System.Serializable]
    public class ImmersiveBannerResult
    {
        public ImmersiveBannerResultData[] data;
    }

    [System.Serializable]
    public class ImmersiveBannerResultData
    {
        public string id;
        public string image;
        public string link;
        public int no;
    }
    // IMMERSIVE BANNER
}
