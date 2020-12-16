using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AddressableManager : MonoBehaviour
{
    

    public GameObject myGameObj;
    private AsyncOperationHandle<GameObject> player;
    public Text text;

     void Start()
    {
        LoadPlayer();
        
    }
   public void spawn()
    {
        
    }

    private void Update()
    {
        if (player.IsValid())
        {
            text.text = string.Format("Loading: {0}%", (int)(player.PercentComplete * 100));
            
        }
        
    }
    private void OnLoadDone(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Failed)
        {
            Debug.Log("Failed to load hazards, retrying in 1 second...");
            Invoke("LoadPlayer", 1);
            return;
        }
        myGameObj = obj.Result;
    }
    void LoadPlayer()
    {
        player = Addressables.LoadAssetAsync<GameObject>("Mesh");
        player.Completed += OnLoadDone;
        text.text = string.Format("Loading: {0}%", 0);
    }
    void LoadScene()
    {
        SceneManager.LoadSceneAsync("addressable");
    }
}
