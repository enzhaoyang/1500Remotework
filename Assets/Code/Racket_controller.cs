using UnityEngine;

public class Racket_controller : MonoBehaviour
{
    void Update()
    {
        // 1. 获取 Quest 3 右手柄在现实中的位置和旋转角度
        Vector3 controllerPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion controllerRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        // 2. 把真实的位置和角度，同步给游戏里的刀 (教程中提到乘以15倍为了放大动作，我们先用1倍保证原汁原味)
        transform.localPosition = controllerPos;
        transform.localRotation = controllerRot;
    }
}