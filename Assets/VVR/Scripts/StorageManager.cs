using UnityEngine;
using System.IO;
using System;

namespace VVR.Storage
{
    public class StorageManager : MonoBehaviour
    {
        public static string VolumetricContentPath
        {
            get
            {
                return Path.Combine(GetFriendlyFilesPath(), "VVR/Content/Volumetric");
            }
        }

        public static void SetUpVolumetricContentPath()
        {
            if (!Directory.Exists(VolumetricContentPath))
                Directory.CreateDirectory(VolumetricContentPath);
        }

        public static string CachePath
        {
            get
            {
                return Path.Combine(GetFriendlyCachePath(), "VVR/Cache");
            }
        }

        public static void SetupCachePath()
        {
            try
            {
                if (!Caching.GetCacheByPath(CachePath).valid)
                {
                    if (!Directory.Exists(CachePath))
                        Directory.CreateDirectory(CachePath);

                    Cache newCache = Caching.AddCache(CachePath);

                    //Set current cache for writing to the new cache if the cache is valid
                    if (newCache.valid)
                        Caching.currentCacheForWriting = newCache;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        public static bool RemoveCache()
        {
            bool success = Caching.ClearCache();
            if (!success)
            {
                Debug.Log("Unable to clear cache");
            }
            return success;
        }

        public static string GetFriendlyCachePath()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject applicationContext = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaObject path = applicationContext.Call<AndroidJavaObject>("getCacheDir");
        string filesPath = path.Call<string>("getCanonicalPath");
        return filesPath;
#else
            return Application.temporaryCachePath;
#endif
        }

        public static string GetFriendlyFilesPath()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject applicationContext = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaObject path = applicationContext.Call<AndroidJavaObject>("getFilesDir");
        string filesPath = path.Call<string>("getCanonicalPath");
        return filesPath;
#else
            return Application.persistentDataPath;
#endif
        }

        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public static string GetFileExtension(string name)
        {
            return Path.GetExtension(name);
        }

        // โฟลเดอร์ที่เก็บวีดีโอ volumetric
        public static string GetVolumetricFilePath(string path)
        {
            string name = StorageManager.GetFileName(path);
            return Path.Combine(VolumetricContentPath, name);
        }

        // เช็คว่ามีไฟล์วีดีโอ volumetric อยู่ใน device storage มั้ย โดยเช็คจาก url ที่ส่งมาจาก server (ชื่อไฟล์)
        public static bool HasVolumetricContent(string filepath)
        {
            string localPath = StorageManager.GetVolumetricFilePath(filepath);
            bool exists = StorageManager.FileExists(localPath);
            return exists;
        }
    }

}
