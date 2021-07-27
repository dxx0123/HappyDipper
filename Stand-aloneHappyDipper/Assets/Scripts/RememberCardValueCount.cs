using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RememberCardValueCount : MonoBehaviour
{
    CardManager cardManager;

    private void Start()
    {
        cardManager = this.GetComponent<CardManager>();
    }
    /// <summary>
    /// 获取该副卡牌中各个值的数量
    /// </summary>
    /// <param name="cardsList">卡牌</param>
    /// <returns>返回<值，数量>的字典</returns>
    public Dictionary<CardManager.ValueType, int> GetCardsValueCount(List<GameObject> cardsList)
    {
        Dictionary<CardManager.ValueType, int> valueCountDict = new Dictionary<CardManager.ValueType, int>();
        int count; //存储值个数
        CardManager.ValueType vType; 
        //遍历卡牌类型
        for (int i = 0; i < 15; i++)
        {
            count = 0;
            vType = (CardManager.ValueType)i;
            for (int j = 0; j < cardsList.Count; j++)
            {
                if ((CardManager.ValueType)i == cardManager.cardsVDict[cardsList[j]])
                {
                    count++;
                }
            }
            valueCountDict.Add(vType, count);
        }
        return valueCountDict;
    }
}
