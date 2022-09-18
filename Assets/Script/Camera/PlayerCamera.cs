using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("�������")]
    [SerializeField] private float cameraTransX;
    [SerializeField] private float cameraTransY;

    public float Pitch { get; private set; }   //̧���Ƕ�
    public float Yaw { get; private set; }     //����Ƕ�
    [Header("�ӽ�������")]
    public float mouseSensitivity=5;   //���������
    public float cameraRotatingSpeed=20;//�ֱ�������
    public float bufferYSpeed=5;
    private float mouseY; //�������Y������
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

    //������������lateupdate�������������ͬ�����أ�����Ѱ�������Ϣ�����±���
    private void LateUpdate()
    {
        UpdateRotation();
        UpdatePositionY();
        UpdateArmLengh();
        NoThroughWall();


    }

    /// <summary>
    /// �����ӽ���ת
    /// </summary>
    private void UpdateRotation()
    {
        if (MouseManager.Instance.showMouse) return;

        Yaw += Input.GetAxis("Mouse X")* mouseSensitivity;
        Yaw += Input.GetAxis("Camera Rate X") * cameraRotatingSpeed * Time.deltaTime;
        mouseY += Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY += Input.GetAxis("Camera Rate Y")*cameraRotatingSpeed * Time.deltaTime;
        Pitch = Mathf.Clamp(mouseY, 10f, 90f);  //����Y�᷶Χ����ֹ��������߹��Ͳ�����ģ���߷�ת
        transform.rotation = Quaternion.Euler(Pitch, Yaw, 0);
    }
    /// <summary>
    /// Y�Ỻ������Ч��
    /// </summary>
    private void UpdatePositionY()
    {
        
        //�ӳ�Y���λ�ƣ���������Ч��
        Vector3 position = followTarget.position;
        float newY = Mathf.Lerp(transform.position.y, position.y, Time.deltaTime * bufferYSpeed);
        transform.position = new Vector3(position.x, newY, position.z);
    }
    /// <summary>
    /// �����������Z�ᣬ��������ı۳�
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
        // ����ԭ����λ��
        Vector3 orignalPosition = transform.localPosition;
        float currTime = 0f;
        // ���ʱ��С������ָ����
        while (currTime < time)
        {
            // ���x��y���ƫ����
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;
            transform.localPosition = orignalPosition + new Vector3(x, y, 0);
            // ��ʱ������
            currTime += Time.deltaTime;
            // �ӳ�һ֡
            yield return null;
        }

        // �𶯽�������λ
        transform.localPosition = orignalPosition;
    }


    //TODO��ʵ��������Զ���λ���ܣ���Ĭ��ʱ��û�м������������󣬽�������Ļָ���Ĭ�ϵ�transform��
    //�ж�����Ӧ�øĳ����κμ������ʱ��Ϊ
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
