using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }
    //最后加模板存储数据
    [Header("Inventory Data")]
    public InventoryData_SO inventoryData;
    public InventoryData_SO equipmentData;
    public InventoryData_SO actionData;

    public InventoryData_SO inventoryDataTemplate;
    public InventoryData_SO equipmentDataTemplate;
    public InventoryData_SO actionDataTemplate;


    [Header("ContainerS")]
    public ContainerUI inventoryUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    [Header("Drag Canvas")]
    public Canvas dragCanvas;
    public DragData currentDrag;

    [Header("Inventory Control")]
    public GameObject statsPanel;
    public GameObject bagPanel;
    bool statsIsOpen = false;
    bool bagIsOpen = false;
    public bool StatsIsOpen { get { return statsIsOpen; } }
    public bool BagIsOpen { get { return bagIsOpen; } }

    [Header("Stats Text")]
    public Text healthText;
    public Text attackText;
    public Text curHealthText;
    public Text defeneText;

    [Header("Tooltip")]
    public ItemTooltip tooltip;

    protected override void Awake()
    {
        base.Awake();
        if (inventoryDataTemplate!=null)
            inventoryData = Instantiate(inventoryDataTemplate);
        if (actionDataTemplate != null)
            actionData = Instantiate(actionDataTemplate);
        if (equipmentDataTemplate != null)
            equipmentData = Instantiate(equipmentDataTemplate);
    }
    private void Start()
    {
        LoadData();
        RefreshAllUI();
    }
    public void RefreshAllUI()
    {
        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            statsIsOpen = !statsIsOpen;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            bagIsOpen = !bagIsOpen;
        }
        statsPanel.SetActive(statsIsOpen);
        bagPanel.SetActive(bagIsOpen);

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log(equipmentData.items[0].itemData.itemName);
            Debug.Log(GameManager.Instance.playerState.characterData.name);
        }

    }
    private void LateUpdate()
    {
        UpdateStatsText(GameManager.Instance.playerState.MaxHealth, GameManager.Instance.playerState.CurrentHealth,
  GameManager.Instance.playerState.GetComponent<WeaponController>().attackData.minDamge,
  GameManager.Instance.playerState.GetComponent<WeaponController>().attackData.maxDamge,
  GameManager.Instance.playerState.BaseDefance+GameManager.Instance.playerState.CurArm,
  GameManager.Instance.playerState.CurrentDefance+GameManager.Instance.playerState.CurArm);


    }

    public void CloseAllInventory()
    {
        CloseBag();
        CloseState();    
    }
    public void CloseBag()
    {
        bagIsOpen = false;
    }
    public void CloseState()
    {
        statsIsOpen = false;

    }
    public  void SaveData()
    {
        SaveManager.Instance.Save(inventoryData, inventoryData.name);
        SaveManager.Instance.Save(actionData, actionData.name);
        SaveManager.Instance.Save(equipmentData, equipmentData.name);
    }
    public void LoadData()
    {
        SaveManager.Instance.Load(inventoryData, inventoryData.name);
        SaveManager.Instance.Load(actionData, actionData.name);
        SaveManager.Instance.Load(equipmentData, equipmentData.name);
    }
    public void UpdateStatsText(int maxhealth, int curHealth, int minAttack,int maxAttack,int maxdefene,int curdefene)
    {
        healthText.text = maxhealth.ToString();
        attackText.text = minAttack + "-" + maxAttack;
        curHealthText.text = curHealth.ToString();
        defeneText.text = maxdefene + "-" + curdefene;

    }
    #region 检测拖拽物品是否在每个Slot范围内
    public bool CheckInInventoryUI(Vector3 position)
    {
        for (int i = 0; i < inventoryUI.slotHolders.Length; i++)
        {
            RectTransform t = inventoryUI.slotHolders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckInActionUI(Vector3 position)
    {
        for (int i = 0; i < actionUI.slotHolders.Length; i++)
        {
            RectTransform t = actionUI.slotHolders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckInEquipmentUI(Vector3 position)
    {
        for (int i = 0; i < equipmentUI.slotHolders.Length; i++)
        {
            RectTransform t = equipmentUI.slotHolders[i].transform as RectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }


    #endregion

    #region 检测任务物品
    public void CheckQuestItemInBag(string questItemName)
    {
        foreach (var item in inventoryData.items)
        {
            if (item.itemData!=null)
            {
                if (item.itemData.itemName==questItemName)
                {
                    QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
                }
            }
        }
        foreach (var item in actionData.items)
        {
            if (item.itemData != null)
            {
                if (item.itemData.itemName == questItemName)
                {
                    QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
                }
            }
        }

    }
    #endregion

    //检测背包和快捷栏物品
    public InventoryItem QuestItemInBag(ItemData_SO questItem)
    {
        return inventoryData.items.Find(i=>i.itemData==questItem);
    }
    public InventoryItem QuestItemInAction(ItemData_SO questItem)
    {
        return actionData.items.Find(i => i.itemData == questItem);
    }
}
