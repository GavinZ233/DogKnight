using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType { SameScene, DifferentScene }

    public string sceneName;
    public TransitionType transitionType;
    public DestinationTag destinationTag;

    private bool canTrans;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)&&canTrans)
        {
            ScenesController.Instance.TransitionMachine(this);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = false;
        }

    }
}
