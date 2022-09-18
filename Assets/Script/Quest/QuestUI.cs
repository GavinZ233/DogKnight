using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuestUI : Singleton<QuestUI>
{
    [Header("Elements")]
    public GameObject questPanel;
    public ItemTooltip tooltip;
    [Header("Quest Name")]
    public RectTransform questListTransform;
    public QuestNameButton questNameButton;
    [Header("Text Content")]
    public Text questContentText;
    [Header("Requirement")]
    public RectTransform requireTransform;
    public QuestRequirement requirement;
    [Header("Rewards")]
    public RectTransform rewardTransform;
    public ItemUI rewardUI;

    bool questIsOpen = false;
    public bool QuestIsOpen { get { return questIsOpen; } }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            questIsOpen = !questIsOpen;
            questContentText.text = string.Empty;
            //显示面板内容
            SetupQuestList();

        }
        questPanel.SetActive(questIsOpen);
        if (!questIsOpen)
        {
            tooltip.gameObject.SetActive(false);
        }


    }

    public void SetupQuestList()//删除现有，重新生成新的
    {
        foreach (Transform item in questListTransform)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in rewardTransform)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (var task in QuestManager.Instance.tasks)
        {
            var newTask = Instantiate(questNameButton, questListTransform);
            newTask.SetupNameButton(task.questData);
        }
    }

    public void SetupRequireList(QuestData_SO questData)
    {
        questContentText.text = questData.description;
        foreach (Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }
        foreach (var require in questData.questRequires)
        {
            var q = Instantiate(requirement, requireTransform);
            if (questData.isFinished)
            {
                q.SetupRequirement(require.name, questData.isFinished);
            }
            else
            q.SetupRequirement(require.name, require.requireAmount, require.currentAmount);
        }
    }

    public void SetupRewardItem(ItemData_SO itemData,int amount)
    {
        var item = Instantiate(rewardUI, rewardTransform);
        item.SetupItemUI(itemData, amount);
    }

    public void CloseQuestUI()
    {
        questIsOpen = false;
    }
}
