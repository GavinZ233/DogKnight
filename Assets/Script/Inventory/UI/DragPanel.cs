using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DragPanel : MonoBehaviour, IDragHandler,IPointerDownHandler
{
    RectTransform rectTransform;
    Canvas canvas;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = InventoryManager.Instance.GetComponent<Canvas>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        //RectTransform.anchoredPosition此 RectTransform 的轴心相对于锚点参考点的位置
        //PointerEventData.delta上次更新以来的指针增量
        //Canvas.scaleFactor用于缩放整个画布，同时仍使其适合屏幕。仅在 renderMode 为屏幕空间时适用
        rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetSiblingIndex(2);
    }
}
