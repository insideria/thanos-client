using UnityEngine;
using GameDefine;
using Thanos.GameState;
using Thanos.Resource;

public class LoadScene : MonoBehaviour {



    public delegate void LoadFinish(Thanos.GameState.GameStateTypeEnum St);
    public LoadFinish OnLoadFinish;

    public static LoadScene Instance {
		private set;
	    get;
	}

	public UIProgressBar m_ProGressbar;

	public bool isCloseBySelf = true;

    public GameStateTypeEnum GState
    {
				set;
				get;
	}

    ResourceAsyncOperation async = null;

	bool isEnd = false;
     
    private float startX = 0f;

    private GameObject objEffect;

    private UILabel labelTip;

   // private float xOffset = 45f;


	void Awake () {
		Instance = this;
        startX = m_ProGressbar.foregroundWidget.transform.localPosition.x;// +xOffset;
        objEffect = m_ProGressbar.transform.Find("load_flash").gameObject;
        labelTip = transform.Find("ShowTip/ShowTip_001/TipLabel/randLable1").GetComponent<UILabel>();
	}

    int Progress;
	// Use this for initialization
	void Start () {
        Progress = 0;
        isEnd = false;
	}

    const int MaxProgressPerc = 95;
	
	// Update is called once per frame
	void Update () 
    {
        if (async == null)
        {
            return;
        }
        //进度条设置
        if (m_ProGressbar != null)
        {
            int pg = (int)m_ProGressbar.value;//进度条條的值 
            if (Progress < MaxProgressPerc)//如果进度条的值小于最大值95，进度条的值自身加10
            {
                Progress += 10;
            }
            if (pg > MaxProgressPerc)//如果pg的值大于最大值，
            {
                Progress = pg;
            }
            m_ProGressbar.value = Progress / 100.0f; // async.progress;
            SetEffectPos();//进度条位置设置
        }

        //加載場景結束
        if(async.Complete && !isEnd)
        {
            if (OnLoadFinish != null) 
            {
                OnLoadFinish(GState);//设置状态GState指的是
                isEnd = true;
                //进入场景后预加载特效
                ReadPreLoadConfig.Instance.PreLoad();
            }	
            if(isCloseBySelf)
                CloseLoading();//销毁此对象
        }
    }

    public void ReleaseResource()
    {

        ReadReleaseResourceConfig.Instance.ForceReleaseResource();
     
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

	public void CloseLoading(){
		DestroyImmediate(this.gameObject);
	}
	void OnEnable(){
		if (m_ProGressbar != null) {
			m_ProGressbar.value = 0.0f;	 
            SetEffectPos();
		}

        int[] idArray = GameMethod.GetRandIntArrayFromList(LoadingTipData.GetIdList(), 1);
        string tip = ConfigReader.GetLoadingTipInfo(idArray[0]).Tip;
        labelTip.text = tip;
	}

	void OnDisable(){

	}

	void OnDestroy(){
		if (Instance != null) {
			Instance = null;		
		}
	}
   
	public void LoadAsignedSene(string name)
    {  
        Thanos.Effect.EffectManager.Instance.DestroyAllEffect(); 
        ObjectPool.Instance.Clear();
        async = ResourcesManager.Instance.loadLevel(name, null);
	}

    

    void SetEffectPos()
    {
        float value = m_ProGressbar.value;

        Vector3 pos = new Vector3(value * m_ProGressbar.foregroundWidget.width + startX, m_ProGressbar.foregroundWidget.transform.localPosition.y, m_ProGressbar.foregroundWidget.transform.localPosition.z);
     
        objEffect.transform.localPosition = pos;
    }



}
