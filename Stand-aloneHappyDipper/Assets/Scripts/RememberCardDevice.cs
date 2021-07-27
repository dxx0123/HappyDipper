using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RememberCardDevice : MonoBehaviour
{
    private Button openButton;
    private Button closeButton;

    public RectTransform rememberCardDeviceRT;

    //private Dictionary<CardManager.ValueType, int> valueCountDict = new Dictionary<CardManager.ValueType, int>();   //�洢�ڼ���������Ҫ��ʾ�ĸ���ֵ��Ӧ������

    private void Start()
    {
        //Debug.Log("�����ʱ��Start()");
        rememberCardDeviceRT = this.GetComponent<RectTransform>();
        //��ȡ��ť�������
        openButton = rememberCardDeviceRT.Find("OpenButton").GetComponent<Button>();
        closeButton = rememberCardDeviceRT.Find("CloseButton").GetComponent<Button>();
        //���ð�ť״̬��ʼ��
        ChangeOnClick(true);
        //��Ӽ����¼�
        closeButton.onClick.AddListener(CloseButtonOnClick);
        openButton.onClick.AddListener(OpenButtonOnClick);

        //valueCountDict = CardManager.instance.cardsListValueCount;   //��Ҫ��ʾ�Ŀ���ֵ���������ʼֵ�����п���
        //SetRememberCardDeviceValue();   //��ʾ���п��Ƶĸ���ֵ��Ӧ������
    }


    /// <summary>
    /// ���ü������и���ֵ�ĸ���
    /// </summary>
    public void SetRememberCardDeviceValue(Dictionary<CardManager.ValueType, int> valueCountDict)
    {
        //Debug.Log(closeButton);
        RectTransform rememberCardData=(RectTransform)closeButton.GetComponent<RectTransform>().Find("RememberCardData");
        //Debug.Log(valueCountDict.Count);
        foreach(KeyValuePair<CardManager.ValueType,int> valueCountItem in valueCountDict)
        {
            foreach(RectTransform childRTItem in rememberCardData)
            {
                if(childRTItem.name == valueCountItem.Key.ToString())
                {
                    childRTItem.Find("Count").GetComponentInChildren<Text>().text = valueCountItem.Value.ToString();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// ���¼������е����ݣ����ڳ��ƽ׶Σ�
    /// </summary>
    /// <param name="valueCountDict">���Ƶ�ֵ�Լ�����</param>
    public void UpdateRememberCardDeviceValue(Dictionary<CardManager.ValueType, int> valueCountDict)
    {
        RectTransform rememberCardData = (RectTransform)closeButton.GetComponent<RectTransform>().Find("RememberCardData");

        foreach (KeyValuePair<CardManager.ValueType, int> valueCountItem in valueCountDict)
        {
            foreach (RectTransform childRTItem in rememberCardData)
            {
                if (childRTItem.name == valueCountItem.Key.ToString())
                {
                    int remainNumber=int.Parse(childRTItem.Find("Count").GetComponentInChildren<Text>().text) - valueCountItem.Value;
                    childRTItem.Find("Count").GetComponentInChildren<Text>().text = remainNumber.ToString();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// ������������ť�����¼�
    /// </summary>
    private void OpenButtonOnClick()
    {
        ChangeOnClick(false);

        DG.Tweening.Sequence quence = DOTween.Sequence(); //����DOTween����
        float duration = 0.3f; //����ʱ��

        //�������ƿ�����Сһ��
        quence.Append(CardManager.instance.cardsLandlordCopy[0].GetComponent<RectTransform>().DOScale(new Vector3(0.5f, 0.5f, 0f), duration ));
        quence.Join(CardManager.instance.cardsLandlordCopy[1].GetComponent<RectTransform>().DOScale(new Vector3(0.5f, 0.5f, 0f), duration ));
        quence.Join(CardManager.instance.cardsLandlordCopy[2].GetComponent<RectTransform>().DOScale(new Vector3(0.5f, 0.5f, 0f), duration ));
        //�������ƿ����ƶ����������·�
        for (int i = 0; i < CardManager.instance.cardsLandlordCopy.Count; i++)
        {
            if (i == 0)
            {
                quence.Append(CardManager.instance.cardsLandlordCopy[i].GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-80f + (i * 80f), -265f, 0f), duration));  //���������Ƶĵ����Ƶ�λ��
                //quence.Append(CardManager.instance.cardsLandlordCopy[i].GetComponent<RectTransform>().DOLocalMove(new Vector3(-80f + (i * 80f), 255f, 0f), duration ));  //���������Ƶĵ����Ƶ�λ��
            }
            else
            {
                quence.Join(CardManager.instance.cardsLandlordCopy[i].GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-80f + (i * 80f), -265f, 0f), duration));  //���������Ƶĵ����Ƶ�λ��
                //quence.Join(CardManager.instance.cardsLandlordCopy[i].GetComponent<RectTransform>().DOLocalMove(new Vector3(-80f + (i * 80f), 255f, 0f), duration ));  //���������Ƶĵ����Ƶ�λ��
            }
        }
    }
    /// <summary>
    /// �������رհ�ť�����¼�
    /// </summary>
    private void CloseButtonOnClick()
    {
        //����������ƿ���Ϊ�գ����������ť�����Ч
        //if (CardManager.instance.cardsLandlordCopy.Count == 0)
        //{
        //    return;
        //}

        ChangeOnClick(true);

        DG.Tweening.Sequence quence = DOTween.Sequence(); //����DOTween����
        float duration = 0.3f; //����ʱ��

        //�������ƿ������󵽿��ƴ�С
        quence.Append(CardManager.instance.cardsLandlordCopy[0].GetComponent<RectTransform>().DOScale(new Vector3(1.0f, 1.0f, 0f), duration));
        quence.Join(CardManager.instance.cardsLandlordCopy[1].GetComponent<RectTransform>().DOScale(new Vector3(1.0f, 1.0f, 0f), duration));
        quence.Join(CardManager.instance.cardsLandlordCopy[2].GetComponent<RectTransform>().DOScale(new Vector3(1.0f, 1.0f, 0f), duration));
        //�������ƿ����ƶ���ԭ������λ��
        for (int i = 0; i < CardManager.instance.cardsLandlordCopy.Count; i++)
        {
            if (i == 0)
            {
                quence.Append(CardManager.instance.cardsLandlordCopy[i].GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-200f + (i * 200f), -140f, 0f), duration));  //���������Ƶĵ����Ƶ�λ��
            }
            else
            {
                quence.Join(CardManager.instance.cardsLandlordCopy[i].GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-200f + (i * 200f), -140f, 0f), duration));  //���������Ƶĵ����Ƶ�λ��
            }
        }
    }
    /// <summary>
    /// ���ÿ������رհ�ť��״̬
    /// </summary>
    /// <param name="isSetActive"></param>
    public void ChangeOnClick(bool isSetActive)
    {
        openButton.gameObject.SetActive(isSetActive);
        closeButton.gameObject.SetActive(!isSetActive);
    }
}
