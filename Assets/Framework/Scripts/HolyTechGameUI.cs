using UnityEngine;
using Thanos.GameState;
using Thanos.Resource;
using Thanos.View;

public class HolyTechUI : MonoBehaviour
{
	//PP AS用户界面
	private GameObject mToolBarObj;
	
	public static HolyTechUI Instance
	{
		private set;
		get;
	}
	
	void Awake()
	{
		if (Instance != null)
		{
			DestroyImmediate(this.gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this.gameObject);
	}
	
	void Start()
	{
		//创建PPSDK按钮
		#if SDK_PP
		CreatePPSdkButton();
		#elif SDK_AS
		CreateASSdkButton();
		
		#endif
	}
	void OnDestroy()
	{
		
	}
	
	public void CreatePPSdkButton()
	{      
		ResourceItem ppUnit = ResourcesManager.Instance.loadImmediate("Guis/PP", ResourceType.PREFAB);
		mToolBarObj = Instantiate(ppUnit.Asset) as GameObject;
		DontDestroyOnLoad(mToolBarObj);
		
		//绑定到camera
		GameObject cameraObj = GameObject.Find("Camera");
		mToolBarObj.transform.parent = cameraObj.transform;
		mToolBarObj.transform.localScale = new Vector3(1, 1, 1);
		mToolBarObj.transform.localPosition = new Vector3(0,0,0);
		
		//添加listener
		GameObject positonObj = mToolBarObj.transform.Find("Position").gameObject;
		ButtonOnPress positonButtonOnPress = positonObj.GetComponent<ButtonOnPress>();
		
		//添加listener    
		positonButtonOnPress.AddListener(PositionOnPress);
		
		
	}
	
	public void ShowPPSdkButton(bool flag)
	{
		if (mToolBarObj == null)
		{
			CreatePPSdkButton();
			Debug.Log("------------ mPPObje is null");
		}
		
		mToolBarObj.SetActive(flag);
	}
	
	public void CreateASSdkButton()
	{      
		ResourceItem ASUnit = ResourcesManager.Instance.loadImmediate("Guis/Aisi", ResourceType.PREFAB);
		mToolBarObj = Instantiate(ASUnit.Asset) as GameObject;
		DontDestroyOnLoad(mToolBarObj);
		
		//绑定到camera
		GameObject cameraObj = GameObject.Find("Camera");
		mToolBarObj.transform.parent = cameraObj.transform;
		mToolBarObj.transform.localScale = new Vector3(1, 1, 1);
		mToolBarObj.transform.localPosition = new Vector3 (0, 0, 0);
		
		//添加listener
		GameObject positonObj = mToolBarObj.transform.Find("Position").gameObject;
		ButtonOnPress positonButtonOnPress = positonObj.GetComponent<ButtonOnPress>();	
		//添加listener    
		positonButtonOnPress.AddListener(PositionOnPress);
			
	}

	public void ShowASSdkButton(bool flag)
	{
		if (mToolBarObj == null)
		{

			CreateASSdkButton();			
		}

		mToolBarObj.SetActive(flag);
	}
	
	public void PositionOnPress(int ie, bool isPress)
	{
		//SdkConector.ShowCenter ();//PP需要添加按钮显示个人充值中心
    }
    
    //打开UI
	public void OnOpenUI(string path)
	{
		ResourceItem GobjUnit = ResourcesManager.Instance.loadImmediate(path, ResourceType.PREFAB);
		GameObject Gobj = Instantiate(GobjUnit.Asset) as GameObject;                               
		
		
		Gobj.transform.parent = GetUIParent().transform;
		Gobj.transform.localScale = Vector3.one;
		Gobj.transform.localPosition = Vector3.zero;
	}
	
	/// <summary>
	/// 在UI摄像机下打开UI 在YSDGame中的游戏事件响应的OpenWaitUI中调用
	/// </summary>
	/// <param name="path"></param> 
	public void OnOpenUIPathCamera(string path)
	{
		ResourceItem GobjUnit = ResourcesManager.Instance.loadImmediate(path, ResourceType.PREFAB);
		GameObject Gobj = Instantiate(GobjUnit.Asset) as GameObject;                               	
		
		Gobj.transform.parent = GameDefine.GameMethod.GetUiCamera.transform;
		Gobj.transform.localScale = Vector3.one;
		Gobj.transform.localPosition = Vector3.zero;
	}
	
    //销毁UI
	public void OnDestroyUI(GameObject obj)
	{
		DestroyImmediate(obj);
	}
	
    //获取父物体 摄像机
	private GameObject GetUIParent()
	{
		switch (GameStateManager.Instance.GetCurState().GetStateType())
		{
		case GameStateTypeEnum.Login:
		{
			BaseWindow pWindow = WindowManager.Instance.GetWindow(EWindowType.EWT_LoginWindow);
			if (pWindow != null && pWindow.GetRoot()!=null)
			{
                //获取的是摄像机
				return pWindow.GetRoot().gameObject;
			}
			return null;
		}
		case GameStateTypeEnum.Play:
        {
            BaseWindow pWindow = WindowManager.Instance.GetWindow(EWindowType.EWT_GamePlayWindow);
            if (pWindow != null && pWindow.GetRoot() != null)
            {
                return pWindow.GetRoot().gameObject;
            }
            return null;
        }
		case GameStateTypeEnum.Over:
		{
			BaseWindow pWindow = WindowManager.Instance.GetWindow(EWindowType.EWT_ScoreWindow);
			if (pWindow != null && pWindow.GetRoot() != null)
			{
				return pWindow.GetRoot().gameObject;
			}
			return null;
		}
		}
		
		return null;
	}
	
}
