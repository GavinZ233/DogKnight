using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ItemTooltip : MonoBehaviour
{
    public Text itemNameText;
    public Text itemInfoText;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        UpdatePosition();
    }
    private void Update()
    {
        UpdatePosition();
    }
    public void SetupToolTip(ItemData_SO itemData)
    {
        itemNameText.text = itemData.itemName;
        itemInfoText.text = itemData.desctiption;
    }
    public void UpdatePosition()
    {
        Vector3 mousePos = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float width = corners[2].x - corners[1].x;
        float height = corners[1].y - corners[0].y;

        if (mousePos.y<height)
        {
            rectTransform.position = mousePos + Vector3.up*height*0.5f+Vector3.up*5;
        }
        else if (Screen.width-mousePos.x<width)
        {
            rectTransform.position = mousePos + Vector3.left * width * 0.5f + Vector3.left * 5;
        }
        else rectTransform.position = mousePos + Vector3.right * width * 0.5f + Vector3.right * 5;
    }
}
