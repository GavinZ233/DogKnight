using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPool : Singleton<RockPool>
{
    public List<GameObject> rockPool;
    public GameObject rockPrefab;
    [SerializeField] float rockCount;
    void Start()
    {
        rockPool = new List<GameObject>();
        for (int i = 0; i < rockCount; i++)
        {
            GameObject obj = Instantiate(rockPrefab);
            obj.SetActive(false);
            rockPool.Add(obj);
            obj.transform.SetParent(this.transform);
        }
    }

    public GameObject GetRockPool()
    {
        for (int i = 0; i < rockPool.Count; i++)
        {
            if (!rockPool[i].activeInHierarchy)//判断是否在场景中
            {
                return rockPool[i];
            }
        }
        return null;
    }


}
