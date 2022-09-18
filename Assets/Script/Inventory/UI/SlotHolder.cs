using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType {BAG,WEAPON,ARMOR,ACTION }
public class SlotHolder : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public SlotType slotType;
    public ItemUI itemUI;

    public void  OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount%2==0)
        {
            UseItem();
        }
    }
    public void UseItem()
    {//itemUI.Bag.items[itemUI.Index].itemData替换成GetItem()
        if(itemUI.GetItem()!=null)
            if (itemUI.GetItem().itemType==ItemType.Useable&&itemUI.Bag.items[itemUI.Index].amount>0)//第二个是选择背包数据中的物品数据
        {
            GameManager.Instance.playerState.ApplyHealth(itemUI.GetItem().useableData.healthPoint);

             itemUI.Bag.items[itemUI.Index].amount -= 1;

                //检查任务物品更新进度
                QuestManager.Instance.UpdateQuestProgress(itemUI.GetItem().itemName, -1);
        }
        
        UpdateItem();
    }
    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.BAG:
                itemUI.Bag = InventoryManager.Instance.inventoryData;
                break;
            case SlotType.WEAPON:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                //装备武器 切换武器
                if (GameManager.Instance.playerState != null)
                { 
                    if (itemUI.Bag.items[itemUI.Index].itemData != null)
                    {
                        GameManager.Instance.playerState.GetComponent<WeaponController>().ChangeWeapon(itemUI.Bag.items[itemUI.Index].itemData);
                    }
                    else
                    {
                        GameManager.Instance.playerState.GetComponent<WeaponController>().UnEquipWeapon();
                    }
                }
                break;
            case SlotType.ARMOR:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                if (GameManager.Instance.playerState != null)
                {
                    if (itemUI.Bag.items[itemUI.Index].itemData != null)
                    {
                        GameManager.Instance.playerState.EquipArm(itemUI.Bag.items[itemUI.Index].itemData.armData);
                    }
                    else
                    {
                        GameManager.Instance.playerState.UnEquipArm();
                    }
                }
                break;
            case SlotType.ACTION:
                itemUI.Bag = InventoryManager.Instance.actionData;

                break;
        }
        var item = itemUI.Bag.items[itemUI.Index];
        itemUI.SetupItemUI(item.itemData, item.amount);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemUI.GetItem())
        {
            InventoryManager.Instance.tooltip.SetupToolTip(itemUI.GetItem());
            InventoryManager.Instance.tooltip.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);

    }
    private void OnDisable()
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);
    }
}
