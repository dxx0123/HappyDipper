using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using Antlr.Runtime.Tree;

[RequireComponent(typeof(RememberCardValueCount))]
public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    private RectTransform thisParentRT; //该脚本所在物体的RT引用
    private GamePlayManager gamePlayManager; //脚本GamePlayManager的引用

    public Dictionary<GameObject, ValueType> cardsVDict = new Dictionary<GameObject, ValueType>();  //存储所有卡牌以及它们的Value
    Dictionary<GameObject, PictureType> cardsPDict = new Dictionary<GameObject, PictureType>();   //存储所有卡牌以及它们的Picture
    
    public List<GameObject> cardsList = new List<GameObject>();   //存储所有卡牌
    public Dictionary<ValueType, int> cardsListValueCount = new Dictionary<ValueType, int>();  //存储所有卡牌的各个值数量

    public List<GameObject> cardsLeftPlayer = new List<GameObject>();   //左边玩家所持卡牌
    public Dictionary<ValueType, int> cardsLeftPlayerValueCount = new Dictionary<ValueType, int>();   //存储左边玩家卡牌的各个值数量

    public List<GameObject> cardsPlayer = new List<GameObject>();   //玩家所持卡牌
    public Dictionary<ValueType, int> cardsPlayerValueCount = new Dictionary<ValueType, int>();   //存储玩家卡牌的各个值数量

    public List<GameObject> cardsRightPlayer = new List<GameObject>();   //右边玩家所持卡牌
    public Dictionary<ValueType, int> cardsRightPlayerValueCount = new Dictionary<ValueType, int>();   //存储右边玩家卡牌的各个值数量

    List<GameObject> cardsLandlord = new List<GameObject>();   //地主牌
    public List<GameObject> cardsLandlordCopy=new List<GameObject>();  //复制地主牌放置在屏幕上方
    public Dictionary<ValueType, int> cardsLandlordValueCount = new Dictionary<ValueType, int>();   //存储地主卡牌的各个值数量

    public RememberCardValueCount rememberCardValueCount;   //计算卡牌各个值数量的类引用
    public RememberCardDevice rememberCardDevice; //记牌器脚本的类引用

    public GameObject leftPlayerRemainCardsCountGO; //左边玩家剩余卡牌数
    public GameObject rightPlayerRemainCardsCountGO; //右边玩家剩余卡牌数

    private Vector3[] rememberCardDevicePos; //存储记牌器位置（0—>屏外，1—>屏内）

    public RectTransform playerJoyBeansRT; //玩家欢乐豆的RT

    /// <summary>
    /// 卡牌Value类型
    /// </summary>
    public enum ValueType
    {
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        TEN,
        JJ,
        QQ,
        KK,
        AA,
        TWO,
        BLACKJOKER,
        REDJOKER
    }
    /// <summary>
    /// 卡牌picture类型
    /// </summary>
    enum PictureType
    {
        HEARTS,
        DIAMONDS,
        CLUBS,
        SPADES,
        BLACK_JOKER,
        RED_JOKER
    }
    /// <summary>
    /// 赋予脚本引用
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 将所有卡牌都存入集合cardsList中
    /// </summary>
    /// <param name="cards"></param>
    private void Start()
    {
        thisParentRT = (RectTransform)this.GetComponent<Transform>().parent; //设置父物体RT引用的临时变量

        rememberCardValueCount = this.GetComponent<RememberCardValueCount>(); //获取计算牌值数量的类引用
        //Debug.Log(thisParentRT.Find("RememberCardDevice"));
        rememberCardDevice = thisParentRT.Find("RememberCardDevice").GetComponent<RememberCardDevice>(); //获取记牌器脚本的引用

        gamePlayManager=thisParentRT.AddComponent<GamePlayManager>(); //父物体BG中添加脚本GamePlayManager组件，并获取其引用

        //左边玩家剩余卡牌数（默认禁用）
        leftPlayerRemainCardsCountGO = (GameObject)Instantiate(Resources.Load("Prefabs/Cards/RemainCardsCountPre"), this.transform);
        leftPlayerRemainCardsCountGO.name = "leftPlayerRemainCardsCountGO";
        //leftPlayerRemainCardsCountGO.GetComponent<RectTransform>().localPosition = new Vector3(-880f, 35f, 0f);
        leftPlayerRemainCardsCountGO.SetActive(false);
        //右边玩家剩余卡牌数（默认禁用）
        rightPlayerRemainCardsCountGO = (GameObject)Instantiate(Resources.Load("Prefabs/Cards/RemainCardsCountPre"), this.transform);
        leftPlayerRemainCardsCountGO.name = "rightPlayerRemainCardsCountGO";
        //rightPlayerRemainCardsCountGO.GetComponent<RectTransform>().localPosition = new Vector3(880f, 35f, 0f);
        rightPlayerRemainCardsCountGO.SetActive(false);

        GainAllCard(cardsList, cardsVDict, cardsPDict);   //将所有卡牌存入集合cardsList中
        cardsListValueCount = rememberCardValueCount.GetCardsValueCount(cardsList);   //获取所有卡牌中各个值的数量

        rememberCardDevicePos = new Vector3[2] { new Vector3(5000f, 5000f, 0f) , new Vector3(99f, -101f, 0f) }; //记牌器位置初始化

        playerJoyBeansRT = (RectTransform)thisParentRT.Find("PlayerJoyBeans"); //获取玩家欢乐豆的RT

        GameCardInit();  //游戏卡牌初始化
    }
    /// <summary>
    /// 游戏卡牌初始化
    /// 所有卡牌乱序
    /// 分配卡牌
    /// 获取左中右地主玩家卡牌各个值的数量
    /// 左中右玩家卡牌排序
    /// 卡牌分配动画
    /// </summary>
    public void GameCardInit()
    {
        //设置玩家欢乐豆
        playerJoyBeansRT.parent = thisParentRT.Find("Player"); //设置父物体
        AdaptiveScript.Instance.SetAnchor(playerJoyBeansRT.gameObject,AdaptiveScript.ANCHOR.MIDDLE); //设置玩家欢乐豆锚点位置
        playerJoyBeansRT.localPosition = new Vector3(118f, -64f, 0f); //设置位置
        playerJoyBeansRT.localScale = new Vector3(0.3f, 0.3f, 1f); //缩小

        CardsDistribution(CardsOutOfOrder(cardsList));     //将乱序后的卡牌进行分配

        cardsLeftPlayerValueCount = rememberCardValueCount.GetCardsValueCount(cardsLeftPlayer);   //获取左边玩家卡牌中各个值的数量
        cardsPlayerValueCount = rememberCardValueCount.GetCardsValueCount(cardsPlayer);   //获取玩家卡牌中各个值的数量
        cardsRightPlayerValueCount = rememberCardValueCount.GetCardsValueCount(cardsRightPlayer);   //获取右边玩家卡牌中各个值的数量
        cardsLandlordValueCount = rememberCardValueCount.GetCardsValueCount(cardsLandlord);   //获取地主卡牌中各个值的数量

        //左边玩家、玩家、右边玩家的卡牌进行排序
        CardsSort(cardsLeftPlayer, cardsLeftPlayerValueCount);
        CardsSort(cardsPlayer, cardsPlayerValueCount);
        CardsSort(cardsRightPlayer, cardsRightPlayerValueCount);

        rememberCardDevice.SetRememberCardDeviceValue(cardsListValueCount); //设置记牌器中各个值
        rememberCardDevice.GetComponent<RectTransform>().localPosition = rememberCardDevicePos[0]; //将记牌器的位置设置到屏幕外

        //卡牌分配动画（动画结束，开始游戏）
        CardsMove();
    }
    /// <summary>
    /// 卡牌重置
    /// </summary>
    public void GameCardReset()
    {
        for(int i=0;i<cardsList.Count;i++)
        {
            RectTransform cardsListRT = cardsList[i].GetComponent<RectTransform>();
            cardsListRT.Find("Back").gameObject.SetActive(true);  //启用所有卡牌的背面
            cardsListRT.position = this.transform.position; //所有卡牌均设置到初始位置
            cardsListRT.rotation = Quaternion.Euler(0f, 0f, 0f);  //取消卡牌的旋转
        }

        if (cardsLandlordCopy.Count!=0)
        {
            for (int i = 0;i<cardsLandlordCopy.Count;i++)
            {
                Destroy(cardsLandlordCopy[i]); //删除复制地主牌的中卡牌
            }
            cardsLandlordCopy.Clear(); //清空复制地主牌集合
        }

        ///清除各个玩家所存储的卡牌元素
        //左边玩家
        cardsLeftPlayer.Clear();  //清除左边玩家卡牌集合所有元素
        cardsLeftPlayerValueCount.Clear();  //清除左边玩家卡牌值数量的字典所有元素
        //中间玩家
        cardsPlayer.Clear();
        cardsPlayerValueCount.Clear();
        //右边玩家
        cardsRightPlayer.Clear();
        cardsRightPlayerValueCount.Clear();
        //地主牌
        cardsLandlord.Clear();
        cardsLandlordValueCount.Clear();
        //游戏卡牌初始化
        GameCardInit();
    }
    /// <summary>
    /// 发放地主牌
    /// </summary>
    public void IssueLandlordCards(int landlordPlayer)
    {
        //Debug.Log("发放地主牌，地主玩家是："+landlordPlayer);
        //声明临时动画序列
        DG.Tweening.Sequence quence = DOTween.Sequence();
        //卡牌从起点移动到终点所需时间
        float duration = 0.2f;
        //卡牌之间的偏移量
        float offset = 50f;

        //给地主玩家添加地主牌、值数量
        switch (landlordPlayer)
        {
            case 0:
                //添加地主牌给地主玩家的集合中
                for (int i = 0; i < cardsLandlord.Count; i++)
                {
                    cardsLandlord[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(true); //启用背面
                    cardsLeftPlayer.Add(cardsLandlord[i]);
                }
                //添加地主牌值数量给地主玩家的值数量中
                foreach (KeyValuePair<ValueType, int> item in cardsLandlordValueCount)
                {
                    if (cardsLeftPlayerValueCount.ContainsKey(item.Key))
                    {
                        cardsLeftPlayerValueCount[item.Key] += cardsLandlordValueCount[item.Key];  //添加相同值的数量
                    }
                    else
                    {
                        cardsLeftPlayerValueCount.Add(item.Key,item.Value);
                    }
                }
                //卡牌排序
                CardsSort(cardsLeftPlayer,cardsLeftPlayerValueCount);    
                //设置卡牌位置
                for (int i = 0; i < cardsLandlord.Count; i++)
                {
                    RectTransform rt0 = cardsLandlord[i].GetComponent<RectTransform>();
                    quence.Append(rt0.DOLocalMove(AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.LEFT, new Vector3(80f, 35f, 0f)), 0.1f));
                    //quence.Append(rt0.DOLocalMove(new Vector3(-880f, 35f, 0f), 0.1f));
                }
                leftPlayerRemainCardsCountGO.GetComponent<Text>().text = (int.Parse(leftPlayerRemainCardsCountGO.GetComponent<Text>().text) + cardsLandlord.Count).ToString();
                //RectTransform cardsLeftPlayerRT;
                //for (int i=0;i<cardsLeftPlayer.Count;i++)
                //{
                //    cardsLeftPlayerRT = cardsLeftPlayer[i].GetComponent<RectTransform>();
                //    cardsLeftPlayerRT.localPosition = new Vector3(-830f, 410f - (i * offset), 0f);
                //    cardsLeftPlayerRT.localRotation = Quaternion.Euler(0f,0f,-90f);
                //}
                break;
            case 1:
                //添加地主牌给地主玩家的集合中
                for (int i = 0; i < cardsLandlord.Count; i++)
                {
                    cardsLandlord[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //禁用背面
                    cardsPlayer.Add(cardsLandlord[i]);
                }
                //添加地主牌值数量给地主玩家的值数量中
                foreach (KeyValuePair<ValueType, int> item in cardsLandlordValueCount)
                {
                    if (cardsPlayerValueCount.ContainsKey(item.Key))
                    {
                        cardsPlayerValueCount[item.Key] += cardsLandlordValueCount[item.Key];  //添加相同值的数量
                    }
                    else
                    {
                        cardsPlayerValueCount.Add(item.Key, item.Value);
                    }
                }
                //卡牌排序
                CardsSort(cardsPlayer, cardsPlayerValueCount);
                Vector3 cardsPlayerTargetPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.BUTTOM, new Vector3(0, 140, 0)) + new Vector3(-(cardsPlayer.Count / 2f) * offset, 0f, 0f);
                //设置卡牌位置
                RectTransform cardsPlayerRT;
                for (int i = 0; i < cardsPlayer.Count; i++)
                {
                    cardsPlayerRT = cardsPlayer[i].GetComponent<RectTransform>();
                    
                    cardsPlayerRT.localPosition = cardsPlayerTargetPos+new Vector3(i * offset, 0f, 0f);
                    //cardsPlayerRT.localPosition = new Vector3(-540f + (i * (offset + 10f)), -400f, 0f);
                }
                break;
            case 2:
                //添加地主牌给地主玩家的集合中
                for (int i = 0; i < cardsLandlord.Count; i++)
                {
                    cardsLandlord[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(true); //启用背面
                    cardsRightPlayer.Add(cardsLandlord[i]);
                }
                //添加地主牌值数量给地主玩家的值数量中
                foreach (KeyValuePair<ValueType, int> item in cardsLandlordValueCount)
                {
                    //cardsRightPlayerValueCount[item.Key] += cardsLandlordValueCount[item.Key];  //添加相同值的数量

                    if (cardsRightPlayerValueCount.ContainsKey(item.Key))
                    {
                        cardsRightPlayerValueCount[item.Key] += cardsLandlordValueCount[item.Key];  //添加相同值的数量
                    }
                    else
                    {
                        cardsRightPlayerValueCount.Add(item.Key, item.Value);
                    }
                }
                //卡牌排序
                CardsSort(cardsRightPlayer, cardsRightPlayerValueCount);
                //设置卡牌位置
                for (int i = 0; i < cardsLandlord.Count; i++)
                {
                    RectTransform rt0 = cardsLandlord[i].GetComponent<RectTransform>();
                    quence.Append(rt0.DOLocalMove(AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.RIGHT, new Vector3(-80f, 35f, 0f)), 0.1f));
                    //quence.Append(rt0.DOLocalMove(new Vector3(880f, 35f, 0f), 0.1f));
                }
                rightPlayerRemainCardsCountGO.GetComponent<Text>().text = (int.Parse(rightPlayerRemainCardsCountGO.GetComponent<Text>().text) + cardsLandlord.Count).ToString();
                //RectTransform cardsRightPlayerRT;
                //for (int i = 0; i < cardsRightPlayer.Count; i++)
                //{
                //    cardsRightPlayerRT = cardsRightPlayer[i].GetComponent<RectTransform>();
                //    cardsRightPlayerRT.localPosition = new Vector3(830f, -390f + (i * offset), 0f);
                //    cardsRightPlayerRT.localRotation = Quaternion.Euler(0f,0f,90f);
                //}
                break;
        }

        //设置地主牌动画
        for (int i = 0; i < cardsLandlord.Count; i++)
        {
            cardsLandlordCopy.Add(Instantiate(cardsLandlord[i], thisParentRT));  //复制地主牌
            cardsLandlordCopy[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(true);  //启用复制地主牌的背面
            AdaptiveScript.Instance.SetAnchor(cardsLandlordCopy[i], AdaptiveScript.ANCHOR.TOP); //设置复制地主卡牌锚点
            cardsLandlordCopy[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-200f + (i * 200f), -140f, 0f);  //设置所复制的地主牌的位置
            //Debug.Log("地主牌："+cardsLandlord[i]);
            //获取卡牌的RT组件引用
            RectTransform cardsLandlordCopyRT = cardsLandlordCopy[i].GetComponent<RectTransform>();
            //定义中间RT引用，用于OnComplete()
            RectTransform itemRT = cardsLandlordCopyRT;
            //卡牌旋转到90°位置，旋转结束后，禁用Back显示卡牌正面
            quence.Append(cardsLandlordCopyRT.DOBlendableLocalRotateBy(new Vector3(0f, 90f, 0f), duration)
                .OnComplete(() => { itemRT.Find("Back").gameObject.SetActive(false); })
                );
            //卡牌反向旋转回0°
            quence.Append(cardsLandlordCopyRT.DOBlendableLocalRotateBy(new Vector3(0f, -90f, 0f), duration));
        }

        rememberCardDevice.GetComponent<RectTransform>().anchoredPosition3D = rememberCardDevicePos[1]; //将记牌器的位置设置到屏幕内
    }

    /// <summary>
    /// 卡牌分配动画
    /// </summary>
    //private bool isMoveComplete = false;   //用于判断卡牌是否发放完毕
    //public bool IsMoveComplete
    //{
    //    get { return isMoveComplete; }
    //}
    private void CardsMove()
    {
        MusicControl.Instance.PLaySound(MusicControl.SOUND.BEGIN_GAME_MUSIC_01); //播放开始游戏背景音乐
        MusicControl.Instance.PLaySound(MusicControl.SOUND.DEAL_CARDS_SOUND); //播放卡牌发放音效

        float duration = 0.02f;  //卡牌从起点移动到终点所需时间
        float offset = 60f;  //卡牌之间的偏移量

        RectTransform cardsLeftRT;  //左边玩家RectTransform组件引用
        Vector3 leftPlayerStartCardPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.LEFT, new Vector3(80f, 35f, 0f)); //设置左边玩家卡牌的起始位置

        RectTransform cardsPlayerRT;  //中间玩家RectTransform组件引用
        Vector3 playerStartCardPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.BUTTOM,new Vector3(0,140,0)) + new Vector3(-(cardsPlayer.Count/2)*offset,0f,0f); //设置中间玩家卡牌的起始位置

        RectTransform cardsRightRT;  //右边玩家RectTransform组件引用
        Vector3 rightPlayerStartCardPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.RIGHT, new Vector3(-80f, 35f, 0f)); //设置右边玩家卡牌的起始位置

        //Debug.LogFormat("leftPlayerStartCardPos={0},playerStartCardPos={1},rightPlayerStartCardPos={2}", leftPlayerStartCardPos, playerStartCardPos, rightPlayerStartCardPos);

        DG.Tweening.Sequence quence = DOTween.Sequence();    //定义动画序列

        //设置左、中、右玩家的卡牌
        for (int i = 0; i < cardsLeftPlayer.Count; i++)
        {
            //仅进行17次循环，依次为左中右玩家分配卡牌
            cardsLeftRT = cardsLeftPlayer[i].GetComponent<RectTransform>();  //定义左边玩家第i张卡牌的RectTransform组件引用
            cardsPlayerRT = cardsPlayer[i].GetComponent<RectTransform>();  //定义中间玩家第i张卡牌的RectTransform组件引用
            cardsRightRT = cardsRightPlayer[i].GetComponent<RectTransform>();  //定义右边玩家第i张卡牌的RectTransform组件引用

            //左边玩家卡牌设置
            quence.Append(cardsLeftRT.DOLocalMove(leftPlayerStartCardPos, duration));  //将动画加入动画序列中
            //quence.Append(cardsLeftRT.DOLocalMove(new Vector3(-880f, 35f, 0f), duration));  //将动画加入动画序列中
            //quence.Append(cardsLeftRT.DOLocalMove(new Vector3(-830f, 410f - (i * offset), 0f), duration));  //将动画加入动画序列中
            //RectTransform itemLeftRT=cardsLeftRT;  //定义中间的RectTransform引用，用于动画委托
            //quence.Join(itemLeftRT.DOLocalRotate(new Vector3(0f, 0f, -90f), duration).OnComplete(() => { itemLeftRT.Find("Back").gameObject.SetActive(false); }));  //将卡牌旋转动画添加到队列中与移动动画一起执行，并设置执行完毕后禁用“Back”

            //中间玩家卡牌设置
            RectTransform itemPlayerRT = cardsPlayerRT;
            quence.Append(itemPlayerRT.DOLocalMove(playerStartCardPos+new Vector3(i * offset, 0f, 0f), duration).OnComplete(() => { itemPlayerRT.Find("Back").gameObject.SetActive(false); }));
            //quence.Append(itemPlayerRT.DOLocalMove(new Vector3(-540f + (i * (offset + 10f)), -400f, 0f), duration).OnComplete(() => { itemPlayerRT.Find("Back").gameObject.SetActive(false); }));

            //右边玩家卡牌设置
            RectTransform itemRightRT = cardsRightRT;
            quence.Append(cardsRightRT.DOLocalMove(rightPlayerStartCardPos, duration));
            //quence.Append(cardsRightRT.DOLocalMove(new Vector3(880f, 35f, 0f), duration));
            //quence.Append(cardsRightRT.DOLocalMove(new Vector3(830f, -390f + (i * offset), 0f), duration));
            //quence.Join(itemRightRT.DOLocalRotate(new Vector3(0f, 0f, 90f), duration).OnComplete(() => { itemRightRT.Find("Back").gameObject.SetActive(false); }));

            //每4张卡牌停顿一次
            //if (i % 1 == 0)
            //{
            //    quence.AppendInterval(0.1f);   //延迟停顿感
            //}
        }

        //启用玩家剩余卡牌数
        RectTransform leftPlayerRemainCardsCountGORT = leftPlayerRemainCardsCountGO.GetComponent<RectTransform>();
        leftPlayerRemainCardsCountGORT.localPosition = leftPlayerStartCardPos;
        leftPlayerRemainCardsCountGORT.SetAsLastSibling();
        leftPlayerRemainCardsCountGO.GetComponent<Text>().text = cardsLeftPlayer.Count.ToString();
        leftPlayerRemainCardsCountGO.SetActive(true);

        RectTransform rightPlayerRemainCardsCountGORT = rightPlayerRemainCardsCountGO.GetComponent<RectTransform>();
        rightPlayerRemainCardsCountGORT.localPosition = rightPlayerStartCardPos; //设置位置
        rightPlayerRemainCardsCountGORT.SetAsLastSibling(); //调整在Hierarchy面板中的位置
        rightPlayerRemainCardsCountGO.GetComponent<Text>().text = cardsRightPlayer.Count.ToString(); //文本显示
        rightPlayerRemainCardsCountGO.SetActive(true); //启用

        //设置地主卡牌
        quence.AppendInterval(0.1f);  //设置延迟，避免左中右玩家卡牌未发放完毕，地主卡牌即先发放
        RectTransform cardsLandlordRT;
        for (int i = 0; i < cardsLandlord.Count; i++)
        {
            cardsLandlordRT = cardsLandlord[i].GetComponent<RectTransform>();
            RectTransform itemLandlordRT = cardsLandlordRT;
            switch (i)
            {
                case 0:
                    quence.Append(itemLandlordRT.DOLocalMove( AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.TOP, new Vector3(-200f, -140f, 0f)) , 20 * duration));
                    break;
                case 1:
                    quence.Append(itemLandlordRT.DOLocalMove( AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.TOP,new Vector3(0f,-140f,0f)), 20 * duration));
                    break;
                case 2:
                    quence.Append(itemLandlordRT.DOLocalMove(AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.TOP, new Vector3(200f, -140f, 0f)), 20 * duration).OnComplete(
                        () => {
                            MusicControl.Instance.StopOnceShotMusic(); //暂停发牌音效
                            gamePlayManager.Init(); //初始化GamePlayManager 并开始游戏
                              }
                        ));
                    break;
            }
            //前两张地主牌移动
            //if(i<cardsLandlord.Count-1)
            //{
            //    quence.Append(itemLandlordRT.DOLocalMove(new Vector3(-200f + (i * 200f), 140f, 0f), 20*duration));
            //}
            ////最后一张地主牌移动
            //else
            //{
            //    quence.Append(itemLandlordRT.DOLocalMove(new Vector3(-200f + (i * 200f), 140f, 0f), 20*duration).OnComplete(
            //        ()=> {
            //                MusicControl.Instance.StopOnceShotMusic(); //暂停发牌音效
            //                gamePlayManager.Init(); //初始化GamePlayManager 并开始游戏
            //        }
            //        ));
            //}
        }
    }
    /// <summary>
    /// 协程：辅助卡牌分配动画
    /// </summary>
    /// <param name="rtf">卡牌RT组件</param>
    /// <param name="waitTime">延迟执行时间</param>
    /// <returns></returns>
    //IEnumerator RTWaitIE(RectTransform rtf, float waitTime)
    //{
    //    yield return new WaitForSeconds(waitTime);
    //    rtf.Find("Back").gameObject.SetActive(false);
    //}
    /// <summary>
    /// 卡牌排序
    /// </summary>
    /// <param name="cards"></param>
    private void CardsSort(List<GameObject> cards,Dictionary<ValueType,int> cardsValueCountDict)
    {
        ///******通过比对卡牌的value值进行排序******///
        GameObject valueMinCard;
        for (int i = 0; i < cards.Count; i++)
        {
            valueMinCard = cards[i];
            for (int j = i; j < cards.Count; j++)
            {
                if ((int)cardsVDict[valueMinCard] > (int)cardsVDict[cards[j]])
                {
                    valueMinCard = cards[j];
                    cards[j] = cards[i];
                    cards[i] = valueMinCard;
                }
            }
        }
        ///****在卡牌的value值进行排序之后，再对相同value的卡牌通过比对picture再进行排序****///
        GameObject pictureMinCard;
        foreach(KeyValuePair<ValueType,int> valueCountItem in cardsValueCountDict)
        {
            //遍历cardsValueCountDict——依次查看cards中相同值的卡牌数
            //判断相同值的卡牌数是否大于1——选择出数量大于1的相同值，进行排序
            if (valueCountItem.Value > 1)
            {
                //通过i循环确定相同值的第一个位置
                for (int i=0;i<cards.Count;i++)
                {
                    if(cardsVDict[cards[i]]==valueCountItem.Key)
                    {
                        //通过j、k循环对相同值进行比对picture排序
                        //i+valueCountItem——相同值的最后一个位置+1（所以不能<=）
                        for (int j=i;j<i+valueCountItem.Value;j++)
                        {
                            pictureMinCard = cards[j];
                            for (int k=j;k<i+valueCountItem.Value;k++)
                            {
                                //比对picture
                                if ((int)cardsPDict[cards[j]] > (int)cardsPDict[cards[k]])
                                {
                                    pictureMinCard = cards[k];
                                    cards[k] = cards[j];
                                    cards[j] = pictureMinCard;
                                }

                            }
                        }
                        break;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 卡牌分配
    /// </summary>
    /// <param name="cardsOutOfOrder">卡牌集合</param>
    private void CardsDistribution(List<GameObject> cardsOutOfOrder)
    {
        for(int i=0;i<cardsOutOfOrder.Count;i++)
        {
            switch (i / 17)
            {
                case 0:
                    cardsLeftPlayer.Add(cardsOutOfOrder[i]);
                    break;
                case 1:
                    cardsPlayer.Add(cardsOutOfOrder[i]);
                    break;
                case 2:
                    cardsRightPlayer.Add(cardsOutOfOrder[i]);
                    break;
                case 3:
                    cardsLandlord.Add(cardsOutOfOrder[i]);
                    break;
            }

            //测试
            //Debug.Log(cardsOutOfOrder[i].name);
            //switch (cardsOutOfOrder[i].name)
            //{
            //    case "FIVE-HEARTS":
            //    case "FIVE-SPADES":
            //    case "FIVE-DIAMONDS":
            //    case "FIVE-CLUBS":
            //    case "NINE-CLUBS":
            //    case "NINE-HEARTS":
            //    case "NINE-DIAMONDS":
            //    case "NINE-SPADES":
            //    case "JJ-HEARTS":
            //    case "JJ-SPADES":
            //    case "QQ-CLUBS":
            //    case "QQ-HEARTS":
            //    case "QQ-DIAMONDS":
            //    case "TWO-CLUBS":
            //    case "TWO-SPADES":
            //    case "BLACKJOKER-BLACK_JOKER":
            //    case "REDJOKER-RED_JOKER":
            //        {
            //            cardsPlayer.Add(cardsOutOfOrder[i]);
            //            //cardsOutOfOrder.Remove(cardsOutOfOrder[i]);
            //            break;
            //        }

            //    case "THREE-CLUBS":
            //    case "THREE-SPADES":
            //    case "THREE-HEARTS":
            //    case "THREE-DIAMONDS":
            //    case "SIX-DIAMONDS":
            //    case "SIX-CLUBS":
            //    case "SIX-HEARTS":
            //    case "SIX-SPADES":
            //    case "SEVEN-HEARTS":
            //    case "SEVEN-SPADES":
            //    case "SEVEN-CLUBS":
            //    case "SEVEN-DIAMONDS":
            //    case "EIGHT-CLUBS":
            //    case "EIGHT-SPADES":
            //    case "EIGHT-DIAMONDS":
            //    case "TWO-DIAMONDS":
            //    case "KK-DIAMONDS":

            //        {
            //            cardsRightPlayer.Add(cardsOutOfOrder[i]);
            //            //cardsOutOfOrder.Remove(cardsOutOfOrder[i]);
            //            break;
            //        }

            //    case "FOUR-HEARTS":
            //    case "FOUR-CLUBS":
            //    case "FOUR-DIAMONDS":
            //    case "FOUR-SPADES":
            //    case "EIGHT-HEARTS":
            //    case "TEN-HEARTS":
            //    case "TEN-CLUBS":
            //    case "TEN-DIAMONDS":
            //    case "TEN-SPADES":
            //    case "JJ-DIAMONDS":
            //    case "JJ-CLUBS":
            //    case "QQ-SPADES":
            //    case "KK-HEARTS":
            //    case "KK-CLUBS":
            //    case "KK-SPADES":
            //    case "AA-CLUBS":
            //    case "AA-SPADES":
            //        {
            //            cardsLeftPlayer.Add(cardsOutOfOrder[i]);
            //            //cardsOutOfOrder.Remove(cardsOutOfOrder[i]);
            //            break;
            //        }
            //    case "TWO-HEARTS":
            //    case "AA-DIAMONDS":
            //    case "AA-HEARTS":
            //        {
            //            cardsLandlord.Add(cardsOutOfOrder[i]);
            //            //cardsOutOfOrder.Remove(cardsOutOfOrder[i]);
            //            break;
            //        }
            //}

        }
        //Debug.Log(cardsPlayer.Count);
        //Debug.Log(cardsRightPlayer.Count);
        //Debug.Log(cardsLeftPlayer.Count);
    }
    /// <summary>
    /// 所有卡牌乱序
    /// </summary>
    /// <param name="cardsList"></param>
    /// <returns>返回乱序之后的卡牌集合</returns>
    private List<GameObject> CardsOutOfOrder(List<GameObject> cardsList)
    {
        List<GameObject> cardsListCopy = new List<GameObject>();
        List<GameObject> cardsListOutOrder = new List<GameObject>();
        for(int i=0;i<cardsList.Count;i++)
        {
            cardsListCopy.Add(cardsList[i]);
        }
        for(int i=0;i<cardsList.Count;i++)
        {
            int random = UnityEngine.Random.Range(0,cardsListCopy.Count);
            cardsListOutOrder.Add(cardsListCopy[random]);
            cardsListCopy.RemoveAt(random);
        }
        return cardsListOutOrder;
    }
    /// <summary>
    /// 将所有卡牌均存入集合中，将所有卡牌及Value值存入字典中
    /// </summary>
    /// <param name="cards"></param>
    private void GainAllCard(List<GameObject> cards,Dictionary<GameObject,ValueType> cardsVDict,Dictionary<GameObject,PictureType> cardsPDict)
    {
        UnityEngine.Object[] cardAtlas = Resources.LoadAll("Cards");  //获取图集
        int cardsCount = 0;
        //依次添加3~2卡牌
        for (int value=0;value<13;value++)
        {
            for (int picture=0;picture<4;picture++)
            {
                ValueType valueItem = (ValueType)value;
                PictureType pictureItem = (PictureType)picture;

                cards.Add(GainOneCard(valueItem,pictureItem,cardAtlas));

                cardsVDict.Add(cards[cardsCount],valueItem);
                cardsPDict.Add(cards[cardsCount++],pictureItem);
            }
        }
        //小王
        cards.Add(GainOneCard(ValueType.BLACKJOKER, PictureType.BLACK_JOKER,cardAtlas)); //卡牌集合中添加小王卡牌
        cardsVDict.Add(cards[cardsCount], ValueType.BLACKJOKER);  //卡牌Value字典中添加小王卡牌
        cardsPDict.Add(cards[cardsCount++], PictureType.BLACK_JOKER);  //卡牌Picture字典中添加小王卡牌
        //大王
        cards.Add(GainOneCard(ValueType.REDJOKER,PictureType.RED_JOKER,cardAtlas));  
        cardsVDict.Add(cards[cardsCount],ValueType.REDJOKER);
        cardsPDict.Add(cards[cardsCount++], PictureType.RED_JOKER);
    }
    /// <summary>
    /// 获得一张指定卡牌
    /// </summary>
    /// <param name="valueItem"></param>
    /// <param name="pictureItem"></param>
    /// <param name="cardAtlas"></param>
    /// <returns></returns>
    private GameObject GainOneCard(ValueType valueItem,PictureType pictureItem,UnityEngine.Object[] cardAtlas)
    {
        GameObject card = Instantiate((GameObject)Resources.Load("Prefabs/Cards/card"), this.transform);  //卡牌实例化
        card.name = valueItem.ToString() + "-" + pictureItem.ToString();  //设置卡牌名称
        card.GetComponent<RectTransform>().position = this.transform.position;    //设置卡片初始位置
        RectTransform cardValueRT = card.GetComponent<RectTransform>().Find("Front").Find("Value").GetComponent<RectTransform>();  //获取卡牌Value的RT引用
        RectTransform cardPictureRT = card.GetComponent<RectTransform>().Find("Front").Find("Picture").GetComponent<RectTransform>();  //获取卡牌的Picture的RT引用
        SetCardFormat(cardValueRT, cardPictureRT, pictureItem);  //设置卡牌Value和Picture的位置
        SetCardSprite(cardValueRT, cardPictureRT, valueItem, pictureItem, cardAtlas);  //设置卡牌Value和Picture的Sprite
        return card;  
    }
    /// <summary>
    /// 设置卡牌的格式（value与picture）
    /// </summary>
    /// <param name="cardValueRT"></param>
    /// <param name="cardPictureRT"></param>
    /// <param name="typeP"></param>
    private void SetCardFormat(RectTransform cardValueRT,RectTransform cardPictureRT,PictureType typeP)
    {
        if (typeP != PictureType.BLACK_JOKER && typeP != PictureType.RED_JOKER)
        {
            cardValueRT.anchoredPosition = new Vector2(-35f,48f);
            cardValueRT.sizeDelta = new Vector2(56.5f,62f);
            cardPictureRT.anchoredPosition = new Vector2(-35.5f,-3f);
            cardPictureRT.sizeDelta = new Vector2(41.3f,41.3f);
        }
    }
    /// <summary>
    /// 设置卡牌的Sprite（value和picture）
    /// </summary>
    /// <param name="cardValueRT"></param>
    /// <param name="cardPictureRT"></param>
    /// <param name="typeV"></param>
    /// <param name="typeP"></param>
    /// <param name="cardAtlas"></param>
    private void SetCardSprite(RectTransform cardValueRT, RectTransform cardPictureRT, ValueType typeV,PictureType typeP,UnityEngine.Object[] cardAtlas)
    {
        string cardValueName;
        string cardPictureName;
        cardPictureName = typeP.ToString();
        if (typeP == PictureType.HEARTS || typeP == PictureType.DIAMONDS || typeP == PictureType.RED_JOKER)
        {
            cardValueName = "RED_" + typeV.ToString();
        }
        else
        {
            cardValueName = "BLACK_" + typeV.ToString();
        }
        //Debug.Log(cardValueName+"----"+cardPictureName);
        for (int i = 0; i < cardAtlas.Length; i++)
        {
            if (cardAtlas[i].GetType() == typeof(UnityEngine.Sprite))
            {
                if(cardAtlas[i].name==cardValueName)
                {
                    cardValueRT.GetComponent<Image>().sprite=(Sprite)cardAtlas[i];
                }
                if(cardAtlas[i].name==cardPictureName)
                {
                    cardPictureRT.GetComponent<Image>().sprite = (Sprite)cardAtlas[i];
                }
            }
        }
    }
    
}

