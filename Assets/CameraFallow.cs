using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFallow : MonoBehaviour {
    GameObject player  ;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("Nvyao_5");
	}
	
	// Update is called once per frame
	void Update () { 
        if (player != null)
        {
            this.transform.position = new Vector3(3, 30, -26)+player.transform.position ;
        }
	}
}
