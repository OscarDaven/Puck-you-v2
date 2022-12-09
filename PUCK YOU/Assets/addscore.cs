using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class addscore : MonoBehaviour
{
	public TMP_Text changingtext;
	public int score = 0;
	// Start is called before the first frame update

	public void updatescore()
	{
		score++;
		changingtext.text = score.ToString();
	}

	
}

