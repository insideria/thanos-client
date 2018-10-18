using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadStateTest : MonoBehaviour {
    private UISprite hpSprite;
    private GameObject BloodBar;
    float progressValue = 100f;
    float deadTime = 0;
    bool isDead = false;
    void Start()
    {
        hpSprite = GameObject.Find("Control_Hp/Foreground").GetComponent<UISprite>();//获取控制血条fillAmount值的对象
        BloodBar = GameObject.Find("HeroLifePlateGreen");
    }

    void Update()
    {
        progressValue -= 0.1f;
        hpSprite.fillAmount = progressValue / 100.0f;
        if (hpSprite.fillAmount == 0)
        {
            //获取当前英雄播放器
            Animation ani = this.gameObject.GetComponent<Animation>();//由于脚本挂载到了英雄身上，所以我们可以通过this关键字获取到当前英雄
            //播放死亡动画
            deadTime += Time.deltaTime;
            if (deadTime<5f) {
                if (!isDead)
                {
                    isDead = true;
                    ani.Play("death");
                }
                return;
            }
            deadTime = 0;
            isDead = false;

            //隐藏血条
            BloodBar.SetActive(false);
            this.gameObject.transform.position = new Vector3(-32f, 0, -67f);
            BloodBar.SetActive(true);
            ani.Play("free");
            hpSprite.fillAmount = 1;
            progressValue = 100f;
        }
    }
}
