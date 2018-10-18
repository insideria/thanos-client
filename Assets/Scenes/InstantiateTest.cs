using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InstantiateTest : MonoBehaviour {

    //public UISprite sprite;

	void Start ()
    {
        //UIEventListener.Get(sprite.gameObject).onClick = (GameObject go) =>
        //{
        //    Debug.Log("点击了此对象");
        //};
        //sprite.GetComponent<UISprite>().spriteName = "14";
        SceneManager.LoadScene("Scene2");
	}

   
}
