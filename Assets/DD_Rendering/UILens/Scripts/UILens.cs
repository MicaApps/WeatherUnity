using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
public class UILens : MonoBehaviour
{
    public Transform WorldAnchor;//世界坐标锚点
    public Light SunLight;//平行光方向
    public Camera camera;//主摄像机
    public List<LensData> lensdatas = new List<LensData>();

    RectTransform rect;
    public void Start()
    {
        rect=GetComponent<RectTransform>();
    }
    public void Update()
    {
        foreach (var item in lensdatas)
        {
            item.image.anchoredPosition = WorldToUgui(WorldAnchor.position+SunLight.transform.forward*item.offset);
            item.image.localScale = Vector3.one*item.scale;
        }
    }
    public Vector2 WorldToUgui(Vector3 position)
    {
        Vector2 screenPoint = camera.WorldToScreenPoint(position);//世界坐标转换为屏幕坐标
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        screenPoint -= screenSize/2;//将屏幕坐标变换为以屏幕中心为原点
        Vector2 anchorPos = screenPoint / screenSize * rect.sizeDelta;//缩放得到UGUI坐标
        return anchorPos;
    }
} 
[Serializable]
public class LensData
{
    public RectTransform image;
    [Range(0,1)]
    public float scale=1;
    public float offset=0;
}