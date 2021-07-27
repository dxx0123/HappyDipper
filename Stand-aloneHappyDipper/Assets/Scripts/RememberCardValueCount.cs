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
    /// ��ȡ�ø������и���ֵ������
    /// </summary>
    /// <param name="cardsList">����</param>
    /// <returns>����<ֵ������>���ֵ�</returns>
    public Dictionary<CardManager.ValueType, int> GetCardsValueCount(List<GameObject> cardsList)
    {
        Dictionary<CardManager.ValueType, int> valueCountDict = new Dictionary<CardManager.ValueType, int>();
        int count; //�洢ֵ����
        CardManager.ValueType vType; 
        //������������
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
