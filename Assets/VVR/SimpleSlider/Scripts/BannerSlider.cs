using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Creates banners and paginator by given banner list.
/// </summary>
public class BannerSlider : MonoBehaviour
{
    [Header("Settings")]
    public List<Banner> Banners;
    public bool Random;
    public bool Elastic = true;

    [Header("UI")]
    public Transform BannerGrid;
    public Button BannerPrefab;
    public Transform PaginationGrid;
    public Toggle PagePrefab;
    public HorizontalScrollSnap HorizontalScrollSnap;

    [Header("PopUp")]
    public GameObject videoPanel;
    public GameObject videoPanel1;
    public GameObject webView;
    public Text videoTitle;
    public Text description;


    public ScrollRect Menu;


    public void OnValidate()
    {
        GetComponent<ScrollRect>().content.GetComponent<GridLayoutGroup>().cellSize = GetComponent<RectTransform>().sizeDelta;
    }

    public IEnumerator Start()
    {


        foreach (Transform child in BannerGrid)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in PaginationGrid)
        {
            Destroy(child.gameObject);
        }

        foreach (var banner in Banners)
        {
            var instance = Instantiate(BannerPrefab, BannerGrid);
            var button = instance.GetComponent<Button>();

            button.onClick.RemoveAllListeners();

            if (string.IsNullOrEmpty(banner.Url))
            {
                //if (banner.data != null)
                //{
                //    Banner b = banner;
                //    button.onClick.AddListener(delegate { PushData(b); });
                //}
                //else
                //{
                    
                    button.onClick.AddListener(() => { StartCoroutine(ForceScrollDown()); });
                    
                //}
            }
            else
            {
                button.onClick.AddListener(() => {
                    Debug.Log(banner.Url);


                    //var safeBrowsing = UniWebViewSafeBrowsing.Create(banner.Url);

                    // Show it on screen.   
                    //safeBrowsing.Show();
                });
            }

            instance.GetComponent<Image>().sprite = banner.Sprite;

            if (Banners.Count > 1)
            {
                var toggle = Instantiate(PagePrefab, PaginationGrid);

                toggle.group = PaginationGrid.GetComponent<ToggleGroup>();
            }
        }

        yield return null;

        HorizontalScrollSnap.Initialize(Random);
        HorizontalScrollSnap.GetComponent<ScrollRect>().movementType = Elastic ? ScrollRect.MovementType.Elastic : ScrollRect.MovementType.Clamped;
    }

    //public void PushData(Banner banner)
    //{
    //    Debug.Log("name" + banner.data);
    //    videoPanel.SetActive(true);
    //    videoTitle.text = banner.data.title;
    //    description.text = banner.data.description;
    //    if (string.IsNullOrEmpty(banner.data.pathLink))
    //    {
    //        videoPanel1.SetActive(true);
    //        videoPanel.SetActive(false);
    //    }
    //    else
    //    {
    //        PlayerPrefs.SetString("path", banner.data.pathLink);
    //    }


    //}
    IEnumerator ForceScrollDown()
    {
        // Wait for end of frame AND force update all canvases before setting to bottom.
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();
        Menu.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }
}
