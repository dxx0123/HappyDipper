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

    //private Dictionary<CardManager.ValueType, int> valueCountDict = new Dictionary<CardManager.ValueType, int>();   //存储在记牌器上所要显示的各个值对应的数量

    private void Start()
    {
        //Debug.Log("进入计时器Start()");
        rememberCardDeviceRT = this.GetComponent<RectTransform>();
        //获取按钮组件引用
        openButton = rememberCardDeviceRT.Find("OpenButton").GetComponent<Button>();
        closeButton = rememberCardDeviceRT.Find("CloseButton").GetComponent<Button>();
        //设置按钮状态初始化
        ChangeOnClick(true);
        //添加监听事件
        closeButton.onClick.AddListener(CloseButtonOnClick);
        openButton.onClick.AddListener(OpenButtonOnClick);

        //valueCountDict = CardManager.instance.cardsListValueCount;   //给要显示的卡牌值数量赋予初始值：所有卡牌
        //SetRememberCardDeviceValue();   //显示所有卡牌的各个值对应的数量
    }


    /// <summary>
    /// 设置记牌器中各个值的个数
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
    /// 更新记牌器中的数据（用于出牌阶段）
    /// </summary>
    /// <param name="valueCountDict">出牌的值以及数量</param>
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
    /// 记牌器开启按钮监听事件
    /// </summary>
    private void OpenButtonOnClick()
    {
        ChangeOnClick(false);

        DG.Tweening.Sequence quence = DOTween.Sequence(); //声明DOTween变量
        float duration = 0.3f; //动画时长

        //地主复制卡牌缩小一半
        quence.Append(CardManager.instance.cardsLandlordCopy[0].GetComponent<RectTransform>().DOScale(new Vector3(0.5f, 0.5f, 0f), duration ));
        quence.Join(CardManager.instance.cardsLandlordCopy[1].GetComponent<RectTransform>().DOScale(new Vector3(0.5f, 0.5f, 0f), duration ));
        quence.Join(CardManager.instance.cardsLandlordCopy[2].GetComponent<RectTransform>().DOScale(new Vector3(0.5f, 0.5f, 0f), duration ));
        //地主复制卡牌移动至记牌器下方
        for (int i = 0; i < CardManager.instance.cardsLandlordCopy.Count; i++)
        {
            if (i == 0)
            {
                quence.Append(CardManager.instance.cardsLandlordCopy[i].GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-80f + (i * 80f), -265f, 0f), duration));  //设置所复制的地主牌的位置
                //quence.Append(CardManager.instance.cardsLandlordCopy[i].GetComponent<RectTransform>().DOLocalMove(new Vector3(-80f + (i * 80f), 255f, 0f), duration ));  //设置所复制的地主牌的位置
            }
            else
            {
                quence.Join(CardManager.instance.cardsLandlordCopy[i].GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-80f + (i * 80f), -265f, 0f), duration));  //设置所复制的地主牌的位置
                //quence.Join(CardManager.instance.cardsLandlordCopy[i].GetComponent<RectTransform>().DOLocalMove(new Vector3(-80f + (i * 80f), 255f, 0f), duration ));  //设置所复制的地主牌的位置
            }
        }
    }
    /// <summary>
    /// 记牌器关闭按钮监听事件
    /// </summary>
    private void CloseButtonOnClick()
    {
        //如果地主复制卡牌为空，则记牌器按钮点击无效
        //if (CardManager.instance.cardsLandlordCopy.Count == 0)
        //{
        //    return;
        //}

        ChangeOnClick(true);

        DG.Tweening.Sequence quence = DOTween.Sequence(); //声明DOTween变量
        float duration = 0.3f; //动画时长

        //地主复制卡牌扩大到卡牌大小
        quence.Append(CardManager.instance.cardsLandlordCopy[0].GetComponent<RectTransform>().DOScale(new Vector3(1.0f, 1.0f, 0f), duration));
        quence.Join(CardManager.instance.cardsLandlordCopy[1].GetComponent<RectTransform>().DOScale(new Vector3(1.0f, 1.0f, 0f), duration));
        quence.Join(CardManager.instance.cardsLandlordCopy[2].GetComponent<RectTransform>().DOScale(new Vector3(1.0f, 1.0f, 0f), duration));
        //地主复制卡牌移动至原地主牌位置
        for (int i = 0; i < CardManager.instance.cardsLandlordCopy.Count; i++)
        {
            if (i == 0)
            {
                quence.Append(CardManager.instance.cardsLandlordCopy[i].GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-200f + (i * 200f), -140f, 0f), duration));  //设置所复制的地主牌的位置
            }
            else
            {
                quence.Join(CardManager.instance.cardsLandlordCopy[i].GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(-200f + (i * 200f), -140f, 0f), duration));  //设置所复制的地主牌的位置
            }
        }
    }
    /// <summary>
    /// 设置开启、关闭按钮的状态
    /// </summary>
    /// <param name="isSetActive"></param>
    public void ChangeOnClick(bool isSetActive)
    {
        openButton.gameObject.SetActive(isSetActive);
        closeButton.gameObject.SetActive(!isSetActive);
    }
}
