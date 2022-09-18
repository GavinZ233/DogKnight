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
            //�����Ʒ������
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.RefreshUI();
            ////װ������
            //// GameManager.Instance.playerStats.EquipWeapon(itemData);

            //����������ȣ�����������򲻻ᱻ����
            QuestManager.Instance.UpdateQuestProgress(itemData.name, itemData.itemAmount);
            Destroy(gameObject);
        }
    }
}
