using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where  T:Singleton<T>
{
    private static T instance;
        
    public static T Instance
    {
        get { return instance; }
    }

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else instance = (T)this;
    }
    /// <summary>
    /// 是否启动
    /// </summary>
    public static bool IsInitialized
    {
        get { return instance != null; }
    }

    protected virtual void OnDestroy()
    {//删除物体时也注销掉静态变量，防止报错
        if (instance==this)
        {
            instance = null;
        }
    }
}
