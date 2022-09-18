using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCanvas : Singleton<LoadCanvas>
{
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
