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
    public bool puckActive;



	// public TextMeshProUGUI ipAddressText;
	// [SerializeField]  TMP_InputField ip;
	// [SerializeField]  string ipAddress;
	// [SerializeField]  UnityTransport transport;
    // public string ipAddress;



    private void Awake(){
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            puckActive = true;
            mainMenuCanvas.enabled = false;

    		// GetLocalIPAddress();

        });

        clientBtn.onClick.AddListener(() => {

    		// ipAddress = ip.text;
		    // SetIpAddress();
            NetworkManager.Singleton.StartClient();

            puckActive = true;
            mainMenuCanvas.enabled = false;
        });
    }


	// public string GetLocalIPAddress() {
	// 	var host = Dns.GetHostEntry(Dns.GetHostName());
	// 	foreach (var ip in host.AddressList) {
	// 		if (ip.AddressFamily == AddressFamily.InterNetwork) {

    //             TMP_text ipAddressField = GameObject.Find("IPfield").GetComponent<TMP_Text>();

	// 			ipAddressField.text = ip.ToString();

	// 			ipAddress = ip.ToString();
	// 			return ip.ToString();
	// 		}
	// 	}
	// 	throw new System.Exception("No network adapters with an IPv4 address in the system!");
	// }



	// /* Sets the Ip Address of the Connection Data in Unity Transport
	// to the Ip Address which was input in the Input Field */
	// // ONLY FOR CLIENT SIDE
	// public void SetIpAddress() {

	// 	UnityTransport transport = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().NetworkManager.Singleton.GetComponent<UnityTransport>();
	// 	transport.ConnectionData.Address = ipAddress;
	// }


}