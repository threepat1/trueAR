using System;
using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using UnityEngine;
using VVR.API;
using UnityEngine.UI;

namespace VVR.ImmersiveContents
{
    public class ImmersiveBanner : MonoBehaviour
    {
        public static ImmersiveBanner Instance;

        public PlaylistMediaPlayer bannerMediaPlaylist;
        public GameObject bannerPrefab;
        public Transform bannerContainer;
        public CanvasGroup bannerPlaceholder;

        [Header("Banner controller ui")]
        public Sprite OnPauseIcon;
        public Sprite OnPlayIcon;
        public Sprite OnMuteIcon;
        public Sprite OnUnmuteIcon;

        public Button btnPlayBanner, btnNextBanner, btnPrevBanner, btnMuteBanner;

        private bool isBannerReady = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            LoadImmersiveBanner();
        }

        void LoadImmersiveBanner()
        {
            MainAPI.Instance.SendRequst(MainAPI.RequestType.GET_IMMERSIVE_BANNER, "", OnLoadImmersiveBannerSuccess);
        }

        private void OnLoadImmersiveBannerSuccess(string res, string err)
        {
            ImmersiveBannerAPIResult result = JsonUtility.FromJson<ImmersiveBannerAPIResult>(res);
            if (result.status != "success")
            {
                Debug.Log(result.message);
                return;
            }

            Init360Banner(result.result.data);
        }

        private void Init360Banner(ImmersiveBannerResultData[] banners)
        {
            if (banners.Length > 0)
            {
                GameObject ui = Instantiate(bannerPrefab, bannerContainer);
                btnPlayBanner = ui.transform.Find("BannerBtnPlay").GetComponent<Button>();
                btnPlayBanner.onClick.AddListener(OnClickPlayBanner);
                
                btnNextBanner = ui.transform.Find("BannerBtnNext").GetComponent<Button>();
                btnNextBanner.onClick.AddListener(NextBanner);

                btnPrevBanner = ui.transform.Find("BannerBtnPrev").GetComponent<Button>();
                btnPrevBanner.onClick.AddListener(PrevBanner);

                btnMuteBanner = ui.transform.Find("BannerBtnMute").GetComponent<Button>();
                btnMuteBanner.onClick.AddListener(MuteBanner);

                bannerPlaceholder.transform.SetAsLastSibling();
                StartCoroutine(HideBannerPlaceholder());
                isBannerReady = true;
            }
            else
            {
                return;
            }

            bannerMediaPlaylist.LoopMode = PlaylistMediaPlayer.PlaylistLoopMode.Loop;
            foreach (var item in banners)
            {
                MediaPlaylist.MediaItem it = new MediaPlaylist.MediaItem
                {
                    fileLocation = MediaPlayer.FileLocation.AbsolutePathOrURL,
                    filePath = item.image,
                    autoPlay = true,
                    startMode = PlaylistMediaPlayer.StartMode.Immediate,
                    progressMode = PlaylistMediaPlayer.ProgressMode.BeforeFinish,
                };
                bannerMediaPlaylist.Playlist.Items.Add(it);
            }
            // Play first banner
            bannerMediaPlaylist.JumpToItem(0);

            btnPlayBanner.interactable = true;
            btnNextBanner.interactable = true;
            btnPrevBanner.interactable = true;
            btnMuteBanner.interactable = true;
        }

        IEnumerator HideBannerPlaceholder()
        {
            float speed = 1f;
            while (bannerPlaceholder.alpha > 0.0)
            {
                bannerPlaceholder.alpha -= speed * Time.deltaTime;
                yield return null;
            }
            bannerPlaceholder.gameObject.SetActive(false);
        }

        private void OnClickPlayBanner()
        {
            if(bannerMediaPlaylist.CurrentPlayer.Control.IsPlaying())
            {
                PauseBanner();
            }
            else
            {
                PlayBanner();
            }
        }

        public void PauseBanner()
        {
            if (!isBannerReady)
            {
                return;
            }
            Debug.Log("Pause banner");
            bannerMediaPlaylist.CurrentPlayer.Pause();
            btnPlayBanner.GetComponent<Image>().sprite = OnPlayIcon;
        }

        public void PlayBanner()
        {
            if (!isBannerReady)
            {
                return;
            }
            Debug.Log("Play banner");
            bannerMediaPlaylist.CurrentPlayer.Play();
            btnPlayBanner.GetComponent<Image>().sprite = OnPauseIcon;
        }

        public void NextBanner()
        {
            if (!isBannerReady)
            {
                return;
            }
            bannerMediaPlaylist.NextItem();
        }

        public void PrevBanner()
        {
            if (!isBannerReady)
            {
                return;
            }
            bannerMediaPlaylist.PrevItem();
        }

        public void MuteBanner()
        {
            if (!isBannerReady)
            {
                return;
            }
            bannerMediaPlaylist.CurrentPlayer.Control.MuteAudio(!bannerMediaPlaylist.CurrentPlayer.Control.IsMuted());
            btnMuteBanner.GetComponent<Image>().sprite = bannerMediaPlaylist.CurrentPlayer.Control.IsMuted() ? OnMuteIcon : OnUnmuteIcon;
        }
    }
}