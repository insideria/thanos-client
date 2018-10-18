using Thanos.Network;
using UnityEngine;

public class test2 : HolyTechGameBase
{

	// Use this for initialization
	void Start () {
        NetworkManager.Instance.Init("127.0.0.1", 49998, NetworkManager.ServerType.BalanceServer, true);
	}
	
	// Update is called once per frame
	void Update () {
        NetworkManager.Instance.Update(Time.deltaTime);		
	}
}
