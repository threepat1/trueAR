using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VVR.API;
using UnityEngine.UI;
using VVR.GUI;
using VVR.AR;
using VVR.Storage;

namespace VVR.ImmersiveContents
{
    public class ImmersivItems : MonoBehaviour
    {
        public Image imgThumbnails;
        public Button btnAction;
        public Texture2D imgPlaceholder;

        public ImmersiveItemsResultData itemsData;

        public void Init(ImmersiveItemsResultData data)
        {
            itemsData = data;
            btnAction.onClick.AddListener(OnClickItems);
            Davinci.get()
            .load(itemsData.image)
            .setLoadingPlaceholder(imgPlaceholder)
            .setCached(true)
            .into(imgThumbnails)
            .start();
        }

        public void OnClickItems()
        {
            if (!StorageManager.HasVolumetricContent(itemsData.video))
            {
                ImmersiveManager.Instance.DownloadVolumetricContent(this);
                return;
            }

            if (itemsData.requiredMarker)
            {
                // Required marker
                ARViewer.Instance.AppendNewVolumetricContent(itemsData);
            }
            else
            {
                // No required marker
                //ARViewer.Instance.SetVolumetricContentWithoutMarkerData(itemsData);
                ARViewer.Instance.ActivateVolumetricContentWithoutMarker(true, itemsData);
                // spawn ar object here
            }

            // Open AR view
            GUIManager.Instance.ChangeActivePage(GUIManager.ActivePage.ARViewer);

            // Set sponsor logo
            ARViewer.Instance.SetSponsorButton(itemsData);
        }
    }
}