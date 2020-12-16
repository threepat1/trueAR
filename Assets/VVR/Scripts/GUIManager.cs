using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using VVR.ImmersiveContents;
using VVR.AR;

namespace VVR.GUI
{
    public class GUIManager : MonoBehaviour
    {
        public enum ActivePage
        {
            Category, ContentList, ARViewer
        }

        private static GUIManager _Instance;
        public static GUIManager Instance
        {
            get
            {
                return _Instance;
            }
        }

        public Canvas m_canvas;
        private Vector2 m_canvasResolution;

        [Header("Notch safe area")]
        public RectTransform[] SafeAreaRectTransform;
        public float SafeAreaGap = 40;
        private Rect LastSafeArea = new Rect(0, 0, 0, 0);

        [Header("Page")]
        public ActivePage m_activePage = ActivePage.Category;
        public RectTransform page_category;
        public RectTransform page_arViewer;
        public List<RectTransform> page_category_rebuildLayout;
        public List<RectTransform> page_arViewer_rebuildLayout;
        public float pageTrnsitionDuration = 0.25f;

        private bool isBannerPauseBeforeLeaveCategoryPage = false;
        private bool isBannerMuteBeforeLeaveCategoryPage = false;

        void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
        }

        private void Start()
        {
            page_arViewer.gameObject.SetActive(false);

            m_canvasResolution = m_canvas.GetComponent<CanvasScaler>().referenceResolution;
            ChangeActivePage(ActivePage.Category);
        }

        private void Update()
        {
            UpdateNotchSafeArea();
        }

        public void ChangeActivePage(string newPage)
        {
            ActivePage parsed_enum = (ActivePage)System.Enum.Parse(typeof(ActivePage), newPage);
            ChangeActivePage(parsed_enum);
        }

        public void ChangeActivePage(ActivePage newPage)
        {
            m_activePage = newPage;
            switch (m_activePage)
            {
                case ActivePage.Category:
                    OpenPageCategory();
                    break;
                case ActivePage.ARViewer:
                    OpenPageARViewer();
                    break;
                default:
                    break;
            }
            Debug.Log("Change active page to " + newPage.ToString());
        }

        void UpdateNotchSafeArea()
        {
            Rect safeArea = GetSafeArea();

            if (safeArea != LastSafeArea)
            {
                //Debug.Log("======= Safe area log =======");
                //Debug.Log("Safe area log: Width = " + safeArea.width);
                //Debug.Log("Safe area log: Height = " + safeArea.height);
                //Debug.Log("Safe area log: X = " + safeArea.x);
                //Debug.Log("Safe area log: Y = " + safeArea.y);
                //Debug.Log("======= Safe area log =======");
                ApplySafeArea(safeArea);
            }
        }

        Rect GetSafeArea()
        {
            return Screen.safeArea;
        }

        void ApplySafeArea(Rect r)
        {
            float _y = SafeAreaGap;
            if (r.y > SafeAreaGap)
            {
                _y = r.y + SafeAreaGap;
            }

            foreach (var rect in SafeAreaRectTransform)
            {
                // Set height to safe area rect transform
                rect.sizeDelta = new Vector2(0, _y);
            }
            LastSafeArea = r;
        }

        #region Open page method
        public void OpenPageCategory()
        {
            page_category.DOAnchorPos(Vector2.zero, pageTrnsitionDuration);
            page_arViewer.gameObject.SetActive(false);

            ARViewer.Instance.StopVolumetricContent();

            if(isBannerPauseBeforeLeaveCategoryPage)
            {
                ImmersiveBanner.Instance.PlayBanner();
            }
        }

        private void OnClosePageCategory()
        {
            //if(!ImmersiveBanner.Instance.bannerMediaPlaylist.Control.IsPaused())
            //{
                isBannerPauseBeforeLeaveCategoryPage = !ImmersiveBanner.Instance.bannerMediaPlaylist.Control.IsPaused();
            //}

            ImmersiveBanner.Instance.PauseBanner();
        }

        public void OpenPageARViewer()
        {
            page_category.DOAnchorPos(new Vector2(-m_canvasResolution.x, 0), pageTrnsitionDuration);
            page_arViewer.gameObject.SetActive(true);

            OnClosePageCategory();
        }
        #endregion

        #region Layout rebuild method
        public void RebuildPageCategory()
        {
            foreach (var item in page_category_rebuildLayout)
            {
                if(item == null)
                {
                    continue;
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            }
        }

        public void RebuildPageARViewer()
        {
            foreach (var item in page_arViewer_rebuildLayout)
            {
                if (item == null)
                {
                    continue;
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            }
        }
        #endregion
    }
}
