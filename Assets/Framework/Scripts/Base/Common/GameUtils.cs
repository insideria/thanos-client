using UnityEngine;
using System;
using Thanos.GameData;

namespace Thanos
{
    class GameUtils
    {
        private static float s_fMaxAzimuth = 360;
        private static float s_fMaxZenith = 180;
        private static float s_fPIDegree = 180;
        private static DateTime s_BaseTime = new DateTime(1970, 1, 1, 0, 0, 0);

        public static float MaxAzimuth
        {
            get { return s_fMaxAzimuth; }
        }
        public static float MaxZenith
        {
            get { return s_fMaxZenith; }
        }
        public static float PIDegree
        {
            get { return s_fPIDegree; }
        }

        public static void CheckMarkMaxIndex(UInt64 sGUID)
        {
        }

        public static ObjectTypeEnum GetGUIDType(UInt64 sGUID)
        {
            return (ObjectTypeEnum)(sGUID >> 48);
        }

        public static void ClearCharArray(char[] aStr, UInt32 un32Size)
        {
            for (UInt32 i = 0; i < un32Size; i++)
            {
                aStr[i] = '\0';
            }
        }

        public static Boolean IfTypeNPC(ObjectTypeEnum eOT)
        {
            if ((Int32)ObjectTypeEnum.NPCBegin < (Int32)eOT && (Int32)eOT < (Int32)ObjectTypeEnum.NPCBegin + (Int32)ConstEnum.Level1Inter)
            {
                return true;
            }
            return false;
        }

        public static Boolean IfTypeHero(ObjectTypeEnum eOT)
        {
            if ((Int32)ObjectTypeEnum.HeroBegin < (Int32)eOT && (Int32)eOT < (Int32)ObjectTypeEnum.HeroBegin + (Int32)ConstEnum.Level1Inter)
            {
                return true;
            }
            return false;
        }

        public static Int64 GetClientUTCSec()
        {
            TimeSpan sTimeSpan = DateTime.UtcNow - s_BaseTime;
            return (Int64)sTimeSpan.TotalSeconds;
        }

        public static Int64 GetClientLocalSec()
        {
            TimeSpan sTimeSpan = DateTime.Now - s_BaseTime;
            return (Int64)sTimeSpan.TotalSeconds;
        }

        public static Int64 GetClientUTCMillisec()
        {
            TimeSpan sTimeSpan = DateTime.UtcNow - s_BaseTime;
            return (Int64)sTimeSpan.TotalMilliseconds;
        }

        public static Int64 GetClientLocalMillisec()
        {
            TimeSpan sTimeSpan = DateTime.Now - s_BaseTime;
            return (Int64)sTimeSpan.TotalMilliseconds;
        }

        public static float GetApproxDist(Vector3 sFromPos, Vector3 sToPos)
        {
            float fApproximatelyDist = 0;
            Vector3 sTempDist = sToPos - sFromPos;
            fApproximatelyDist += Mathf.Abs(sTempDist.x) + Mathf.Abs(sTempDist.y) + Mathf.Abs(sTempDist.z);
            return fApproximatelyDist;
        }

        public static float GetApproxDistXAndZ(Vector3 sFromPos, Vector3 sToPos)
        {
            float fApproximatelyDist = 0;
            Vector3 sTempDist = sToPos - sFromPos;
            fApproximatelyDist += Mathf.Abs(sTempDist.x) + Mathf.Abs(sTempDist.z);
            return fApproximatelyDist;
        }

        //字符串是否为有效的名字（只包含中文，字母或数字)
        public static bool CheckName(string text)
        {
            char ch;
            for (int i = 0; i < text.Length; i++)
            {
                ch = text[i];
                if (!((ch >= 0x4e00 && ch <= 0x9fbb) || (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9')))
                {
                    return false;
                }
            }

            return true;
        }

        public static string ShowCount(int time)
        {
            string timeString = "";
            if (time < 60)
                timeString = "00:" + SecondToStr(time);
            else
                timeString = SecondToStr(time / 60) + ":" + SecondToStr(time % 60);
            return timeString;
        }

        public static string SecondToStr(int time)
        {
            string timeString = "";
            if (time < 10)
                timeString = "0" + time.ToString();
            else
                timeString = "" + time.ToString();

            return timeString;
        }

        public static string GetUUID() {
            var uuid = Guid.NewGuid().ToString();
            return uuid;
        }
    }
}