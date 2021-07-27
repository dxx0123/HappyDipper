//using System;
//using System.Collections;
//using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class OutCardClass : MonoBehaviour
{
    //public static OutCardClass Instance { get; private set; } //�����ű�����
    /// <summary>
    /// ��������
    /// </summary>
    public enum OUT_CARD_TYPE
    {
        INCONFORMITY, //������
        SINGLE, //����
        STRAIGHT,  //˳��
        DOUBLE, //����
        PAIRS, //����
        THREE_WITHOUT, //������
        THREE_WITH_ONE, //����һ
        THREE_WITH_TWO, //������
        PLANES_WITHOUT, //�ɻ�����
        PLANES_WITH_SINGLE, //�ɻ�����
        PLANES_WITH_DOUBLE, //�ɻ�����
        FOUR_WITH_TWO_SINGLE, //�Ĵ���
        FOUR_WITH_TWO_DOUBLE, //�Ĵ�����
        BOMBS, //ը��
        KING_EXPLOSIONS //��ը
    }

    public int lastOutCardType =-1 ; //�洢��һ�������Ƶĳ������ͣ�ת����int������ǰû����ҳ���ʱ��Ϊ-1��
    public int lastOutCardPlayer=-1; //�洢��һ���������
    private int lastOutCardCount = -1; //�洢��һ�������ƵĿ�������������˳�ӡ����ԡ��ɻ���
    public int maxValueLastOutCardsMaxCount=-1; //�洢��һ�����ƿ��������������ֵ�����е����ֵ

    private CardManager.ValueType maxValueCurrentOutCardsMaxCount; //�洢��ǰ���ƿ��������������ֵ�����е����ֵ

    public List<GameObject> outCardsList = new List<GameObject>(); //��Ҫ���Ŀ��Ƽ���
    public List<GameObject> playerPreOutCardsList = new List<GameObject>(); //���Ԥ���ƣ���AI��ң�
    public List<GameObject> playerOnClickPreOutCardsList = new List<GameObject>(); //��ҵ��Ԥѡ���ƣ���AI��ң�

    public List<GameObject> leftPlayerLastOutCardsList = new List<GameObject>(); //��������һ�����Ƽ���
    public List<GameObject> playerLastOutCardsList = new List<GameObject>(); //�м������һ�����Ƽ���
    public List<GameObject> rightPlayerLastOutCardsList = new List<GameObject>(); //�ұ������һ�����Ƽ���

    /// <summary>
    /// �ű�������ʼ��
    /// </summary>
    public void Init()
    {
        lastOutCardType = -1; //��һ���������Ƶĳ�������
        lastOutCardPlayer = -1; //��һ���������
        lastOutCardCount = -1; //��һ���������Ƶ�����
        maxValueLastOutCardsMaxCount = -1; //��һ�����ƿ��������������ֵ�����е����ֵ

        //��տ��Ƽ���
        outCardsList.Clear();
        playerPreOutCardsList.Clear();
        playerOnClickPreOutCardsList.Clear();

        leftPlayerLastOutCardsList.Clear();
        playerLastOutCardsList.Clear();
        rightPlayerLastOutCardsList.Clear();
    }


    /// <summary>
    /// ��ȡָ�����Ƶĳ�������
    /// </summary>
    CardManager.ValueType minValueOutCardsMaxCount;  //�洢���ƿ��������������ֵ�����е���С��ֵ
    CardManager.ValueType maxValueOutCardsMaxCount;  //�洢���ƿ��������������ֵ�����е�����ֵ
    /// <summary>
    /// ��ȡ��ѡ���Ƶĳ�������
    /// �������ƿ����е�������������ValueType����maxValueLastOutCardsMaxCount��
    /// </summary>
    /// <param name="cards">ѡ��Ŀ���</param>
    /// <returns>���س�������</returns>
    private OUT_CARD_TYPE GetOutCardType(List<GameObject> outCards)
    {
        if (outCards.Count <= 0) return OUT_CARD_TYPE.INCONFORMITY; //������ƿ���Ϊ�գ��򷵻ز��Ϸ�

        Dictionary<CardManager.ValueType, int> outCardsValueCountDict = new Dictionary<CardManager.ValueType, int>();  //�����ƿ��Ƶ�ֵ�����ֵ�
        CardManager.ValueType valueT; //����ֵ���ͱ���
        //�������ƿ����е����п��ƣ�ͳ�������ֵ������
        foreach (GameObject go in outCards)
        {
            //��ȡ�ÿ��Ƶ�ֵ����
            valueT = CardManager.instance.cardsVDict[go];
            //�жϳ��Ƶ�ֵ�����ֵ����Ƿ��Ѵ��ڸ�ֵ����
            if (outCardsValueCountDict.ContainsKey(valueT))
            {
                outCardsValueCountDict[valueT]++; //��ֵ���͵�����+1
            }
            else
            {
                outCardsValueCountDict.Add(valueT, 1);
            }
        }
        //Debug.Log("��ȡ�������ͣ�outCardsValueCountDict.Count="+outCardsValueCountDict.Count);

        List<int> outCardsValueC = new List<int>(); //�洢����ֵ�����ֵ��еĸ���ֵ������
        int outCardsAllCount = 0; //����������,��ʼֵΪ0
        //��������ֵ�����ֵ䣬�ֱ�洢����������������
        foreach (KeyValuePair<CardManager.ValueType, int> item in outCardsValueCountDict)
        {
            //Debug.Log("��ȡ���������У�item.key="+item.Key);
            //Debug.Log("��ȡ���������У�item.Value=" + item.Value);
            outCardsValueC.Add(item.Value); //��ӳ����и���ֵ��������������
            outCardsAllCount += item.Value; //ͳ�Ƴ��Ƶ��ܿ�����
        }
        
        outCardsValueC.Sort(); //�������ϴ�С�����������
        //foreach (int item in outCardsValueC)
        //{
        //    Debug.Log("��ȡ���������У�outCardsValueC's item=" + item);
        //}

        List<CardManager.ValueType> outCardsMaxCountValueType = new List<CardManager.ValueType>(); //�洢����ֵ�����ֵ��е������ֵ�ĸ���ֵ
        int maxCount = outCardsValueC[outCardsValueC.Count-1]; //�Ӹ���ֵ�����ļ�������ȡ���������ֵ
        //Debug.Log("��ȡ�������ͣ�maxCount="+maxCount);
        //��ȡ�������ֵ����Ӧ��ValueType�����洢��ֵ������
        var maxCountValueTypeInDict = outCardsValueCountDict.Where(q => q.Value == maxCount).Select(q=>q.Key); //���Ҿ������������ֵ
        outCardsMaxCountValueType.AddRange(maxCountValueTypeInDict); //���������������ֵ���뼯����
        //Debug.Log("��ȡ�������ͣ�outCardsMaxCountValueType.Count="+ outCardsMaxCountValueType.Count);
        //foreach (KeyValuePair<CardManager.ValueType, int> item in outCardsValueCountDict)
        //{
        //    if (item.Value == maxCount)
        //    {
        //        outCardsMaxCountValueType.Add(item.Key);
        //    }
        //}

        //��ȡ�������ֵ����ӦValueType�����е����ValueType
        maxValueCurrentOutCardsMaxCount = outCardsMaxCountValueType[0];
        foreach (CardManager.ValueType item in outCardsMaxCountValueType)
        {
            if ((int)maxValueCurrentOutCardsMaxCount < (int)item)
            {
                maxValueCurrentOutCardsMaxCount = item;
            }
        }
        //Debug.Log("��ȡ�������ͣ�maxValueCurrentOutCardsMaxCount="+ maxValueCurrentOutCardsMaxCount);

        int outCardsValueCountDictPairCount = outCardsValueCountDict.Count;//���ƿ���ֵ�����ֵ��м�ֵ�Ե�����
        //���ƿ���������ֵ��Ӧ���������������Ϊ1
        if (maxCount==1)
        {
            //�������Ϊ1 �� ֻ��һ�ָ������Ŀ������ͣ����ƣ�
            if(outCardsMaxCountValueType.Count == 1)
            {
                //Debug.Log("��ǰ���ƿ���Ϊ����");
                return OUT_CARD_TYPE.SINGLE;
            }
            //�������Ϊ1 �� ֻ�д�����С�������ֿ������ͣ���ը�������ϣ�
            else if (outCardsMaxCountValueType.Count ==2)
            {
                //Debug.Log("�ж��Ƿ��Ǵ�С��");
                //�ж��Ƿ������С��
                if(outCardsMaxCountValueType.Contains(CardManager.ValueType.BLACKJOKER) && outCardsMaxCountValueType.Contains(CardManager.ValueType.REDJOKER))
                {
                    //Debug.Log("��ǰ���ƿ���Ϊ��ը");
                    return OUT_CARD_TYPE.KING_EXPLOSIONS;
                }
                else
                {
                    //Debug.Log("��ը������");
                    return OUT_CARD_TYPE.INCONFORMITY;
                }
            }
            //�������Ϊ1 �� �������Ŀ������ʹ���5��˳�ӡ������ϣ�
            else if (outCardsMaxCountValueType.Count>=5)
            {
                //�ж��Ƿ�������������2
                if(!outCardsMaxCountValueType.Contains(CardManager.ValueType.TWO) && ISContinuous(outCardsMaxCountValueType))
                {
                    //Debug.Log("��ǰ���ƿ���Ϊ˳��");
                    return OUT_CARD_TYPE.STRAIGHT;
                }
                else
                {
                    //Debug.Log("˳�Ӳ�����");
                    return OUT_CARD_TYPE.INCONFORMITY;
                }
            }
            //�������Ϊ�����ϣ������������Ϸ�������3��4�ȣ�
            else
            {
                //Debug.Log("�������Ϊ1���������������");
                return OUT_CARD_TYPE.INCONFORMITY;
            }
        }
        //���ƿ���������ֵ��Ӧ���������������Ϊ2
        else if (maxCount == 2)
        {
            //�������Ϊ2 �� ֻ��һ�ָ������Ŀ������� �� û�����������Ŀ������ͣ����ƣ�
            //��ΪmaxCount==2��������������ֻ����1
            if ( outCardsMaxCountValueType.Count==1 && outCardsValueCountDictPairCount==1)
            {
                //Debug.Log("��������Ϊ����");
                return OUT_CARD_TYPE.DOUBLE;
            }
            //�������Ϊ2 �� �д���3�ָ������Ŀ������� �� û�����������Ŀ������ͣ����ԣ�
            else if(outCardsMaxCountValueType.Count>=3 && outCardsValueCountDictPairCount == outCardsMaxCountValueType.Count)
            {
                if (!outCardsMaxCountValueType.Contains(CardManager.ValueType.TWO) && ISContinuous(outCardsMaxCountValueType))
                {
                    //Debug.Log("��������Ϊ����");
                    return OUT_CARD_TYPE.PAIRS;
                }
                else
                {
                    //Debug.Log("���Գ������Ͳ��Ϸ�");
                    return OUT_CARD_TYPE.INCONFORMITY;
                }
            }
            //�������Ϊ������
            else
            {
                //Debug.Log("�������Ϊ2���������Ͳ��Ϸ�");
                return OUT_CARD_TYPE.INCONFORMITY;
            }
        }
        //���ƿ���������ֵ��Ӧ���������������Ϊ3
        else if (maxCount == 3)
        {
            //�������Ϊ3 �� ֻ��һ�ָ������Ŀ������ͣ�������������һ���������������ϣ�
            //Debug.Log("�����������������Ϊ3");
            if (outCardsMaxCountValueType.Count == 1)
            {
                //������ֻ�и��������Ŀ������ͣ����������ֵ�����������ֵ������һ�£�����������
                if (outCardsValueCountDictPairCount == 1)
                {
                    //Debug.Log("��������Ϊ������");
                    return OUT_CARD_TYPE.THREE_WITHOUT;
                }
                //������һ�������Ŀ���ֵ���ͣ��ÿ�������ֵ������Ϊ1������һ����Ϊ2����������
                else if(outCardsValueCountDictPairCount==2)
                {
                    //Debug.Log("�ж��Ƿ���������");
                    if(outCardsValueC[0] == 1)
                    {
                        //Debug.Log("��������Ϊ����һ");
                        return OUT_CARD_TYPE.THREE_WITH_ONE;
                    }
                    else if (outCardsValueC[0]==2)
                    {
                        //Debug.Log("��������Ϊ������");
                        return OUT_CARD_TYPE.THREE_WITH_TWO;
                    }
                    //����������Ϊ������
                    else
                    {
                        //Debug.Log("�����д����Ϸ�");
                        return OUT_CARD_TYPE.INCONFORMITY;
                    }
                }
                //�������Ϊ������
                else
                {
                    //Debug.Log("�����������Ͳ��Ϸ�");
                    return OUT_CARD_TYPE.INCONFORMITY;
                }
            }
            //�������Ϊ3 �� ���������ϸ������Ŀ������ͣ��ɻ��������ɻ��������ɻ����ԡ������ϣ�
            else if(outCardsMaxCountValueType.Count>=2)
            {
                //�����а���2Ϊ���Ϸ�
                if (outCardsMaxCountValueType.Contains(CardManager.ValueType.TWO))
                {
                    //Debug.Log("�ɻ�����2���Ϸ�");
                    return OUT_CARD_TYPE.INCONFORMITY;
                }
                //�ж��Ƿ���������ԭֵ���ϡ�ȥ�����ֵ���ֵ���ϡ�ȥ����Сֵ���ֵ���ϣ�
                else if (ISContinuous(outCardsMaxCountValueType) || ISContinuous(RemoveMaxOrMinForContinue(outCardsMaxCountValueType,true)) || ISContinuous(RemoveMaxOrMinForContinue(outCardsMaxCountValueType,false)))
                {
                    //Debug.Log("�ɻ��ж�����");
                    //��ȡ�������ֵ����ӦValueType�����е����ValueType�����ɻ���3�������
                    maxValueCurrentOutCardsMaxCount = outCardsMaxCountValueType[0];
                    foreach (CardManager.ValueType item in outCardsMaxCountValueType)
                    {
                        if ((int)maxValueCurrentOutCardsMaxCount < (int)item)
                        {
                            maxValueCurrentOutCardsMaxCount = item;
                        }
                    }

                    //������ֻ�и��������Ŀ������ͣ���ֵ�Եĸ���==���������ֵ���͵ĸ�����(�ɻ�����)
                    if (outCardsValueCountDictPairCount == outCardsMaxCountValueType.Count)
                    {
                        //Debug.Log("�������ͷɻ�����");
                        return OUT_CARD_TYPE.PLANES_WITHOUT;
                    }
                    //�������������������Ŀ������ͣ����������Ŀ������͵ĸ�����==outCardsValueV.Count�����ɻ�������
                    //���������Ŀ������͵ĸ�����=���п�������-3*����������ֵ�ĸ���
                    else if(outCardsAllCount==4*outCardsMaxCountValueType.Count)
                    {
                        //Debug.Log("�������ͷɻ�����");
                        return OUT_CARD_TYPE.PLANES_WITH_SINGLE;
                    }
                    //�������������������Ŀ������ͣ������������Ŀ������͸���=��ֵ�Ը���-���������ֵ���͸��������ɻ����ԣ�
                    else if ((outCardsValueCountDictPairCount - outCardsMaxCountValueType.Count) == outCardsMaxCountValueType.Count)
                    {
                        for(int i=0;i<outCardsValueC.Count- outCardsMaxCountValueType.Count; i++)
                        {
                            //�ж�����������ֵ���͵ĸ��������Ƿ�Ϊ2���ԣ�
                            if(outCardsValueC[i]!=2)
                            {
                                //Debug.Log("�ɻ����Բ��Ϸ�");
                                return OUT_CARD_TYPE.INCONFORMITY;
                            }
                        }
                        //Debug.Log("�������ͷɻ�����");
                        return OUT_CARD_TYPE.PLANES_WITH_DOUBLE;
                    }
                    //�������Ϊ���Ϸ�
                    else
                    {
                        //Debug.Log("�ɻ��������Ͳ��Ϸ�");
                        return OUT_CARD_TYPE.INCONFORMITY;
                    }
                }
                //��������Ϊ���Ϸ�
                else
                {
                    //Debug.Log("�ɻ����������Ϸ�");
                    return OUT_CARD_TYPE.INCONFORMITY;
                }
            }
            //�������Ϊ���Ϸ�
            else
            {
                //Debug.Log("�������Ϊ3�������Ͳ��Ϸ�");
                return OUT_CARD_TYPE.INCONFORMITY;
            }
        }
        //���ƿ���������ֵ��Ӧ�������������Ϊ4
        else if (maxCount == 4)
        {
            //�������Ϊ1 �� ֻ��һ�ָ������Ŀ���ֵ����
            if(outCardsMaxCountValueType.Count == 1)
            {
                //������ֻ�и������Ŀ���ֵ���ͣ�ը����
                if(outCardsValueCountDictPairCount==1)
                {
                    //Debug.Log("��������Ϊը��");
                    return OUT_CARD_TYPE.BOMBS;
                }
                //�����������������Ŀ���ֵ���ͣ����������Ŀ���ֵ���͵�������=2���Ĵ����������Ҳ�ͬʱ������С��
                else if (outCardsAllCount == 6 && !(outCardsValueCountDict.ContainsKey(CardManager.ValueType.BLACKJOKER) && outCardsValueCountDict.ContainsKey(CardManager.ValueType.REDJOKER)))
                {
                    //Debug.Log("��������Ϊը������");
                    return OUT_CARD_TYPE.FOUR_WITH_TWO_SINGLE;
                }
                //���������������������ֿ���ֵ���ͣ���ÿ�ֿ���ֵ���͵�����=2���Ĵ����ԣ�
                else if(outCardsValueCountDictPairCount-outCardsMaxCountValueType.Count==2 && outCardsValueC[0]==2 && outCardsValueC[1] == 2)
                {
                    //Debug.Log("��������Ϊը������");
                    return OUT_CARD_TYPE.FOUR_WITH_TWO_DOUBLE;
                }
                //�������Ϊ���Ϸ�
                else
                {
                    //Debug.Log("ը���������Ͳ��Ϸ�");
                    return OUT_CARD_TYPE.INCONFORMITY;
                }
            }
            //�������Ϊ���Ϸ�
            else
            {
                //Debug.Log("�������Ϊ4�������Ͳ��Ϸ�");
                return OUT_CARD_TYPE.INCONFORMITY;
            }
        }
        else
        {
            //Debug.Log("�����������Ͳ��Ϸ�");
            return OUT_CARD_TYPE.INCONFORMITY;
        }
        //return OUT_CARD_TYPE.INCONFORMITY;
    }
    /// <summary>
    /// �жϿ���ֵ�Ƿ���������
    /// ����ļ��ϸ���ֵ��С������������
    /// �洢�����ļ��ϵ����ֵ����Сֵ������ȥ��������
    /// </summary>
    /// <param name="cards">Ҫ�����жϵ�ValueType����</param>
    /// <returns>����������true</returns>������������false
    private bool ISContinuous(List<CardManager.ValueType> cards)
    {
        //�洢ValueType����ת����int֮��ĸ���ֵ
        List<int> cardsVTtoInt = new List<int>();
        foreach(CardManager.ValueType item in cards)
        {
            cardsVTtoInt.Add((int)item);
        }

        //int������������
        cardsVTtoInt.Sort();

        //ԭ���Ͻ�����������
        for (int i = 0; i < cardsVTtoInt.Count; i++)
        {
            cards[i] = (CardManager.ValueType)cardsVTtoInt[i];
        }
        maxValueOutCardsMaxCount = cards[cards.Count - 1]; //�洢ֵ���ϵ����ֵ
        minValueOutCardsMaxCount = cards[0]; //�洢ֵ���ϵ���Сֵ

        //�ж�����
        int lastItem = cardsVTtoInt[0]; //������һ��VT��intֵ��Ĭ��Ϊ���ϵĵ�һ����
        for(int i = 1; i < cardsVTtoInt.Count; i++)
        {
            //���жϲ������������ֱ���˳�ѭ��
            if((cardsVTtoInt[i]-lastItem)!=1)
            {
                break;
            }
            //ѭ����������������ֵ�����1������true
            if(i == cardsVTtoInt.Count-1)
            {
                return true;
            }
            //����ǰֵ��ֵ��lastItem��������һ��ѭ��
            lastItem = cardsVTtoInt[i];
        }

        return false;
    }
    /// <summary>
    /// ȥ��ֵ���ϵ���Сֵ�����ֵ��ʹ�����п������������������ɻ������Ƶ������333444555---888��
    /// </summary>
    /// <param name="cardsValueTypeList">��ֵ��С��������õ�ֵ����</param>
    /// <param name="maxTminF">trueȥ�����ֵ��falseȥ����Сֵ</param>
    private List<CardManager.ValueType> RemoveMaxOrMinForContinue(List<CardManager.ValueType> cardsValueTypeList,bool maxTminF)
    {
        //ȥ�����ֵ
        if(maxTminF)
        {
            //�ж���֮ǰ�Ƿ���ɾ����Сֵ�����У�����ӻ���
            if(!cardsValueTypeList.Contains(minValueOutCardsMaxCount))
            {
                cardsValueTypeList.Add(minValueOutCardsMaxCount);
            }
            cardsValueTypeList.Remove(cardsValueTypeList[cardsValueTypeList.Count - 1]); //�Ƴ������е����ֵ
        }
        //ȥ����Сֵ
        else
        {
            //�ж���֮ǰ�Ƿ���ɾ�����ֵ�����У�����ӻ���
            if(!cardsValueTypeList.Contains(maxValueOutCardsMaxCount))
            {
                cardsValueTypeList.Add(maxValueOutCardsMaxCount);
            }
            cardsValueTypeList.Remove(minValueOutCardsMaxCount); //�Ƴ������е���Сֵ
        }
        return cardsValueTypeList;
    }



    /// <summary>
    /// ��ȡ��ҿ�������С�Ŀɳ�����
    /// </summary>
    List<int> allCanOutCardsList = new List<int>(); //���ڴ洢���п��Գ��Ŀ���ֵ����ת���ɵ�int����
    Dictionary<CardManager.ValueType, int> cardsValueCountDictAuxiliary = new Dictionary<CardManager.ValueType, int>(); //���Ƶĸ���ֵ�����ֵ�
    /// <summary>
    /// ��ȡ��С�ɳ��Ƶļ���
    /// </summary>
    /// <param name="cards">��ҿ���</param>
    /// <param name="thanMaxValueType"></param>
    /// <returns></returns>
    public List<GameObject> GetMinCanOutCardsByType(List<GameObject> cards,int thanMaxValueType,int lastOutCardType)
    {
        //Debug.Log("��ȡ��С�ɳ��Ƶļ���");
        //Debug.Log("��ȡ��С�ɳ��Ƶļ���,��ǰ��������=" + cards.Count);
        //Debug.Log("��ȡ��С�ɳ��Ƶļ���,thanMaxValueType=" + thanMaxValueType);
        //Debug.Log("��ȡ��С�ɳ��Ƶļ���,lastOutCardType=" + lastOutCardType);

        CardsSelectAuxiliary(cards); //���ÿ��Ƹ�������
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //���ڴ洢�ɳ��Ŀ���

        //û�г�����������
        if (lastOutCardType==-1)
        {
            List<GameObject> minSingleCanOutCardList = GetMinSingleCanOutCards(cards, thanMaxValueType); //��ȡ����
            //�е���
            if (minSingleCanOutCardList.Count!=0)
            {
                minCanOutCardsList.AddRange(minSingleCanOutCardList); //�����Ƹ�ֵ�����Ƽ���
            }

            List<GameObject> minDoubleCanOutCardsList = GetMinDoubleCanOutCards(cards, thanMaxValueType); //��ȡ����
            //�ж���
            if (minDoubleCanOutCardsList.Count != 0)
            {
                //���û�е��ƣ���ֱ�Ӹ�ֵ����
                if (minCanOutCardsList.Count==0)
                {
                    minCanOutCardsList.AddRange(minDoubleCanOutCardsList);
                }
                //�е��ƣ��ҵ���ֵ���ڶ���
                if (minCanOutCardsList.Count!= 0 && CardManager.instance.cardsVDict[minCanOutCardsList[0]] > CardManager.instance.cardsVDict[minDoubleCanOutCardsList[0]])
                {
                    minCanOutCardsList.Clear(); //��մ洢��Ԫ��
                    minCanOutCardsList.AddRange(minDoubleCanOutCardsList); //��Ӷ����е�Ԫ��
                }
            }

            List<GameObject> minThreeCanOutCardsList = GetMinThreeCanOutCards(cards, thanMaxValueType); //��ȡ����
            //������
            if (minThreeCanOutCardsList.Count!=0)
            {
                //���û�е��ơ����ƣ���ֱ�Ӹ�ֵ����
                if (minCanOutCardsList.Count==0)
                {
                    minCanOutCardsList.AddRange(minThreeCanOutCardsList);
                }
                //�е��ơ������ж��ơ������е��ƺͶ��ƣ�minCanOutCardsList�д洢������Сֵ��GameObject��
                //�洢����Сֵ��������
                if (minCanOutCardsList.Count!=0 && CardManager.instance.cardsVDict[minCanOutCardsList[0]] > CardManager.instance.cardsVDict[minThreeCanOutCardsList[0]])
                {
                    minThreeCanOutCardsList = minThreeCanOutCardsList.Concat(minCanOutCardsList).ToList<GameObject>(); //����С�ĵ��ƻ��߶���һ����ֵ�������У�����һ ���� ��������
                    minCanOutCardsList.Clear(); //��ճ��ƿ�����Ԫ��
                    minCanOutCardsList.AddRange(minThreeCanOutCardsList); //������������Ԫ����������Ƽ�����
                }
            }

            //û�е��ơ����ơ�����ʱ
            if (minCanOutCardsList.Count==0)
            {
                minCanOutCardsList = GetMinBombCanOutCards(cards,thanMaxValueType); //��ȡը��
                //û��ը��
                if(minCanOutCardsList.Count==0)
                {
                    minCanOutCardsList = GetKingExplosionCanOutCards(cards); //��ȡ��ը
                }
            }
        }
        //�г�����������
        else
        {
            //������һ�����������Լ��ȶ�ֵ�����г���
            switch (lastOutCardType)
            {
                //����
                case 1:
                    List<GameObject> minSingleCanOutCardList = GetMinSingleCanOutCards(cards,thanMaxValueType); //��ȡ����
                    //û�е���
                    if (minSingleCanOutCardList.Count==0)
                    {
                        minSingleCanOutCardList = GetMinDoubleCanOutCards(cards,thanMaxValueType); //��ȡ����
                        //û�ж���
                        if (minSingleCanOutCardList.Count==0)
                        {
                            minSingleCanOutCardList = GetMinThreeCanOutCards(cards,thanMaxValueType); //��ȡ����
                        }
                    }
                    //�е��ơ����ơ���������
                    if (minSingleCanOutCardList.Count!=0)
                    {
                        minCanOutCardsList.Add(minSingleCanOutCardList[0]); //����ֻ��Ҫ�������һ�ſ���
                    }
                    //û�е��ơ����ƻ������ƣ�ը������ը��
                    else
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը ��ը������Ҫ�ȶԴ�С��
                    }
                    break;
                //˳��
                case 2:
                    //Debug.Log("˳�Ӵ���δд");
                    minCanOutCardsList = GetMinStraightCanOutCards(cards,thanMaxValueType,lastOutCardCount);
                    //û��˳�ӣ�ը������ը��
                    if(minCanOutCardsList.Count==0)
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը ��ը������Ҫ�ȶԴ�С��
                    }
                    break;
                //����
                case 3:
                    List<GameObject> minDoubleCanOutCardsList = GetMinDoubleCanOutCards(cards,thanMaxValueType); //��ȡ����
                    //û�ж���
                    if (minDoubleCanOutCardsList.Count == 0)
                    {
                        minDoubleCanOutCardsList = GetMinThreeCanOutCards(cards,thanMaxValueType); //��ȡ����
                    }
                    //�ж��ơ�����
                    if(minDoubleCanOutCardsList.Count!=0)
                    {
                        minCanOutCardsList.Add(minDoubleCanOutCardsList[0]); //�������������������
                        minCanOutCardsList.Add(minDoubleCanOutCardsList[1]);
                    }
                    //û�ж��ơ�����
                    else
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը ��ը������Ҫ�ȶԴ�С��
                    }
                    break;
                //����
                case 4:
                    minCanOutCardsList = GetMinPairsCanOutCards(cards,thanMaxValueType,lastOutCardCount); //��ȡ����
                    //û������
                    if (minCanOutCardsList.Count==0)
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը ��ը������Ҫ�ȶԴ�С��
                    }
                    break;
                //������
                case 5:
                    minCanOutCardsList = GetMinThreeCanOutCards(cards,thanMaxValueType); //��ȡ����
                    if(minCanOutCardsList.Count==0)
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը ��ը������Ҫ�ȶԴ�С��
                    }
                    break;
                //����һ
                case 6:
                    List<GameObject> minThreeOneCanOutCardsList = GetMinThreeCanOutCards(cards,thanMaxValueType); //��ȡ����
                    List<GameObject> minOneThreeCanOutCardList = GetMinCanOutCardsByType(cards, 0, (int)OUT_CARD_TYPE.SINGLE); //��ȡ���ƣ����Ʋ��޶���С
                    //���������е��ƣ��ҵ��Ʋ��Ǵ������л�ȡ��
                    if (minThreeOneCanOutCardsList.Count!=0 && minOneThreeCanOutCardList.Count==1 && !minThreeOneCanOutCardsList.Contains(minOneThreeCanOutCardList[0]))
                    {
                        minThreeOneCanOutCardsList = minThreeOneCanOutCardsList.Concat(minOneThreeCanOutCardList).ToList<GameObject>(); //�������һ
                        minCanOutCardsList = minThreeOneCanOutCardsList;
                    }
                    //û�����ƻ���û�е��ƣ���ը������ը
                    else
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը
                    }
                    break;
                //������
                case 7:
                    List<GameObject> minThreeDoubleCanOutCardsList = GetMinThreeCanOutCards(cards, thanMaxValueType); //��ȡ����
                    List<GameObject> minDoubleThreeCanOutCardsList = GetMinCanOutCardsByType(cards, 0, (int)OUT_CARD_TYPE.DOUBLE); //��ȡ���ƣ����Ʋ��޶���С
                    //���������ж���,�Ҷ��Ʋ��Ǵ������л�ȡ�ģ�//���Ʋ��Ǵ�С����
                    if (minThreeDoubleCanOutCardsList.Count != 0 && minDoubleThreeCanOutCardsList.Count == 2 && !minThreeDoubleCanOutCardsList.Contains(minDoubleThreeCanOutCardsList[0]) && CardManager.instance.cardsVDict[minDoubleThreeCanOutCardsList[0]] == CardManager.instance.cardsVDict[minDoubleThreeCanOutCardsList[1]])
                    {
                        minThreeDoubleCanOutCardsList = minThreeDoubleCanOutCardsList.Concat(minDoubleThreeCanOutCardsList).ToList<GameObject>(); //���������
                        minCanOutCardsList = minThreeDoubleCanOutCardsList;
                    }
                    //û�����ƻ���û�ж��ƣ���ը������ը
                    else
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը
                    }
                    break;
                //�ɻ�����
                case 8:
                    minCanOutCardsList = GetMinPlanesWithoutCanOutCards(cards,thanMaxValueType,lastOutCardCount); //��ȡ�ɻ�����
                    //û�зɻ�����
                    if (minCanOutCardsList.Count==0)
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը
                    }
                    break;
                //�ɻ�����
                case 9:
                    minCanOutCardsList = GetMinPlanesWithSingleCanOutCards(cards,thanMaxValueType,lastOutCardCount); //��ȡ�ɻ�����
                    //û�зɻ�����
                    if (minCanOutCardsList.Count == 0)
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը
                    }
                    break;
                //�ɻ�����
                case 10:
                    //Debug.Log("�ɻ�����");
                    minCanOutCardsList = GetMinPlanesWithDoubleCanOutCards(cards, thanMaxValueType, lastOutCardCount); //��ȡ�ɻ�����
                    //û�зɻ�����
                    if (minCanOutCardsList.Count == 0)
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը
                    }
                    break;
                //�Ĵ���
                case 11:
                    List<GameObject> minFourSingleCanOutCardsList = GetMinBombCanOutCards(cards,thanMaxValueType); //��ȡը��������Ĵ�����Ҫ�ȶԴ�С
                    //�и���ֵ��ը��
                    if (minFourSingleCanOutCardsList.Count==4)
                    {
                        List<GameObject> minSingleFourCanOutCardsList01 = GetMinCanOutCardsByType(cards, 0, (int)OUT_CARD_TYPE.SINGLE); //��ȡ���ƣ����Ʋ���Ҫ�ȶԴ�С
                        //�е���
                        if (minSingleFourCanOutCardsList01.Count == 1)
                        {
                            cards.Remove(minSingleFourCanOutCardsList01[0]); //�Ƴ���һ������
                            List<GameObject> minSingleFourCanOutCardsList02 = GetMinCanOutCardsByType(cards, 0, (int)OUT_CARD_TYPE.SINGLE); //��ȡ��01��ĵ��ƣ����Ʋ���Ҫ�ȶԴ�С
                            cards.Add(minSingleFourCanOutCardsList01[0]);  //��ӻر��Ƴ��ĵ���
                            //�еڶ��ŵ��ƣ��������ŵ��Ʋ��ǴӶ����л�ȡ���Ӷ����л�ȡ�����ֿճ�һ���ƣ�
                            if (minSingleFourCanOutCardsList02.Count == 1 && CardManager.instance.cardsVDict[minSingleFourCanOutCardsList01[0]]!= CardManager.instance.cardsVDict[minSingleFourCanOutCardsList02[0]])
                            {
                                //��ϳ��Ĵ����ĸ�ʽ
                                minFourSingleCanOutCardsList = minFourSingleCanOutCardsList.Concat(minSingleFourCanOutCardsList01).ToList<GameObject>();
                                minFourSingleCanOutCardsList = minFourSingleCanOutCardsList.Concat(minSingleFourCanOutCardsList02).ToList<GameObject>();
                                minCanOutCardsList = minFourSingleCanOutCardsList;
                            }
                            //û�еڶ��ŵ��ƣ��޷��Ĵ���ѹ����ֻ��ը������ը
                            else
                            {
                                minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը
                            }
                        }
                        //û�е��ƣ��޷��Ĵ���ѹ����ֻ��ը������ը
                        else
                        {
                            minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը
                        }
                    }
                    //�޷��Ĵ���ѹ����ֻ��ը������ը
                    else
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards,-1,(int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը
                    }
                    break;
                //�Ĵ�����
                case 12:
                    List<GameObject> minFourDoubleCanOutCardsList = GetMinBombCanOutCards(cards,thanMaxValueType); //��ȡը������Ҫ�ȶԴ�С
                    //�и���ֵ��ը��
                    if (minFourDoubleCanOutCardsList.Count == 4)
                    {
                        List<GameObject> minDoubleFourCanOutCardsList01 = GetMinCanOutCardsByType(cards, 0, (int)OUT_CARD_TYPE.DOUBLE); //��ȡ���ƣ����Ʋ���Ҫ�ȶԴ�С
                        //�ж���(���Ǵ�С��)
                        if (minDoubleFourCanOutCardsList01.Count == 2 && minDoubleFourCanOutCardsList01[0]==minDoubleFourCanOutCardsList01[1])
                        {
                            cards.Remove(minDoubleFourCanOutCardsList01[0]); //�Ƴ���һ������
                            cards.Remove(minDoubleFourCanOutCardsList01[1]);
                            List<GameObject> minDoubleFourCanOutCardsList02 = GetMinCanOutCardsByType(cards, 0, (int)OUT_CARD_TYPE.DOUBLE); //��ȡ��01����ƣ����Ʋ���Ҫ�ȶԴ�С
                            cards.Add(minDoubleFourCanOutCardsList01[0]);  //��ӻر��Ƴ��Ķ���
                            cards.Add(minDoubleFourCanOutCardsList01[1]);
                            //�еڶ������ƣ����Ǵ�С����
                            if (minDoubleFourCanOutCardsList02.Count == 2 && minDoubleFourCanOutCardsList02[0] == minDoubleFourCanOutCardsList02[1])
                            {
                                //��ϳ��Ĵ����Եĸ�ʽ
                                minFourDoubleCanOutCardsList = minFourDoubleCanOutCardsList.Concat(minDoubleFourCanOutCardsList01).ToList<GameObject>();
                                minFourDoubleCanOutCardsList = minFourDoubleCanOutCardsList.Concat(minDoubleFourCanOutCardsList02).ToList<GameObject>();
                                minCanOutCardsList = minFourDoubleCanOutCardsList;
                            }
                            //�޷��Ĵ�����ѹ����ֻ��ը������ը
                            else
                            {
                                minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը
                            }
                        }
                        //�޷��Ĵ�����ѹ����ֻ��ը������ը
                        else
                        {
                            minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը
                        }
                    }
                    //�޷��Ĵ�����ѹ����ֻ��ը������ը
                    else
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //��ȡը������ը
                    }
                    break;
                //ը��
                case 13:
                    minCanOutCardsList = GetMinBombCanOutCards(cards,thanMaxValueType); //��ȡը��
                    //û��ը��
                    if (minCanOutCardsList.Count==0)
                    {
                        minCanOutCardsList = GetKingExplosionCanOutCards(cards); //��ȡ��ը
                    }
                    break;
                //��ը
                case 14:
                    minCanOutCardsList = GetKingExplosionCanOutCards(cards); //��ȡ��ը
                    break;
            }
        }

        //Debug.Log("��ȡ��С�ɳ��Ƶļ���,���ƿ���������"+minCanOutCardsList.Count);
        return minCanOutCardsList;
    }

    /// <summary>
    /// ����ѡ���������ڵ�ǰ����ɸѡ�����ʵĳ��ƣ�
    /// </summary>
    private void CardsSelectAuxiliary(List<GameObject> cards)
    {
        //��տ��Ƶĸ���ֵ�����ֵ䣨�����һ�����ݣ�
        if (cardsValueCountDictAuxiliary.Count!=0)
        {
            cardsValueCountDictAuxiliary.Clear();
        }

        //��ȡ����ֵ�����ֵ�
        cardsValueCountDictAuxiliary = CardManager.instance.rememberCardValueCount.GetCardsValueCount(cards);
    }

    /// <summary>
    /// ��ȡ��ǰ��ҿ����п��Գ�����С����
    /// </summary>
    /// <param name="cards">��ǰ��ҿ���</param>
    /// <param name="thanMaxValueType">���бȶԵ����ֵ</param>
    /// <param name="isNeedAuxiliary">�Ƿ���Ҫ�����������г�ʼ��</param>
    /// <returns></returns>
    public List<GameObject> GetMinSingleCanOutCards(List<GameObject> cards,int thanMaxValueType/*,bool isNeedAuxiliary*/)
    {
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //���ڴ洢�ɳ��Ŀ���
        //�ж��Ƿ���Ҫ�����������г�ʼ��
        //if(isNeedAuxiliary)
        //{
        //    CardsSelectAuxiliary(cards); //���ø�������
        //}
        //ɸѡ������һ�������������ֵ�����ֵ���洢��allCanOutCardsList������
        SetAllCanOutCardsListByTraverseDict(OUT_CARD_TYPE.SINGLE,thanMaxValueType);
        //��ǰ�������з��ϵĵ���ʱ
        if(allCanOutCardsList.Count!=0)
        {
            minCanOutCardsList = FindGOByValueType(cards,(CardManager.ValueType)allCanOutCardsList[0]); //�����пɳ��Ƶļ�����ѡ����Сֵ����GameObject
        }
        //��ǰ������û�з��ϵĵ���
        //else
        //{
        //    List<GameObject> min01CanOutCardsList = GetMinDoubleCanOutCards(cards,thanMaxValueType,false); //û�е������˫�ơ��������л�ȡ����ֱ��ը��
        //    //����Ƕ��ƻ������ƣ���ȡ������һ�ſ�����Ϊ����
        //    if(min01CanOutCardsList.Count==2 || min01CanOutCardsList.Count==3)
        //    {
        //        minCanOutCardsList.Add(min01CanOutCardsList[0]); //����ֻ��Ҫ�������һ�ſ��Ƽ���
        //    }
        //    else
        //    {
        //        minCanOutCardsList = min01CanOutCardsList; //��ը������ը����null��ֵ����С����
        //    }
        //}
        return minCanOutCardsList;
    }

    /// <summary>
    /// ��ȡ��ǰ��ҿ����п��Գ�����С˳��
    /// </summary>
    /// <param name="cards">��ǰ��ҿ���</param>
    /// <param name="thanMaxValueType">��Ҫ�ȶԵ����ֵ</param>
    /// <param name="count">˳�ӿ��Ƹ���</param>
    /// <returns></returns>
    private List<GameObject> GetMinStraightCanOutCards(List<GameObject> cards,int thanMaxValueType,int count)
    {
        int thanMinValueType = thanMaxValueType - count + 1; //��ȡ˳������С���Ƶ�ֵ��int��
        //Debug.Log("��ȡ��ǰ��ҿ����п��Գ�����С˳�ӣ�thanMinValueType="+thanMinValueType);
        Dictionary<GameObject, int> cardsValueTypeGODict = new Dictionary<GameObject, int>(); //�洢���ƶ�Ӧ��ValueType��ת����int��
        List<int> cardsValueTypeList = new List<int>(); //�洢�����еĿ���ֵת����int
        //Dictionary<CardManager.ValueType, int> cardsValueTypeCountDict = new Dictionary<CardManager.ValueType, int>(); //�洢����ֵ��Ӧ������
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //���ڴ洢�ɳ��Ŀ���

        //��ȡ���������ж�Ӧ��ValueTypeת����int�����ظ�������ȡ�������ƶ�Ӧ��ֵת����int
        foreach (GameObject item in cards)
        {
            int iValueType = (int)CardManager.instance.cardsVDict[item];
            //�����ǰ����ֵ�����в������ÿ���ֵ�������ÿ���ֵ
            if (!cardsValueTypeList.Contains(iValueType))
            {
                cardsValueTypeList.Add(iValueType);
            }

            cardsValueTypeGODict.Add(item, iValueType); //��ǰ���Ƽ���Ӧ��ValueType
        }
        cardsValueTypeList.Sort(); //���ϰ���ֵ��С��������

        int minCanOutValueType = -1; //���ڴ洢�ɳ���˳���е���Сֵ��Ĭ������Ϊ-1
        //�Ե�ǰ��������ֵ����ѭ��������Ҫ�ȶԵ���Сֵ��ʼѭ�������������ֵ������ѭ����ʼֵΪ �ȶ���Сֵ������ֵ������Сֵ ����֮������ֵ��
        for (int i=thanMinValueType>cardsValueTypeList[0]?thanMinValueType:cardsValueTypeList[0];i<=cardsValueTypeList[cardsValueTypeList.Count-1];)
        {
            //�������ֵ�����в�������ֵ������С�ڻ������һ�������е���Сֵ,�������һ��ѭ��
            if (!cardsValueTypeList.Contains(i) || i<=thanMinValueType)
            {
                i++;
                continue;
            }
            //Debug.Log("i="+i);
            int currentCount = 0; //��ǰ������˳�Ӹ���
            //������i��ʼ������˳�Ӹ����������ֵ
            for (int j=i;j<i+count;j++)
            {
                //�����ǰ����ֵ���Ƿ����������ֵ������
                if (cardsValueTypeList.Contains(j) && j != (int)CardManager.ValueType.TWO)
                {
                    //Debug.Log("j="+j);
                    currentCount++;
                    if (currentCount==count)
                    {
                        minCanOutValueType = i;
                        i = cardsValueTypeList[cardsValueTypeList.Count-1]+1; //ͨ������i��ֵ�˳�����ѭ��
                        break; //�˳���ǰѭ��
                    }
                }
                else
                {
                    i = j + 1; //��i���õ�˳�Ӷ���λ�õ���һ����
                    break;
                }
            }
        }

        //����з��ϵ�˳��
        if (minCanOutValueType!=-1)
        {
            //�����ϵĿ�����һ����minCanOutCardsList��
            for (int i=0;i<count;i++)
            {
                //Debug.Log("minCanOutValueType="+minCanOutValueType);
                var gainCardsByType = cardsValueTypeGODict.Where(q => q.Value == minCanOutValueType).Select(q => q.Key); //��cardsValueTypeGODict�в��ҷ���ValueTypeֵ�����п���
                foreach (var item in gainCardsByType)
                {
                    minCanOutCardsList.Add(item); //˳�ӽ�������һ�ſ���
                    break;
                }
                minCanOutValueType++; //�洢��һ��ֵ�Ŀ���
            }
        }

        //foreach (GameObject item in minCanOutCardsList)
        //{
        //    Debug.Log("��ȡ��С˳�ӣ�minCanOutCardsList=" + item);
        //}
        return minCanOutCardsList;
    }

    /// <summary>
    /// ��ȡ��ǰ��ҿ����п��Գ�����С����
    /// </summary>
    /// <param name="cards">��ǰ��ҿ���</param>
    /// <param name="thanMaxValueType">���бȶԵ����ֵ</param>
    /// <param name="isNeedAuxiliary">�Ƿ���Ҫ�����������г�ʼ��</param>
    /// <returns></returns>
    private List<GameObject> GetMinDoubleCanOutCards(List<GameObject> cards, int thanMaxValueType/*, bool isNeedAuxiliary*/)
    {
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //���ڴ洢�ɳ��Ŀ���
        //�ж��Ƿ���Ҫ�����������г�ʼ��
        //if (isNeedAuxiliary)
        //{
        //    CardsSelectAuxiliary(cards); //���ø�������
        //}
        //ɸѡ������һ�������������ֵ�����ֵ���洢��allCanOutCardsList������
        SetAllCanOutCardsListByTraverseDict(OUT_CARD_TYPE.DOUBLE, thanMaxValueType);
        //��ǰ�������з��ϵĶ���ʱ
        if (allCanOutCardsList.Count != 0)
        {
            minCanOutCardsList = FindGOByValueType(cards, (CardManager.ValueType)allCanOutCardsList[0]); //�����пɳ��Ƶļ�����ѡ����Сֵ����GameObject
        }
        //else
        //{
        //    List<GameObject> min01CanOutCardsList = GetMinThreeCanOutCards(cards, thanMaxValueType, false); //û��˫����������л�ȡ����ֱ��ը����������ը
        //    //���������
        //    if (min01CanOutCardsList.Count == 3)
        //    {
        //        minCanOutCardsList.Add(min01CanOutCardsList[0]); //����ֻ��Ҫ����������ſ��Ƽ���
        //        minCanOutCardsList.Add(min01CanOutCardsList[1]);
        //    }
        //    //ը������ը����Null
        //    else
        //    {
        //        minCanOutCardsList = min01CanOutCardsList; //��ը������ը��Nullֱ�Ӹ�ֵ����С����
        //    }
        //}
        return minCanOutCardsList;
    }

    /// <summary>
    /// ��ȡ��ǰ��ҿ����п��Գ�����С����
    /// </summary>
    /// <param name="cards">��ҿ���</param>
    /// <param name="thanMaxValueType">��Ҫ�ȶԵ����ֵ</param>
    /// <param name="lastCount">��һ����������</param>
    /// <returns></returns>
    private List<GameObject> GetMinPairsCanOutCards(List<GameObject> cards, int thanMaxValueType, int lastCount)
    {
        int thanMinValueType = thanMaxValueType - lastCount/2 + 1; //��ȡ˳��������С���Ƶ�ֵ��int��
        Dictionary<GameObject, int> cardsValueTypeGODict = new Dictionary<GameObject, int>(); //�洢���ƶ�Ӧ��ValueType��ת����int��
        List<int> cardsValueTypeList = new List<int>(); //�洢�����з��������Ŀ���ֵת����int
        Dictionary<CardManager.ValueType, int> cardsValueTypeCountDict = new Dictionary<CardManager.ValueType, int>(); //�洢����ֵ��Ӧ������
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //���ڴ洢�ɳ��Ŀ���

        cardsValueTypeCountDict = CardManager.instance.rememberCardValueCount.GetCardsValueCount(cards); //��ȡ����ֵ��Ӧ������
        //��ȡ���������ж�Ӧ��ValueTypeת����int�����ظ�������ȡ�������ƶ�Ӧ��ֵת����int
        foreach (GameObject item in cards)
        {
            int iValueType = (int)CardManager.instance.cardsVDict[item];
            //�����ǰ����ֵ������>=2 �� <=3 ����ȥը���Ŀ����ԣ��� ��ǰ����ֵ�����в������ÿ���ֵ�������ÿ���ֵ
            if (cardsValueTypeCountDict[(CardManager.ValueType)iValueType]>=2 && cardsValueTypeCountDict[(CardManager.ValueType)iValueType] <=3 && !cardsValueTypeList.Contains(iValueType))
            {
                cardsValueTypeList.Add(iValueType);
            }
            cardsValueTypeGODict.Add(item, iValueType);
        }
        cardsValueTypeList.Sort(); //���ϰ���ֵ��С��������

        //��������з��ϵĶ���ֵ����С����Ҫ�Ķ���ֵ���������ߣ�����ֵ���ϵ����ֵ-�ȶԵ���Сֵ��< �����ֵ���ֱ�ӷ��ؿռ���
        if (cardsValueTypeList.Count < (lastCount/2) || cardsValueTypeList[cardsValueTypeList.Count-1]-thanMinValueType < (lastCount/2))
        {
            return minCanOutCardsList;
        }

        int minCanOutValueType = -1; //���ڴ洢�ɳ��������е���Сֵ��Ĭ������Ϊ-1
        //�Ե�ǰ��������ֵ����ѭ��������Ҫ�ȶԵ���Сֵ��ʼѭ�������������ֵ������ѭ����ʼֵΪ �ȶ���Сֵ������ֵ������Сֵ ����֮������ֵ����i�����ڱȽϵ���ֵ��
        for (int i=thanMinValueType>cardsValueTypeList[0]?thanMinValueType:cardsValueTypeList[0]; i<=cardsValueTypeList[cardsValueTypeList.Count-1];)
        {
            //�����ǰ����ֵ���ϲ������ÿ���ֵ���������һ��ѭ��
            if (!cardsValueTypeList.Contains(i) || i <= thanMinValueType)
            {
                i++;
                continue;
            }

            int currentCount = 0; //���ڼ�����ǰ���ϵĶ�������
            //��ǰ���ư���i����j��i��ʼ������ֱ����Ҫ�����ֵ���鿴�Ƿ����е�ÿ��ֵcardsValueTypeList���а���������Ϊ2��
            for (int j=i;j<i+lastCount/2;j++)
            {
                if (cardsValueTypeList.Contains(j) && j!=(int)CardManager.ValueType.TWO)
                {
                    currentCount++;
                    //�����ǰ����������Ҫ�������򽫵�ǰ��������Сֵ��ֵ��minCanOutValueType��Ȼ������ѭ��
                    if (currentCount==lastCount/2)
                    {
                        minCanOutValueType = i;
                        i = cardsValueTypeList[cardsValueTypeList.Count - 1] + 1; //�������������forѭ��
                        break; //������ǰforѭ��
                    }
                }
                else
                {
                    i = j + 1; //����i����һ��ѭ��ֵ��ֱ��������jѭ��ʱ�Ѿ��жϹ���ֵ��
                    break;
                }
            }
        }

        //����з��ϵ�����
        if (minCanOutValueType != -1)
        {
            //�����ϵĿ�����һ����minCanOutCardsList��
            for (int i = 0; i < lastCount/2; i++)
            {
                var gainCardsByType = cardsValueTypeGODict.Where(q => q.Value == minCanOutValueType).Select(q => q.Key); //��cardsValueTypeGODict�в��ҷ���ValueTypeֵ�����п���
                int currentC = 0; //�洢��ǰ�Ѿ�����ĸ�ֵ���Ƹ���
                foreach (var item in gainCardsByType)
                {
                    minCanOutCardsList.Add(item); //������Ҫ�������ſ���
                    currentC++;
                    if (currentC==2)
                    {
                        break;
                    }
                }
                minCanOutValueType++; //�洢��һ��ֵ�Ŀ���
            }
        }

        return minCanOutCardsList;
    }

    /// <summary>
    /// ��ȡ��ǰ��ҿ����п��Գ�����С����
    /// </summary>
    /// <param name="cards">��ǰ��ҿ���</param>
    /// <param name="thanMaxValueType">���бȶԵ����ֵ</param>
    /// <param name="isNeedAuxiliary">�Ƿ���Ҫ�����������г�ʼ��</param>
    /// <returns></returns>
    private List<GameObject> GetMinThreeCanOutCards(List<GameObject> cards,int thanMaxValueType/*,bool isNeedAuxiliary*/)
    {
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //���ڴ洢�ɳ��Ŀ���
        //�ж��Ƿ���Ҫ�����������г�ʼ��
        //if (isNeedAuxiliary)
        //{
        //    CardsSelectAuxiliary(cards); //���ø�������
        //}
        //ɸѡ������һ�������������ֵ�����ֵ���洢��allCanOutCardsList������
        SetAllCanOutCardsListByTraverseDict(OUT_CARD_TYPE.THREE_WITHOUT, thanMaxValueType);
        //��ǰ�������з��ϵ�����ʱ
        if (allCanOutCardsList.Count != 0)
        {
            minCanOutCardsList = FindGOByValueType(cards, (CardManager.ValueType)allCanOutCardsList[0]); //�����пɳ��Ƶļ�����ѡ����Сֵ����GameObject
        }
        //else
        //{
        //    //ֱ�ӽ�ը������ը����null��ֵ������
        //    minCanOutCardsList = GetMinBombCanOutCards(cards, -1, false); //ը������ը����Ҫ�ȶ�ValueType
        //}
        return minCanOutCardsList;
    }
    
    /// <summary>
    /// ��ȡ��ǰ��ҿ����п��Գ�����С�ɻ�����
    /// </summary>
    /// <param name="cards">��ǰ��ҿ���</param>
    /// <param name="thanMaxValueType">�ȶԵ����ֵ</param>
    /// <param name="count">��һ��������</param>
    /// <returns></returns>
    private List<GameObject> GetMinPlanesWithoutCanOutCards(List<GameObject> cards,int thanMaxValueType,int lastCount)
    {

        int thanMinValueType = thanMaxValueType - lastCount / 3 + 1; //��ȡ�ɻ�����С���Ƶ�ֵ��int��
        Dictionary<GameObject, int> cardsValueTypeGODict = new Dictionary<GameObject, int>(); //�洢���ƶ�Ӧ��ValueType��ת����int��
        List<int> cardsValueTypeList = new List<int>(); //�洢�����з��������Ŀ���ֵת����int
        Dictionary<CardManager.ValueType, int> cardsValueTypeCountDict = new Dictionary<CardManager.ValueType, int>(); //�洢����ֵ��Ӧ������
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //���ڴ洢�ɳ��Ŀ���

        cardsValueTypeCountDict = CardManager.instance.rememberCardValueCount.GetCardsValueCount(cards); //��ȡ����ֵ��Ӧ������
        //��ȡ���������ж�Ӧ��ValueTypeת����int�����ظ�������ȡ�������ƶ�Ӧ��ֵת����int
        foreach (GameObject item in cards)
        {
            int iValueType = (int)CardManager.instance.cardsVDict[item];
            //�����ǰ����ֵ������=3 ����ȥը���Ŀ����ԣ��� ��ǰ����ֵ�����в������ÿ���ֵ�������ÿ���ֵ
            if (cardsValueTypeCountDict[(CardManager.ValueType)iValueType] == 3 && !cardsValueTypeList.Contains(iValueType))
            {
                cardsValueTypeList.Add(iValueType);
            }
            cardsValueTypeGODict.Add(item, iValueType);
        }
        cardsValueTypeList.Sort(); //���ϰ���ֵ��С��������

        //��������з��ϵĶ���ֵ����С����Ҫ�Ķ���ֵ���������ߣ�����ֵ���ϵ����ֵ-�ȶԵ���Сֵ��< �����ֵ���ֱ�ӷ��ؿռ���
        if (cardsValueTypeList.Count < (lastCount / 3) || cardsValueTypeList[cardsValueTypeList.Count - 1] - thanMinValueType < (lastCount / 3))
        {
            return minCanOutCardsList;
        }

        int minCanOutValueType = -1; //���ڴ洢�ɳ��Ʒɻ��е���Сֵ��Ĭ������Ϊ-1
        //�Ե�ǰ��������ֵ����ѭ��������Ҫ�ȶԵ���Сֵ��ʼѭ�������������ֵ������ѭ����ʼֵΪ �ȶ���Сֵ������ֵ������Сֵ ����֮������ֵ��
        for (int i = thanMinValueType > cardsValueTypeList[0] ? thanMinValueType : cardsValueTypeList[0]; i <= cardsValueTypeList[cardsValueTypeList.Count - 1];)
        {
            //�����ǰ����ֵ���ϲ������ÿ���ֵ���������һ��ѭ��
            if (!cardsValueTypeList.Contains(i))
            {
                i++;
                continue;
            }

            int currentCount = 0; //���ڼ�����ǰ���ϵĶ�������
            //��ǰ���ư���i����j��i��ʼ������ֱ����Ҫ�����ֵ���鿴�Ƿ����е�ÿ��ֵcardsValueTypeList���а���
            for (int j = i; j < i + lastCount / 3; j++)
            {
                if (cardsValueTypeList.Contains(j) && j != (int)CardManager.ValueType.TWO)
                {
                    currentCount++;
                    //�����ǰ����������Ҫ�������򽫵�ǰ��������Сֵ��ֵ��minCanOutValueType��Ȼ������ѭ��
                    if (currentCount == lastCount / 3)
                    {
                        minCanOutValueType = i;
                        i = cardsValueTypeList[cardsValueTypeList.Count - 1] + 1; //�������������forѭ��
                        break; //������ǰforѭ��
                    }
                }
                else
                {
                    i = j + 1; //����i����һ��ѭ��ֵ��ֱ��������jѭ��ʱ�Ѿ��жϹ���ֵ��
                    break;
                }
            }
        }

        //����з��ϵķɻ�
        if (minCanOutValueType != -1)
        {
            //�����ϵĿ�����һ����minCanOutCardsList��
            for (int i = 0; i < lastCount/3; i++)
            {
                var gainCardsByType = cardsValueTypeGODict.Where(q => q.Value == minCanOutValueType).Select(q => q.Key); //��cardsValueTypeGODict�в��ҷ���ValueTypeֵ�����п���
                foreach (var item in gainCardsByType)
                {
                    minCanOutCardsList.Add(item); //�����Ƽ�����ӵ�minCanOutCardsList��
                }
                minCanOutValueType++; //�洢��һ��ֵ�Ŀ���
            }
        }

        return minCanOutCardsList;
    }

    /// <summary>
    /// ��ȡ��ǰ��ҿ����п��Գ�����С�ɻ�����
    /// </summary>
    /// <param name="cards">��ǰ��ҿ���</param>
    /// <param name="thanMaxValueType">�ȶԵ����ֵ</param>
    /// <param name="lastCount">��һ��������</param>
    /// <returns></returns>
    private List<GameObject> GetMinPlanesWithSingleCanOutCards(List<GameObject> cards, int thanMaxValueType, int lastCount)
    {
        int thanMinValueType = thanMaxValueType - lastCount / 4 + 1; //��ȡ�ɻ�����С���Ƶ�ֵ��int��
        Dictionary<GameObject, int> cardsValueTypeGODict = new Dictionary<GameObject, int>(); //�洢���ƶ�Ӧ��ValueType��ת����int��
        List<int> cardsValueTypeList = new List<int>(); //�洢�����з��������Ŀ���ֵת����int
        List<int> cardsWithValueTypeList = new List<int>(); //�洢�����з��ϴ��������Ŀ���ֵת����int
        Dictionary<CardManager.ValueType, int> cardsValueTypeCountDict = new Dictionary<CardManager.ValueType, int>(); //�洢����ֵ��Ӧ������
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //���ڴ洢�ɳ��Ŀ���
        List<GameObject> minWithCanOutCardsList = new List<GameObject>(); //���ڴ洢���ƵĿ���

        cardsValueTypeCountDict = CardManager.instance.rememberCardValueCount.GetCardsValueCount(cards); //��ȡ����ֵ��Ӧ������
        //��ȡ���������ж�Ӧ��ValueTypeת����int�����ظ�������ȡ�������ƶ�Ӧ��ֵת����int
        foreach (GameObject item in cards)
        {
            int iValueType = (int)CardManager.instance.cardsVDict[item]; //��ȡ��ǰ���ƵĿ���ֵ

            // �洢�����������ֵ�������ǰ����ֵ������=3 ����ȥը���Ŀ����ԣ��� ��ǰ����ֵ�����в������ÿ���ֵ �� ����ֵ����Ҫ�ȶԵ���Сֵ
            if (cardsValueTypeCountDict[(CardManager.ValueType)iValueType] == 3 &&  !cardsValueTypeList.Contains(iValueType) && iValueType > thanMinValueType)
            {
                cardsValueTypeList.Add(iValueType);
            }
            //�洢����ֵ
            else
            {
                //��ȥը���Ŀ��ܣ�ȡ�������ڴ��ƵĿ��ƣ�����ȥ���ֵ���Ƽ������Ѵ�ֵ�����ƣ�
                if (cardsValueTypeCountDict[(CardManager.ValueType)iValueType]!=4 && !cardsWithValueTypeList.Contains(iValueType) && !cardsValueTypeList.Contains(iValueType))
                {
                    cardsWithValueTypeList.Add(iValueType);
                }
            }

            cardsValueTypeGODict.Add(item, iValueType); //��� ����-����ֵ �ֵ���
        }

        cardsValueTypeList.Sort(); //�����������ֵ���ϰ���ֵ��С��������
        cardsWithValueTypeList.Sort(); //���ƿ���ֵ���ϰ���ֵ��С��������

        //��������з��ϵķɻ�����С����Ҫ�ķɻ����������ߣ�����ֵ���ϵ����ֵ-�ȶԵ���Сֵ��< �����ֵ���ֱ�ӷ��ؿռ��ϣ����ߴ�������������
        if (cardsValueTypeList.Count < (lastCount / 4) || cardsValueTypeList[cardsValueTypeList.Count - 1] - thanMinValueType < (lastCount / 4) || cardsWithValueTypeList.Count<(lastCount/4))
        {
            return minCanOutCardsList;
        }

        int minCanOutValueType = -1; //���ڴ洢�ɳ��Ʒɻ��е���Сֵ��Ĭ������Ϊ-1
        //�Ե�ǰ��������ֵ����ѭ��������Ҫ�ȶԵ���Сֵ��ʼѭ�������������ֵ������ѭ����ʼֵΪ �ȶ���Сֵ������ֵ������Сֵ ����֮������ֵ��
        for (int i = cardsValueTypeList[0]; i <= cardsValueTypeList[cardsValueTypeList.Count - 1];)
        {
            //�����ǰ����ֵ���ϲ������ÿ���ֵ���������һ��ѭ��
            if (!cardsValueTypeList.Contains(i))
            {
                i++;
                continue;
            }

            int currentCount = 0; //���ڼ�����ǰ���ϵķɻ�����
            //��ǰ���ư���i����j��i��ʼ������ֱ����Ҫ�����ֵ���鿴�Ƿ����е�ÿ��ֵcardsValueTypeList���а���
            for (int j = i; j < i + lastCount / 4; j++)
            {
                if (cardsValueTypeList.Contains(j) && j != (int)CardManager.ValueType.TWO)
                {
                    currentCount++;
                    //�����ǰ�ɻ�����������Ҫ�����򽫵�ǰ��������Сֵ��ֵ��minCanOutValueType��Ȼ������ѭ��
                    if (currentCount == lastCount / 4)
                    {
                        minCanOutValueType = i;
                        i = cardsValueTypeList[cardsValueTypeList.Count - 1] + 1; //�������������forѭ��
                        break; //������ǰforѭ��
                    }
                }
                else
                {
                    i = j + 1; //����i����һ��ѭ��ֵ��ֱ��������jѭ��ʱ�Ѿ��жϹ���ֵ��
                    break;
                }
            }
        }

        //����з��ϵķɻ���
        if (minCanOutValueType != -1)
        {
            //�����ϵĿ�����һ����minCanOutCardsList��
            for (int i = 0; i < lastCount/4; i++)
            {
                var gainCardsByType = cardsValueTypeGODict.Where(q => q.Value == minCanOutValueType).Select(q => q.Key); //��cardsValueTypeGODict�в��ҷ���ValueTypeֵ�����п���
                foreach (var item in gainCardsByType)
                {
                    minCanOutCardsList.Add(item); //�����Ƽ�����ӵ�minCanOutCardsList��
                }
                minCanOutValueType++; //�洢��һ��ֵ�Ŀ���
            }

            //��Ӵ���
            int withCount = 0; //���㵱ǰ�洢�Ĵ�������
            for (int i=0;i<lastCount/4;i++)
            {
                var gainWithCardsByType = cardsValueTypeGODict.Where(q => q.Value == cardsWithValueTypeList[i]).Select(q => q.Key); //��cardsValueTypeGODict�в��ҷ���ValueTypeֵ�����п���
                foreach (var item in gainWithCardsByType)
                {
                    minCanOutCardsList.Add(item); //��Ӵ��Ƶ�minCanCardsList��
                    withCount++;
                    if (withCount == lastCount / 4)
                    {
                        i = lastCount / 4; //����������ѭ��
                        break; //������ѭ��
                    }
                }
            }
        }

        return minCanOutCardsList;
    }

    /// <summary>
    /// ��ȡ��ǰ��ҿ����п��Գ�����С�ɻ�����
    /// </summary>
    /// <param name="cards">��ǰ��ҿ���</param>
    /// <param name="thanMaxValueType">�ȶԵ����ֵ</param>
    /// <param name="lastCount">��һ��������</param>
    /// <returns></returns>
    private List<GameObject> GetMinPlanesWithDoubleCanOutCards(List<GameObject> cards, int thanMaxValueType, int lastCount)
    {
        int thanMinValueType = thanMaxValueType - lastCount / 5 + 1; //��ȡ�ɻ�����С���Ƶ�ֵ��int��
        Dictionary<GameObject, int> cardsValueTypeGODict = new Dictionary<GameObject, int>(); //�洢���ƶ�Ӧ��ValueType��ת����int��
        List<int> cardsValueTypeList = new List<int>(); //�洢�����з��������Ŀ���ֵת����int
        List<int> cardsWithValueTypeList = new List<int>(); //�洢�����з��ϴ��������Ŀ���ֵת����int
        Dictionary<CardManager.ValueType, int> cardsValueTypeCountDict = new Dictionary<CardManager.ValueType, int>(); //�洢����ֵ��Ӧ������
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //���ڴ洢�ɳ��Ŀ���
        List<GameObject> minWithCanOutCardsList = new List<GameObject>(); //���ڴ洢���ƵĿ���

        cardsValueTypeCountDict = CardManager.instance.rememberCardValueCount.GetCardsValueCount(cards); //��ȡ����ֵ��Ӧ������
        //��ȡ���������ж�Ӧ��ValueTypeת����int�����ظ�������ȡ�������ƶ�Ӧ��ֵת����int
        foreach (GameObject item in cards)
        {
            int iValueType = (int)CardManager.instance.cardsVDict[item]; //��ȡ��ǰ���ƵĿ���ֵ

            // �洢�����������ֵ�������ǰ����ֵ������=3 ����ȥը���Ŀ����ԣ��� ��ǰ����ֵ�����в������ÿ���ֵ �� ����ֵ����Ҫ�ȶԵ���Сֵ
            if (cardsValueTypeCountDict[(CardManager.ValueType)iValueType] == 3 && !cardsValueTypeList.Contains(iValueType) && iValueType > thanMinValueType)
            {
                cardsValueTypeList.Add(iValueType);
            }
            //�洢����ֵ
            else
            {
                //��ȥը���Ŀ��ܣ�ȡ�������ڴ��ƵĿ��ƣ����ơ����ƣ�����ȥ�����������ֵ���Ѵ�ֵ�����ƣ�
                if (cardsValueTypeCountDict[(CardManager.ValueType)iValueType]>=2 && !cardsWithValueTypeList.Contains(iValueType) && !cardsValueTypeList.Contains(iValueType) && cardsValueTypeCountDict[(CardManager.ValueType)iValueType] != 4)
                {
                    cardsWithValueTypeList.Add(iValueType);
                }
            }

            cardsValueTypeGODict.Add(item, iValueType); //��� ����-����ֵ �ֵ���
        }

        cardsValueTypeList.Sort(); //�����������ֵ���ϰ���ֵ��С��������
        cardsWithValueTypeList.Sort(); //���ƿ���ֵ���ϰ���ֵ��С��������

        //foreach (var item in cardsValueTypeList)
        //{
        //    Debug.Log("�ɻ����ԣ�cardsValueTypeList.item="+item);
        //}
        //foreach (var item in cardsWithValueTypeList)
        //{
        //    Debug.Log("�ɻ����ԣ�cardsWithValueTypeList.item=" + item);
        //}

        //��������з��ϵķɻ�����С����Ҫ�ķɻ����������ߣ�����ֵ���ϵ����ֵ-�ȶԵ���Сֵ��< �����ֵ���ֱ�ӷ��ؿռ��ϣ����ߴ�������������
        if (cardsValueTypeList.Count < (lastCount / 5) || cardsValueTypeList[cardsValueTypeList.Count - 1] - thanMinValueType < (lastCount / 5) || cardsWithValueTypeList.Count < (lastCount / 5))
        {
            return minCanOutCardsList;
        }

        int minCanOutValueType = -1; //���ڴ洢�ɳ��Ʒɻ��е���Сֵ��Ĭ������Ϊ-1
        //�Ե�ǰ��������ֵ����ѭ��������Ҫ�ȶԵ���Сֵ��ʼѭ�������������ֵ������ѭ����ʼֵΪ �ȶ���Сֵ������ֵ������Сֵ ����֮������ֵ��
        for (int i = cardsValueTypeList[0]; i <= cardsValueTypeList[cardsValueTypeList.Count - 1];)
        {
            //�����ǰ����ֵ���ϲ������ÿ���ֵ���������һ��ѭ��
            if (!cardsValueTypeList.Contains(i))
            {
                i++;
                continue;
            }

            int currentCount = 0; //���ڼ�����ǰ���ϵķɻ�����
            //��ǰ���ư���i����j��i��ʼ������ֱ����Ҫ�����ֵ���鿴�Ƿ����е�ÿ��ֵcardsValueTypeList���а���
            for (int j = i; j < i + lastCount / 5; j++)
            {
                if (cardsValueTypeList.Contains(j) && j != (int)CardManager.ValueType.TWO)
                {
                    currentCount++;
                    //�����ǰ�ɻ�����������Ҫ�����򽫵�ǰ��������Сֵ��ֵ��minCanOutValueType��Ȼ������ѭ��
                    if (currentCount == lastCount / 5)
                    {
                        minCanOutValueType = i;
                        i = cardsValueTypeList[cardsValueTypeList.Count - 1] + 1; //�������������forѭ��
                        break; //������ǰforѭ��
                    }
                }
                else
                {
                    i = j + 1; //����i����һ��ѭ��ֵ��ֱ��������jѭ��ʱ�Ѿ��жϹ���ֵ��
                    break;
                }
            }
        }

        //����з��ϵķɻ���
        if (minCanOutValueType != -1)
        {
            //�����ϵĿ�����һ����minCanOutCardsList��
            for (int i = 0; i < lastCount / 5; i++)
            {
                var gainCardsByType = cardsValueTypeGODict.Where(q => q.Value == minCanOutValueType).Select(q => q.Key); //��cardsValueTypeGODict�в��ҷ���ValueTypeֵ�����п���
                foreach (var item in gainCardsByType)
                {
                    minCanOutCardsList.Add(item); //�����Ƽ�����ӵ�minCanOutCardsList��
                }
                minCanOutValueType++; //�洢��һ��ֵ�Ŀ���
            }

            //��Ӵ���
            int withDoubleCount = 0; //���㵱ǰ�洢���ƶ���
            //int withCount = 0; //���㵱ǰ�洢������
            for (int i = 0; i < lastCount / 5; i++)
            {
                var gainWithCardsByType = cardsValueTypeGODict.Where(q => q.Value == cardsWithValueTypeList[i]).Select(q => q.Key); //��cardsValueTypeGODict�в��ҷ���ValueTypeֵ�����п���
                foreach (var item in gainWithCardsByType)
                {
                    minCanOutCardsList.Add(item); //��Ӵ��Ƶ�minCanCardsList��
                    withDoubleCount++;
                    if (withDoubleCount == 2)
                    {
                        //withCount++;
                        //�������㹻
                        //if (withCount==lastCount/5)
                        //{
                        //    i = lastCount / 5; //����������ѭ��
                        //}
                        withDoubleCount = 0;
                        break; //������ѭ��
                    }
                }
            }
        }

        return minCanOutCardsList;
    }

    /// <summary>
    /// ��ȡ��ǰ��ҿ����п��Գ�����Сը��
    /// </summary>
    /// <param name="cards">��ǰ��ҿ���</param>
    /// <param name="thanMaxValueType">���бȶԵ����ֵ</param>
    /// <param name="isNeedAuxiliary">�Ƿ���Ҫ�����������г�ʼ��</param>
    /// <returns></returns>
    private List<GameObject> GetMinBombCanOutCards(List<GameObject> cards, int thanMaxValueType/*, bool isNeedAuxiliary*/)
    {
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //���ڴ洢�ɳ��Ŀ���
        //�ж��Ƿ���Ҫ�����������г�ʼ��
        //if (isNeedAuxiliary)
        //{
        //    CardsSelectAuxiliary(cards); //���ø�������
        //}
        //ɸѡ������һ�������������ֵ�����ֵ���洢��allCanOutCardsList������
        SetAllCanOutCardsListByTraverseDict(OUT_CARD_TYPE.BOMBS, thanMaxValueType);
        //��ǰ�������з��ϵ�ը��ʱ
        if (allCanOutCardsList.Count != 0)
        {
            minCanOutCardsList = FindGOByValueType(cards, (CardManager.ValueType)allCanOutCardsList[0]); //�����пɳ��Ƶļ�����ѡ����Сֵ����GameObject
        }
        //else
        //{
        //    //ֱ�ӽ���ը����null��ֵ������
        //    minCanOutCardsList = GetKingExplosionCanOutCards(cards, -1, false); //��ը����Ҫ�ȶ�ValueType
        //}
        return minCanOutCardsList;
    }

    /// <summary>
    /// ��ȡ��ǰ��ҿ����п��Գ�����ը
    /// </summary>
    /// <param name="cards">��ǰ��ҿ���</param>
    /// <param name="thanMaxValueType">���бȶԵ����ֵ</param>
    /// <param name="isNeedAuxiliary">�Ƿ���Ҫ�����������г�ʼ��</param>
    /// <returns></returns>
    private List<GameObject> GetKingExplosionCanOutCards(List<GameObject> cards/*, int thanMaxValueType*//*, bool isNeedAuxiliary*/)
    {
        //Debug.Log("��ȡ��ը");
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //���ڴ洢�ɳ��Ŀ���
        //����С��������null�ļ��Ϻϲ���minCanOutCardsList������
        minCanOutCardsList = minCanOutCardsList.Concat(FindGOByValueType(cards, CardManager.ValueType.BLACKJOKER)).ToList<GameObject>();
        //���д���������null�ļ��Ϻϲ���minCanOutCardsList������
        minCanOutCardsList = minCanOutCardsList.Concat(FindGOByValueType(cards,CardManager.ValueType.REDJOKER)).ToList<GameObject>();
        //foreach (GameObject go in minCanOutCardsList)
        //{
        //    Debug.Log("��ȡ��ը��"+go.name);
        //}
        //���ֻ�д�������С�����򽫸�Ԫ���Ƴ�
        if (minCanOutCardsList.Count==1)
        {
            minCanOutCardsList.Clear(); //��մ洢�������еĿ���
        }
        //������ը������null
        return minCanOutCardsList;  
    }

    /// <summary>
    /// ͨ�������ֵ䣬�����п��Գ��ƵĿ���ֵת��Ϊint���뼯��AllCanOutCardsList��
    /// ���Լ���AllCanOutCardsList��������
    /// </summary>
    /// <param name="outCardType">��������</param>
    /// <param name="thanMaxValueType">valueType���бȶԵ���ֵ</param>
    private void SetAllCanOutCardsListByTraverseDict(OUT_CARD_TYPE outCardType, int thanMaxValueType)
    {
        //��տɳ�����ת����int�Ŀ���ֵ���ϣ������һ�����ݣ�
        if (allCanOutCardsList.Count != 0)
        {
            allCanOutCardsList.Clear();
        }
        int count = 0; //������������Ӧֵ����
        //���ݳ������ͣ�����ɸѡ��ֵ��������ֵ
        switch (outCardType)
        {
            case OUT_CARD_TYPE.SINGLE:
            case OUT_CARD_TYPE.STRAIGHT:
            case OUT_CARD_TYPE.KING_EXPLOSIONS:
                count = 1;
                break;
            case OUT_CARD_TYPE.DOUBLE:
            case OUT_CARD_TYPE.PAIRS:
                count = 2;
                break;
            case OUT_CARD_TYPE.THREE_WITHOUT:
            case OUT_CARD_TYPE.THREE_WITH_ONE:
            case OUT_CARD_TYPE.THREE_WITH_TWO:
            case OUT_CARD_TYPE.PLANES_WITHOUT:
            case OUT_CARD_TYPE.PLANES_WITH_SINGLE:
            case OUT_CARD_TYPE.PLANES_WITH_DOUBLE:
                count = 3;
                break;
            case OUT_CARD_TYPE.BOMBS:
                count = 4;
                break;
        }

        //����ֵ�����ֵ�
        foreach (KeyValuePair<CardManager.ValueType, int> item in cardsValueCountDictAuxiliary)
        {
            //ɸѡ�� ֵ ���� ָ��ֵ �� ����item
            if (item.Value == count && (int)item.Key > thanMaxValueType)
            {
                allCanOutCardsList.Add((int)item.Key); 
            }
        }

        //���������ͬʱ����С�������������������ڼ�����ȥ��
        if (allCanOutCardsList.Contains((int)CardManager.ValueType.BLACKJOKER) && allCanOutCardsList.Contains((int)CardManager.ValueType.REDJOKER))
        {
            allCanOutCardsList.Remove((int)CardManager.ValueType.BLACKJOKER); //ȥ��С��
            allCanOutCardsList.Remove((int)CardManager.ValueType.REDJOKER); //ȥ������
        }

        //���з��ϵ�ֵʱ���Լ��Ͻ�������
        if (allCanOutCardsList.Count != 0)
        {
            allCanOutCardsList.Sort();
        }
    }

    /// <summary>
    /// ����valueType���ҿ����ж�Ӧ�Ŀ���
    /// </summary>
    /// <param name="cards">���ҵĿ���</param>
    /// <param name="valueType">���ҵ�ֵ</param>
    /// <returns>�����ڿ������ҵ���GameObject����</returns>
    private List<GameObject> FindGOByValueType(List<GameObject> cards,CardManager.ValueType valueType)
    {
        List<GameObject> gosList = new List<GameObject>(); //���ڴ洢�ҵ���GameObject
        //��cardsVDict�ֵ���ѡ���valueTypeֵ���͵Ŀ���
        var gos = CardManager.instance.cardsVDict.Where(q => q.Value == valueType).Select(q => q.Key);
        //foreach (var item in gos)
        //{
        //    Debug.Log("FindGOByValueType��"+item.name);
        //}
        foreach(var item in gos)
        {
            if (cards.Contains(item))
            {
                gosList.Add(item);
            }
        }
        return gosList;
    }



    /// <summary>
    /// ������һ��������Ϣ������ǰ�����Ŀ�����Ϣ��¼����һ��������Ϣ�У�
    /// ��¼�������͡��������ֵ��������ҡ���������
    /// <param name="cards">��ǰ�����ƵĿ���</param>
    /// <param name="cards">��ǰ�������</param>
    /// </summary>
    public void SetLastOutCardInfo(List<GameObject> cards,int outCardPlayer)
    {
        lastOutCardType = (int)GetOutCardType(cards); //��ȡ�������ͣ����������������Value��maxValueCurrentOutCardsMaxCount
        //Debug.Log("������һ��������Ϣ��lastOutCardType="+lastOutCardType);
        maxValueLastOutCardsMaxCount = (int)maxValueCurrentOutCardsMaxCount; //�� ��ǰ���Ƶ��������ֵ ���� ��һ�������������ֵ ��
        lastOutCardPlayer = outCardPlayer; //������һ���������
        lastOutCardCount = cards.Count; //������һ�����ƵĿ�������
        outCardsList.Clear(); //������ƿ��Ƽ���
    }

    /// <summary>
    /// �ж���ҿ����Ƿ���Գ��ƣ��������͡�����ֵ��>ͨ���ȶ���һ�����ƣ�
    /// </summary>
    /// <param name="cards">��ҳ��ƵĿ���</param>
    /// <param name="currentPlayer">��ǰ���</param>
    /// <returns>Ĭ�Ϸ���false</returns>
    public bool IsCanOutCards(List<GameObject> cards,int currentPlayer)
    {
        OUT_CARD_TYPE currentOutCardType = GetOutCardType(cards); //��ȡ��ǰ���ƿ��Ƶĳ�������
        //Debug.Log("�ж�����Ƿ���Գ��ƣ���������="+currentOutCardType);
        //��ǰ�������Ͳ��Ϸ�
        if (currentOutCardType == OUT_CARD_TYPE.INCONFORMITY) return false;

        // ��ǰ���Ϊ��һ��������� || ��ǰ�������Ϊ��һ���������
        if (lastOutCardType == -1 || currentPlayer == lastOutCardPlayer)  return true; //���Գ�����Ϸ����͵Ŀ���

        //�жϵ�ǰ��ҳ��������Ƿ������һ����������
        if ((int)currentOutCardType != lastOutCardType)
        {
            //�����ǰ���ƿ���Ϊը��������ը������Գ���
            if ((int)currentOutCardType==13 || (int)currentOutCardType==14)
            {
                return true;
            }
            return false;
        }
        //�жϵ�ǰ��ҳ������������ֵ�Ƿ������һ��
        if ((int)maxValueCurrentOutCardsMaxCount > maxValueLastOutCardsMaxCount) return true;

        return false;
    }
    /// <summary>
    /// ���ƿ�������
    /// </summary>
    /// <param name="cards">���ƿ���</param>
    public void SetOutCardsListSort(List<GameObject> cards)
    {
        List<GameObject> cardsCopy = new List<GameObject>(); //�����м�洢����
        Dictionary<CardManager.ValueType, int> cardsValueTypeCountDict = new Dictionary<CardManager.ValueType, int>(); //�洢ָ�����Ƹ���ֵ������
        Dictionary<GameObject, CardManager.ValueType> cardsValueTypeGODict = new Dictionary<GameObject, CardManager.ValueType>(); //�洢�������Ƶ�ֵ

        //�洢�������ƶ�Ӧ��ValueType
        foreach (GameObject item in cards)
        {
            if (!cardsValueTypeGODict.ContainsKey(item))
            {
                cardsValueTypeGODict.Add(item, CardManager.instance.cardsVDict[item]);

            }
            //Debug.Log("����ǰ�����ƣ�" + item.name);
        }

        cardsValueTypeCountDict =CardManager.instance.rememberCardValueCount.GetCardsValueCount(cards); //��ȡ�����и���ֵ�����������ֵ���
        List<int> countValuesCount = new List<int>(); //�洢�����и���ֵ������
        //����ֵ�ĸ����������뼯����
        foreach (int item in cardsValueTypeCountDict.Values)
        {
            //Debug.Log("���ƿ���������cardsValueTypeCountDict.Values=" + item);
            //һ������ֻ���һ��
            if (!countValuesCount.Contains(item))
            {
                countValuesCount.Add(item);
            }
        }
        countValuesCount.Sort(); //��������С��С��������
        //Debug.Log("countValuesToInt.Count="+countValuesToInt.Count);

        for(int i = countValuesCount.Count - 1; i > -1; i--)
        {
            var countValues = cardsValueTypeCountDict.Where(q => q.Value == countValuesCount[i]).Select(q => q.Key); //Ѱ��cardsValueTypeCountDict�ֵ���ָ����������Ӧ������ֵ
            List<int> countVtoIList = new List<int>(); //�洢countValueת����int���ֵ
            foreach (var item in countValues)
            {
                //Debug.Log("countValues.item="+item);
                countVtoIList.Add((int)item); //��ֵת����int����countVtoIList��
            }
            countVtoIList.Sort(); //��ֵ��С��������
            foreach (int item in countVtoIList)
            {
                var gosByValue = cardsValueTypeGODict.Where(q => q.Value == (CardManager.ValueType)item).Select(q=>q.Key); //����ֵ��ȡGameObjects
                //foreach (var gosItem in gosByValue)
                //{
                //    Debug.Log("gosByValue.item="+gosItem);
                //}
                cardsCopy.AddRange(gosByValue); //��GameObject����cardsCopy��
            }
        }

        cards.Clear(); 
        cards.AddRange(cardsCopy); //�����������cardsCopy����cards��

        //foreach (GameObject item in cards)
        //{
        //    Debug.Log("���ź󣬿��ƣ�" + item.name);
        //}
    }
    /// <summary>
    /// ���ó��ƿ�����Hierarchy����е�λ�ã�����ڵ������
    /// </summary>
    /// <param name="cards">��Ҫ����λ�õĿ��Ƽ���</param>
    public void SetOutCardsSitsInHierarchy(List<GameObject> cards)
    {
        for (int i=0;i<cards.Count;i++)
        {
            cards[i].GetComponent<RectTransform>().SetSiblingIndex(i); //�����ƵĿ������õ����п��Ƶ���ǰ��λ��
        }
    }

}
