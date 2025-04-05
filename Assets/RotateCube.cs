using System.Collections;
using UnityEngine;

public class RotateCube : MonoBehaviour
{
    // 判断是否按下鼠标
    bool isMousePressed;
    // 记录上次鼠标位置
    Vector3 lastMousePos;
    // 旋转速度
    public float rotationSpeedX = 20.0f; 
    public float rotationSpeedY = 20.0f; 
    // 用于存储X轴的旋转
    private float rotationX = 0f; 
    // 用于存储Y轴的旋转
    private float rotationY = 0f; 
    // 旋转限制角度
    private float rotationLimit = 45.0f; 
    // 回弹速度
    public float bouncebackSpeed = 1f;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // 计算鼠标位置改变
            Vector3 delta = Input.mousePosition - lastMousePos;
            // 添加到Y轴旋转并改变方向
            rotationY -= Input.GetAxisRaw("Mouse X")* rotationSpeedX * Time.deltaTime;
            // 添加到X轴旋转并改变方向
            rotationX -= Input.GetAxisRaw("Mouse Y")* rotationSpeedY * Time.deltaTime;
            // 使得Y轴旋转和X轴旋转在限制范围内
            rotationY = Mathf.Clamp(rotationY, -15, 15);
            rotationX = Mathf.Clamp(rotationX, -10, 10);
        }
        else
        {
            rotationY = 0;
            rotationX = 0;
        }
        transform.rotation =Quaternion.Slerp(transform.rotation,Quaternion.Euler(new Vector3(rotationX, rotationY, 0f)),Time.deltaTime*10) ;
    }
}