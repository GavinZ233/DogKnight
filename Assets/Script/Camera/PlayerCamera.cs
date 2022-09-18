using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("相机坐标")]
    [SerializeField] private float cameraTransX;
    [SerializeField] private float cameraTransY;

    public float Pitch { get; private set; }   //抬升角度
    public float Yaw { get; private set; }     //侧面角度
    [Header("视角灵敏度")]
    public float mouseSensitivity=5;   //鼠标灵敏度
    public float cameraRotatingSpeed=20;//手柄灵敏度
    public float bufferYSpeed=5;
    private float mouseY; //限制鼠标Y轴区域
    [SerializeField] private AnimationCurve armLengthCurvel;
    private Transform followTarget;
    private Transform followCamera;
   // private Transform lookAtPoint;
    private float cameraOrigZ;

    //private float oldYaw;
    //private float oldPitch;
    private void Awake()
    {
        followCamera = transform.GetChild(0);
        followCamera.position = new Vector3(cameraTransX,cameraTransY,-6);


        cameraOrigZ = transform.position.z;
    }
    public void InitCamera(Transform target)
    {
        followTarget = target;
        transform.position = target.position;
        //lookAtPoint = lookatPoint;
    }

    //相机如果不放在lateupdate会造成相机与玩家同步加载，过快寻找玩家信息，导致报空
    private void LateUpdate()
    {
        UpdateRotation();
        UpdatePositionY();
        UpdateArmLengh();
        NoThroughWall();


    }

    /// <summary>
    /// 更新视角旋转
    /// </summary>
    private void UpdateRotation()
    {
        if (MouseManager.Instance.showMouse) return;

        Yaw += Input.GetAxis("Mouse X")* mouseSensitivity;
        Yaw += Input.GetAxis("Camera Rate X") * cameraRotatingSpeed * Time.deltaTime;
        mouseY += Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY += Input.GetAxis("Camera Rate Y")*cameraRotatingSpeed * Time.deltaTime;
        Pitch = Mathf.Clamp(mouseY, 10f, 90f);  //限制Y轴范围，防止摄像机过高过低产生穿模或者反转
        transform.rotation = Quaternion.Euler(Pitch, Yaw, 0);
    }
    /// <summary>
    /// Y轴缓慢跟随效果
    /// </summary>
    private void UpdatePositionY()
    {
        
        //延迟Y轴的位移，产生缓动效果
        Vector3 position = followTarget.position;
        float newY = Mathf.Lerp(transform.position.y, position.y, Time.deltaTime * bufferYSpeed);
        transform.position = new Vector3(position.x, newY, position.z);
    }
    /// <summary>
    /// 更新摄像机的Z轴，即摄像机的臂长
    /// </summary>
    private void UpdateArmLengh()
    {
        float arm = Mathf.Clamp(armLengthCurvel.Evaluate(Pitch) , 3, 14);
        followCamera.localPosition=new Vector3(0,0,arm * -1);
    }

    private void NoThroughWall()
    {
        RaycastHit hit;
        if (Physics.Linecast(transform.position, followCamera.position, out hit))
        {
            string tag = hit.collider.gameObject.tag;
            if (tag != "MainCamera"&& tag != "Enemy" && tag != "Player")
            {
                float currentDistance = Vector3.Distance(hit.point, transform.position);
                if (currentDistance < cameraOrigZ)
                {
                    followCamera.position = hit.point ;
                }
            }
        }

    }

    public void Shake()
    {
        StartCoroutine(ShakeCamera(0.1f,0.1f));
    }

    IEnumerator ShakeCamera(float time,float strength)
    {
        // 保存原本的位置
        Vector3 orignalPosition = transform.localPosition;
        float currTime = 0f;
        // 如果时间小于外面指定的
        while (currTime < time)
        {
            // 随机x和y轴的偏移量
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;
            transform.localPosition = orignalPosition + new Vector3(x, y, 0);
            // 让时间增加
            currTime += Time.deltaTime;
            // 延迟一帧
            yield return null;
        }

        // 震动结束，复位
        transform.localPosition = orignalPosition;
    }


    //TODO：实现摄像机自动归位功能，即默认时间没有键盘与鼠标操作后，将摄像机的恢复到默认的transform。
    //判断条件应该改成无任何键鼠操作时归为
    //private bool NoMoveMouse()
    //{
    //    if (Yaw == oldYaw && Pitch == oldPitch)
    //    {
    //        return true;

    //    }
    //    oldYaw = Yaw;
    //    oldPitch = Pitch;
    //    return false;
    //}
    //private void BackCamera()
    //{
    //    Debug.Log(Yaw + Pitch);
    //    if (NoMoveMouse())
    //    {

    //        StartCoroutine("BackCameraOnTik");
    //    }
    //}


}
