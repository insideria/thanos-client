using UnityEngine;

namespace UICommon
{
	public static class UICommonMethod
	{		 
		public static void TweenPositionBegin(GameObject go, float duration, Vector3 v3,UITweener.Style state){
			UITweener uit = TweenPosition.Begin (go, duration, v3);
			uit.style = state;
		}

		public static void TweenRotationBegin(GameObject go, float duration, Vector3 v3,UITweener.Style state){
			UITweener uit = TweenRotation.Begin(go,duration,Quaternion.Euler(v3));
			uit.style = state;
		}

		public static void TweenScaleBegin(GameObject go, float duration, Vector3 v3,UITweener.Style state){
			UITweener uit = TweenScale.Begin(go,duration,v3);
			uit.style = state;
		}

		public static void TweenColorBegin(GameObject go, float duration, float a){
			UIWidget[] mWidgets = go.GetComponentsInChildren<UIWidget>();
			foreach(UIWidget mWidget in mWidgets){
				TweenColor.Begin(
					mWidget.gameObject,
					duration,
					new Color(mWidget.color.r,mWidget.color.g,mWidget.color.b,a));
			} 
		}

        public static UITweener TweenAlphaBegin(GameObject go, float duration, float a)
        {
            UIWidget[] mWidgets = go.GetComponentsInChildren<UIWidget>();
            UITweener tween = null;
            foreach (UIWidget mWidget in mWidgets)
            {
                UITweener tween2 = TweenAlpha.Begin(mWidget.gameObject,duration, a);
                if (tween == null) {
                    tween = tween2;
                }
            }
            return tween;
        }
		public static Vector3 GetWorldPos(Vector2 screenPos){
			Vector3 pos = Vector3.zero;
            Camera camera = GameDefine.GameMethod.GetUiCamera;
            if (GameDefine.GameMethod.GetUiCamera != null)
            {
				pos = camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -camera.gameObject.transform.position.z));
			}
			
			return pos;
		}

        public static Vector3 GetScreenPos(Vector3 pos)
        {
            Camera camera = GameDefine.GameMethod.GetUiCamera;
            Vector3 posNew = new Vector3();
            if (camera != null)
            {
                posNew = camera.WorldToScreenPoint(pos);
            }

            return posNew;
        }

		public static void TweenAlphaBegin(GameObject obj,float duration,float alpha,UITweener.Style state){
			UITweener uit = TweenAlpha.Begin(obj,duration,alpha);
			uit.style = state;
		}
	}
}
