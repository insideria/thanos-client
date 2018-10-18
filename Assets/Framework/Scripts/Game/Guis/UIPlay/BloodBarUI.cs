using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic; 
using Thanos.GameEntity;
using GameDefine;

using Thanos.Tools;

public class BloodBarUI : MonoBehaviour 
{
    public string mResName;
    private bool mNeedUpdate = true;
	#region 血条类型
	public XueTiaoType xueTiaoType{
		private set;
		get;
	}
	#endregion


	#region 血条的归属者
    public IEntity xueTiaoOwner
    {
        private set;
        get;
    }


	#endregion

	#region  私有变量区	 
	protected UISprite hpSprite = null;  
	private Camera _ui2dCamera;
	private Camera _mainCamera;  
	private float xueTiaoHeight = 0f;
    protected UILabel labelCost;
    protected const int CanControl = 1; 
	#endregion

    public UILabel uiDebugInfo;
	
	#region 获得所需要的transform ,ex hpSlider,mpSlider,name label
	
    public virtual void Init(IEntity entity){
		_mainCamera = Camera.main;
		_ui2dCamera = GameMethod.GetUiCamera;
        uiDebugInfo = this.transform.Find("Name").GetComponent<UILabel>();
        uiDebugInfo.gameObject.SetActive(true);
        if (entity.entityType != EntityTypeEnum.Player)
        {
            uiDebugInfo.text = null;
        }
        //uiDebugInfo.text = CTools.GetGUIDIndex(entity.GameObjGUID).ToString();
		xueTiaoOwner = entity;
	
        xueTiaoHeight = GetXueTiaoHeight();



	}
	#endregion 

	#region 受到伤害时，血条的扣除显示方式
	public virtual void XueTiaoDmgShow(float currentHp,float dmg, float maxHp){ 

	}
	#endregion

	#region 初始化血条
	public virtual void ResetBloodBarValue(){
		hpSprite.fillAmount = 0f;
	}
	#endregion

	#region 创建血条之后，接收到服务器发送的血条信息，初始化血条
	public virtual void SetXueTiaoInfo(){
		UpdateHP();
	}
	#endregion 

    //更新血条
	public void UpdateHP(){

		hpSprite.fillAmount = xueTiaoOwner.Hp /xueTiaoOwner.HpMax;

	}

    public virtual void IsBloodBarCpVib(bool isVis)
    {
        if (labelCost == null)
            return;
        if (isVis)
        {
            NpcConfigInfo info = ConfigReader.GetNpcInfo(xueTiaoOwner.NpcGUIDType);

            int cp = (int)info.NpcConsumeCp;
            if (info.NpcCanControl == CanControl)
            {
                if (!xueTiaoOwner.IsSameCamp(PlayerManager.Instance.LocalPlayer.EntityCamp) 
                    && (xueTiaoOwner.entityType == EntityTypeEnum.AltarSoldier || xueTiaoOwner.entityType == EntityTypeEnum.Soldier))
                {
                    cp *= 2;                                   
                }
                if (info.NpcType != (int)EntityTypeEnum.Player || info.NpcType != (int)EntityTypeEnum.Building)
                {
                    labelCost.text = "CP " + cp.ToString();//小兵的cp
                }        
            }
        }           
        labelCost.gameObject.SetActive(isVis);
    }
	protected virtual float GetXueTiaoHeight(){
		NpcConfigInfo info = ConfigReader.GetNpcInfo(xueTiaoOwner.NpcGUIDType);
		return info.NpcXueTiaoHeight;
	}

    protected virtual void Update()
	{
        if (mNeedUpdate && xueTiaoOwner != null && xueTiaoOwner.realObject != null)
        {
			UpdatePosition(xueTiaoOwner.realObject.transform);
		}
	}

    public void SetVisible(bool val)
    {
        if (!val)
        {
            transform.localPosition = new Vector3(5000, 0, 0);
        }

        mNeedUpdate = val;
    }

	public void UpdatePosition(Transform target)
	{

        if (_mainCamera == null || _ui2dCamera == null)
            return;
        // 血条对应在3d场景的位置
		Vector3 pos_3d = target.position + new Vector3(0f, xueTiaoHeight , 0f);
		// 血条对应在屏幕的位置
		Vector2 pos_screen = _mainCamera.WorldToScreenPoint(pos_3d);
		// 血条对应在ui中的位置
		Vector3 pos_ui = _ui2dCamera.ScreenToWorldPoint(pos_screen);
 
		transform.position = Vector3.Slerp(transform.position,pos_ui,Time.time);
	
	} 
 
}
