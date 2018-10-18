using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace Thanos.Resource
{
    public class LogManager : Singleton<LogManager>
    {
        Vector2 mScrollViewVector;
        Vector2 mScrollSelectVector;
        bool mDebugStringVisible;

        Dictionary<string, string> mLogDict;
        string mSelectTypeName;
        string mLogRootPath;

        public LogManager()
        {
            mScrollViewVector = Vector2.zero;
            mScrollSelectVector = Vector2.zero;
            mDebugStringVisible = true;

            mLogDict = new Dictionary<string, string>();
            mSelectTypeName = "DEFAULT";
            mLogDict.Add("DEFAULT", "");

            mLogRootPath =
#if UNITY_EDITOR
 Application.dataPath.Replace("Assets", "") +
#else
#if UNITY_STANDALONE_WIN
                Application.dataPath + "/" + 
#endif
#endif
 "Log/";
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            if (!Directory.Exists(mLogRootPath))
                Directory.CreateDirectory(mLogRootPath);
#endif
        }

        public void Show()
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            GUI.skin = GameManager.Instance.GetLogSkin();
            if (GUI.Button(new Rect((int)(Screen.width * 0.6), 0, 30, 60), "显示"))
            {
                mDebugStringVisible = !mDebugStringVisible;
            }
            if (mDebugStringVisible && GUI.Button(new Rect((int)(Screen.width * 0.6), 60, 30, 60), "清除"))
            {
                log[selectTyleName] = "";
            }
            if (mDebugStringVisible)
            {
                GUI.BeginGroup(new Rect(0, 20, (int)(Screen.width * 0.6), (int)(Screen.height * 0.6)));
                scrollViewVector = GUI.BeginScrollView(new Rect(0, 0, (int)(Screen.width * 0.6), (int)(Screen.height * 0.6)), scrollViewVector, new Rect(0, 0, (int)(Screen.width * 0.5), log[selectTyleName].Length));
                GUI.Box(new Rect(0, 0, (int)(Screen.width * 0.6), log[selectTyleName].Length * 2), log[selectTyleName]);
                GUI.EndScrollView();
                GUI.EndGroup();

                GUI.BeginGroup(new Rect((int)(Screen.width * 0.6), 120, (int)(Screen.width * 0.6) + 40, 200));
                scrollSelectVector = GUI.BeginScrollView(new Rect(0, 0, 90, 80), scrollSelectVector, new Rect(0, 0, 30, log.Count * 60));
                int index = 0;
                foreach (KeyValuePair<string, string> pair in log)
                {
                    if (GUI.Button(new Rect(0, index * 60, 30, 60), pair.Key))
                    {
                        changeShowType(pair.Key);
                    }
                    ++index;
                }
                GUI.EndScrollView();
                GUI.EndGroup();
            }

#endif
        }
        public void logMsg(string msg, string typeName = "DEFAULT")
        {
            if (!mLogDict.ContainsKey(typeName))
                mLogDict.Add(typeName, "");

            mLogDict["DEFAULT"] = msg + "\n" + mLogDict["DEFAULT"];
            if ("DEFAULT" != typeName)
                mLogDict[typeName] = msg + "\n" + mLogDict[typeName];
        }

        public void saveInfoToDisk(string msg, string typeName = "DEFAULT")
        {
            string fileName = mLogRootPath + typeName + "_" + System.DateTime.Today.ToString("yyyy_MM_dd") + ".txt";
            logMsg(fileName);
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate | FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            string info = System.DateTime.Now.ToString("hh:mm:ss") + " : " + msg + "\n";
            sw.Write(info);
            sw.Close();
            fs.Close();
        }

        public void changeShowType(string typeName = "DEFAULT")
        {
            mSelectTypeName = typeName;
        }
    }

    public static class DebugEx
    {
        public static void Log(object msg)
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            LogManager.Instance.logMsg(msg.ToString());
#endif
#if UNITY_EDITOR
            UnityEngine.Debug.Log(msg.ToString());
#endif
        }

        public static void Log(object msg, string typeName)
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            LogManager.Instance.logMsg(msg.ToString(), typeName);
#endif
#if UNITY_EDITOR
            UnityEngine.Debug.Log(msg.ToString());
#endif
        }

        public static void Log(object msg, bool saveToDisk)
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            LogManager.Instance.logMsg(msg.ToString(), "DEFAULT");
#endif
#if UNITY_EDITOR
            UnityEngine.Debug.Log(msg.ToString());
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            if (saveToDisk)
                LogManager.Instance.saveInfoToDisk(msg.ToString());
#endif
        }

        public static void Log(object msg, string typeName, bool saveToDisk)
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            LogManager.Instance.logMsg(msg.ToString(), typeName);
#endif
#if UNITY_EDITOR
            UnityEngine.Debug.Log(msg.ToString());
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            if (saveToDisk)
                LogManager.Instance.saveInfoToDisk(msg.ToString(), typeName);
#endif
        }

        public static void LogError(object msg)
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            LogManager.Instance.logMsg(msg.ToString());
#endif
#if UNITY_EDITOR
            UnityEngine.Debug.Log(msg.ToString());
#endif
        }

        public static void LogError(string msg)
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            LogManager.Instance.logMsg(msg.ToString());
#endif
#if UNITY_EDITOR
            UnityEngine.Debug.Log(msg);
#endif
        }

        public static void LogWarning(string msg)
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            LogManager.Instance.logMsg(msg.ToString());
#endif
#if UNITY_EDITOR
            UnityEngine.Debug.Log(msg);
#endif
        }

        public static void LogError(object msg, string typeName)
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            LogManager.Instance.logMsg(msg.ToString(), typeName);
#endif
#if UNITY_EDITOR
            UnityEngine.Debug.Log(msg.ToString());
#endif
        }

        public static void LogError(object msg, bool saveToDisk)
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            LogManager.Instance.logMsg(msg.ToString(), "DEFAULT");
#endif
#if UNITY_EDITOR
            UnityEngine.Debug.Log(msg.ToString());
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            if (saveToDisk)
                LogManager.Instance.saveInfoToDisk(msg.ToString());
#endif
        }

        public static void LogError(object msg, string typeName, bool saveToDisk)
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            LogManager.Instance.logMsg(msg.ToString(), typeName);
#endif
#if UNITY_EDITOR
            UnityEngine.Debug.Log(msg.ToString());
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            if (saveToDisk)
                LogManager.Instance.saveInfoToDisk(msg.ToString(), typeName);
#endif
        }

        public static void LogException(System.Exception e)
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            LogManager.Instance.logMsg(e.Message);
#endif
#if UNITY_EDITOR
            UnityEngine.Debug.Log(e.Message);
#endif
        }

        public static void LogException(System.Exception e, string typeName)
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            LogManager.Instance.logMsg(e.Message, typeName);
#endif
#if UNITY_EDITOR
            UnityEngine.Debug.Log(e.Message);
#endif
        }

        public static void LogException(System.Exception e, bool saveToDisk)
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            LogManager.Instance.logMsg(e.Message, "DEFAULT");
#endif
#if UNITY_EDITOR
            UnityEngine.Debug.Log(e.Message);
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            if (saveToDisk)
                LogManager.Instance.saveInfoToDisk(e.Message);
#endif
        }

        public static void LogException(System.Exception e, string typeName, bool saveToDisk)
        {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_STANDALONE_WIN) && SHOW_LOG
            LogManager.Instance.logMsg(e.Message, typeName);
#endif
#if UNITY_EDITOR
            UnityEngine.Debug.Log(e.Message);
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            if (saveToDisk)
                LogManager.Instance.saveInfoToDisk(e.Message, typeName);
#endif
        }
    }
}