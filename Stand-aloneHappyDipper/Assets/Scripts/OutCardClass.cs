//using System;
//using System.Collections;
//using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class OutCardClass : MonoBehaviour
{
    //public static OutCardClass Instance { get; private set; } //公开脚本引用
    /// <summary>
    /// 出牌类型
    /// </summary>
    public enum OUT_CARD_TYPE
    {
        INCONFORMITY, //不符合
        SINGLE, //单牌
        STRAIGHT,  //顺子
        DOUBLE, //对牌
        PAIRS, //连对
        THREE_WITHOUT, //三不带
        THREE_WITH_ONE, //三带一
        THREE_WITH_TWO, //三带二
        PLANES_WITHOUT, //飞机不带
        PLANES_WITH_SINGLE, //飞机带单
        PLANES_WITH_DOUBLE, //飞机带对
        FOUR_WITH_TWO_SINGLE, //四带二
        FOUR_WITH_TWO_DOUBLE, //四带两对
        BOMBS, //炸弹
        KING_EXPLOSIONS //王炸
    }

    public int lastOutCardType =-1 ; //存储上一个所出牌的出牌类型（转换成int）（当前没有玩家出牌时，为-1）
    public int lastOutCardPlayer=-1; //存储上一个出牌玩家
    private int lastOutCardCount = -1; //存储上一个所出牌的卡牌张数（用于顺子、连对、飞机）
    public int maxValueLastOutCardsMaxCount=-1; //存储上一个出牌卡牌中最大数量的值集合中的最大值

    private CardManager.ValueType maxValueCurrentOutCardsMaxCount; //存储当前出牌卡牌中最大数量的值集合中的最大值

    public List<GameObject> outCardsList = new List<GameObject>(); //所要出的卡牌集合
    public List<GameObject> playerPreOutCardsList = new List<GameObject>(); //玩家预出牌（非AI玩家）
    public List<GameObject> playerOnClickPreOutCardsList = new List<GameObject>(); //玩家点击预选卡牌（非AI玩家）

    public List<GameObject> leftPlayerLastOutCardsList = new List<GameObject>(); //左边玩家上一个出牌集合
    public List<GameObject> playerLastOutCardsList = new List<GameObject>(); //中间玩家上一个出牌集合
    public List<GameObject> rightPlayerLastOutCardsList = new List<GameObject>(); //右边玩家上一个出牌集合

    /// <summary>
    /// 脚本变量初始化
    /// </summary>
    public void Init()
    {
        lastOutCardType = -1; //上一个所出卡牌的出牌类型
        lastOutCardPlayer = -1; //上一个出牌玩家
        lastOutCardCount = -1; //上一个所出卡牌的张数
        maxValueLastOutCardsMaxCount = -1; //上一个出牌卡牌中最大数量的值集合中的最大值

        //清空卡牌集合
        outCardsList.Clear();
        playerPreOutCardsList.Clear();
        playerOnClickPreOutCardsList.Clear();

        leftPlayerLastOutCardsList.Clear();
        playerLastOutCardsList.Clear();
        rightPlayerLastOutCardsList.Clear();
    }


    /// <summary>
    /// 获取指定卡牌的出牌类型
    /// </summary>
    CardManager.ValueType minValueOutCardsMaxCount;  //存储出牌卡牌中最大数量的值集合中的最小的值
    CardManager.ValueType maxValueOutCardsMaxCount;  //存储出牌卡牌中最大数量的值集合中的最大的值
    /// <summary>
    /// 获取所选卡牌的出牌类型
    /// 并将出牌卡牌中的最大数量的最大ValueType存入maxValueLastOutCardsMaxCount中
    /// </summary>
    /// <param name="cards">选择的卡牌</param>
    /// <returns>返回出牌类型</returns>
    private OUT_CARD_TYPE GetOutCardType(List<GameObject> outCards)
    {
        if (outCards.Count <= 0) return OUT_CARD_TYPE.INCONFORMITY; //如果出牌卡牌为空，则返回不合法

        Dictionary<CardManager.ValueType, int> outCardsValueCountDict = new Dictionary<CardManager.ValueType, int>();  //所出牌卡牌的值数量字典
        CardManager.ValueType valueT; //声明值类型变量
        //遍历出牌卡牌中的所有卡牌，统计其各个值的数量
        foreach (GameObject go in outCards)
        {
            //获取该卡牌的值类型
            valueT = CardManager.instance.cardsVDict[go];
            //判断出牌的值数量字典中是否已存在该值类型
            if (outCardsValueCountDict.ContainsKey(valueT))
            {
                outCardsValueCountDict[valueT]++; //该值类型的数量+1
            }
            else
            {
                outCardsValueCountDict.Add(valueT, 1);
            }
        }
        //Debug.Log("获取卡牌类型，outCardsValueCountDict.Count="+outCardsValueCountDict.Count);

        List<int> outCardsValueC = new List<int>(); //存储出牌值数量字典中的各个值的数量
        int outCardsAllCount = 0; //卡牌总数量,初始值为0
        //遍历出牌值数量字典，分别存储其数量、卡牌总数
        foreach (KeyValuePair<CardManager.ValueType, int> item in outCardsValueCountDict)
        {
            //Debug.Log("获取出牌类型中，item.key="+item.Key);
            //Debug.Log("获取出牌类型中，item.Value=" + item.Value);
            outCardsValueC.Add(item.Value); //添加出牌中各个值的数量到集合中
            outCardsAllCount += item.Value; //统计出牌的总卡牌数
        }
        
        outCardsValueC.Sort(); //数量集合从小到大进行排序
        //foreach (int item in outCardsValueC)
        //{
        //    Debug.Log("获取出牌类型中，outCardsValueC's item=" + item);
        //}

        List<CardManager.ValueType> outCardsMaxCountValueType = new List<CardManager.ValueType>(); //存储出牌值数量字典中的最大数值的各个值
        int maxCount = outCardsValueC[outCardsValueC.Count-1]; //从各个值数量的集合中提取出最大数量值
        //Debug.Log("获取卡牌类型，maxCount="+maxCount);
        //获取所有最大值所对应的ValueType，并存储于值集合中
        var maxCountValueTypeInDict = outCardsValueCountDict.Where(q => q.Value == maxCount).Select(q=>q.Key); //查找具有最大数量的值
        outCardsMaxCountValueType.AddRange(maxCountValueTypeInDict); //将最大数量的所有值存入集合中
        //Debug.Log("获取卡牌类型，outCardsMaxCountValueType.Count="+ outCardsMaxCountValueType.Count);
        //foreach (KeyValuePair<CardManager.ValueType, int> item in outCardsValueCountDict)
        //{
        //    if (item.Value == maxCount)
        //    {
        //        outCardsMaxCountValueType.Add(item.Key);
        //    }
        //}

        //获取所有最大值所对应ValueType集合中的最大ValueType
        maxValueCurrentOutCardsMaxCount = outCardsMaxCountValueType[0];
        foreach (CardManager.ValueType item in outCardsMaxCountValueType)
        {
            if ((int)maxValueCurrentOutCardsMaxCount < (int)item)
            {
                maxValueCurrentOutCardsMaxCount = item;
            }
        }
        //Debug.Log("获取卡牌类型，maxValueCurrentOutCardsMaxCount="+ maxValueCurrentOutCardsMaxCount);

        int outCardsValueCountDictPairCount = outCardsValueCountDict.Count;//出牌卡牌值数量字典中键值对的数量
        //出牌卡牌中所有值对应的数量中最大数量为1
        if (maxCount==1)
        {
            //最大数量为1 且 只有一种该数量的卡牌类型（单牌）
            if(outCardsMaxCountValueType.Count == 1)
            {
                //Debug.Log("当前出牌卡牌为单牌");
                return OUT_CARD_TYPE.SINGLE;
            }
            //最大数量为1 且 只有大王和小王这两种卡牌类型（王炸、不符合）
            else if (outCardsMaxCountValueType.Count ==2)
            {
                //Debug.Log("判断是否是大小王");
                //判断是否包含大小王
                if(outCardsMaxCountValueType.Contains(CardManager.ValueType.BLACKJOKER) && outCardsMaxCountValueType.Contains(CardManager.ValueType.REDJOKER))
                {
                    //Debug.Log("当前出牌卡牌为王炸");
                    return OUT_CARD_TYPE.KING_EXPLOSIONS;
                }
                else
                {
                    //Debug.Log("王炸不符合");
                    return OUT_CARD_TYPE.INCONFORMITY;
                }
            }
            //最大数量为1 且 该数量的卡牌类型大于5（顺子、不符合）
            else if (outCardsMaxCountValueType.Count>=5)
            {
                //判断是否连续，不包含2
                if(!outCardsMaxCountValueType.Contains(CardManager.ValueType.TWO) && ISContinuous(outCardsMaxCountValueType))
                {
                    //Debug.Log("当前出牌卡牌为顺子");
                    return OUT_CARD_TYPE.STRAIGHT;
                }
                else
                {
                    //Debug.Log("顺子不符合");
                    return OUT_CARD_TYPE.INCONFORMITY;
                }
            }
            //其余情况为不符合，例如数量不合法（例如3、4等）
            else
            {
                //Debug.Log("最大数量为1的其余情况不符合");
                return OUT_CARD_TYPE.INCONFORMITY;
            }
        }
        //出牌卡牌中所有值对应的数量中最大数量为2
        else if (maxCount == 2)
        {
            //最大数量为2 且 只有一种该数量的卡牌类型 且 没有其它数量的卡牌类型（对牌）
            //因为maxCount==2，所以其它数量只能是1
            if ( outCardsMaxCountValueType.Count==1 && outCardsValueCountDictPairCount==1)
            {
                //Debug.Log("出牌类型为对牌");
                return OUT_CARD_TYPE.DOUBLE;
            }
            //最大数量为2 且 有大于3种该数量的卡牌类型 且 没有其它数量的卡牌类型（连对）
            else if(outCardsMaxCountValueType.Count>=3 && outCardsValueCountDictPairCount == outCardsMaxCountValueType.Count)
            {
                if (!outCardsMaxCountValueType.Contains(CardManager.ValueType.TWO) && ISContinuous(outCardsMaxCountValueType))
                {
                    //Debug.Log("出牌类型为连对");
                    return OUT_CARD_TYPE.PAIRS;
                }
                else
                {
                    //Debug.Log("连对出牌类型不合法");
                    return OUT_CARD_TYPE.INCONFORMITY;
                }
            }
            //其余情况为不符合
            else
            {
                //Debug.Log("最大数量为2，出牌类型不合法");
                return OUT_CARD_TYPE.INCONFORMITY;
            }
        }
        //出牌卡牌中所有值对应的数量中最大数量为3
        else if (maxCount == 3)
        {
            //最大数量为3 且 只有一种该数量的卡牌类型（三不带、三带一、三带二、不符合）
            //Debug.Log("所出卡牌中最大数量为3");
            if (outCardsMaxCountValueType.Count == 1)
            {
                //卡牌中只有该种数量的卡牌类型（最大数量的值类型数量与键值对数量一致）（三不带）
                if (outCardsValueCountDictPairCount == 1)
                {
                    //Debug.Log("出牌类型为三不带");
                    return OUT_CARD_TYPE.THREE_WITHOUT;
                }
                //存在另一种数量的卡牌值类型，该卡牌类型值的数量为1（三带一）、为2（三带二）
                else if(outCardsValueCountDictPairCount==2)
                {
                    //Debug.Log("判断是否是三带牌");
                    if(outCardsValueC[0] == 1)
                    {
                        //Debug.Log("出牌类型为三带一");
                        return OUT_CARD_TYPE.THREE_WITH_ONE;
                    }
                    else if (outCardsValueC[0]==2)
                    {
                        //Debug.Log("出牌类型为三带二");
                        return OUT_CARD_TYPE.THREE_WITH_TWO;
                    }
                    //其余带牌情况为不符合
                    else
                    {
                        //Debug.Log("三牌有带不合法");
                        return OUT_CARD_TYPE.INCONFORMITY;
                    }
                }
                //其余情况为不符合
                else
                {
                    //Debug.Log("三牌其余类型不合法");
                    return OUT_CARD_TYPE.INCONFORMITY;
                }
            }
            //最大数量为3 且 有两种以上该数量的卡牌类型（飞机不带、飞机带单、飞机带对、不符合）
            else if(outCardsMaxCountValueType.Count>=2)
            {
                //卡牌中包含2为不合法
                if (outCardsMaxCountValueType.Contains(CardManager.ValueType.TWO))
                {
                    //Debug.Log("飞机中有2不合法");
                    return OUT_CARD_TYPE.INCONFORMITY;
                }
                //判断是否是连续（原值集合、去除最大值后的值集合、去除最小值后的值集合）
                else if (ISContinuous(outCardsMaxCountValueType) || ISContinuous(RemoveMaxOrMinForContinue(outCardsMaxCountValueType,true)) || ISContinuous(RemoveMaxOrMinForContinue(outCardsMaxCountValueType,false)))
                {
                    //Debug.Log("飞机判断连续");
                    //获取所有最大值所对应ValueType集合中的最大ValueType（三飞机带3牌情况）
                    maxValueCurrentOutCardsMaxCount = outCardsMaxCountValueType[0];
                    foreach (CardManager.ValueType item in outCardsMaxCountValueType)
                    {
                        if ((int)maxValueCurrentOutCardsMaxCount < (int)item)
                        {
                            maxValueCurrentOutCardsMaxCount = item;
                        }
                    }

                    //卡牌中只有该种数量的卡牌类型（键值对的个数==最大数量的值类型的个数）(飞机不带)
                    if (outCardsValueCountDictPairCount == outCardsMaxCountValueType.Count)
                    {
                        //Debug.Log("出牌类型飞机不带");
                        return OUT_CARD_TYPE.PLANES_WITHOUT;
                    }
                    //卡牌中有其它种数量的卡牌类型（其它数量的卡牌类型的个数和==outCardsValueV.Count）（飞机带单）
                    //其它数量的卡牌类型的个数和=所有卡牌数量-3*最大个数卡牌值的个数
                    else if(outCardsAllCount==4*outCardsMaxCountValueType.Count)
                    {
                        //Debug.Log("出牌类型飞机带单");
                        return OUT_CARD_TYPE.PLANES_WITH_SINGLE;
                    }
                    //卡牌中有其它种数量的卡牌类型（其它种数量的卡牌类型个数=键值对个数-最大数量的值类型个数）（飞机带对）
                    else if ((outCardsValueCountDictPairCount - outCardsMaxCountValueType.Count) == outCardsMaxCountValueType.Count)
                    {
                        for(int i=0;i<outCardsValueC.Count- outCardsMaxCountValueType.Count; i++)
                        {
                            //判断其它数量的值类型的各个数量是否为2（对）
                            if(outCardsValueC[i]!=2)
                            {
                                //Debug.Log("飞机带对不合法");
                                return OUT_CARD_TYPE.INCONFORMITY;
                            }
                        }
                        //Debug.Log("出牌类型飞机带对");
                        return OUT_CARD_TYPE.PLANES_WITH_DOUBLE;
                    }
                    //其余情况为不合法
                    else
                    {
                        //Debug.Log("飞机其余类型不合法");
                        return OUT_CARD_TYPE.INCONFORMITY;
                    }
                }
                //不连续则为不合法
                else
                {
                    //Debug.Log("飞机不连续不合法");
                    return OUT_CARD_TYPE.INCONFORMITY;
                }
            }
            //其余情况为不合法
            else
            {
                //Debug.Log("最大数量为3其余类型不合法");
                return OUT_CARD_TYPE.INCONFORMITY;
            }
        }
        //出牌卡牌中所有值对应的数量中最大数为4
        else if (maxCount == 4)
        {
            //最大数量为1 且 只有一种该数量的卡牌值类型
            if(outCardsMaxCountValueType.Count == 1)
            {
                //卡牌中只有该数量的卡牌值类型（炸弹）
                if(outCardsValueCountDictPairCount==1)
                {
                    //Debug.Log("出牌类型为炸弹");
                    return OUT_CARD_TYPE.BOMBS;
                }
                //卡牌中有其它数量的卡牌值类型，其它数量的卡牌值类型的数量和=2（四带二），并且不同时包含大小王
                else if (outCardsAllCount == 6 && !(outCardsValueCountDict.ContainsKey(CardManager.ValueType.BLACKJOKER) && outCardsValueCountDict.ContainsKey(CardManager.ValueType.REDJOKER)))
                {
                    //Debug.Log("出牌类型为炸弹带单");
                    return OUT_CARD_TYPE.FOUR_WITH_TWO_SINGLE;
                }
                //卡牌中有其它数量的两种卡牌值类型，且每种卡牌值类型的数量=2（四带两对）
                else if(outCardsValueCountDictPairCount-outCardsMaxCountValueType.Count==2 && outCardsValueC[0]==2 && outCardsValueC[1] == 2)
                {
                    //Debug.Log("出牌类型为炸弹带对");
                    return OUT_CARD_TYPE.FOUR_WITH_TWO_DOUBLE;
                }
                //其余情况为不合法
                else
                {
                    //Debug.Log("炸弹其余类型不合法");
                    return OUT_CARD_TYPE.INCONFORMITY;
                }
            }
            //其余情况为不合法
            else
            {
                //Debug.Log("最大数量为4其余类型不合法");
                return OUT_CARD_TYPE.INCONFORMITY;
            }
        }
        else
        {
            //Debug.Log("整体其余类型不合法");
            return OUT_CARD_TYPE.INCONFORMITY;
        }
        //return OUT_CARD_TYPE.INCONFORMITY;
    }
    /// <summary>
    /// 判断卡牌值是否是连续的
    /// 传入的集合根据值大小进行升序排序
    /// 存储排序后的集合的最大值和最小值（用于去除方法）
    /// </summary>
    /// <param name="cards">要进行判断的ValueType集合</param>
    /// <returns>连续，返回true</returns>不连续，返回false
    private bool ISContinuous(List<CardManager.ValueType> cards)
    {
        //存储ValueType类型转换成int之后的各个值
        List<int> cardsVTtoInt = new List<int>();
        foreach(CardManager.ValueType item in cards)
        {
            cardsVTtoInt.Add((int)item);
        }

        //int集合升序排序
        cardsVTtoInt.Sort();

        //原集合进行升序排序
        for (int i = 0; i < cardsVTtoInt.Count; i++)
        {
            cards[i] = (CardManager.ValueType)cardsVTtoInt[i];
        }
        maxValueOutCardsMaxCount = cards[cards.Count - 1]; //存储值集合的最大值
        minValueOutCardsMaxCount = cards[0]; //存储值集合的最小值

        //判断连续
        int lastItem = cardsVTtoInt[0]; //定义上一个VT的int值，默认为集合的第一个数
        for(int i = 1; i < cardsVTtoInt.Count; i++)
        {
            //先判断不连续的情况，直接退出循环
            if((cardsVTtoInt[i]-lastItem)!=1)
            {
                break;
            }
            //循环结束，各个相邻值均相差1，返回true
            if(i == cardsVTtoInt.Count-1)
            {
                return true;
            }
            //将当前值赋值给lastItem，进入下一轮循环
            lastItem = cardsVTtoInt[i];
        }

        return false;
    }
    /// <summary>
    /// 去除值集合的最小值或最大值，使得其有可能连续（用于三个飞机带三牌的情况：333444555---888）
    /// </summary>
    /// <param name="cardsValueTypeList">按值从小到达排序好的值集合</param>
    /// <param name="maxTminF">true去除最大值、false去除最小值</param>
    private List<CardManager.ValueType> RemoveMaxOrMinForContinue(List<CardManager.ValueType> cardsValueTypeList,bool maxTminF)
    {
        //去除最大值
        if(maxTminF)
        {
            //判断在之前是否有删除最小值，若有，则添加回来
            if(!cardsValueTypeList.Contains(minValueOutCardsMaxCount))
            {
                cardsValueTypeList.Add(minValueOutCardsMaxCount);
            }
            cardsValueTypeList.Remove(cardsValueTypeList[cardsValueTypeList.Count - 1]); //移除集合中的最大值
        }
        //去除最小值
        else
        {
            //判断在之前是否有删除最大值，若有，则添加回来
            if(!cardsValueTypeList.Contains(maxValueOutCardsMaxCount))
            {
                cardsValueTypeList.Add(maxValueOutCardsMaxCount);
            }
            cardsValueTypeList.Remove(minValueOutCardsMaxCount); //移除集合中的最小值
        }
        return cardsValueTypeList;
    }



    /// <summary>
    /// 获取玩家卡牌中最小的可出卡牌
    /// </summary>
    List<int> allCanOutCardsList = new List<int>(); //用于存储所有可以出的卡牌值类型转换成的int类型
    Dictionary<CardManager.ValueType, int> cardsValueCountDictAuxiliary = new Dictionary<CardManager.ValueType, int>(); //卡牌的辅助值数量字典
    /// <summary>
    /// 获取最小可出牌的集合
    /// </summary>
    /// <param name="cards">玩家卡牌</param>
    /// <param name="thanMaxValueType"></param>
    /// <returns></returns>
    public List<GameObject> GetMinCanOutCardsByType(List<GameObject> cards,int thanMaxValueType,int lastOutCardType)
    {
        //Debug.Log("获取最小可出牌的集合");
        //Debug.Log("获取最小可出牌的集合,当前卡牌数量=" + cards.Count);
        //Debug.Log("获取最小可出牌的集合,thanMaxValueType=" + thanMaxValueType);
        //Debug.Log("获取最小可出牌的集合,lastOutCardType=" + lastOutCardType);

        CardsSelectAuxiliary(cards); //调用卡牌辅助函数
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //用于存储可出的卡牌

        //没有出牌类型限制
        if (lastOutCardType==-1)
        {
            List<GameObject> minSingleCanOutCardList = GetMinSingleCanOutCards(cards, thanMaxValueType); //获取单牌
            //有单牌
            if (minSingleCanOutCardList.Count!=0)
            {
                minCanOutCardsList.AddRange(minSingleCanOutCardList); //将单牌赋值给出牌集合
            }

            List<GameObject> minDoubleCanOutCardsList = GetMinDoubleCanOutCards(cards, thanMaxValueType); //获取对牌
            //有对牌
            if (minDoubleCanOutCardsList.Count != 0)
            {
                //如果没有单牌，则直接赋值对牌
                if (minCanOutCardsList.Count==0)
                {
                    minCanOutCardsList.AddRange(minDoubleCanOutCardsList);
                }
                //有单牌，且单牌值大于对牌
                if (minCanOutCardsList.Count!= 0 && CardManager.instance.cardsVDict[minCanOutCardsList[0]] > CardManager.instance.cardsVDict[minDoubleCanOutCardsList[0]])
                {
                    minCanOutCardsList.Clear(); //清空存储的元素
                    minCanOutCardsList.AddRange(minDoubleCanOutCardsList); //添加对牌中的元素
                }
            }

            List<GameObject> minThreeCanOutCardsList = GetMinThreeCanOutCards(cards, thanMaxValueType); //获取三牌
            //有三牌
            if (minThreeCanOutCardsList.Count!=0)
            {
                //如果没有单牌、对牌，则直接赋值三牌
                if (minCanOutCardsList.Count==0)
                {
                    minCanOutCardsList.AddRange(minThreeCanOutCardsList);
                }
                //有单牌、或者有对牌、或者有单牌和对牌（minCanOutCardsList中存储的是最小值的GameObject）
                //存储的最小值大于三牌
                if (minCanOutCardsList.Count!=0 && CardManager.instance.cardsVDict[minCanOutCardsList[0]] > CardManager.instance.cardsVDict[minThreeCanOutCardsList[0]])
                {
                    minThreeCanOutCardsList = minThreeCanOutCardsList.Concat(minCanOutCardsList).ToList<GameObject>(); //将最小的单牌或者对牌一并赋值到三牌中（三带一 或者 三带二）
                    minCanOutCardsList.Clear(); //清空出牌卡牌中元素
                    minCanOutCardsList.AddRange(minThreeCanOutCardsList); //将三牌中所有元素添加至出牌集合中
                }
            }

            //没有单牌、对牌、三牌时
            if (minCanOutCardsList.Count==0)
            {
                minCanOutCardsList = GetMinBombCanOutCards(cards,thanMaxValueType); //获取炸弹
                //没有炸弹
                if(minCanOutCardsList.Count==0)
                {
                    minCanOutCardsList = GetKingExplosionCanOutCards(cards); //获取王炸
                }
            }
        }
        //有出牌类型限制
        else
        {
            //根据上一个出牌类型以及比对值，进行出牌
            switch (lastOutCardType)
            {
                //单牌
                case 1:
                    List<GameObject> minSingleCanOutCardList = GetMinSingleCanOutCards(cards,thanMaxValueType); //获取单牌
                    //没有单牌
                    if (minSingleCanOutCardList.Count==0)
                    {
                        minSingleCanOutCardList = GetMinDoubleCanOutCards(cards,thanMaxValueType); //获取对牌
                        //没有对牌
                        if (minSingleCanOutCardList.Count==0)
                        {
                            minSingleCanOutCardList = GetMinThreeCanOutCards(cards,thanMaxValueType); //获取三牌
                        }
                    }
                    //有单牌、对牌、或者三牌
                    if (minSingleCanOutCardList.Count!=0)
                    {
                        minCanOutCardsList.Add(minSingleCanOutCardList[0]); //单牌只需要添加其中一张卡牌
                    }
                    //没有单牌、对牌或者三牌（炸弹、王炸）
                    else
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸 （炸弹不需要比对大小）
                    }
                    break;
                //顺子
                case 2:
                    //Debug.Log("顺子代码未写");
                    minCanOutCardsList = GetMinStraightCanOutCards(cards,thanMaxValueType,lastOutCardCount);
                    //没有顺子（炸弹、王炸）
                    if(minCanOutCardsList.Count==0)
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸 （炸弹不需要比对大小）
                    }
                    break;
                //对牌
                case 3:
                    List<GameObject> minDoubleCanOutCardsList = GetMinDoubleCanOutCards(cards,thanMaxValueType); //获取对牌
                    //没有对牌
                    if (minDoubleCanOutCardsList.Count == 0)
                    {
                        minDoubleCanOutCardsList = GetMinThreeCanOutCards(cards,thanMaxValueType); //获取三牌
                    }
                    //有对牌、三牌
                    if(minDoubleCanOutCardsList.Count!=0)
                    {
                        minCanOutCardsList.Add(minDoubleCanOutCardsList[0]); //对牌需添加其中两张牌
                        minCanOutCardsList.Add(minDoubleCanOutCardsList[1]);
                    }
                    //没有对牌、三牌
                    else
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸 （炸弹不需要比对大小）
                    }
                    break;
                //连对
                case 4:
                    minCanOutCardsList = GetMinPairsCanOutCards(cards,thanMaxValueType,lastOutCardCount); //获取连对
                    //没有连对
                    if (minCanOutCardsList.Count==0)
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸 （炸弹不需要比对大小）
                    }
                    break;
                //三不带
                case 5:
                    minCanOutCardsList = GetMinThreeCanOutCards(cards,thanMaxValueType); //获取三牌
                    if(minCanOutCardsList.Count==0)
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸 （炸弹不需要比对大小）
                    }
                    break;
                //三带一
                case 6:
                    List<GameObject> minThreeOneCanOutCardsList = GetMinThreeCanOutCards(cards,thanMaxValueType); //获取三牌
                    List<GameObject> minOneThreeCanOutCardList = GetMinCanOutCardsByType(cards, 0, (int)OUT_CARD_TYPE.SINGLE); //获取单牌，单牌不限定大小
                    //有三牌且有单牌，且单牌不是从三牌中获取的
                    if (minThreeOneCanOutCardsList.Count!=0 && minOneThreeCanOutCardList.Count==1 && !minThreeOneCanOutCardsList.Contains(minOneThreeCanOutCardList[0]))
                    {
                        minThreeOneCanOutCardsList = minThreeOneCanOutCardsList.Concat(minOneThreeCanOutCardList).ToList<GameObject>(); //组成三带一
                        minCanOutCardsList = minThreeOneCanOutCardsList;
                    }
                    //没有三牌或者没有单牌，出炸弹、王炸
                    else
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸
                    }
                    break;
                //三带二
                case 7:
                    List<GameObject> minThreeDoubleCanOutCardsList = GetMinThreeCanOutCards(cards, thanMaxValueType); //获取三牌
                    List<GameObject> minDoubleThreeCanOutCardsList = GetMinCanOutCardsByType(cards, 0, (int)OUT_CARD_TYPE.DOUBLE); //获取对牌，对牌不限定大小
                    //有三牌且有对牌,且对牌不是从三牌中获取的（//对牌不是大小王）
                    if (minThreeDoubleCanOutCardsList.Count != 0 && minDoubleThreeCanOutCardsList.Count == 2 && !minThreeDoubleCanOutCardsList.Contains(minDoubleThreeCanOutCardsList[0]) && CardManager.instance.cardsVDict[minDoubleThreeCanOutCardsList[0]] == CardManager.instance.cardsVDict[minDoubleThreeCanOutCardsList[1]])
                    {
                        minThreeDoubleCanOutCardsList = minThreeDoubleCanOutCardsList.Concat(minDoubleThreeCanOutCardsList).ToList<GameObject>(); //组成三带对
                        minCanOutCardsList = minThreeDoubleCanOutCardsList;
                    }
                    //没有三牌或者没有对牌，出炸弹、王炸
                    else
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸
                    }
                    break;
                //飞机不带
                case 8:
                    minCanOutCardsList = GetMinPlanesWithoutCanOutCards(cards,thanMaxValueType,lastOutCardCount); //获取飞机不带
                    //没有飞机不带
                    if (minCanOutCardsList.Count==0)
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸
                    }
                    break;
                //飞机带单
                case 9:
                    minCanOutCardsList = GetMinPlanesWithSingleCanOutCards(cards,thanMaxValueType,lastOutCardCount); //获取飞机带单
                    //没有飞机带单
                    if (minCanOutCardsList.Count == 0)
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸
                    }
                    break;
                //飞机带对
                case 10:
                    //Debug.Log("飞机带对");
                    minCanOutCardsList = GetMinPlanesWithDoubleCanOutCards(cards, thanMaxValueType, lastOutCardCount); //获取飞机带对
                    //没有飞机带对
                    if (minCanOutCardsList.Count == 0)
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸
                    }
                    break;
                //四带二
                case 11:
                    List<GameObject> minFourSingleCanOutCardsList = GetMinBombCanOutCards(cards,thanMaxValueType); //获取炸弹，组成四带二需要比对大小
                    //有更大值的炸弹
                    if (minFourSingleCanOutCardsList.Count==4)
                    {
                        List<GameObject> minSingleFourCanOutCardsList01 = GetMinCanOutCardsByType(cards, 0, (int)OUT_CARD_TYPE.SINGLE); //获取单牌，单牌不需要比对大小
                        //有单牌
                        if (minSingleFourCanOutCardsList01.Count == 1)
                        {
                            cards.Remove(minSingleFourCanOutCardsList01[0]); //移除上一个单牌
                            List<GameObject> minSingleFourCanOutCardsList02 = GetMinCanOutCardsByType(cards, 0, (int)OUT_CARD_TYPE.SINGLE); //获取除01外的单牌，单牌不需要比对大小
                            cards.Add(minSingleFourCanOutCardsList01[0]);  //添加回被移除的单牌
                            //有第二张单牌，且这两张单牌不是从对牌中获取（从对牌中获取，则又空出一张牌）
                            if (minSingleFourCanOutCardsList02.Count == 1 && CardManager.instance.cardsVDict[minSingleFourCanOutCardsList01[0]]!= CardManager.instance.cardsVDict[minSingleFourCanOutCardsList02[0]])
                            {
                                //组合成四带二的格式
                                minFourSingleCanOutCardsList = minFourSingleCanOutCardsList.Concat(minSingleFourCanOutCardsList01).ToList<GameObject>();
                                minFourSingleCanOutCardsList = minFourSingleCanOutCardsList.Concat(minSingleFourCanOutCardsList02).ToList<GameObject>();
                                minCanOutCardsList = minFourSingleCanOutCardsList;
                            }
                            //没有第二张单牌，无法四带二压死，只能炸弹、王炸
                            else
                            {
                                minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸
                            }
                        }
                        //没有单牌，无法四带二压死，只能炸弹、王炸
                        else
                        {
                            minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸
                        }
                    }
                    //无法四带二压死，只能炸弹、王炸
                    else
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards,-1,(int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸
                    }
                    break;
                //四带两对
                case 12:
                    List<GameObject> minFourDoubleCanOutCardsList = GetMinBombCanOutCards(cards,thanMaxValueType); //获取炸弹，需要比对大小
                    //有更大值的炸弹
                    if (minFourDoubleCanOutCardsList.Count == 4)
                    {
                        List<GameObject> minDoubleFourCanOutCardsList01 = GetMinCanOutCardsByType(cards, 0, (int)OUT_CARD_TYPE.DOUBLE); //获取对牌，对牌不需要比对大小
                        //有对牌(不是大小王)
                        if (minDoubleFourCanOutCardsList01.Count == 2 && minDoubleFourCanOutCardsList01[0]==minDoubleFourCanOutCardsList01[1])
                        {
                            cards.Remove(minDoubleFourCanOutCardsList01[0]); //移除上一个对牌
                            cards.Remove(minDoubleFourCanOutCardsList01[1]);
                            List<GameObject> minDoubleFourCanOutCardsList02 = GetMinCanOutCardsByType(cards, 0, (int)OUT_CARD_TYPE.DOUBLE); //获取除01外对牌，对牌不需要比对大小
                            cards.Add(minDoubleFourCanOutCardsList01[0]);  //添加回被移除的对牌
                            cards.Add(minDoubleFourCanOutCardsList01[1]);
                            //有第二个对牌（不是大小王）
                            if (minDoubleFourCanOutCardsList02.Count == 2 && minDoubleFourCanOutCardsList02[0] == minDoubleFourCanOutCardsList02[1])
                            {
                                //组合成四带两对的格式
                                minFourDoubleCanOutCardsList = minFourDoubleCanOutCardsList.Concat(minDoubleFourCanOutCardsList01).ToList<GameObject>();
                                minFourDoubleCanOutCardsList = minFourDoubleCanOutCardsList.Concat(minDoubleFourCanOutCardsList02).ToList<GameObject>();
                                minCanOutCardsList = minFourDoubleCanOutCardsList;
                            }
                            //无法四带两对压死，只能炸弹、王炸
                            else
                            {
                                minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸
                            }
                        }
                        //无法四带两对压死，只能炸弹、王炸
                        else
                        {
                            minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸
                        }
                    }
                    //无法四带两对压死，只能炸弹、王炸
                    else
                    {
                        minCanOutCardsList = GetMinCanOutCardsByType(cards, -1, (int)OUT_CARD_TYPE.BOMBS); //获取炸弹、王炸
                    }
                    break;
                //炸弹
                case 13:
                    minCanOutCardsList = GetMinBombCanOutCards(cards,thanMaxValueType); //获取炸弹
                    //没有炸弹
                    if (minCanOutCardsList.Count==0)
                    {
                        minCanOutCardsList = GetKingExplosionCanOutCards(cards); //获取王炸
                    }
                    break;
                //王炸
                case 14:
                    minCanOutCardsList = GetKingExplosionCanOutCards(cards); //获取王炸
                    break;
            }
        }

        //Debug.Log("获取最小可出牌的集合,出牌卡牌数量："+minCanOutCardsList.Count);
        return minCanOutCardsList;
    }

    /// <summary>
    /// 卡牌选择辅助（用于当前卡牌筛选出合适的出牌）
    /// </summary>
    private void CardsSelectAuxiliary(List<GameObject> cards)
    {
        //清空卡牌的辅助值数量字典（清除上一次数据）
        if (cardsValueCountDictAuxiliary.Count!=0)
        {
            cardsValueCountDictAuxiliary.Clear();
        }

        //获取卡牌值数量字典
        cardsValueCountDictAuxiliary = CardManager.instance.rememberCardValueCount.GetCardsValueCount(cards);
    }

    /// <summary>
    /// 获取当前玩家卡牌中可以出的最小单牌
    /// </summary>
    /// <param name="cards">当前玩家卡牌</param>
    /// <param name="thanMaxValueType">进行比对的最大值</param>
    /// <param name="isNeedAuxiliary">是否需要辅助函数进行初始化</param>
    /// <returns></returns>
    public List<GameObject> GetMinSingleCanOutCards(List<GameObject> cards,int thanMaxValueType/*,bool isNeedAuxiliary*/)
    {
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //用于存储可出的卡牌
        //判断是否需要辅助函数进行初始化
        //if(isNeedAuxiliary)
        //{
        //    CardsSelectAuxiliary(cards); //调用辅助函数
        //}
        //筛选出比上一个出牌最大数的值更大的值，存储于allCanOutCardsList集合中
        SetAllCanOutCardsListByTraverseDict(OUT_CARD_TYPE.SINGLE,thanMaxValueType);
        //当前卡牌中有符合的单牌时
        if(allCanOutCardsList.Count!=0)
        {
            minCanOutCardsList = FindGOByValueType(cards,(CardManager.ValueType)allCanOutCardsList[0]); //从所有可出牌的集合中选出最小值查找GameObject
        }
        //当前卡牌中没有符合的单牌
        //else
        //{
        //    List<GameObject> min01CanOutCardsList = GetMinDoubleCanOutCards(cards,thanMaxValueType,false); //没有单牌则从双牌、或三牌中获取，或直接炸弹
        //    //如果是对牌或者三牌，则取出其中一张卡牌作为单牌
        //    if(min01CanOutCardsList.Count==2 || min01CanOutCardsList.Count==3)
        //    {
        //        minCanOutCardsList.Add(min01CanOutCardsList[0]); //单牌只需要添加其中一张卡牌即可
        //    }
        //    else
        //    {
        //        minCanOutCardsList = min01CanOutCardsList; //将炸弹、王炸或者null赋值给最小出牌
        //    }
        //}
        return minCanOutCardsList;
    }

    /// <summary>
    /// 获取当前玩家卡牌中可以出的最小顺子
    /// </summary>
    /// <param name="cards">当前玩家卡牌</param>
    /// <param name="thanMaxValueType">需要比对的最大值</param>
    /// <param name="count">顺子卡牌个数</param>
    /// <returns></returns>
    private List<GameObject> GetMinStraightCanOutCards(List<GameObject> cards,int thanMaxValueType,int count)
    {
        int thanMinValueType = thanMaxValueType - count + 1; //获取顺子中最小卡牌的值（int）
        //Debug.Log("获取当前玩家卡牌中可以出的最小顺子，thanMinValueType="+thanMinValueType);
        Dictionary<GameObject, int> cardsValueTypeGODict = new Dictionary<GameObject, int>(); //存储卡牌对应的ValueType（转换成int）
        List<int> cardsValueTypeList = new List<int>(); //存储卡牌中的卡牌值转换成int
        //Dictionary<CardManager.ValueType, int> cardsValueTypeCountDict = new Dictionary<CardManager.ValueType, int>(); //存储卡牌值对应的数量
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //用于存储可出的卡牌

        //获取各个卡牌中对应的ValueType转换成int（不重复）、获取各个卡牌对应的值转换成int
        foreach (GameObject item in cards)
        {
            int iValueType = (int)CardManager.instance.cardsVDict[item];
            //如果当前卡牌值集合中不包含该卡牌值，则存入该卡牌值
            if (!cardsValueTypeList.Contains(iValueType))
            {
                cardsValueTypeList.Add(iValueType);
            }

            cardsValueTypeGODict.Add(item, iValueType); //当前卡牌及对应的ValueType
        }
        cardsValueTypeList.Sort(); //集合按照值从小到大排序

        int minCanOutValueType = -1; //用于存储可出牌顺子中的最小值，默认设置为-1
        //对当前卡牌所有值进行循环，从需要比对的最小值开始循环，到卡牌最大值结束（循环初始值为 比对最小值、卡牌值集合最小值 两者之间的最大值）
        for (int i=thanMinValueType>cardsValueTypeList[0]?thanMinValueType:cardsValueTypeList[0];i<=cardsValueTypeList[cardsValueTypeList.Count-1];)
        {
            //如果卡牌值集合中不包含该值，并且小于或等于上一个出牌中的最小值,则进入下一个循环
            if (!cardsValueTypeList.Contains(i) || i<=thanMinValueType)
            {
                i++;
                continue;
            }
            //Debug.Log("i="+i);
            int currentCount = 0; //当前计数的顺子个数
            //遍历从i开始到所需顺子个数的最大卡牌值
            for (int j=i;j<i+count;j++)
            {
                //如果当前卡牌值中是否包含该所需值，计数
                if (cardsValueTypeList.Contains(j) && j != (int)CardManager.ValueType.TWO)
                {
                    //Debug.Log("j="+j);
                    currentCount++;
                    if (currentCount==count)
                    {
                        minCanOutValueType = i;
                        i = cardsValueTypeList[cardsValueTypeList.Count-1]+1; //通过设置i的值退出最外循环
                        break; //退出当前循环
                    }
                }
                else
                {
                    i = j + 1; //将i设置到顺子断连位置的下一个数
                    break;
                }
            }
        }

        //如果有符合的顺子
        if (minCanOutValueType!=-1)
        {
            //将符合的卡牌逐一存入minCanOutCardsList中
            for (int i=0;i<count;i++)
            {
                //Debug.Log("minCanOutValueType="+minCanOutValueType);
                var gainCardsByType = cardsValueTypeGODict.Where(q => q.Value == minCanOutValueType).Select(q => q.Key); //在cardsValueTypeGODict中查找符合ValueType值的所有卡牌
                foreach (var item in gainCardsByType)
                {
                    minCanOutCardsList.Add(item); //顺子仅需其中一张卡牌
                    break;
                }
                minCanOutValueType++; //存储下一个值的卡牌
            }
        }

        //foreach (GameObject item in minCanOutCardsList)
        //{
        //    Debug.Log("获取最小顺子，minCanOutCardsList=" + item);
        //}
        return minCanOutCardsList;
    }

    /// <summary>
    /// 获取当前玩家卡牌中可以出的最小对牌
    /// </summary>
    /// <param name="cards">当前玩家卡牌</param>
    /// <param name="thanMaxValueType">进行比对的最大值</param>
    /// <param name="isNeedAuxiliary">是否需要辅助函数进行初始化</param>
    /// <returns></returns>
    private List<GameObject> GetMinDoubleCanOutCards(List<GameObject> cards, int thanMaxValueType/*, bool isNeedAuxiliary*/)
    {
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //用于存储可出的卡牌
        //判断是否需要辅助函数进行初始化
        //if (isNeedAuxiliary)
        //{
        //    CardsSelectAuxiliary(cards); //调用辅助函数
        //}
        //筛选出比上一个出牌最大数的值更大的值，存储于allCanOutCardsList集合中
        SetAllCanOutCardsListByTraverseDict(OUT_CARD_TYPE.DOUBLE, thanMaxValueType);
        //当前卡牌中有符合的对牌时
        if (allCanOutCardsList.Count != 0)
        {
            minCanOutCardsList = FindGOByValueType(cards, (CardManager.ValueType)allCanOutCardsList[0]); //从所有可出牌的集合中选出最小值查找GameObject
        }
        //else
        //{
        //    List<GameObject> min01CanOutCardsList = GetMinThreeCanOutCards(cards, thanMaxValueType, false); //没有双牌则从三牌中获取，或直接炸弹，或者王炸
        //    //如果是三牌
        //    if (min01CanOutCardsList.Count == 3)
        //    {
        //        minCanOutCardsList.Add(min01CanOutCardsList[0]); //对牌只需要添加其中两张卡牌即可
        //        minCanOutCardsList.Add(min01CanOutCardsList[1]);
        //    }
        //    //炸弹、王炸或者Null
        //    else
        //    {
        //        minCanOutCardsList = min01CanOutCardsList; //将炸弹、王炸或Null直接赋值给最小出牌
        //    }
        //}
        return minCanOutCardsList;
    }

    /// <summary>
    /// 获取当前玩家卡牌中可以出的最小连对
    /// </summary>
    /// <param name="cards">玩家卡牌</param>
    /// <param name="thanMaxValueType">需要比对的最大值</param>
    /// <param name="lastCount">上一个出牌牌数</param>
    /// <returns></returns>
    private List<GameObject> GetMinPairsCanOutCards(List<GameObject> cards, int thanMaxValueType, int lastCount)
    {
        int thanMinValueType = thanMaxValueType - lastCount/2 + 1; //获取顺连对中最小卡牌的值（int）
        Dictionary<GameObject, int> cardsValueTypeGODict = new Dictionary<GameObject, int>(); //存储卡牌对应的ValueType（转换成int）
        List<int> cardsValueTypeList = new List<int>(); //存储卡牌中符合条件的卡牌值转换成int
        Dictionary<CardManager.ValueType, int> cardsValueTypeCountDict = new Dictionary<CardManager.ValueType, int>(); //存储卡牌值对应的数量
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //用于存储可出的卡牌

        cardsValueTypeCountDict = CardManager.instance.rememberCardValueCount.GetCardsValueCount(cards); //获取卡牌值对应的数量
        //获取各个卡牌中对应的ValueType转换成int（不重复）、获取各个卡牌对应的值转换成int
        foreach (GameObject item in cards)
        {
            int iValueType = (int)CardManager.instance.cardsVDict[item];
            //如果当前卡牌值的数量>=2 且 <=3 （除去炸弹的可能性）且 当前卡牌值集合中不包含该卡牌值，则存入该卡牌值
            if (cardsValueTypeCountDict[(CardManager.ValueType)iValueType]>=2 && cardsValueTypeCountDict[(CardManager.ValueType)iValueType] <=3 && !cardsValueTypeList.Contains(iValueType))
            {
                cardsValueTypeList.Add(iValueType);
            }
            cardsValueTypeGODict.Add(item, iValueType);
        }
        cardsValueTypeList.Sort(); //集合按照值从小到大排序

        //如果卡牌中符合的对牌值个数小于需要的对牌值个数，或者（卡牌值集合的最大值-比对的最小值）< 所需的值差，则直接返回空集合
        if (cardsValueTypeList.Count < (lastCount/2) || cardsValueTypeList[cardsValueTypeList.Count-1]-thanMinValueType < (lastCount/2))
        {
            return minCanOutCardsList;
        }

        int minCanOutValueType = -1; //用于存储可出牌连对中的最小值，默认设置为-1
        //对当前卡牌所有值进行循环，从需要比对的最小值开始循环，到卡牌最大值结束（循环初始值为 比对最小值、卡牌值集合最小值 两者之间的最大值）（i是用于比较的牌值）
        for (int i=thanMinValueType>cardsValueTypeList[0]?thanMinValueType:cardsValueTypeList[0]; i<=cardsValueTypeList[cardsValueTypeList.Count-1];)
        {
            //如果当前卡牌值集合不包含该卡牌值，则进入下一个循环
            if (!cardsValueTypeList.Contains(i) || i <= thanMinValueType)
            {
                i++;
                continue;
            }

            int currentCount = 0; //用于计数当前符合的对数个数
            //当前卡牌包括i，则j从i开始遍历，直到需要的最大值，查看是否其中的每种值cardsValueTypeList都有包括（不能为2）
            for (int j=i;j<i+lastCount/2;j++)
            {
                if (cardsValueTypeList.Contains(j) && j!=(int)CardManager.ValueType.TWO)
                {
                    currentCount++;
                    //如果当前对数符合需要对数，则将当前对数的最小值赋值给minCanOutValueType，然后跳出循环
                    if (currentCount==lastCount/2)
                    {
                        minCanOutValueType = i;
                        i = cardsValueTypeList[cardsValueTypeList.Count - 1] + 1; //用于跳出最外的for循环
                        break; //跳出当前for循环
                    }
                }
                else
                {
                    i = j + 1; //设置i的下一个循环值（直接跳过在j循环时已经判断过的值）
                    break;
                }
            }
        }

        //如果有符合的连对
        if (minCanOutValueType != -1)
        {
            //将符合的卡牌逐一存入minCanOutCardsList中
            for (int i = 0; i < lastCount/2; i++)
            {
                var gainCardsByType = cardsValueTypeGODict.Where(q => q.Value == minCanOutValueType).Select(q => q.Key); //在cardsValueTypeGODict中查找符合ValueType值的所有卡牌
                int currentC = 0; //存储当前已经保存的该值卡牌个数
                foreach (var item in gainCardsByType)
                {
                    minCanOutCardsList.Add(item); //连对需要其中两张卡牌
                    currentC++;
                    if (currentC==2)
                    {
                        break;
                    }
                }
                minCanOutValueType++; //存储下一个值的卡牌
            }
        }

        return minCanOutCardsList;
    }

    /// <summary>
    /// 获取当前玩家卡牌中可以出的最小三牌
    /// </summary>
    /// <param name="cards">当前玩家卡牌</param>
    /// <param name="thanMaxValueType">进行比对的最大值</param>
    /// <param name="isNeedAuxiliary">是否需要辅助函数进行初始化</param>
    /// <returns></returns>
    private List<GameObject> GetMinThreeCanOutCards(List<GameObject> cards,int thanMaxValueType/*,bool isNeedAuxiliary*/)
    {
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //用于存储可出的卡牌
        //判断是否需要辅助函数进行初始化
        //if (isNeedAuxiliary)
        //{
        //    CardsSelectAuxiliary(cards); //调用辅助函数
        //}
        //筛选出比上一个出牌最大数的值更大的值，存储于allCanOutCardsList集合中
        SetAllCanOutCardsListByTraverseDict(OUT_CARD_TYPE.THREE_WITHOUT, thanMaxValueType);
        //当前卡牌中有符合的三牌时
        if (allCanOutCardsList.Count != 0)
        {
            minCanOutCardsList = FindGOByValueType(cards, (CardManager.ValueType)allCanOutCardsList[0]); //从所有可出牌的集合中选出最小值查找GameObject
        }
        //else
        //{
        //    //直接将炸弹、王炸或者null赋值给集合
        //    minCanOutCardsList = GetMinBombCanOutCards(cards, -1, false); //炸弹、王炸不需要比对ValueType
        //}
        return minCanOutCardsList;
    }
    
    /// <summary>
    /// 获取当前玩家卡牌中可以出的最小飞机不带
    /// </summary>
    /// <param name="cards">当前玩家卡牌</param>
    /// <param name="thanMaxValueType">比对的最大值</param>
    /// <param name="count">上一个出牌数</param>
    /// <returns></returns>
    private List<GameObject> GetMinPlanesWithoutCanOutCards(List<GameObject> cards,int thanMaxValueType,int lastCount)
    {

        int thanMinValueType = thanMaxValueType - lastCount / 3 + 1; //获取飞机中最小卡牌的值（int）
        Dictionary<GameObject, int> cardsValueTypeGODict = new Dictionary<GameObject, int>(); //存储卡牌对应的ValueType（转换成int）
        List<int> cardsValueTypeList = new List<int>(); //存储卡牌中符合条件的卡牌值转换成int
        Dictionary<CardManager.ValueType, int> cardsValueTypeCountDict = new Dictionary<CardManager.ValueType, int>(); //存储卡牌值对应的数量
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //用于存储可出的卡牌

        cardsValueTypeCountDict = CardManager.instance.rememberCardValueCount.GetCardsValueCount(cards); //获取卡牌值对应的数量
        //获取各个卡牌中对应的ValueType转换成int（不重复）、获取各个卡牌对应的值转换成int
        foreach (GameObject item in cards)
        {
            int iValueType = (int)CardManager.instance.cardsVDict[item];
            //如果当前卡牌值的数量=3 （除去炸弹的可能性）且 当前卡牌值集合中不包含该卡牌值，则存入该卡牌值
            if (cardsValueTypeCountDict[(CardManager.ValueType)iValueType] == 3 && !cardsValueTypeList.Contains(iValueType))
            {
                cardsValueTypeList.Add(iValueType);
            }
            cardsValueTypeGODict.Add(item, iValueType);
        }
        cardsValueTypeList.Sort(); //集合按照值从小到大排序

        //如果卡牌中符合的对牌值个数小于需要的对牌值个数，或者（卡牌值集合的最大值-比对的最小值）< 所需的值差，则直接返回空集合
        if (cardsValueTypeList.Count < (lastCount / 3) || cardsValueTypeList[cardsValueTypeList.Count - 1] - thanMinValueType < (lastCount / 3))
        {
            return minCanOutCardsList;
        }

        int minCanOutValueType = -1; //用于存储可出牌飞机中的最小值，默认设置为-1
        //对当前卡牌所有值进行循环，从需要比对的最小值开始循环，到卡牌最大值结束（循环初始值为 比对最小值、卡牌值集合最小值 两者之间的最大值）
        for (int i = thanMinValueType > cardsValueTypeList[0] ? thanMinValueType : cardsValueTypeList[0]; i <= cardsValueTypeList[cardsValueTypeList.Count - 1];)
        {
            //如果当前卡牌值集合不包含该卡牌值，则进入下一个循环
            if (!cardsValueTypeList.Contains(i))
            {
                i++;
                continue;
            }

            int currentCount = 0; //用于计数当前符合的对数个数
            //当前卡牌包括i，则j从i开始遍历，直到需要的最大值，查看是否其中的每种值cardsValueTypeList都有包括
            for (int j = i; j < i + lastCount / 3; j++)
            {
                if (cardsValueTypeList.Contains(j) && j != (int)CardManager.ValueType.TWO)
                {
                    currentCount++;
                    //如果当前对数符合需要对数，则将当前对数的最小值赋值给minCanOutValueType，然后跳出循环
                    if (currentCount == lastCount / 3)
                    {
                        minCanOutValueType = i;
                        i = cardsValueTypeList[cardsValueTypeList.Count - 1] + 1; //用于跳出最外的for循环
                        break; //跳出当前for循环
                    }
                }
                else
                {
                    i = j + 1; //设置i的下一个循环值（直接跳过在j循环时已经判断过的值）
                    break;
                }
            }
        }

        //如果有符合的飞机
        if (minCanOutValueType != -1)
        {
            //将符合的卡牌逐一存入minCanOutCardsList中
            for (int i = 0; i < lastCount/3; i++)
            {
                var gainCardsByType = cardsValueTypeGODict.Where(q => q.Value == minCanOutValueType).Select(q => q.Key); //在cardsValueTypeGODict中查找符合ValueType值的所有卡牌
                foreach (var item in gainCardsByType)
                {
                    minCanOutCardsList.Add(item); //将三牌集合添加到minCanOutCardsList中
                }
                minCanOutValueType++; //存储下一个值的卡牌
            }
        }

        return minCanOutCardsList;
    }

    /// <summary>
    /// 获取当前玩家卡牌中可以出的最小飞机带单
    /// </summary>
    /// <param name="cards">当前玩家卡牌</param>
    /// <param name="thanMaxValueType">比对的最大值</param>
    /// <param name="lastCount">上一个出牌数</param>
    /// <returns></returns>
    private List<GameObject> GetMinPlanesWithSingleCanOutCards(List<GameObject> cards, int thanMaxValueType, int lastCount)
    {
        int thanMinValueType = thanMaxValueType - lastCount / 4 + 1; //获取飞机中最小卡牌的值（int）
        Dictionary<GameObject, int> cardsValueTypeGODict = new Dictionary<GameObject, int>(); //存储卡牌对应的ValueType（转换成int）
        List<int> cardsValueTypeList = new List<int>(); //存储卡牌中符合条件的卡牌值转换成int
        List<int> cardsWithValueTypeList = new List<int>(); //存储卡牌中符合带牌条件的卡牌值转换成int
        Dictionary<CardManager.ValueType, int> cardsValueTypeCountDict = new Dictionary<CardManager.ValueType, int>(); //存储卡牌值对应的数量
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //用于存储可出的卡牌
        List<GameObject> minWithCanOutCardsList = new List<GameObject>(); //用于存储带牌的卡牌

        cardsValueTypeCountDict = CardManager.instance.rememberCardValueCount.GetCardsValueCount(cards); //获取卡牌值对应的数量
        //获取各个卡牌中对应的ValueType转换成int（不重复）、获取各个卡牌对应的值转换成int
        foreach (GameObject item in cards)
        {
            int iValueType = (int)CardManager.instance.cardsVDict[item]; //获取当前卡牌的卡牌值

            // 存储最大数量卡牌值，如果当前卡牌值的数量=3 （除去炸弹的可能性）且 当前卡牌值集合中不包含该卡牌值 且 卡牌值大于要比对的最小值
            if (cardsValueTypeCountDict[(CardManager.ValueType)iValueType] == 3 &&  !cardsValueTypeList.Contains(iValueType) && iValueType > thanMinValueType)
            {
                cardsValueTypeList.Add(iValueType);
            }
            //存储带牌值
            else
            {
                //除去炸弹的可能（取出可用于带牌的卡牌）（除去最大值卡牌集合中已存值的三牌）
                if (cardsValueTypeCountDict[(CardManager.ValueType)iValueType]!=4 && !cardsWithValueTypeList.Contains(iValueType) && !cardsValueTypeList.Contains(iValueType))
                {
                    cardsWithValueTypeList.Add(iValueType);
                }
            }

            cardsValueTypeGODict.Add(item, iValueType); //添加 卡牌-卡牌值 字典项
        }

        cardsValueTypeList.Sort(); //最大数量卡牌值集合按照值从小到大排序
        cardsWithValueTypeList.Sort(); //带牌卡牌值集合按照值从小到大排序

        //如果卡牌中符合的飞机个数小于需要的飞机个数，或者（卡牌值集合的最大值-比对的最小值）< 所需的值差，则直接返回空集合，或者带牌数数量不够
        if (cardsValueTypeList.Count < (lastCount / 4) || cardsValueTypeList[cardsValueTypeList.Count - 1] - thanMinValueType < (lastCount / 4) || cardsWithValueTypeList.Count<(lastCount/4))
        {
            return minCanOutCardsList;
        }

        int minCanOutValueType = -1; //用于存储可出牌飞机中的最小值，默认设置为-1
        //对当前卡牌所有值进行循环，从需要比对的最小值开始循环，到卡牌最大值结束（循环初始值为 比对最小值、卡牌值集合最小值 两者之间的最大值）
        for (int i = cardsValueTypeList[0]; i <= cardsValueTypeList[cardsValueTypeList.Count - 1];)
        {
            //如果当前卡牌值集合不包含该卡牌值，则进入下一个循环
            if (!cardsValueTypeList.Contains(i))
            {
                i++;
                continue;
            }

            int currentCount = 0; //用于计数当前符合的飞机个数
            //当前卡牌包括i，则j从i开始遍历，直到需要的最大值，查看是否其中的每种值cardsValueTypeList都有包括
            for (int j = i; j < i + lastCount / 4; j++)
            {
                if (cardsValueTypeList.Contains(j) && j != (int)CardManager.ValueType.TWO)
                {
                    currentCount++;
                    //如果当前飞机个数符合需要数，则将当前对数的最小值赋值给minCanOutValueType，然后跳出循环
                    if (currentCount == lastCount / 4)
                    {
                        minCanOutValueType = i;
                        i = cardsValueTypeList[cardsValueTypeList.Count - 1] + 1; //用于跳出最外的for循环
                        break; //跳出当前for循环
                    }
                }
                else
                {
                    i = j + 1; //设置i的下一个循环值（直接跳过在j循环时已经判断过的值）
                    break;
                }
            }
        }

        //如果有符合的飞机数
        if (minCanOutValueType != -1)
        {
            //将符合的卡牌逐一存入minCanOutCardsList中
            for (int i = 0; i < lastCount/4; i++)
            {
                var gainCardsByType = cardsValueTypeGODict.Where(q => q.Value == minCanOutValueType).Select(q => q.Key); //在cardsValueTypeGODict中查找符合ValueType值的所有卡牌
                foreach (var item in gainCardsByType)
                {
                    minCanOutCardsList.Add(item); //将三牌集合添加到minCanOutCardsList中
                }
                minCanOutValueType++; //存储下一个值的卡牌
            }

            //添加带牌
            int withCount = 0; //计算当前存储的带牌数量
            for (int i=0;i<lastCount/4;i++)
            {
                var gainWithCardsByType = cardsValueTypeGODict.Where(q => q.Value == cardsWithValueTypeList[i]).Select(q => q.Key); //在cardsValueTypeGODict中查找符合ValueType值的所有卡牌
                foreach (var item in gainWithCardsByType)
                {
                    minCanOutCardsList.Add(item); //添加带牌到minCanCardsList中
                    withCount++;
                    if (withCount == lastCount / 4)
                    {
                        i = lastCount / 4; //用于跳出外循环
                        break; //跳出内循环
                    }
                }
            }
        }

        return minCanOutCardsList;
    }

    /// <summary>
    /// 获取当前玩家卡牌中可以出的最小飞机带对
    /// </summary>
    /// <param name="cards">当前玩家卡牌</param>
    /// <param name="thanMaxValueType">比对的最大值</param>
    /// <param name="lastCount">上一个出牌数</param>
    /// <returns></returns>
    private List<GameObject> GetMinPlanesWithDoubleCanOutCards(List<GameObject> cards, int thanMaxValueType, int lastCount)
    {
        int thanMinValueType = thanMaxValueType - lastCount / 5 + 1; //获取飞机中最小卡牌的值（int）
        Dictionary<GameObject, int> cardsValueTypeGODict = new Dictionary<GameObject, int>(); //存储卡牌对应的ValueType（转换成int）
        List<int> cardsValueTypeList = new List<int>(); //存储卡牌中符合条件的卡牌值转换成int
        List<int> cardsWithValueTypeList = new List<int>(); //存储卡牌中符合带牌条件的卡牌值转换成int
        Dictionary<CardManager.ValueType, int> cardsValueTypeCountDict = new Dictionary<CardManager.ValueType, int>(); //存储卡牌值对应的数量
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //用于存储可出的卡牌
        List<GameObject> minWithCanOutCardsList = new List<GameObject>(); //用于存储带牌的卡牌

        cardsValueTypeCountDict = CardManager.instance.rememberCardValueCount.GetCardsValueCount(cards); //获取卡牌值对应的数量
        //获取各个卡牌中对应的ValueType转换成int（不重复）、获取各个卡牌对应的值转换成int
        foreach (GameObject item in cards)
        {
            int iValueType = (int)CardManager.instance.cardsVDict[item]; //获取当前卡牌的卡牌值

            // 存储最大数量卡牌值，如果当前卡牌值的数量=3 （除去炸弹的可能性）且 当前卡牌值集合中不包含该卡牌值 且 卡牌值大于要比对的最小值
            if (cardsValueTypeCountDict[(CardManager.ValueType)iValueType] == 3 && !cardsValueTypeList.Contains(iValueType) && iValueType > thanMinValueType)
            {
                cardsValueTypeList.Add(iValueType);
            }
            //存储带牌值
            else
            {
                //除去炸弹的可能（取出可用于带牌的卡牌：对牌、三牌）（除去最大数量卡牌值中已存值的三牌）
                if (cardsValueTypeCountDict[(CardManager.ValueType)iValueType]>=2 && !cardsWithValueTypeList.Contains(iValueType) && !cardsValueTypeList.Contains(iValueType) && cardsValueTypeCountDict[(CardManager.ValueType)iValueType] != 4)
                {
                    cardsWithValueTypeList.Add(iValueType);
                }
            }

            cardsValueTypeGODict.Add(item, iValueType); //添加 卡牌-卡牌值 字典项
        }

        cardsValueTypeList.Sort(); //最大数量卡牌值集合按照值从小到大排序
        cardsWithValueTypeList.Sort(); //带牌卡牌值集合按照值从小到大排序

        //foreach (var item in cardsValueTypeList)
        //{
        //    Debug.Log("飞机带对，cardsValueTypeList.item="+item);
        //}
        //foreach (var item in cardsWithValueTypeList)
        //{
        //    Debug.Log("飞机带对，cardsWithValueTypeList.item=" + item);
        //}

        //如果卡牌中符合的飞机个数小于需要的飞机个数，或者（卡牌值集合的最大值-比对的最小值）< 所需的值差，则直接返回空集合，或者带牌数数量不够
        if (cardsValueTypeList.Count < (lastCount / 5) || cardsValueTypeList[cardsValueTypeList.Count - 1] - thanMinValueType < (lastCount / 5) || cardsWithValueTypeList.Count < (lastCount / 5))
        {
            return minCanOutCardsList;
        }

        int minCanOutValueType = -1; //用于存储可出牌飞机中的最小值，默认设置为-1
        //对当前卡牌所有值进行循环，从需要比对的最小值开始循环，到卡牌最大值结束（循环初始值为 比对最小值、卡牌值集合最小值 两者之间的最大值）
        for (int i = cardsValueTypeList[0]; i <= cardsValueTypeList[cardsValueTypeList.Count - 1];)
        {
            //如果当前卡牌值集合不包含该卡牌值，则进入下一个循环
            if (!cardsValueTypeList.Contains(i))
            {
                i++;
                continue;
            }

            int currentCount = 0; //用于计数当前符合的飞机个数
            //当前卡牌包括i，则j从i开始遍历，直到需要的最大值，查看是否其中的每种值cardsValueTypeList都有包括
            for (int j = i; j < i + lastCount / 5; j++)
            {
                if (cardsValueTypeList.Contains(j) && j != (int)CardManager.ValueType.TWO)
                {
                    currentCount++;
                    //如果当前飞机个数符合需要数，则将当前对数的最小值赋值给minCanOutValueType，然后跳出循环
                    if (currentCount == lastCount / 5)
                    {
                        minCanOutValueType = i;
                        i = cardsValueTypeList[cardsValueTypeList.Count - 1] + 1; //用于跳出最外的for循环
                        break; //跳出当前for循环
                    }
                }
                else
                {
                    i = j + 1; //设置i的下一个循环值（直接跳过在j循环时已经判断过的值）
                    break;
                }
            }
        }

        //如果有符合的飞机数
        if (minCanOutValueType != -1)
        {
            //将符合的卡牌逐一存入minCanOutCardsList中
            for (int i = 0; i < lastCount / 5; i++)
            {
                var gainCardsByType = cardsValueTypeGODict.Where(q => q.Value == minCanOutValueType).Select(q => q.Key); //在cardsValueTypeGODict中查找符合ValueType值的所有卡牌
                foreach (var item in gainCardsByType)
                {
                    minCanOutCardsList.Add(item); //将三牌集合添加到minCanOutCardsList中
                }
                minCanOutValueType++; //存储下一个值的卡牌
            }

            //添加带牌
            int withDoubleCount = 0; //计算当前存储带牌对数
            //int withCount = 0; //计算当前存储的数量
            for (int i = 0; i < lastCount / 5; i++)
            {
                var gainWithCardsByType = cardsValueTypeGODict.Where(q => q.Value == cardsWithValueTypeList[i]).Select(q => q.Key); //在cardsValueTypeGODict中查找符合ValueType值的所有卡牌
                foreach (var item in gainWithCardsByType)
                {
                    minCanOutCardsList.Add(item); //添加带牌到minCanCardsList中
                    withDoubleCount++;
                    if (withDoubleCount == 2)
                    {
                        //withCount++;
                        //若对数足够
                        //if (withCount==lastCount/5)
                        //{
                        //    i = lastCount / 5; //用于跳出外循环
                        //}
                        withDoubleCount = 0;
                        break; //跳出内循环
                    }
                }
            }
        }

        return minCanOutCardsList;
    }

    /// <summary>
    /// 获取当前玩家卡牌中可以出的最小炸弹
    /// </summary>
    /// <param name="cards">当前玩家卡牌</param>
    /// <param name="thanMaxValueType">进行比对的最大值</param>
    /// <param name="isNeedAuxiliary">是否需要辅助函数进行初始化</param>
    /// <returns></returns>
    private List<GameObject> GetMinBombCanOutCards(List<GameObject> cards, int thanMaxValueType/*, bool isNeedAuxiliary*/)
    {
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //用于存储可出的卡牌
        //判断是否需要辅助函数进行初始化
        //if (isNeedAuxiliary)
        //{
        //    CardsSelectAuxiliary(cards); //调用辅助函数
        //}
        //筛选出比上一个出牌最大数的值更大的值，存储于allCanOutCardsList集合中
        SetAllCanOutCardsListByTraverseDict(OUT_CARD_TYPE.BOMBS, thanMaxValueType);
        //当前卡牌中有符合的炸弹时
        if (allCanOutCardsList.Count != 0)
        {
            minCanOutCardsList = FindGOByValueType(cards, (CardManager.ValueType)allCanOutCardsList[0]); //从所有可出牌的集合中选出最小值查找GameObject
        }
        //else
        //{
        //    //直接将王炸或者null赋值给集合
        //    minCanOutCardsList = GetKingExplosionCanOutCards(cards, -1, false); //王炸不需要比对ValueType
        //}
        return minCanOutCardsList;
    }

    /// <summary>
    /// 获取当前玩家卡牌中可以出的王炸
    /// </summary>
    /// <param name="cards">当前玩家卡牌</param>
    /// <param name="thanMaxValueType">进行比对的最大值</param>
    /// <param name="isNeedAuxiliary">是否需要辅助函数进行初始化</param>
    /// <returns></returns>
    private List<GameObject> GetKingExplosionCanOutCards(List<GameObject> cards/*, int thanMaxValueType*//*, bool isNeedAuxiliary*/)
    {
        //Debug.Log("获取王炸");
        List<GameObject> minCanOutCardsList = new List<GameObject>(); //用于存储可出的卡牌
        //将有小王、或者null的集合合并到minCanOutCardsList集合中
        minCanOutCardsList = minCanOutCardsList.Concat(FindGOByValueType(cards, CardManager.ValueType.BLACKJOKER)).ToList<GameObject>();
        //将有大王、或者null的集合合并到minCanOutCardsList集合中
        minCanOutCardsList = minCanOutCardsList.Concat(FindGOByValueType(cards,CardManager.ValueType.REDJOKER)).ToList<GameObject>();
        //foreach (GameObject go in minCanOutCardsList)
        //{
        //    Debug.Log("获取王炸："+go.name);
        //}
        //如果只有大王或者小王，则将该元素移除
        if (minCanOutCardsList.Count==1)
        {
            minCanOutCardsList.Clear(); //清空存储到集合中的卡牌
        }
        //返回王炸、或者null
        return minCanOutCardsList;  
    }

    /// <summary>
    /// 通过遍历字典，将所有可以出牌的卡牌值转换为int存入集合AllCanOutCardsList中
    /// 并对集合AllCanOutCardsList进行排序
    /// </summary>
    /// <param name="outCardType">出牌类型</param>
    /// <param name="thanMaxValueType">valueType进行比对的数值</param>
    private void SetAllCanOutCardsListByTraverseDict(OUT_CARD_TYPE outCardType, int thanMaxValueType)
    {
        //清空可出的已转换成int的卡牌值集合（清除上一次数据）
        if (allCanOutCardsList.Count != 0)
        {
            allCanOutCardsList.Clear();
        }
        int count = 0; //出牌类型所对应值数量
        //根据出牌类型，设置筛选的值数量的数值
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

        //遍历值数量字典
        foreach (KeyValuePair<CardManager.ValueType, int> item in cardsValueCountDictAuxiliary)
        {
            //筛选出 值 大于 指定值 的 单牌item
            if (item.Value == count && (int)item.Key > thanMaxValueType)
            {
                allCanOutCardsList.Add((int)item.Key); 
            }
        }

        //如果单牌中同时含有小、大王，则将这两张牌在集合中去除
        if (allCanOutCardsList.Contains((int)CardManager.ValueType.BLACKJOKER) && allCanOutCardsList.Contains((int)CardManager.ValueType.REDJOKER))
        {
            allCanOutCardsList.Remove((int)CardManager.ValueType.BLACKJOKER); //去除小王
            allCanOutCardsList.Remove((int)CardManager.ValueType.REDJOKER); //去除大王
        }

        //当有符合的值时，对集合进行排序
        if (allCanOutCardsList.Count != 0)
        {
            allCanOutCardsList.Sort();
        }
    }

    /// <summary>
    /// 根据valueType查找卡牌中对应的卡牌
    /// </summary>
    /// <param name="cards">查找的卡牌</param>
    /// <param name="valueType">查找的值</param>
    /// <returns>返回在卡牌中找到的GameObject集合</returns>
    private List<GameObject> FindGOByValueType(List<GameObject> cards,CardManager.ValueType valueType)
    {
        List<GameObject> gosList = new List<GameObject>(); //用于存储找到的GameObject
        //从cardsVDict字典中选择出valueType值类型的卡牌
        var gos = CardManager.instance.cardsVDict.Where(q => q.Value == valueType).Select(q => q.Key);
        //foreach (var item in gos)
        //{
        //    Debug.Log("FindGOByValueType："+item.name);
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
    /// 设置上一个出牌信息（将当前所出的卡牌信息记录到上一个出牌信息中）
    /// 记录出牌类型、出牌最大值、出牌玩家、出牌张数
    /// <param name="cards">当前所出牌的卡牌</param>
    /// <param name="cards">当前出牌玩家</param>
    /// </summary>
    public void SetLastOutCardInfo(List<GameObject> cards,int outCardPlayer)
    {
        lastOutCardType = (int)GetOutCardType(cards); //获取出牌类型，及存入最大数量的Value至maxValueCurrentOutCardsMaxCount
        //Debug.Log("设置上一个出牌信息，lastOutCardType="+lastOutCardType);
        maxValueLastOutCardsMaxCount = (int)maxValueCurrentOutCardsMaxCount; //将 当前卡牌的最大数量值 存入 上一个出牌最大数量值 中
        lastOutCardPlayer = outCardPlayer; //设置上一个出牌玩家
        lastOutCardCount = cards.Count; //设置上一个出牌的卡牌张数
        outCardsList.Clear(); //清除出牌卡牌集合
    }

    /// <summary>
    /// 判断玩家卡牌是否可以出牌（出牌类型、出牌值—>通过比对上一个出牌）
    /// </summary>
    /// <param name="cards">玩家出牌的卡牌</param>
    /// <param name="currentPlayer">当前玩家</param>
    /// <returns>默认返回false</returns>
    public bool IsCanOutCards(List<GameObject> cards,int currentPlayer)
    {
        OUT_CARD_TYPE currentOutCardType = GetOutCardType(cards); //获取当前出牌卡牌的出牌类型
        //Debug.Log("判断玩家是否可以出牌，出牌类型="+currentOutCardType);
        //当前出牌类型不合法
        if (currentOutCardType == OUT_CARD_TYPE.INCONFORMITY) return false;

        // 当前玩家为第一个出牌玩家 || 当前出牌玩家为上一个出牌玩家
        if (lastOutCardType == -1 || currentPlayer == lastOutCardPlayer)  return true; //可以出任意合法类型的卡牌

        //判断当前玩家出牌类型是否符合上一个出牌类型
        if ((int)currentOutCardType != lastOutCardType)
        {
            //如果当前出牌卡牌为炸弹或者王炸，则可以出牌
            if ((int)currentOutCardType==13 || (int)currentOutCardType==14)
            {
                return true;
            }
            return false;
        }
        //判断当前玩家出牌最大数量的值是否大于上一个
        if ((int)maxValueCurrentOutCardsMaxCount > maxValueLastOutCardsMaxCount) return true;

        return false;
    }
    /// <summary>
    /// 出牌卡牌排序
    /// </summary>
    /// <param name="cards">出牌卡牌</param>
    public void SetOutCardsListSort(List<GameObject> cards)
    {
        List<GameObject> cardsCopy = new List<GameObject>(); //用于中间存储卡牌
        Dictionary<CardManager.ValueType, int> cardsValueTypeCountDict = new Dictionary<CardManager.ValueType, int>(); //存储指定卡牌各个值的数量
        Dictionary<GameObject, CardManager.ValueType> cardsValueTypeGODict = new Dictionary<GameObject, CardManager.ValueType>(); //存储各个卡牌的值

        //存储各个卡牌对应的ValueType
        foreach (GameObject item in cards)
        {
            if (!cardsValueTypeGODict.ContainsKey(item))
            {
                cardsValueTypeGODict.Add(item, CardManager.instance.cardsVDict[item]);

            }
            //Debug.Log("重排前，卡牌：" + item.name);
        }

        cardsValueTypeCountDict =CardManager.instance.rememberCardValueCount.GetCardsValueCount(cards); //获取卡牌中各个值的数量存入字典中
        List<int> countValuesCount = new List<int>(); //存储卡牌中各个值的数量
        //卡牌值的各个数量存入集合中
        foreach (int item in cardsValueTypeCountDict.Values)
        {
            //Debug.Log("出牌卡牌排序中cardsValueTypeCountDict.Values=" + item);
            //一种数量只添加一次
            if (!countValuesCount.Contains(item))
            {
                countValuesCount.Add(item);
            }
        }
        countValuesCount.Sort(); //按数量大小从小到大排序
        //Debug.Log("countValuesToInt.Count="+countValuesToInt.Count);

        for(int i = countValuesCount.Count - 1; i > -1; i--)
        {
            var countValues = cardsValueTypeCountDict.Where(q => q.Value == countValuesCount[i]).Select(q => q.Key); //寻找cardsValueTypeCountDict字典中指定数量所对应的所有值
            List<int> countVtoIList = new List<int>(); //存储countValue转换成int后的值
            foreach (var item in countValues)
            {
                //Debug.Log("countValues.item="+item);
                countVtoIList.Add((int)item); //将值转换成int存入countVtoIList中
            }
            countVtoIList.Sort(); //按值从小到大排序
            foreach (int item in countVtoIList)
            {
                var gosByValue = cardsValueTypeGODict.Where(q => q.Value == (CardManager.ValueType)item).Select(q=>q.Key); //根据值获取GameObjects
                //foreach (var gosItem in gosByValue)
                //{
                //    Debug.Log("gosByValue.item="+gosItem);
                //}
                cardsCopy.AddRange(gosByValue); //将GameObject存入cardsCopy中
            }
        }

        cards.Clear(); 
        cards.AddRange(cardsCopy); //将重新排序的cardsCopy存入cards中

        //foreach (GameObject item in cards)
        //{
        //    Debug.Log("重排后，卡牌：" + item.name);
        //}
    }
    /// <summary>
    /// 设置出牌卡牌在Hierarchy面板中的位置（解决遮挡情况）
    /// </summary>
    /// <param name="cards">需要调整位置的卡牌集合</param>
    public void SetOutCardsSitsInHierarchy(List<GameObject> cards)
    {
        for (int i=0;i<cards.Count;i++)
        {
            cards[i].GetComponent<RectTransform>().SetSiblingIndex(i); //将出牌的卡牌设置到所有卡牌的最前面位置
        }
    }

}
