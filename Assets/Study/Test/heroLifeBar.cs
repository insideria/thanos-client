using UnityEngine;

public class heroLifeBar : MonoBehaviour {
    
    public UISprite mpGreenSprite = null;
    public UISprite hpGreenSprite = null;
    public GameObject mPlayerGO = null;
    public GameObject mHeroLife = null;

	// Use this for initialization
	void Start () {
		
	}

    Vector3 WorldToUI()
    {
        Vector3 ff = new Vector3(0, 0, 0);
        float height = 5;

        Vector3 pt = mPlayerGO.transform.position;
        pt += new Vector3(-4f, height, 0f);
        var posScreen = GameObject.Find("Main Camera").GetComponent<Camera>().WorldToScreenPoint(pt);

        ff = NGUITools.FindCameraForLayer(mHeroLife.layer).ScreenToWorldPoint(posScreen);
        return ff;
    }

    void UpdateHp(int value)
    {
        hpGreenSprite.fillAmount = value / 100.0f;
    }

    void UpdateMp(int value)
    {
        mpGreenSprite.fillAmount = value / 100.0f;
    }
	
    float timer1 = 100;
    float timer2 = 100;
	// Update is called once per frame
	void Update () {
        timer1 -= 0.1f;
        UpdateHp(System.Convert.ToInt32(timer1));
        timer2 -= 0.2f;
        UpdateMp(System.Convert.ToInt32(timer2));

        mHeroLife.transform.localPosition = WorldToUI();
	}
}
