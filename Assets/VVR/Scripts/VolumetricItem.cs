using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using prometheus;
using VVR.Storage;

public class VolumetricItem : MonoBehaviour
{
    public string id;
    public string src; // path จาก api
    public string localSrc; // path จาก device storage

    MeshPlayerPlugin volumetricPlayer;

    void Start()
    {
        volumetricPlayer = GetComponent<MeshPlayerPlugin>();
        IniVolumtricContent();
    }

    private void IniVolumtricContent()
    {
        volumetricPlayer.SourceType = VoumetricSourceType(localSrc);
        volumetricPlayer.SourceUrl = localSrc;
        volumetricPlayer.DataInStreamingAssets = false;
    }

    private SOURCE_TYPE VoumetricSourceType(string filepath)
    {
        string ext = StorageManager.GetFileExtension(filepath).ToLower();
        if(ext == "rtmp")
        {
            return SOURCE_TYPE.RTMP;
        }
        return SOURCE_TYPE.PLAYBACK;
    }
}
