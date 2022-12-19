using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    public Canvas mainMenuCanvas;
    public Button hostBtn;
    public Button clientBtn;

    private void Awake(){
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            mainMenuCanvas.enabled = false;
        });
        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            mainMenuCanvas.enabled = false;
        });
    }
}