using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VVR.ImmersiveContents;

public class ClickBannerPausePlay : MonoBehaviour
{
    public PlaylistMediaPlayer player;
    Button bttn;
    public Sprite pauseImg, playImg;

    void Start()
    {
      
        bttn = this.GetComponent<Button>();
        player = GameObject.FindGameObjectWithTag("Playlist").GetComponent<PlaylistMediaPlayer>(); ;
        bttn.onClick.RemoveAllListeners();
        bttn.onClick.AddListener(ImmersiveBanner.Instance.PauseBanner);
        pauseImg = Resources.Load<Sprite>("UI/pause_white");
        playImg = Resources.Load<Sprite>("UI/play_white");
        
    }

    void Update()
    {
        //SwitchPausePlay();
    }

    void SwitchPausePlay()
    {
        bttn.onClick.RemoveAllListeners();
        if (player.Control.IsPlaying())
        {
           
            bttn.onClick.AddListener(ImmersiveBanner.Instance.PauseBanner);
            bttn.image.sprite = pauseImg;

        }
        if (player.Control.IsPaused())
        {
            
            bttn.onClick.AddListener(ImmersiveBanner.Instance.PlayBanner);
            bttn.image.sprite = playImg;
        }
    }
}
