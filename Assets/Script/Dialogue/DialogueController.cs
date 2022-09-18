using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DialogueController : MonoBehaviour
{
    public DialogueData_SO currentDialogueData;
    bool canTalk = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&&currentDialogueData!=null)
        {
            canTalk = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);

            canTalk = false;

        }
    }

    private void Update()
    {
        if (canTalk&&Input.GetKeyDown(KeyCode.E))
        {
            OpenDialogue();
        }
    }

    public void OpenDialogue()
    {
        DialogueUI.Instance.UpdateDialogueData(currentDialogueData);
        DialogueUI.Instance.UpdateMainDialogue(currentDialogueData.dialoguePieces[0]);
    }
}
