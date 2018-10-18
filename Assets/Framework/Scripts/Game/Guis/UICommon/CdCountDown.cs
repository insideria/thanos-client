using UnityEngine;
using System;

public class CdCountDown : MonoBehaviour
{
    public UISprite cdSlider;

    private float timeLimit;

    private bool canCountCd = false;

    private DateTime StartCountTime;

    public System.Action<CdCountDown> CdCountDownEvent;

    float start = 0f;

    void Start()
    {
    
        SetCdShow(false);
    }

    void Update()
    {
        DoCdCount();
    }

    public void SetCdShow(bool visiable)
    {
        cdSlider.fillAmount = 1;
        if (cdSlider.gameObject.activeInHierarchy != visiable)
        {
            cdSlider.gameObject.SetActive(visiable);
        }
    }

    public void StartCdCountDown(float time, float last)
    {
        if (canCountCd)
            return;
        timeLimit = time;

        start = time - last;

        SetCdShow(true);
        StartCountTime = DateTime.Now;
        canCountCd = true;
    }
    public void EndCdCountDown()
    {
        if (cdSlider.gameObject.activeInHierarchy)
        {
            cdSlider.gameObject.SetActive(false);
        }
        cdSlider.fillAmount = 0;
        canCountCd = false;
        if (CdCountDownEvent != null)
            CdCountDownEvent(this);
    }

    void DoCdCount()
    {
        if (!canCountCd)
            return;
        TimeSpan endtime = DateTime.Now - StartCountTime; 
        if (endtime.TotalSeconds >= (timeLimit - start))
        {
            EndCdCountDown();
            return;
        }

        cdSlider.fillAmount = 1f - (start / timeLimit) - ((float)endtime.TotalSeconds / timeLimit);
    }
}

