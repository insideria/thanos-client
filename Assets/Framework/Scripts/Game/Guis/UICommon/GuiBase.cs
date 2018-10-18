using UnityEngine;

public class GuiBase : MonoBehaviour {

    public AudioClip Sound;
    protected virtual void OnPress(bool pressed)
    {
        if (!pressed || Sound == null) 
        {
            return;
        }
        //AudioManager.Instance.PlayEffectAudio(Sound);

        NGUITools.PlaySound(Sound);
    }

    public delegate void HandleOnPress(int ie, bool pressed);
    public HandleOnPress Handler;

    public void AddListener(int ie, HandleOnPress handler)
    {
        PrIe = ie;
        Handler += handler;
    }

    public void AddListener(HandleOnPress handler)
    {
        Handler += handler;
    }

    public void RemoveListener(HandleOnPress handler)
    {
        Handler -= handler;
    }

    public int PrIe
    {
        get;
        protected set;
    }
}
