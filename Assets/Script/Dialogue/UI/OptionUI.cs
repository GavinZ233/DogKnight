using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OptionUI : MonoBehaviour
{
    public Text optionText;
    private Button thisButton;
    private DialoguePiece currentPiece;

    private string nextPieceID;

    private bool takeQuest;

    private void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OnOptionClicked);
    }
    public void UpdateOption(DialoguePiece piece, DialogueOption option)
    {
        currentPiece = piece;
        optionText.text = option.text;
        nextPieceID = option.targetID;
        takeQuest = option.takeQuest;
    }

    public void OnOptionClicked()
    {
        //是否有任务且option勾选takeQuest
        if (currentPiece.quest != null)
        {
            //var newTask = new QuestManager.QuestTask
            //{
            //    questData = Instantiate(currentPiece.quest)
            //};
            var newTask = new QuestManager.QuestTask();
            newTask.questData = Instantiate(currentPiece.quest);
            if (takeQuest)
            {
                //添加到任务列表
                //判断你是否已经有任务
                if (QuestManager.Instance.HaveQuest(newTask.questData))
                {
                    //判断你是否完成基于奖励
                    if (QuestManager.Instance.GetTask(newTask.questData).IsComplete)
                    {
                        newTask.questData.GiveRewards();
                        QuestManager.Instance.GetTask(newTask.questData).IsFinished = true;
                    }
                }
                else
                {
                    QuestManager.Instance.tasks.Add(newTask);
                    QuestManager.Instance.GetTask(newTask.questData).IsStarted = true;

                    foreach (var requireItem in newTask.questData.RequireTargetName())
                    {
                        InventoryManager.Instance.CheckQuestItemInBag(requireItem);
                    }
                }
            }
        }

        if (nextPieceID == "")
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
            return;
        }
        else
        {
            DialogueUI.Instance.UpdateMainDialogue(DialogueUI.Instance.currentData.dialogueIndex[nextPieceID]);
        }
    }

}
