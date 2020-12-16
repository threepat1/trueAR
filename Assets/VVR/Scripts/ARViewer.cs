using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using VVR.API;
using VVR.Storage;

namespace VVR.AR
{
    public class ARViewer : MonoBehaviour
    {
        public static ARViewer Instance;

        public ARTrackedImageManager mARTrackedImageManager;
        private MutableRuntimeReferenceImageLibrary myRuntimeReferenceImageLibrary;

        public ARSessionOrigin sessionOrigin;
        public ARSession session;

        public GameObject volumetricPrefab;
        public GameObject volumetricWithoutMarkerPrefab;

        public GameObject volumetricWithoutMarkerObject;
        public Vector2 volumetricWithoutMarkerIniPosDistance = new Vector2(1, -0.5f);

        public Text debugText;
        public Image debugImage;

        private bool isARInitialize = false;

        public List<Button> m_sponsorButton; 

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            DebugText(null);
            mARTrackedImageManager = sessionOrigin.gameObject.AddComponent<ARTrackedImageManager>();
        }

        private void OnEnable()
        {
            ARSession.stateChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            ARSession.stateChanged -= OnStateChanged;
        }

        private void OnStateChanged(ARSessionStateChangedEventArgs args)
        {
            switch (args.state)
            {
                case ARSessionState.None:
                    break;
                case ARSessionState.CheckingAvailability:
                    break;
                case ARSessionState.Installing:
                    break;
                case ARSessionState.NeedsInstall:
                    break;
                case ARSessionState.Ready:
                    break;
                case ARSessionState.SessionInitializing:
                    isARInitialize = true;
                    break;
                case ARSessionState.SessionTracking:
                    break;
                case ARSessionState.Unsupported:
                    break;
            }
        }

        public void AppendNewVolumetricContent(ImmersiveItemsResultData contentData)
        {
            DebugText(null);
            MainAPI.Instance.SendImageRequest(contentData.marker, (mkrSprite, mkrError) =>
            {
                Texture2D _m = MainAPI.Instance.Texture2DFromSprite(mkrSprite);
                string _n = contentData.id;
                GameObject _g = volumetricPrefab;
                VolumetricItem _v = _g.GetComponentInChildren<VolumetricItem>();
                _v.id = contentData.id;
                _v.src = contentData.video;
                _v.localSrc = StorageManager.GetVolumetricFilePath(contentData.video);
                AddMarker(_m, _n, _g);
            });
        }

        public void AddMarker(Texture2D sampleMarker, string sampleMarkerName, GameObject samplePrefab)
        {
            if (!isARInitialize)
            {
                DebugText("AR state is not ready..");
                return;
            }

            //if (CheckMarkerExists(sampleMarkerName))
            //{
            //    DebugText("Marker name " + sampleMarkerName + " is already exist..");
            //    return;
            //}

            DebugText("Begin add marker..");
            DebugImage(MainAPI.Instance.SpriteFromTexture2D(sampleMarker));

            if (IsSupportsMutableLibrary())
            {
                DebugText("supportsMutableLibrary is enabled..");

                var texture = sampleMarker;
                DebugText("Marker name: " + sampleMarkerName);

                ResetRuntimeImageLibrary();

                Unity.Jobs.JobHandle jobHandle = myRuntimeReferenceImageLibrary.ScheduleAddImageJob(texture, sampleMarkerName, 0.2f);
                jobHandle.Complete();

                //myRuntimeReferenceImageLibrary

                if (myRuntimeReferenceImageLibrary != null)
                {
                    DebugText("myRuntimeReferenceImageLibrary: " + myRuntimeReferenceImageLibrary.count);
                    DebugText("supportedTextureFormatCount: " + myRuntimeReferenceImageLibrary.supportedTextureFormatCount);

                    mARTrackedImageManager.referenceLibrary = myRuntimeReferenceImageLibrary;
                }
                else
                {
                    DebugText("myRuntimeReferenceImageLibrary is NULL");
                }
                mARTrackedImageManager.maxNumberOfMovingImages = 1;
                mARTrackedImageManager.trackedImagePrefab = samplePrefab;
                mARTrackedImageManager.enabled = true;
            }
            else
            {
                DebugText("supportsMutableLibrary is disabled");
            }
        }

        private bool CheckMarkerExists(string markerName)
        {
            if (mARTrackedImageManager == null)
            {
                DebugText("mARTrackedImageManager = null");
                return false;
            }

            if (mARTrackedImageManager.referenceLibrary == null)
            {
                DebugText("mARTrackedImageManager.referenceLibrary = null");
                return false;
            }

            for (int i = 0; i < mARTrackedImageManager.referenceLibrary.count; i++)
            {
                if (mARTrackedImageManager.referenceLibrary[i].name == markerName)
                {
                    return true;
                }
            }
            return false;
        }

        private void DebugText(string text = null)
        {
            if (text == null)
            {
                debugText.text = "";
                return;
            }

            if (debugText != null)
            {
                debugText.text += text + "\n";
            }
            Debug.Log(text);
        }

        private void DebugImage(Sprite sprite)
        {
            if (debugImage != null)
            {
                debugImage.sprite = sprite;
            }
        }

        private bool IsSupportsMutableLibrary()
        {
            if (mARTrackedImageManager.descriptor == null)
            {
                return false;
            }
            return mARTrackedImageManager.descriptor.supportsMutableLibrary;
        }

        public void ResetRuntimeImageLibrary()
        {
            if (IsSupportsMutableLibrary())
            {
                myRuntimeReferenceImageLibrary =
                    mARTrackedImageManager.CreateRuntimeLibrary() as MutableRuntimeReferenceImageLibrary;
                mARTrackedImageManager.referenceLibrary = myRuntimeReferenceImageLibrary;
            }
        }

        public void SetVolumetricContentWithoutMarkerData(ImmersiveItemsResultData contentData)
        {
            VolumetricItem _v = volumetricWithoutMarkerObject.GetComponentInChildren<VolumetricItem>();
            _v.id = contentData.id;
            _v.src = contentData.video;
            _v.localSrc = StorageManager.GetVolumetricFilePath(contentData.video);
            //_v.gameObject.transform.localScale = new Vector3(1, 1, 1);
        }

        public void ActivateVolumetricContentWithoutMarker(bool state, ImmersiveItemsResultData contentData = null)
        {
            if(state)
            {
                if(contentData == null)
                {
                    return;
                }

                volumetricWithoutMarkerObject = Instantiate(volumetricWithoutMarkerPrefab) as GameObject;
                SetVolumetricContentWithoutMarkerData(contentData);
                var targetPosition = Camera.main.transform.position
                    + Camera.main.transform.forward * volumetricWithoutMarkerIniPosDistance.x
                    + Camera.main.transform.up * volumetricWithoutMarkerIniPosDistance.y;

                Vector3 lookPos = Camera.main.transform.position - volumetricWithoutMarkerObject.transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);

                sessionOrigin.MakeContentAppearAt(
                    volumetricWithoutMarkerObject.transform,
                    targetPosition,
                    rotation
                );

                //Vector3 lookPos = Camera.main.transform.position - volumetricWithoutMarkerObject.transform.position;
                //lookPos.y = 0;
                //Quaternion rotation = Quaternion.LookRotation(lookPos);
                //volumetricWithoutMarkerObject.transform.rotation =
                //    Quaternion.Slerp(volumetricWithoutMarkerObject.transform.rotation, rotation, 0);
            }
            else
            {
                Destroy(volumetricWithoutMarkerObject);
            }
            //volumetricWithoutMarkerObject.SetActive(state);
        }

        public void StopVolumetricContent()
        {
            ResetRuntimeImageLibrary();
            ActivateVolumetricContentWithoutMarker(false);
        }

        public void SetSponsorButton(ImmersiveItemsResultData contentData)
        {
            m_sponsorButton[0].onClick.RemoveAllListeners();
            if (!string.IsNullOrWhiteSpace(contentData.sponsor.sponsor_1_image))
            {
                Davinci.get()
                .load(contentData.sponsor.sponsor_1_image)
                .setCached(true)
                .into(m_sponsorButton[0].GetComponentInChildren<Image>())
                .start();

                if (!string.IsNullOrWhiteSpace(contentData.sponsor.sponsor_1_link))
                {
                    m_sponsorButton[0].onClick.AddListener(delegate
                    {
                        OpenSponsorLink(contentData.sponsor.sponsor_1_link);
                    });
                }
                m_sponsorButton[0].gameObject.SetActive(true);
            }
            else
            {
                m_sponsorButton[0].gameObject.SetActive(false);
            }

            m_sponsorButton[1].onClick.RemoveAllListeners();
            if (!string.IsNullOrWhiteSpace(contentData.sponsor.sponsor_2_image))
            {
                Davinci.get()
                .load(contentData.sponsor.sponsor_2_image)
                .setCached(true)
                .into(m_sponsorButton[1].GetComponentInChildren<Image>())
                .start();

                if (!string.IsNullOrWhiteSpace(contentData.sponsor.sponsor_2_link))
                {
                    m_sponsorButton[1].onClick.AddListener(delegate
                    {
                        OpenSponsorLink(contentData.sponsor.sponsor_2_link);
                    });
                }
                m_sponsorButton[1].gameObject.SetActive(true);
            }
            else
            {
                m_sponsorButton[1].gameObject.SetActive(false);
            }
        }

        public void OpenSponsorLink(string url)
        {
            var safeBrowsing = UniWebViewSafeBrowsing.Create(url);
            safeBrowsing.Show();
        }
    }
}

