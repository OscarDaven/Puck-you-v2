using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    public void OnButtonClick()
    {
		SceneManager.LoadScene(0);//loads main menu
	}
}
