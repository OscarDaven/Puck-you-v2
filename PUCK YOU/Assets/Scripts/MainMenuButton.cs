using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    public void OnButtonClick()
    {
      this.enabled = false;
      Canvas mainmenu = GameObject.FindGameObjectWithTag("MainMenuScreen").GetComponent<Canvas>();
      mainmenu.enabled = true;
	}
}
