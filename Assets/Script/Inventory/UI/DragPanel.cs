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
        //RectTransform.anchoredPosition�� RectTransform �����������ê��ο����λ��
        //PointerEventData.delta�ϴθ���������ָ������
        //Canvas.scaleFactor������������������ͬʱ��ʹ���ʺ���Ļ������ renderMode Ϊ��Ļ�ռ�ʱ����
        rectTransform.anchoredPosition += eventData.delta/canvas.scaleFactor;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetSiblingIndex(2);
    }
}
