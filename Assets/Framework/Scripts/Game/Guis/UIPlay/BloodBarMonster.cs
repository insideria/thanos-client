using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic; 
using Thanos.GameEntity;

public class BloodBarMonster : BloodBarUI 
{	
    void Awake()
    {
        hpSprite = transform.Find("Control - Hp/Foreground").GetComponent<UISprite>();
        labelCost = transform.Find("CP").GetComponent<UILabel>();
    }
	public override void SetXueTiaoInfo ()
	{
		base.SetXueTiaoInfo ();
		NpcConfigInfo info = ConfigReader.GetNpcInfo(xueTiaoOwner.NpcGUIDType);
		if(info.NpcCanControl == CanControl){
			int cp = (int)info.NpcConsumeCp;
			labelCost.text = "CP "+ cp.ToString();
			labelCost.gameObject.SetActive(true); 
		}
		else{
			labelCost.gameObject.SetActive(false); 
		}
	}

	public override void ResetBloodBarValue ()
	{
		base.ResetBloodBarValue();
		labelCost.text = null;
		labelCost.gameObject.SetActive(false);
	}

    public override void IsBloodBarCpVib(bool isVis)
    {
        base.IsBloodBarCpVib(isVis);
    }
}
