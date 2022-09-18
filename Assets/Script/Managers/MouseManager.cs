using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
/// <summary>
/// Êó±êËø¶¨ÓëÒþ²Ø
/// </summary>
public class MouseManager : Singleton<MouseManager>
{
    public bool showMouse;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);

        showMouse = false;
        HideCursor();

    }
    private void Update()
    {
        if (InventoryManager.IsInitialized||QuestUI.IsInitialized)
        {
            if (InventoryManager.Instance.StatsIsOpen || InventoryManager.Instance.BagIsOpen||QuestUI.Instance.QuestIsOpen||DialogueUI.IsInitialized && DialogueUI.Instance.dialogueIsOpen)
            {
                ShowCursor();
            }
            else
            {
                HideCursor();
            }

        }


        showMouse = Cursor.lockState == CursorLockMode.Confined;

    }
     
    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


}
