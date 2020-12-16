using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace VVR.API
{
    public class MainAPI : MonoBehaviour
    {
        private static MainAPI _Instance;
        public static MainAPI Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new GameObject().AddComponent<MainAPI>();
                }

                return _Instance;
            }
        }

        public string apiHost = "https://true5gar.webdashboard.online/app/api/v1/";
        public string apiToken
        {
            get
            {
                return playerAPIToken + ";" + playerBundleId + ";" + playerPlatform;
            }
        }

        public static string playerPlatform
        {
            get
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                    return "ios";
                return "android";
            }
        }

        public static string playerAPIToken
        {
            get
            {
                return "9VkluxlNUZ6uGiPohMyQNEmemZ28ndgLhpgubYRjOOctoYTqsiQ3e14cSPkdBE6X";
            }
        }

        public static string playerBundleId
        {
            get
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                    return "com.tpd.true5gar";
                return "com.tpd.true5gar";
            }
        }

        public enum RequestType
        {
            GET_IMMERSIVE_CATEGORY,
            GET_IMMERSIVE_ITEMS,
            GET_IMMERSIVE_BANNER,
        }

        public enum HTTPMethod
        {
            POST, GET
        }

        public void SendRequst(RequestType type, string param, Action<string, string> callback, string body = null)
        {
            switch (type)
            {
                case RequestType.GET_IMMERSIVE_CATEGORY:
                    {
                        StartCoroutine(RequestProccess("GET", apiHost + "/immersive-category" + param, callback, body));
                        break;
                    }
                case RequestType.GET_IMMERSIVE_ITEMS:
                    {
                        StartCoroutine(RequestProccess("GET", apiHost + "/immersive-items" + param, callback, body));
                        break;
                    }
                case RequestType.GET_IMMERSIVE_BANNER:
                    {
                        StartCoroutine(RequestProccess("GET", apiHost + "/test-banner" + param, callback, body));
                        break;
                    }
            }
        }

        public void SendRequst(HTTPMethod method, string url, string param, Action<string, string> callback, string body = null)
        {
            StartCoroutine(RequestProccess(method.ToString(), url + param, callback, body));
        }

        public void SendImageRequest(string url, Action<Sprite, string> callback)
        {
            StartCoroutine(RequestImageProcess(url, callback));
        }

        private IEnumerator RequestProccess(string method, string url, Action<string, string> callback, string body = null)
        {

            UnityWebRequest request = new UnityWebRequest(url, method);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", @"application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiToken);

            if (body != null)
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                callback?.Invoke("", request.error);
            }
            else
            {
                callback?.Invoke(request.downloadHandler.text, null);
            }

            request.Dispose();
        }

        private IEnumerator RequestImageProcess(string url, Action<Sprite, string> callback)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                callback?.Invoke(null, request.error);
            }
            else
            {
                var texture = DownloadHandlerTexture.GetContent(request);
                Sprite webSprite = SpriteFromTexture2D(texture);
                callback?.Invoke(webSprite, null);
            }

            request.Dispose();
        }

        public Sprite SpriteFromTexture2D(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        public Texture2D Texture2DFromSprite(Sprite sprite)
        {
            try
            {
                if (sprite.rect.width != sprite.texture.width)
                {
                    Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                    Color[] colors = newText.GetPixels();
                    Color[] newColors = sprite.texture.GetPixels((int)System.Math.Ceiling(sprite.textureRect.x),
                                                                 (int)System.Math.Ceiling(sprite.textureRect.y),
                                                                 (int)System.Math.Ceiling(sprite.textureRect.width),
                                                                 (int)System.Math.Ceiling(sprite.textureRect.height));
                    //Debug.Log(colors.Length + "_" + newColors.Length);
                    newText.SetPixels(newColors);
                    newText.Apply();
                    return newText;
                }
                else
                    return sprite.texture;
            }
            catch
            {
                return sprite.texture;
            }
        }
    }
}