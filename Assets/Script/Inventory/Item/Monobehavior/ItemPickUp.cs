using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //添加物品到背包
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.RefreshUI();
            ////装备武器
            //// GameManager.Instance.playerStats.EquipWeapon(itemData);

            //更新任务进度，无任务进度则不会被接受
            QuestManager.Instance.UpdateQuestProgress(itemData.name, itemData.itemAmount);
            Destroy(gameObject);
        }
    }
}
