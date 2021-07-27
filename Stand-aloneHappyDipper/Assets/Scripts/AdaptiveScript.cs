using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdaptiveScript : MonoBehaviour
{
    public static AdaptiveScript Instance; //�����ű����á������ڿ�����Adaptive��

    private void Awake()
    {
        Instance = this;
    }

    //��Ļö�٣�����ѡ����Ļ�������ң�
    public enum SCREEN 
    {
        TOP, //��
        BUTTOM, //��
        LEFT, //��
        RIGHT //��
    }

    public Vector3 screenTop; //�洢��Ļ��λ��
    public Vector3 screenButtom; //�洢��Ļ��λ��
    public Vector3 screenLeft; //�洢��Ļ��λ��
    public Vector3 screenRight; //�洢��Ļ��λ��

    //ê��ö�٣�����ѡ��ê��λ�ã�
    public enum ANCHOR 
    {
        MIDDLE, //�м�
        LEFT_TOP, //����
        LEFT_BUTTOM, //����
        RIGHT_TOP, //����
        TOP, //��
        BUTTOM //��
    }

    /// <summary>
    /// ��ʼ������Ļλ�ã�
    /// </summary>
    void Start()
    {
        screenTop = new Vector3(0f,( 1920f/( (float)Screen.width/Screen.height) ) / 2,0f); //CanvasScaler�е�Match����Ϊ0�������1920��Ϊ��׼
        screenButtom = new Vector3(0f, -(1920f / ((float)Screen.width / Screen.height)) / 2, 0f);
        screenLeft = new Vector3(-1920/2f,0f, 0f);
        screenRight = new Vector3(1920 / 2f, 0f, 0f);
        //Debug.Log("Screen.width="+Screen.width);
        //Debug.Log("Screen.height=" + Screen.height);
        //Debug.LogFormat("screenTop={0},screenButtom={1},screenLeft={2},screenRight={3}",screenTop,screenButtom,screenLeft,screenRight);
    }

    /// <summary>
    /// ��������ê��
    /// </summary>
    /// <param name="go">ָ������</param>
    /// <param name="anchor">ָ��ê��</param>
    public void SetAnchor(GameObject go,ANCHOR anchor)
    {
        RectTransform goRT = go.GetComponent<RectTransform>(); //��ȡ����RectTransform���������
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
    /// ����ƫ�ƺ�λ�ò�����(�����岻��BGʱ�����ݻ�ȡ����Ļ��ƫ������������)
    /// </summary>
    /// <param name="screen">��Ļλ��</param>
    /// <param name="offsetVec">ƫ����</param>
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
