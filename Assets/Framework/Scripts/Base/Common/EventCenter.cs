/*
 * Advanced C# messenger by Ilya Suzdalnitski. V1.0
 * 
 * Based on Rod Hyde's "CSharpMessenger" and Magnus Wolffelt's "CSharpMessenger Extended".
 * 
 * Features:
 	* Prevents a MissingReferenceException because of a reference to a destroyed message handler.
 	* Option to log all messages
 	* Extensive error detection, preventing silent bugs
 * 
 * Usage examples:
 	1. Messenger.AddListener<GameObject>("prop collected", PropCollected);
 	   Messenger.Broadcast<GameObject>("prop collected", prop);
 	2. Messenger.AddListener<float>("speed changed", SpeedChanged);
 	   Messenger.Broadcast<float>("speed changed", 0.5f);
 * 
 * Messenger cleans up its evenTable automatically upon loading of a new level.
 * 
 * Don't forget that the messages that should survive the cleanup, should be marked with Messenger.MarkAsPermanent(string)
 * 
 */
 
//#define LOG_ALL_MESSAGES
//#define LOG_ADD_LISTENER
//#define LOG_BROADCAST_MESSAGE
#define REQUIRE_LISTENER
 
using System;
using System.Collections.Generic;
using UnityEngine;
 
//消息中心
static internal class EventCenter {
 
	//Disable the unused variable warning
#pragma warning disable 0414
	//Ensures that the MessengerHelper will be created automatically upon start of the game.
//	static private MessengerHelper mMessengerHelper = ( new GameObject("MessengerHelper") ).AddComponent< MessengerHelper >();
#pragma warning restore 0414
 
    //存储所有的事件（类型  ，委托事件）
    static public  Dictionary<Int32, Delegate> mEventTable = new Dictionary<Int32, Delegate>();
	//Message handlers that should never be removed, regardless of calling Cleanup
    static public List<Int32> mPermanentMessages = new List<Int32> ();
	
	//Marks a certain message as permanent.标记永久性消息
    static public void MarkAsPermanent(Int32 eventType) {
#if LOG_ALL_MESSAGES
		Debug.Log("Messenger MarkAsPermanent \t\"" + eventType + "\"");
#endif
 
		mPermanentMessages.Add( eventType );
	}
 
 
	static public void Cleanup()
	{
#if LOG_ALL_MESSAGES
		Debug.Log("MESSENGER Cleanup. Make sure that none of necessary listeners are removed.");
#endif
 
        List< Int32 > messagesToRemove = new List<Int32>();
 
        foreach (KeyValuePair<Int32, Delegate> pair in mEventTable) {
			bool wasFound = false;
 
            foreach (Int32 message in mPermanentMessages) {
				if (pair.Key == message) {
					wasFound = true;
					break;
				}
			}
 
			if (!wasFound)
				messagesToRemove.Add( pair.Key );
		}
 
        foreach (Int32 message in messagesToRemove) {
			mEventTable.Remove( message );
		}
	}
 
	static public void PrGameEventEnumEventTable()
	{
		Debug.Log("\t\t\t=== MESSENGER PrGameEventEnumEventTable ===");
 
        foreach (KeyValuePair<Int32, Delegate> pair in mEventTable) {
			Debug.Log("\t\t\t" + pair.Key + "\t\t" + pair.Value);
		}
 
		Debug.Log("\n");
	}
 
    //当有事件添加的的时候，检测这个事件有没有注册过这个方法。
    static public void OnListenerAdding(Int32 eventType, Delegate listenerBeingAdded) {

       // 如果这个表中不包含这个类型，那么就添加这个
        if (!mEventTable.ContainsKey(eventType)) {
            mEventTable.Add(eventType, null );
        }
        //定义委托
        Delegate d = mEventTable[eventType];

        if (d != null && d.GetType() != listenerBeingAdded.GetType()) {
            throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
        }
    }
 
    static public void OnListenerRemoving(Int32 eventType, Delegate listenerBeingRemoved) {
#if LOG_ALL_MESSAGES
		Debug.Log("MESSENGER OnListenerRemoving \t\"" + eventType + "\"\t{" + listenerBeingRemoved.Target + " -> " + listenerBeingRemoved.Method + "}");
#endif
 
        if (mEventTable.ContainsKey(eventType)) {
            Delegate d = mEventTable[eventType];
 
            if (d == null) {
                throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
            } else if (d.GetType() != listenerBeingRemoved.GetType()) {
                throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
            }
        } else {
            throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
        }
    }

    internal static void Broadcast(object userEvent_NetMessage_NotifyMatchTeamSwitch)
    {
        throw new NotImplementedException();
    }

    //移除了事件后，将事件的类型也移除掉
    static public void OnListenerRemoved(Int32 eventType) {
       
        if (mEventTable[eventType] == null) {
            mEventTable.Remove(eventType);
        }
    }
 
    static public void OnBroadcasting(Int32 eventType) {
#if REQUIRE_LISTENER
        if (!mEventTable.ContainsKey(eventType)) {
        }
#endif
    }
 
    static public BroadcastException CreateBroadcastSignatureException(Int32 eventType) {
        return new BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
    }
 
    public class BroadcastException : Exception {
        public BroadcastException(string msg)
            : base(msg) {
        }
    }
 
    public class ListenerException : Exception {
        public ListenerException(string msg)
            : base(msg) {
        }
    }

    //No parameters  这个方法是重写的方法
    static public void AddListener(Int32 eventType, Action handler) {
        OnListenerAdding(eventType, handler); // 当有事件添加的的时候，检测这个事件有没有注册过这个方法。
        mEventTable[eventType] = (Action)mEventTable[eventType] + handler;
    }
 
	//Single parameter
    static public void AddListener<T>(Int32 eventType, Action<T> handler) {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Action<T>)mEventTable[eventType] + handler;
    }
 
	//Two parameters
    static public void AddListener<T, U>(Int32 eventType, Action<T, U> handler) {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Action<T, U>)mEventTable[eventType] + handler;
    }
 
	//Three parameters
    static public void AddListener<T, U, V>(Int32 eventType, Action<T, U, V> handler) {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Action<T, U, V>)mEventTable[eventType] + handler;
    }
	
	//Four parameters
    static public void AddListener<T, U, V, X>(Int32 eventType, Action<T, U, V, X> handler) {
        OnListenerAdding(eventType, handler);
        mEventTable[eventType] = (Action<T, U, V, X>)mEventTable[eventType] + handler;
    }
 
	//No parameters
    static public void RemoveListener(Int32 eventType, Action handler) {
        OnListenerRemoving(eventType, handler);   //检测此消息是否绑定了此事件
        mEventTable[eventType] = (Action)mEventTable[eventType] - handler;//移除事件
        OnListenerRemoved(eventType);    //移除事件后，将消息也移除掉
    }
 
	//Single parameter
    static public void RemoveListener<T>(Int32 eventType, Action<T> handler) {
        OnListenerRemoving(eventType, handler);
        mEventTable[eventType] = (Action<T>)mEventTable[eventType] - handler;
        OnListenerRemoved(eventType);
    }
 
	//Two parameters
    static public void RemoveListener<T, U>(Int32 eventType, Action<T, U> handler) {
        OnListenerRemoving(eventType, handler);
        mEventTable[eventType] = (Action<T, U>)mEventTable[eventType] - handler;
        OnListenerRemoved(eventType);
    }
 
	//Three parameters
    static public void RemoveListener<T, U, V>(Int32 eventType, Action<T, U, V> handler) {
        OnListenerRemoving(eventType, handler);
        mEventTable[eventType] = (Action<T, U, V>)mEventTable[eventType] - handler;
        OnListenerRemoved(eventType);
    }
	
	//Four parameters
    static public void RemoveListener<T, U, V, X>(Int32 eventType, Action<T, U, V, X> handler) {
        OnListenerRemoving(eventType, handler);
        mEventTable[eventType] = (Action<T, U, V, X>)mEventTable[eventType] - handler;
        OnListenerRemoved(eventType);
    }

  
    
   //广播事件
    static public void Broadcast(Int32 eventType) {
//#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
//		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
//#endif
       // OnBroadcasting(eventType);//没啥用
 
        Delegate d;//获取委托方法
        if (mEventTable.TryGetValue(eventType, out d)) {
            Action callback = d as Action;
 
            if (callback != null) {
                callback();
            } else {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }

    static public void SendEvent(Thanos.FEvent evt)
    {
        Broadcast(evt.GetEventId(), evt);
    }
 
	//Single parameter
    static public void Broadcast<T>(Int32 eventType, T arg1) {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);
 
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d)) {
            Action<T> callback = d as Action<T>;
 
            if (callback != null) {
                callback(arg1);
            } else {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
	}
 
	//Two parameters
    static public void Broadcast<T, U>(Int32 eventType, T arg1, U arg2) {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);
 
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d)) {
            Action<T, U> callback = d as Action<T, U>;
 
            if (callback != null) {
                callback(arg1, arg2);
            } else {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }
 
	//Three parameters
    static public void Broadcast<T, U, V>(Int32 eventType, T arg1, U arg2, V arg3) {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);
 
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d)) {
            Action<T, U, V> callback = d as Action<T, U, V>;
 
            if (callback != null) {
                callback(arg1, arg2, arg3);
            } else {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }
	
	//Four parameters
    static public void Broadcast<T, U, V, X>(Int32 eventType, T arg1, U arg2, V arg3, X arg4) {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);
 
        Delegate d;
        if (mEventTable.TryGetValue(eventType, out d)) {
            Action<T, U, V, X> callback = d as Action<T, U, V, X>;
 
            if (callback != null) {
                callback(arg1, arg2, arg3, arg4);
            } else {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }	
}
 /*
//This manager will ensure that the messenger's mEventTable will be cleaned up upon loading of a new level.
public sealed class MessengerHelper : MonoBehaviour {
	void Awake ()
	{
		DontDestroyOnLoad(gameObject);	
	}
 
	//Clean up mEventTable every time a new level loads.
	public void OnDisable() {
		Messenger.Cleanup();
	}
}
*/