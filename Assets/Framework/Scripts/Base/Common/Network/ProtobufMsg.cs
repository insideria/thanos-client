using System;
using System.IO;
using UnityEngine;

namespace Thanos.Network
{
    public class ProtobufMsg
    {
        //封装proto解析
        public static bool MessageDecode<T>(out T pMsg, Stream stream)
        {
            try
            {
                pMsg = ProtoBuf.Serializer.Deserialize<T>(stream);
                if (null == pMsg)
                {
                    Debug.LogError("Proto解析为Null");
                    pMsg = default(T);
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                Debug.LogError("Proto解析异常");
                pMsg = default(T);
                return false;
            }
        }

        //封装proto解析
        public static T MessageDecode<T>(Stream stream)
        {
            try
            {
                T pMsg = ProtoBuf.Serializer.Deserialize<T>(stream);
                if (null == pMsg)
                {
                    Debug.LogError("Proto解析为Null");
                    return default(T);
                }

                return pMsg;
            }
            catch (Exception)
            {
                Debug.LogError("Proto解析异常");
                return default(T);
            }
        }
    }
}