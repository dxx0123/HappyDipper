using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdaptiveScript : MonoBehaviour
{
    public static AdaptiveScript Instance; //公开脚本引用【附加于空物体Adaptive】

    private void Awake()
    {
        Instance = this;
    }

    //屏幕枚举（用于选择屏幕上下左右）
    public enum SCREEN 
    {
        TOP, //上
        BUTTOM, //下
        LEFT, //左
        RIGHT //右
    }

    public Vector3 screenTop; //存储屏幕上位置
    public Vector3 screenButtom; //存储屏幕下位置
    public Vector3 screenLeft; //存储屏幕左位置
    public Vector3 screenRight; //存储屏幕右位置

    //锚点枚举（用于选择锚点位置）
    public enum ANCHOR 
    {
        MIDDLE, //中间
        LEFT_TOP, //左上
        LEFT_BUTTOM, //左下
        RIGHT_TOP, //右上
        TOP, //上
        BUTTOM //下
    }

    /// <summary>
    /// 初始化（屏幕位置）
    /// </summary>
    void Start()
    {
        screenTop = new Vector3(0f,( 1920f/( (float)Screen.width/Screen.height) ) / 2,0f); //CanvasScaler中的Match设置为0，因此以1920作为基准
        screenButtom = new Vector3(0f, -(1920f / ((float)Screen.width / Screen.height)) / 2, 0f);
        screenLeft = new Vector3(-1920/2f,0f, 0f);
        screenRight = new Vector3(1920 / 2f, 0f, 0f);
        //Debug.Log("Screen.width="+Screen.width);
        //Debug.Log("Screen.height=" + Screen.height);
        //Debug.LogFormat("screenTop={0},screenButtom={1},screenLeft={2},screenRight={3}",screenTop,screenButtom,screenLeft,screenRight);
    }

    /// <summary>
    /// 设置物体锚点
    /// </summary>
    /// <param name="go">指定物体</param>
    /// <param name="anchor">指定锚点</param>
    public void SetAnchor(GameObject go,ANCHOR anchor)
    {
        RectTransform goRT = go.GetComponent<RectTransform>(); //获取物体RectTransform组件的引用
        switch (anchor)
        {
            case ANCHOR.MIDDLE:
                goRT.anchorMin=new Vector2(0.5f,0.5f);
                goRT.anchorMax = new Vector2(0.5f,0.5f);
                break;
            case ANCHOR.LEFT_TOP:
                goRT.anchorMin = new Vector2(0f, 1f);
                goRT.anchorMax = new Vector2(0f, 1f);
                break;
            case ANCHOR.LEFT_BUTTOM:
                goRT.anchorMin = new Vector2(0f, 0f);
                goRT.anchorMax = new Vector2(0f, 0f);
                break;
            case ANCHOR.RIGHT_TOP:
                goRT.anchorMin = new Vector2(1f, 1f);
                goRT.anchorMax = new Vector2(1f, 1f);
                break;
            case ANCHOR.TOP:
                goRT.anchorMin = new Vector2(0.5f, 1f);
                goRT.anchorMax = new Vector2(0.5f, 1f);
                break;
            case ANCHOR.BUTTOM:
                goRT.anchorMin = new Vector2(0.5f, 0f);
                goRT.anchorMax = new Vector2(0.5f, 0f);
                break;
        }
    }

    /// <summary>
    /// 计算偏移后位置并返回(父物体不是BG时，根据获取的屏幕及偏移量进行设置)
    /// </summary>
    /// <param name="screen">屏幕位置</param>
    /// <param name="offsetVec">偏移量</param>
    public Vector3 SetPositionByScreenOffset(SCREEN screen,Vector3 offsetVec)
    {
        Vector3 targetPos=new Vector3(0f,0f,0f);
        switch (screen)
        {
            case SCREEN.TOP:
                targetPos = screenTop + offsetVec;
                break;
            case SCREEN.BUTTOM:
                targetPos = screenButtom + offsetVec;
                break;
            case SCREEN.LEFT:
                targetPos = screenLeft + offsetVec;
                break;
            case SCREEN.RIGHT:
                targetPos = screenRight + offsetVec;
                break;
        }
        return targetPos;
    }
}
