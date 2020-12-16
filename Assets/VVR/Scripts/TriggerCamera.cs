using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class TriggerCamera : MonoBehaviour, IDragHandler
{
    private GameObject cam;
    private DragCamera dragCam;

    void Start()
    {
        cam = GameObject.FindWithTag("VideoCamera");
        dragCam = cam.GetComponent<DragCamera>();
    }

    public void OnDrag(PointerEventData data)
    {
        if (cam != null)
        {
            dragCam.OnDrag(data);
        }
    }

}
