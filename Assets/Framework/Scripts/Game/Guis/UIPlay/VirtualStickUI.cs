using UnityEngine; 
using System;
using System.Collections;
using System.Collections.Generic;
using UICommon ; 
using Thanos.GameEntity;
using Thanos.GameData;
using Thanos.GameData;
using Thanos.FSM;
using Thanos.GuideDate;
using Thanos.Model;
using Thanos.GameState;

public class VirtualStickUI : MonoBehaviour 
{
    #region 公有方法

    //设置遥感是否可用
   public void SetVirtualStickUsable(bool enable)
	{
		canUse = enable; 		
		if (!enable) 
        {
			CloseStick();	
		}
	}

	#endregion

	#region 私有方法 
    void Awake()
    {
        Init();
    }

    // 初始化遥感状态
	void Init()
	{ 
		Instance = this;
		orignalPos = transform.localPosition;
        VirtualStickState = StickState.InActiveState;//初始化为禁用状态
		point = transform.Find("stick");
		btnSelf = transform.GetComponent<ButtonOnPress>();
	}


	void OnEnable()
	{		
		canUse = true;
		SetVisiable(false);
        btnSelf.AddListener(PressVirtual, ButtonOnPress.EventType.PressType);//监控遥感状态
        Thanos.Ctrl.UIGuideCtrl.Instance.AddUiGuideEventBtn(btnSelf.gameObject);//将要触发事件的新手引导的按钮放入列表

    }

    void OnDisable() {
        btnSelf.RemoveListener(PressVirtual, ButtonOnPress.EventType.PressType);//移除监听器
        VirtualStickState = StickState.InActiveState;//禁用状态
    }
 
	/// <summary>
	/// 点击时显示摇杆，松开始隐藏摇杆
	/// </summary>
	/// <param name="index">Index.</param>
	/// <param name="isDown">If set to <c>true</c> is down.</param>
   void PressVirtual(int ie,bool isDown)
    {
		if(isDown){
			ShowStick();
		}else{
			CloseStick();
		}
	}
 
	void OnDrag(Vector2 pos)
	{
		if(canUse == false) return;
		Vector2 touchPos = UICamera.currentTouch.pos;//获取触摸位置		 
		SetPointPos(touchPos);//设置遥感位置
        VirtualStickState = StickState.MoveState;//设置遥感状态为移动状态
		SendMove();//发送移动消息
	}

    /// <summary>
    /// 显示摇杆
    /// </summary>
	void ShowStick()
	{
		if(canUse == false) return;
    
        Vector2 touchPos = UICamera.currentTouch.pos;

        SetPointPos(touchPos);

        VirtualStickState = StickState.MoveState;//遥感移动状态
      
        //位置设置
        transform.position = new Vector3(UICommonMethod.GetWorldPos(touchPos).x, UICommonMethod.GetWorldPos(touchPos).y, transform.position.z);
        point.localPosition = Vector3.zero;
    
        //颜色变亮
		SetVisiable(true);		 
	} 

    /// <summary>
    /// 关闭遥感
    /// </summary>
	void CloseStick()
	{

        if (VirtualStickState == StickState.MoveState)
        {
            SendStop();
        }
        GameObject player = GamePlay.Instance.LocalPlayer;
        if (player==null) {
            return;
        }
        Animation ani = player.GetComponent<Animation>();
        if (ani!=null)
        {
            ani.Play("free");//进入自由状态
        }
        SetVisiable(false);//遥感颜色变暗
        VirtualStickState = StickState.InActiveState;//遥感状态为禁用状态
        beforeDir = Vector3.zero;
	}

    //发送结束
	void SendStop(){
		HolyGameLogic.Instance.EmsgToss_AskStopMove ();
	}
	
	private const float SendMoveInterVal = 0.05f;  //移动间隔
	private float MoveSendTime;
	void SendMove()
	{
        //获取实体并控制

        Vector3 direction = GetPointerDirection();	//获取遥感与中心点的方向	
        GameObject entity = GamePlay.Instance.LocalPlayer;//获取实体
        if (entity == null) return;

        entity.transform.position = entity.transform.position;//获取实体的位置
        entity.transform.LookAt(entity.transform.position + direction);//实体朝向   
        //运动正方向
        Vector3 dir = new Vector3(0, 0, 0);

        Quaternion rot = Quaternion.Euler(0, 0f, 0);
        dir = rot * entity.transform.forward;

        //SendTime = Time.time; 
        HolyGameLogic.Instance.EmsgToss_AskMoveDir(dir);//向服务器请求移动
        beforeDir = dir;

	}

    //玩家移动
    private void PlayerAdMove(Vector3 mvDir) {

        ISelfPlayer player = PlayerManager.Instance.LocalPlayer;//获取本地玩家
        if (Thanos.Skill.BuffManager.Instance.isHaveStopBuff(player.GameObjGUID)) return;    
        if (player.FSM == null)  return;    
        if (player.FSM.State == FsmStateEnum.DEAD || player.FSM.State == FsmStateEnum.RUN || player.FSM.State == FsmStateEnum.FORCEMOVE
            || player.FSM.State == FsmStateEnum.RELIVE)
        {
            return;
        }
        float mvSpeed = player.EntityFSMMoveSpeed;//获取玩家速度
        if(mvSpeed <= 0)
        {
            mvSpeed = 3.0f;
        }
        //指定玩家状态机的位置，速度和方向。
        player.EntityFSMChangedata(player.realObject.transform.position, mvDir, mvSpeed);
        player.OnFSMStateChange(PlayerAdMoveFSM.Instance);
    }

	void SetVisiable(bool visiable)
	{
		if (visiable) {
            //遥感颜色设置为亮色
			UICommonMethod.TweenColorBegin(gameObject, 0f, 1f);
		}else {
            //遥感颜色设置为暗色
            UICommonMethod.TweenColorBegin(gameObject, 0f, 0.5f);
			transform.localPosition = orignalPos;
			point.localPosition = Vector3.zero;
		}
	}

    //设置遥感位置
	void SetPointPos(Vector2 pos)
	{
        //根据屏幕坐标获取世界坐标的位置
		Vector3 newPos = UICommonMethod.GetWorldPos(pos)+new Vector3(0f,0f,point.position.z);
		
        //当移动的距离大于边界时
		if(Vector3.Distance(newPos,transform.position) > adjustRadius)
		{		
			Vector3 direction = newPos - transform.position;//获得方向
			direction.Normalize();//归一化
			direction *= adjustRadius;//防止遥感越过遥感边界
            newPos = transform.position + direction; //设置遥感位置
		}
        //否则直接为遥感设置位置  Point是可移动的中心位置的的对象
		point.position = newPos;		 
	} 

    //获取方向
	Vector3 GetPointerDirection()
	{
		Vector3 direction = point.position - transform.position;
		
		direction = new Vector3(direction.x, 0f, direction.y);
		direction.Normalize();
		
		return direction;
	}
	
	Vector3 NormalVector3(Vector3 pos)
	{
		float x = (float)Math.Round(pos.x,1);
		float y = (float)Math.Round(pos.y,1);
		float z = (float)Math.Round(pos.z,1);
		Vector3 posTemp = new Vector3(x,y,z);
		return posTemp;
	}
	
	bool Vect3Compare(Vector3 pos1,Vector3 pos2)
	{  
		if(pos1.x != pos2.x){	
			return false ;
		}
		if(pos1.y != pos2.y){			 
			return false ;
		}
		if(pos1.z != pos2.z){ 
			return false ;
		}
		
		return true;
	}
 
	#endregion

	#region 变量区	
	public static VirtualStickUI Instance {
		get;
		private set;
	}


	public enum StickState
	{
		ActiveState,
		MoveState,
		InActiveState,
	}

	public StickState VirtualStickState{
		get;
		private set;
	}

	private Transform point;//遥感的位置

	private ButtonOnPress btnSelf;

	private Vector3 orignalPos = new Vector3();

	private bool canUse = true; 

	public float adjustRadius = 0.3f;

	private Vector3 beforeDir;
	#endregion
}
