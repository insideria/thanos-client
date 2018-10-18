using UnityEngine;
using System.Net.NetworkInformation;

public class MacAddressIosAgent
{
    public static string GetMacAddressByDNet()
    {
        string physicalAddress = "";

        NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface adaper in nice)
        {

            Debug.Log(adaper.Description);

            if (adaper.Description == "en0")
            {
                physicalAddress = adaper.GetPhysicalAddress().ToString();
                break;
            }
            else
            {
                physicalAddress = adaper.GetPhysicalAddress().ToString();

                if (physicalAddress != "")
                {
                    break;
                };
            }
        }

        return physicalAddress;
    }  

}