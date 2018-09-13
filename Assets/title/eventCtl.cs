using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class eventCtl : MonoBehaviour 
{

    public void ChangeScene()
    {
        SceneManager.LoadScene("gamescene");
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
