using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroRotation : MonoBehaviour
{
    private bool onDrag = false;  //是否被拖拽//    
    private float axisX = 1;    //鼠标沿竖直方向移动的增量//   
    void OnMouseDrag()     //鼠标拖拽时的操作// 
    {
        onDrag = true;
        //获得鼠标增量// 
        axisX = -Input.GetAxis("Mouse X");

    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            OnMouseDrag();
            onDrag = false;
        
            this.transform.Rotate(new Vector3(0, axisX, 0) * 4f, Space.World);
        }
    }
}
