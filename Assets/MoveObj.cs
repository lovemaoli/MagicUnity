using UnityEngine;
using System.Collections;

public class MoveObj : MonoBehaviour {

    float speed = 10.0f;   //移动速度
    float rotationSpeed = 100.0f;  //旋转速度
    // Update is called once per frame
    void Update()
    {
        // 使用上下方向键或者W、S键来控制前进后退
        float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        //使用左右方向键或者A、D键来控制左右旋转
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        transform.Translate(0, 0, translation); //沿着Z轴移动
        transform.Rotate(0, rotation, 0); //绕Y轴旋转
    }

}