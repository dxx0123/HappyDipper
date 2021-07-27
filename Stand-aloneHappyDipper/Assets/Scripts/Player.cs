using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using System.Linq;
//using System.Numerics;

public class Player : MonoBehaviour
{
    public static Player Instance { private set; get; }  //存储该脚本的引用
    private RectTransform thisGORT;  //脚本所挂在物体的RT引用
    public GamePlayManager gamePlayManager;  //存储GamePlayManager的脚本引用
    public OutCardClass outCardClass; //存储OutCard的脚本引用

    public GameObject[] afterChooseTextGO;   //存储左、中、右玩家在选项选择之后的提示文本的GameObject
    public Text[] afterChooseText;  //存储左、中、右玩家在选项选择之后的提示文本
    private Vector3[] posTB;   //存储左、中、右玩家text、按钮的显示位置

    //private Vector3[] landlordFlagPos;  //存储地主标志的位置（帽子）
    private GameObject landlordGO;  //地主标志GO

    public GameObject playerButtonGO;  //玩家【要】按钮
    public GameObject noPlayerButtonGO;  //玩家【不要】按钮
    public GameObject thirdPlayerButtonGO;  //玩家【第三种】按钮

    public List<int> canGradLandlordPlayer = new List<int>();  //集合存储能够进行抢地主的玩家，存储玩家号码，默认均可进行抢地主

    //存储玩家按钮的位置：0—>Yes  , 1—>No , 2—>有三个按钮时的第三个选项位置
    private Vector3[] playerButtonPos = new Vector3[] { new Vector3(1100f, 290f, 0f), new Vector3(600f, 290f, 0f), new Vector3(1397f, 290f, 0f) };

    public string textRandom;  //保存随机的选项

    public int currentPlayer;  //设置默认当前玩家为左边玩家
    public int firstLandlordPlayer;  //第一个叫地主玩家
    private int landlordPlayer; //地主

    public int currentDouble;  //当前加倍数默认为1
    private int doubleStageCount; //加倍阶段循环次数

    private int joyBeansBasicDouble=1; //欢乐豆基础数

    //private int winPlayer; //存储赢家（第一个把牌打完的玩家）
    private bool isLandlordWin = false; //存储是否是地主赢

    private List<bool> isAlreadyAlarmList = new List<bool>(); //存储玩家是否已经警报过
    private bool isFreePlay; //判断当前卡牌是否是自由出牌（默认false）

    private RectTransform gameOverPanelRT; //游戏结束panel
    private RectTransform gameOverBGRT; //游戏结束BG的RT
    private Image gameOverTitleImage; //游戏结束Title的图片引用
    private Text gameOverJoyBeanCountText; //游戏结束欢乐豆计数（获得、失去的数量）
    private Text gameOverPlayerRemainJoyBeanText; //游戏结束玩家剩余欢乐豆数量
    private Button gameOverOnceAgainButton; //游戏结束，再来一次按钮
    private Button gameOverExitButton; //游戏结束，退出游戏按钮

    /// <summary>
    /// 脚本引用
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        thisGORT = this.GetComponent<RectTransform>();  //该脚本所挂载的物体的RectTransform组件

        //分别存储左、中、右玩家text、按钮的位置
        posTB = new Vector3[] {
            new Vector3(285f, 47f, 0f),
            new Vector3(855f, 290f, 0f),
            new Vector3(285f, 47f, 0f)
        };
        //数组初始化：选项选择文本GameObject
        afterChooseTextGO = new GameObject[] {
            (GameObject)Instantiate(Resources.Load("Prefabs/Game/Text"),thisGORT.Find("LeftPlayer")),
            (GameObject)Instantiate(Resources.Load("Prefabs/Game/Text"),thisGORT.Find("Player")),
            (GameObject)Instantiate(Resources.Load("Prefabs/Game/Text"),thisGORT.Find("RightPlayer"))
        };
        afterChooseTextGO[0].GetComponent<RectTransform>().localPosition = posTB[0];
        afterChooseTextGO[1].GetComponent<RectTransform>().localPosition = posTB[1];
        afterChooseTextGO[2].GetComponent<RectTransform>().localPosition = posTB[2];
        afterChooseTextGO[2].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 180f, 0f);
        //数组初始化：选项选择之后的提示文本
        afterChooseText = new Text[] {
            afterChooseTextGO[0].GetComponent<Text>(),
            afterChooseTextGO[1].GetComponent<Text>(),
            afterChooseTextGO[2].GetComponent<Text>(),
        };
        //设置Text的文本位置
        afterChooseText[0].alignment = TextAnchor.MiddleLeft;
        afterChooseText[1].alignment = TextAnchor.MiddleCenter;
        afterChooseText[2].alignment = TextAnchor.MiddleRight;
        //地主标志位置
        //landlordFlagPos = new Vector3[] {
        //    new Vector3(-876f,339f,0f),
        //    new Vector3(-688f,-264f,0f),
        //    new Vector3(876f,339f,0f)
        //};
        //获取脚本GamePlayerManager的引用
        gamePlayManager = this.GetComponent<GamePlayManager>();

        //游戏结束，获取各个组件引用
        gameOverPanelRT = (RectTransform)this.GetComponent<RectTransform>().Find("GameOverPanel"); //panel
        gameOverBGRT = (RectTransform)gameOverPanelRT.Find("GameOverBG"); //GamePverBG的RT
        gameOverTitleImage = gameOverBGRT.Find("TitleImage").GetComponent<Image>(); //Title
        gameOverJoyBeanCountText = gameOverBGRT.Find("JoyBeansImage").Find("WinOrLoseBeansCountText").GetComponent<Text>(); //欢乐豆获得/扣除数量
        gameOverPlayerRemainJoyBeanText = gameOverBGRT.Find("PlayerRemainJoyBeanText").GetComponent<Text>(); //玩家剩余欢乐豆提示文本
        gameOverOnceAgainButton = gameOverBGRT.Find("OnceAgainButton").GetComponent<Button>(); //【再来一次】按钮
        gameOverExitButton = gameOverBGRT.Find("ExitButton").GetComponent<Button>(); //【退出游戏】按钮
        //游戏结束按钮添加点击事件
        gameOverOnceAgainButton.onClick.AddListener(GameOverOnceAgainButtonOnClick);
        gameOverExitButton.onClick.AddListener(GameOverExitButtonOnClick);

        //初始化
        PlayerInit();
    }
    //Player脚本各声明初始化
    public void PlayerInit()
    {
        //清空可以抢地主的玩家的集合
        if (canGradLandlordPlayer.Count!=0)
        {
            canGradLandlordPlayer.Clear(); //清空集合
        }
        //选择文本、玩家抢地主资格
        for (int i=0;i<3;i++)
        {
            afterChooseText[i].text = null;  //选择之后的文本设为空
            canGradLandlordPlayer.Add(i);  //重置玩家抢地主资格
        }
        
        textRandom = null; //随机选项初始化
        currentPlayer = 0; //设置当前玩家默认为左边玩家
        firstLandlordPlayer = -1; //第一个叫地主玩家初始化
        landlordPlayer = -1; //地主玩家初始化
        currentDouble = 1; //当前加倍数初始化
        doubleStageCount = 3; //加倍阶段循环次数默认为3次（所有玩家均有一次机会加倍）

        isFreePlay = false; //自由出牌默认为false
        //设置玩家报警初始化为false
        //Debug.Log("PlayerInit中isAlreadyAlarmList.Count="+isAlreadyAlarmList.Count);
        if (isAlreadyAlarmList.Count==3)
        {
            for (int i=0;i<isAlreadyAlarmList.Count;i++)
            {
                isAlreadyAlarmList[i] = false;
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                isAlreadyAlarmList.Add(false);
            }
        }
    }

    /// <summary>
    /// 游戏重置（三个玩家不叫地主时）
    /// </summary>
    public void GameReset()
    {
        PlayerInit(); //Player脚本中各声明变量初始化
        CardManager.instance.GameCardReset(); //重新发放卡牌
    }
    /// <summary>
    /// 叫地主
    /// </summary>
    public void CallLandlord()
    {
        //当前没有可以抢地主的玩家（所有玩家都不叫地主）
        if (canGradLandlordPlayer.Count == 0)
        {
            //禁用倒计时
            CountdownManager.Instance.timeCountGO.SetActive(false);
            //禁用加倍
            gamePlayManager.doubleGO.SetActive(false);
            //1s后调用游戏重置方法
            Invoke("GameReset", 1f);
            return;
        }
        //将当前玩家的text设置成空
        afterChooseText[currentPlayer].text = null;
        //随机叫地主或者不叫地主
        int i = Random.Range(0, 2);
        //i = 1;  //测试
        //if(currentPlayer==2) i = 0;  //测试
        if (i == 0)
        {
            textRandom = "叫地主";
            gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.GRAD_LANDLORD;  //有玩家叫地主时，将当前游戏阶段设置为抢地主
            firstLandlordPlayer = currentPlayer;  //存储第一个叫地主的玩家
        }
        else
        {
            textRandom = "不叫";
            canGradLandlordPlayer.Remove(currentPlayer);  //不叫地主的玩家没有抢地主的资格，移除
        }
        if(currentPlayer==0 || currentPlayer==2)
        {
            CountdownManager.Instance.SetCountdown(currentPlayer,3);  //调用倒计时方法
        }
        //玩家（中间玩家）
        else
        {
            //创建player的【叫地主】按钮
            playerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/CallLandlord"), thisGORT.Find("Player"));
            playerButtonGO.GetComponent<RectTransform>().localPosition = new Vector3(1100f, 290f, 0f);
            playerButtonGO.GetComponent<Button>().onClick.AddListener(CallLandlordButtonOnClick);
            //创建player的【不叫】按钮
            noPlayerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/NotCallLandlordButton"), thisGORT.Find("Player"));
            noPlayerButtonGO.GetComponent<RectTransform>().localPosition = new Vector3(600f, 290f, 0f);
            noPlayerButtonGO.GetComponent<Button>().onClick.AddListener(NotCallLandlordButtonOnClick);
            //调用倒计时
            CountdownManager.Instance.SetCountdown(currentPlayer,-1);
        }
    }

    /// <summary>
    /// 设置地主标志
    /// </summary>
    private void SetLandlordFlag()
    {
        //没有地主标志物体，则创建；有，则激活（游戏结束【再来一次】）
        if (landlordGO==null)
        {
            landlordGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/LandlordFlag"), thisGORT);  //创建地主标志
        }
        else
        {
            landlordGO.SetActive(true);
        }

        RectTransform landlordRT = landlordGO.GetComponent<RectTransform>();
        //landlordRT.localPosition = new Vector3(0f, 0f, 0f);  //设置位置
        landlordRT.localScale = Vector3.zero;  //设置初始缩放0
        landlordRT.DOScale(1f, 2f);  //放大
        //landlordRT.DOLocalMove(landlordFlagPos[landlordPlayer],2f);  //移动
        switch (landlordPlayer)
        {
            case 0:
                AdaptiveScript.Instance.SetAnchor(landlordGO, AdaptiveScript.ANCHOR.LEFT_TOP); //设置锚点
                landlordRT.anchoredPosition3D = AdaptiveScript.Instance.screenRight + AdaptiveScript.Instance.screenButtom; //根据锚点设置起始位置
                landlordRT.DOAnchorPos3D(new Vector3(84f, -201f, 0f), 2f);
                //landlordRT.DOLocalMove(new Vector3(84f,-201f,0f),2f);
                break;
            case 1:
                AdaptiveScript.Instance.SetAnchor(landlordGO, AdaptiveScript.ANCHOR.LEFT_BUTTOM); //设置锚点
                landlordRT.anchoredPosition3D = AdaptiveScript.Instance.screenRight + AdaptiveScript.Instance.screenTop; //根据锚点设置起始位置
                landlordRT.DOAnchorPos3D(new Vector3(260f, 280f, 0f), 2f);
                //landlordRT.DOLocalMove(new Vector3(260f, 280f, 0f), 2f);
                break;
            case 2:
                AdaptiveScript.Instance.SetAnchor(landlordGO, AdaptiveScript.ANCHOR.RIGHT_TOP); //设置锚点
                landlordRT.anchoredPosition3D = AdaptiveScript.Instance.screenLeft + AdaptiveScript.Instance.screenButtom; //根据锚点设置起始位置
                landlordRT.DOAnchorPos3D(new Vector3(-84f, -201f, 0f), 2f);
                //landlordRT.DOLocalMove(new Vector3(-84f, -201f, 0f), 2f);
                break;
        }
        //Debug.Log("地主标志设置完毕");
    }
    /// <summary>
    /// 抢地主阶段
    /// </summary>
    public void GradLandlord()
    {
        //如果当前有Invoke正在等待，则返回
        if (IsInvoking())
        {
            return;
        }
        //Debug.Log("进入抢地主阶段，firstCallPlayer="+firstLandlordPlayer);
        //如果第一个叫地主玩家没有抢地主资格，并且可抢地主玩家大于1，则将下一个玩家设置为第一个叫地主的玩家（第一个叫地主玩家不抢地主）
        if (!canGradLandlordPlayer.Contains(firstLandlordPlayer) && canGradLandlordPlayer.Count>=1)
        {
            firstLandlordPlayer = (firstLandlordPlayer + 1) % 3;
        }
        //将当前玩家的text设置成空
        afterChooseText[currentPlayer].text = null;
        //如果只有地主玩家有叫地主资格，则直接进入加倍阶段，并发放地主牌
        if (canGradLandlordPlayer.Count == 1)
        {
            currentPlayer = canGradLandlordPlayer[0];  //将地主玩家设置成当前玩家

            GradLandlordOver(); //抢地主完毕工作

            currentPlayer = (currentPlayer + 1) % 3; //设置下一个玩家

            Invoke("StartGameControl", 2f); //两秒后调用GameControl()，进入下一个（等待动画播放完毕）
        }
        //进入这边判断，当前可抢地主的玩家人数应大于1
        //筛选没有叫地主资格的玩家进入下一个
        else if (!canGradLandlordPlayer.Contains(currentPlayer))
        {
            currentPlayer = (currentPlayer + 1) % 3;  //当前玩家设置为下一个玩家
            gamePlayManager.GameControl();  //进入下一个
        }
        //当前玩家可进行抢地主
        else
        {
            //随机抢地主或者不抢地主
            int i = Random.Range(0, 2);
            //i = 0; //测试

            if (i == 0)
            {
                textRandom = "抢地主";
                currentDouble *= 2;  //抢地主倍数加倍

                //如果当前玩家为第一个叫地主的玩家且抢地主，则进入加倍阶段，并发放地主牌
                if (currentPlayer == firstLandlordPlayer)
                {
                    if (currentPlayer==0 || currentPlayer==2)
                    {
                        Invoke("GradLandlordOver", 2f);  //两秒之后等待倒计时2s倒计时完毕，再进行地主牌发放
                    }
                    //判断是否是中间玩家，中间玩家倒计时为5秒
                    else
                    {
                        Invoke("GradLandlordOver", 5f);  //五秒之后等待倒计时5s倒计时完毕，再进行地主牌发放
                    }
                }
            }
            else
            {
                textRandom = "不抢";
                canGradLandlordPlayer.Remove(currentPlayer);
            }
            //AI的倒计时，设置为3秒，开始倒计时 （左边玩家和右边玩家）
            if (currentPlayer == 0 || currentPlayer == 2)
            {
                CountdownManager.Instance.SetCountdown(currentPlayer, 3);  //调用倒计时方法
            }
            //玩家（中间玩家）
            else
            {
                //创建【抢地主】按钮
                playerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/GradLandlordButton"), thisGORT.Find("Player"));
                playerButtonGO.GetComponent<RectTransform>().localPosition = playerButtonPos[0];
                playerButtonGO.GetComponent<Button>().onClick.AddListener(GradLandlordButtonOnClick);
                //创建【不抢】按钮
                noPlayerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/NotGradLandlordButton"), thisGORT.Find("Player"));
                noPlayerButtonGO.GetComponent<RectTransform>().localPosition = playerButtonPos[1];
                noPlayerButtonGO.GetComponent<Button>().onClick.AddListener(NotGradLandlordButtonOnClick);

                CountdownManager.Instance.SetCountdown(currentPlayer, -1);  //调用倒计时
            }
        }
    }

    /// <summary>
    /// 抢地主完毕
    /// </summary>
    private void GradLandlordOver()
    {
        //Debug.Log("进入抢地主完毕，currentPlayer="+currentPlayer);
        gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.DOUBLE;  //设置加倍阶段
        CountdownManager.Instance.timeCountGO.SetActive(false); //先禁用倒计时，缓解地主牌发放事件
        landlordPlayer = currentPlayer; //设置地主玩家
        gamePlayManager.doubleText.text = "×" + currentDouble.ToString();   //倍数显示
        SetLandlordFlag();  //设置地主标志
        CardManager.instance.IssueLandlordCards(landlordPlayer);  //发放地主牌
    }
    /// <summary>
    /// 用于Invoke间隔时间后调用GameControl
    /// </summary>
    private void StartGameControl()
    {
        CountdownManager.Instance.timeCountGO.SetActive(true); //启用倒计时
        gamePlayManager.GameControl();
    }
    /// <summary>
    /// 加倍阶段结束，清除玩家所有选择后提示（用于延迟后清除）
    /// </summary>
    private void OutCardsStageOverTextSet()
    {
        //出牌阶段选择后提示数组中的所有项清零
        for (int index = 0; index < afterChooseText.Length; index++)
        {
            afterChooseText[index].text = null;
        }
    }
    /// <summary>
    /// 加倍阶段
    /// </summary>
    public void Double()
    {
        //Debug.Log("加倍阶段，currentPlayer=" + currentPlayer);
        //加倍阶段循环次数-1
        doubleStageCount--;
        //Debug.Log("加倍阶段，doubleStageCount=" + doubleStageCount);
        //判断是否所有玩家均经历了加倍阶段
        if(doubleStageCount==-1)
        {
            currentPlayer = landlordPlayer; //地主玩家优先出牌
            gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.OUT_CARD;  //将当前游戏阶段设置为出牌阶段
            //如果没有脚本OutCardClass，则添加该脚本
            if (thisGORT.GetComponent<OutCardClass>() == null)
            {
                outCardClass = thisGORT.AddComponent<OutCardClass>(); //添加脚本OutCard，并将引用存储到outCard中
            }
            foreach(GameObject item in CardManager.instance.cardsPlayer)
            {
                item.GetComponent<RectTransform>().Find("Front").AddComponent<CardOnClickClass>(); //在玩家卡牌上添加卡牌点击脚本
            }
            //Debug.Log("currentPlayer="+currentPlayer);
            //创建玩家按钮(出牌、不出)，并禁用
            playerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/OutCardButton"), thisGORT.Find("Player"));
            playerButtonGO.GetComponent<RectTransform>().localPosition = playerButtonPos[0];
            playerButtonGO.GetComponent<Button>().onClick.AddListener(OutCardButtonOnClick);
            playerButtonGO.SetActive(false);

            noPlayerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/CanNotButton"), thisGORT.Find("Player"));
            noPlayerButtonGO.GetComponent<RectTransform>().localPosition = playerButtonPos[1];
            noPlayerButtonGO.GetComponent<Button>().onClick.AddListener(CanNotButtonOnClick);
            noPlayerButtonGO.SetActive(false);

            CountdownManager.Instance.timeCountGO.SetActive(false); //禁用倒计时

            //1.8秒后清除玩家所有选择后文本
            Invoke("OutCardsStageOverTextSet",1f);
            //两秒后进入下一个
            Invoke("StartGameControl",1.5f);
            return;
        }
        afterChooseText[currentPlayer].text = null; //清除上一个选择之后的提示
        //如果当前倒计时为禁用状态，则启用倒计时
        if (!CountdownManager.Instance.timeCountGO.activeInHierarchy)
        {
            CountdownManager.Instance.timeCountGO.SetActive(true); //启用倒计时
        }
        //随机：0----加倍，1----不加倍
        int i = Random.Range(0, 2);
        if(i==0)
        {
            textRandom = "加倍";
            currentDouble *= 2;
        }
        else
        {
            textRandom = "不加倍";
        }
        if(currentPlayer==0 || currentPlayer==2)
        {
            CountdownManager.Instance.SetCountdown(currentPlayer, 3); //调用倒计时
        }
        else
        {
            playerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/DoubleButton"),thisGORT.Find("Player"));
            playerButtonGO.GetComponent<RectTransform>().localPosition = playerButtonPos[0];
            playerButtonGO.GetComponent<Button>().onClick.AddListener(DoubleButtonOnClick);

            noPlayerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/NotDoubleButton"), thisGORT.Find("Player"));
            noPlayerButtonGO.GetComponent<RectTransform>().localPosition = playerButtonPos[1]-new Vector3(-15f,0f,0f);
            noPlayerButtonGO.GetComponent<Button>().onClick.AddListener(NotDoubleButtonOnClick); //添加点击事件

            thirdPlayerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/SuperDoubleButton"), thisGORT.Find("Player"));
            thirdPlayerButtonGO.GetComponent<RectTransform>().localPosition = playerButtonPos[2];
            thirdPlayerButtonGO.GetComponent<Button>().onClick.AddListener(SuperDoubleButtonOnClick);//添加点击事件

            CountdownManager.Instance.SetCountdown(currentPlayer, -1); //调用倒计时
        }
    }
    /// <summary>
    /// 出牌阶段
    /// </summary>
    public void OutCard()
    {
        //Debug.Log("currentPlayer="+currentPlayer);
        //Debug.Log("进入出牌阶段，currentPlayer="+currentPlayer);
        //一旦有玩家卡牌出完，游戏结束
        //判断左边玩家的卡牌是否全部出完
        if (CardManager.instance.cardsLeftPlayer.Count==0)
        {
            //左边玩家是地主
            if (landlordPlayer==0)
            {
                isLandlordWin = true;
                //Debug.Log("游戏结束，地主玩家获胜");
            }
            else
            {
                isLandlordWin = false;
                //Debug.Log("游戏结束，农民玩家获胜");
            }
            CountdownManager.Instance.timeCountGO.SetActive(false); //禁用倒计时
            gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.GAME_OVER;  //设置当前阶段为游戏结束阶段

            //清空所有玩家的上一个选择提示
            for (int i=0;i<afterChooseText.Length;i++)
            {
                afterChooseText[i].text = null;
            }

            //禁用玩家剩余卡牌数
            CardManager.instance.leftPlayerRemainCardsCountGO.SetActive(false);
            CardManager.instance.rightPlayerRemainCardsCountGO.SetActive(false);

            //将中间玩家卡牌的剩余卡牌数明示
            Vector3 playerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.BUTTOM, new Vector3(0f, 360f, 0f)) + new Vector3(-(CardManager.instance.cardsPlayer.Count * 45) / 2, 0f, 0f); //当前玩家出牌初始位置
            for (int i = 0; i < CardManager.instance.cardsPlayer.Count; i++)
            {
                //outCardClass.SetOutCardsSitsInHierarchy(CardManager.instance.cardsPlayer); //调整出牌卡牌在Hierarchy中的位置
                CardManager.instance.cardsPlayer[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //禁用背面
                CardManager.instance.cardsPlayer[i].GetComponent<RectTransform>().localPosition = playerOutCardInitPos + new Vector3(45 * i, 0f, 0f); //将剩余卡牌设置到出牌的位置
                
            }
            //将右边玩家卡牌的剩余卡牌数明示
            Vector3 rightPlayerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.RIGHT, new Vector3(-350f, 93f, 0f)) + new Vector3(-(CardManager.instance.cardsRightPlayer.Count * 45), 0f, 0f); //当前玩家出牌初始位置
            for (int i=0;i<CardManager.instance.cardsRightPlayer.Count;i++)
            {
                //outCardClass.SetOutCardsSitsInHierarchy(CardManager.instance.cardsRightPlayer); //调整出牌卡牌在Hierarchy中的位置
                CardManager.instance.cardsRightPlayer[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //禁用背面
                CardManager.instance.cardsRightPlayer[i].GetComponent<RectTransform>().localPosition= rightPlayerOutCardInitPos + new Vector3(45 * i, 0f, 0f); //将剩余卡牌设置到出牌的位置
            }

            Invoke("StartGameControl",1.0f); //设置1s后进入下一个
            return;
        }
        //判断中间玩家的卡牌是否全部出完
        if (CardManager.instance.cardsPlayer.Count == 0)
        {
            //winPlayer = 1;
            //Debug.Log("出牌阶段，中间玩家卡牌全部出完");
            if (landlordPlayer == 1)
            {
                isLandlordWin = true;
                //Debug.Log("游戏结束，地主玩家获胜");
            }
            else
            {
                isLandlordWin = false;
                //Debug.Log("游戏结束，农民玩家获胜");
            }
            gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.GAME_OVER;  //设置当前阶段为游戏结束阶段
            //gamePlayManager.GameControl(); //进入下一个
            CountdownManager.Instance.timeCountGO.SetActive(false); //禁用倒计时

            //清空所有玩家的上一个选择提示
            for (int i = 0; i < afterChooseText.Length; i++)
            {
                afterChooseText[i].text = null;
            }

            //禁用玩家剩余卡牌数
            CardManager.instance.leftPlayerRemainCardsCountGO.SetActive(false);
            CardManager.instance.rightPlayerRemainCardsCountGO.SetActive(false);

            //将左边玩家卡牌的剩余卡牌数明示
            Vector3 leftPlayerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.LEFT, new Vector3(400f, 93f, 0f)); //左边玩家出牌初始位置
            for (int i = 0; i < CardManager.instance.cardsLeftPlayer.Count; i++)
            {
                //outCardClass.SetOutCardsSitsInHierarchy(CardManager.instance.cardsLeftPlayer); //调整出牌卡牌在Hierarchy中的位置
                CardManager.instance.cardsLeftPlayer[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //禁用背面
                CardManager.instance.cardsLeftPlayer[i].GetComponent<RectTransform>().localPosition = leftPlayerOutCardInitPos + new Vector3(45f * i, 0f, 0f); //将剩余卡牌设置到出牌的位置
            }
            //将右边玩家卡牌的剩余卡牌数明示
            Vector3 rightPlayerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.RIGHT, new Vector3(-350f, 93f, 0f)) + new Vector3(-(CardManager.instance.cardsRightPlayer.Count * 45), 0f, 0f); //当前玩家出牌初始位置
            for (int i = 0; i < CardManager.instance.cardsRightPlayer.Count; i++)
            {
                //outCardClass.SetOutCardsSitsInHierarchy(CardManager.instance.cardsRightPlayer); //调整出牌卡牌在Hierarchy中的位置
                CardManager.instance.cardsRightPlayer[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //禁用背面
                CardManager.instance.cardsRightPlayer[i].GetComponent<RectTransform>().localPosition = rightPlayerOutCardInitPos + new Vector3(45 * i, 0f, 0f); //将剩余卡牌设置到出牌的位置

            }

            Invoke("StartGameControl", 1.0f); //设置1s后进入下一个
            return;
        }
        //判断右边玩家的卡牌是否全部出完
        if (CardManager.instance.cardsRightPlayer.Count == 0)
        {
            //Debug.Log("出牌阶段，右边玩家卡牌全部出完");
            if (landlordPlayer == 2)
            {
                isLandlordWin = true;
                //Debug.Log("游戏结束，地主玩家获胜");
            }
            else
            {
                isLandlordWin = false;
                //Debug.Log("游戏结束，农民玩家获胜");
            }
            gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.GAME_OVER;  //设置当前阶段为游戏结束阶段
            //gamePlayManager.GameControl(); //进入下一个
            CountdownManager.Instance.timeCountGO.SetActive(false); //禁用倒计时

            //清空所有玩家的上一个选择提示
            for (int i = 0; i < afterChooseText.Length; i++)
            {
                afterChooseText[i].text = null;
            }

            //禁用玩家剩余卡牌数
            CardManager.instance.leftPlayerRemainCardsCountGO.SetActive(false);
            CardManager.instance.rightPlayerRemainCardsCountGO.SetActive(false);

            //将中间玩家卡牌的剩余卡牌数明示
            Vector3 playerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.BUTTOM, new Vector3(0f, 360f, 0f)) + new Vector3(-(CardManager.instance.cardsPlayer.Count * 45) / 2, 0f, 0f); //当前玩家出牌初始位置
            for (int i = 0; i < CardManager.instance.cardsPlayer.Count; i++)
            {
                //outCardClass.SetOutCardsSitsInHierarchy(CardManager.instance.cardsPlayer); //调整出牌卡牌在Hierarchy中的位置
                CardManager.instance.cardsPlayer[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //禁用背面
                CardManager.instance.cardsPlayer[i].GetComponent<RectTransform>().localPosition = playerOutCardInitPos + new Vector3(45f * i, 0f, 0f); //将剩余卡牌设置到出牌的位置

            }
            //将左边玩家卡牌的剩余卡牌数明示
            Vector3 leftPlayerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.LEFT, new Vector3(400f, 93f, 0f)); //左边玩家出牌初始位置
            for (int i = 0; i < CardManager.instance.cardsLeftPlayer.Count; i++)
            {
                //outCardClass.SetOutCardsSitsInHierarchy(CardManager.instance.cardsLeftPlayer); //调整出牌卡牌在Hierarchy中的位置
                CardManager.instance.cardsLeftPlayer[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //禁用背面
                CardManager.instance.cardsLeftPlayer[i].GetComponent<RectTransform>().localPosition = leftPlayerOutCardInitPos + new Vector3(45f * i, 0f, 0f); //将剩余卡牌设置到出牌的位置
            }

            Invoke("StartGameControl", 1.0f); //设置1s后进入下一个
            return;
        }

        afterChooseText[currentPlayer].text = null;  //清除上一个选择提示
        //如果有玩家卡牌只剩下2张，则播放【报警】音效
        if (CardManager.instance.cardsLeftPlayer.Count <= 2 && isAlreadyAlarmList[0] == false)
        {
            MusicControl.Instance.PLaySound(MusicControl.SOUND.ALARM_SOUND); //播放【报警】音效
            isAlreadyAlarmList[0] = true; //将当前玩家已报警过设置为true
        }
        if (CardManager.instance.cardsPlayer.Count <= 2 && isAlreadyAlarmList[1] == false)
        {
            MusicControl.Instance.PLaySound(MusicControl.SOUND.ALARM_SOUND); //播放【报警】音效
            isAlreadyAlarmList[1] = true;
        }
        if (CardManager.instance.cardsRightPlayer.Count <= 2 && isAlreadyAlarmList[2] == false)
        {
            MusicControl.Instance.PLaySound(MusicControl.SOUND.ALARM_SOUND); //播放【报警】音效
            isAlreadyAlarmList[2] = true;
        }

        //自由出牌且必须出牌（第一个出牌的玩家 || 上一个出牌玩家即为当前玩家）
        if (outCardClass.lastOutCardType == -1 || outCardClass.lastOutCardPlayer == currentPlayer)
        {
            //Debug.Log("出牌阶段，自由出牌");
            isFreePlay = true; //当前为自由出牌
            //所有上一轮的已出卡牌移出界外
            //左边玩家
            if (outCardClass.leftPlayerLastOutCardsList.Count!=0)
            {
                foreach (GameObject item in outCardClass.leftPlayerLastOutCardsList)
                {
                    //GameObject.Destroy(item);
                    item.GetComponent<RectTransform>().position = new Vector3(-100f,-100f,0f); //将已出的卡牌设置到界面外
                }
            }
            //中间玩家
            if (outCardClass.playerLastOutCardsList.Count!=0)
            {
                foreach (GameObject item in outCardClass.playerLastOutCardsList)
                {
                    //GameObject.Destroy(item);
                    item.GetComponent<RectTransform>().position = new Vector3(-100f, -100f, 0f); //将已出的卡牌设置到界面外
                }
            }
            //右边玩家
            if (outCardClass.rightPlayerLastOutCardsList.Count!=0)
            {
                foreach (GameObject item in outCardClass.rightPlayerLastOutCardsList)
                {
                    //GameObject.Destroy(item);
                    item.GetComponent<RectTransform>().position = new Vector3(-100f, -100f, 0f); //将已出的卡牌设置到界面外
                }
            }

            switch (currentPlayer)
            {
                //左边玩家
                case 0:
                    outCardClass.outCardsList = outCardClass.GetMinCanOutCardsByType(CardManager.instance.cardsLeftPlayer,-1,-1); //AI玩家获取出牌卡牌
                    CountdownManager.Instance.SetCountdown(currentPlayer,3); //开始倒计时2s，倒计时结束后调用倒计时结束出牌辅助函数
                    break;
                //中间玩家
                case 1:
                    outCardClass.playerPreOutCardsList = outCardClass.GetMinCanOutCardsByType(CardManager.instance.cardsPlayer,-1,-1); //预先存入玩家出牌卡牌（玩家出牌超时的情况）

                    //启用玩家按钮
                    playerButtonGO.SetActive(true);
                    noPlayerButtonGO.SetActive(true);

                    CountdownManager.Instance.SetCountdown(currentPlayer, -1); //开始倒计时
                    break;
                //右边玩家
                case 2:
                    outCardClass.outCardsList = outCardClass.GetMinCanOutCardsByType(CardManager.instance.cardsRightPlayer, -1, -1); //AI玩家获取出牌卡牌
                    CountdownManager.Instance.SetCountdown(currentPlayer,3); //开始倒计时2s，倒计时结束后调用倒计时结束出牌辅助函数
                    break;
            }
        }
        //非自由出牌（前面有玩家出牌）
        else
        {
            //Debug.Log("出牌阶段，非自由出牌,outCardClass.playerPreOutCardsList.Count=" + outCardClass.playerPreOutCardsList.Count);
            //Debug.Log("非自由出牌：(int)outCardClass.maxValueLastOutCardsMaxCount=" + outCardClass.maxValueLastOutCardsMaxCount);
            //Debug.Log("非自由出牌：outCardClass.lastOutCardType"+ outCardClass.lastOutCardType);

            isFreePlay = false; //当前为非自由出牌

            switch (currentPlayer)
            {
                //左边玩家
                case 0:
                    //如果当前玩家为地主玩家，或者上一个出牌玩家为地主玩家，或者队友未报警且队友的最大数量卡牌值小于10且不是炸弹、王炸，则出牌
                    if (landlordPlayer == 0 || outCardClass.lastOutCardPlayer==landlordPlayer || (!isAlreadyAlarmList[outCardClass.lastOutCardPlayer] && outCardClass.maxValueLastOutCardsMaxCount<10 && outCardClass.lastOutCardType<11))
                    {
                        outCardClass.outCardsList = outCardClass.GetMinCanOutCardsByType(CardManager.instance.cardsLeftPlayer, outCardClass.maxValueLastOutCardsMaxCount, outCardClass.lastOutCardType); //AI玩家获取出牌卡牌
                    }
                    
                    //将左边玩家上一轮所出卡牌移出界面外
                    if (outCardClass.leftPlayerLastOutCardsList.Count!=0)
                    {
                        foreach (GameObject go in outCardClass.leftPlayerLastOutCardsList)
                        {
                            //Destroy(go);
                            go.GetComponent<RectTransform>().position = new Vector3(-100f, -100f, 0f); //将已出的卡牌设置到界面外
                        }
                    }
                    CountdownManager.Instance.SetCountdown(currentPlayer,3); //开始倒计时2s，倒计时结束后调用倒计时结束出牌辅助函数
                    break;
                //中间玩家
                case 1:
                    //outCardClass.playerPreOutCardsList = outCardClass.GetMinCanOutCardsByType(CardManager.instance.cardsPlayer, -1, -1); //预先存入玩家出牌卡牌（玩家出牌超时的情况）
                    //删除中间玩家上一轮所出卡牌
                    if (outCardClass.playerLastOutCardsList.Count != 0)
                    {
                        foreach (GameObject go in outCardClass.playerLastOutCardsList)
                        {
                            //Destroy(go);
                            go.GetComponent<RectTransform>().position = new Vector3(-100f, -100f, 0f); //将已出的卡牌设置到界面外
                        }
                    }
                    //启用玩家按钮
                    playerButtonGO.SetActive(true);
                    noPlayerButtonGO.SetActive(true);

                    CountdownManager.Instance.SetCountdown(currentPlayer, -1); //开始倒计时
                    break;
                //右边玩家
                case 2:
                    //如果当前玩家为地主玩家，或者队友未报警且队友的最大数量卡牌值小于10且不是炸弹、王炸，则出牌
                    if (landlordPlayer == 2 || outCardClass.lastOutCardPlayer == landlordPlayer || (!isAlreadyAlarmList[outCardClass.lastOutCardPlayer] && outCardClass.maxValueLastOutCardsMaxCount < 10 && outCardClass.lastOutCardType < 11))
                    {
                        outCardClass.outCardsList = outCardClass.GetMinCanOutCardsByType(CardManager.instance.cardsRightPlayer, outCardClass.maxValueLastOutCardsMaxCount, outCardClass.lastOutCardType); //AI玩家获取出牌卡牌
                    }
                    
                    //将右边玩家上一轮所出卡牌移出界面外
                    if (outCardClass.rightPlayerLastOutCardsList.Count != 0)
                    {
                        foreach (GameObject go in outCardClass.rightPlayerLastOutCardsList)
                        {
                            //Destroy(go);
                            go.GetComponent<RectTransform>().position = new Vector3(-100f, -100f, 0f); //将已出的卡牌设置到界面外
                        }
                    }
                    CountdownManager.Instance.SetCountdown(currentPlayer, 3); //开始倒计时2s，倒计时结束后调用倒计时结束出牌辅助函数
                    break;
            }
        }
    }

    /// <summary>
    /// 游戏结束阶段
    /// </summary>
    public void GameOver()
    {
        gameOverPanelRT.gameObject.SetActive(true); //激活游戏结束Panel
        gameOverPanelRT.SetAsLastSibling(); //在Hierarchy面板中将Panel移至末尾（使之在最上层）

        MusicControl.Instance.StopBGMusic(); //停止播放背景音乐

        Text playerJoyBeansCountText=CardManager.instance.playerJoyBeansRT.Find("Count").GetComponent<Text>(); //获取玩家欢乐豆数量的Text组件

        //设置Title、欢乐豆数
        //玩家是地主
        if (landlordPlayer==1)
        {
            //地主获胜
            if (isLandlordWin)
            {
                gameOverTitleImage.sprite = (Sprite)Resources.Load("UI/GameUI/LandlordWinTitle", typeof(Sprite)); //设置Title

                int gainJoyBeans= joyBeansBasicDouble * currentDouble * 2; //获得的欢乐豆数
                DATA.Instance.SaveData(DATA.Instance.GetData() + gainJoyBeans); //保存数据
                playerJoyBeansCountText.text = DATA.Instance.GetData().ToString(); //设置玩家欢乐豆显示
                gameOverJoyBeanCountText.text = "+" + gainJoyBeans.ToString(); //设置获取的欢乐豆
                gameOverPlayerRemainJoyBeanText.text = "（剩余欢乐豆：" + DATA.Instance.GetData().ToString() + "）"; //设置玩家剩余欢乐豆文本

                gameOverBGRT.GetComponent<Image>().color= new Color((255 / 255f), (224 / 255f), (0 / 255f), (255 / 255f)); //设置胜利背景颜色

                MusicControl.Instance.PLaySound(MusicControl.SOUND.WIN_MUSIC); //播放【胜利】音频

            }
            //地主失败
            else
            {
                gameOverTitleImage.sprite = (Sprite)Resources.Load("UI/GameUI/LandlordLoseTitle", typeof(Sprite)); //设置Title

                int loseJoyBeans = joyBeansBasicDouble * currentDouble * 2; //输的欢乐豆数
                //当前玩家剩余欢乐豆足够扣除
                if ((DATA.Instance.GetData() - loseJoyBeans) > 0)
                {
                    DATA.Instance.SaveData(DATA.Instance.GetData() - loseJoyBeans); //保存数据
                }
                //当前玩家剩余欢乐豆不足以扣除，则只扣除玩家当前所有欢乐豆
                else
                {
                    loseJoyBeans = DATA.Instance.GetData(); //将当前玩家所有欢乐豆赋值给输的欢乐豆数
                    DATA.Instance.SaveData(0); //保存数据
                    playerJoyBeansCountText.color = Color.red; //将玩家欢乐豆的字体设置为红色
                }
                playerJoyBeansCountText.text = DATA.Instance.GetData().ToString(); //设置玩家欢乐豆显示
                //gameOverJoyBeanCountText.text = "-" + loseJoyBeans.ToString() + "（剩余欢乐豆：" + DATA.Instance.GetData().ToString() + "）"; //设置获取的欢乐豆、以及剩余欢乐豆
                gameOverJoyBeanCountText.text = "-" + loseJoyBeans.ToString(); //设置获取的欢乐豆
                gameOverPlayerRemainJoyBeanText.text = "（剩余欢乐豆：" + DATA.Instance.GetData().ToString() + "）"; //设置玩家剩余欢乐豆文本
                //gameOverJoyBeanCountText.text = "-" + joyBeansBasicDouble * currentDouble * 2;//设置获取的欢乐豆

                gameOverBGRT.GetComponent<Image>().color = new Color((66 / 255f), (114 / 255f), (140 / 255f), (255 / 255f)); //设置失败背景颜色

                MusicControl.Instance.PLaySound(MusicControl.SOUND.LOSE_MUSIC); //播放【失败】音频
            }
        }
        //玩家是农民
        else
        {
            //农民失败
            if (isLandlordWin)
            {
                gameOverTitleImage.sprite = (Sprite)Resources.Load("UI/GameUI/FarmerLoseTitle", typeof(Sprite)); //设置Title

                int loseJoyBeans = joyBeansBasicDouble * currentDouble; //输的欢乐豆数
                //当前玩家剩余欢乐豆足够扣除
                if ((DATA.Instance.GetData() - loseJoyBeans)>0)
                {
                    DATA.Instance.SaveData(DATA.Instance.GetData() - loseJoyBeans); //保存数据
                }
                //当前玩家剩余欢乐豆不足以扣除，则只扣除玩家当前所有欢乐豆
                else
                {
                    loseJoyBeans = DATA.Instance.GetData(); //将当前玩家所有欢乐豆赋值给输的欢乐豆数
                    DATA.Instance.SaveData(0); //保存数据
                    playerJoyBeansCountText.color = Color.red; //将玩家欢乐豆的字体设置为红色
                }
                playerJoyBeansCountText.text = DATA.Instance.GetData().ToString(); //设置玩家欢乐豆显示
                gameOverJoyBeanCountText.text = "-" + loseJoyBeans.ToString(); //设置获取的欢乐豆
                gameOverPlayerRemainJoyBeanText.text = "（剩余欢乐豆：" + DATA.Instance.GetData().ToString() + "）";  //设置玩家剩余欢乐豆文本
                //gameOverJoyBeanCountText.text = "-" + joyBeansBasicDouble * currentDouble;//设置获取的欢乐豆

                gameOverBGRT.GetComponent<Image>().color = new Color((66 / 255f), (114 / 255f), (140 / 255f), (255 / 255f)); //设置失败背景颜色

                MusicControl.Instance.PLaySound(MusicControl.SOUND.LOSE_MUSIC); //播放【失败】音频
            }
            //农民胜利
            else
            {
                gameOverTitleImage.sprite = (Sprite)Resources.Load("UI/GameUI/FarmerWinTitle", typeof(Sprite)); //设置Title

                int gainJoyBeans = joyBeansBasicDouble * currentDouble ; //获得的欢乐豆数
                DATA.Instance.SaveData(DATA.Instance.GetData() + gainJoyBeans); //保存数据
                playerJoyBeansCountText.text = DATA.Instance.GetData().ToString(); //设置玩家欢乐豆显示
                gameOverJoyBeanCountText.text = "+" + gainJoyBeans.ToString(); //设置获取的欢乐豆
                gameOverPlayerRemainJoyBeanText.text = "（剩余欢乐豆：" + DATA.Instance.GetData().ToString() + "）";  //设置玩家剩余欢乐豆文本
                //gameOverJoyBeanCountText.text = "+" + joyBeansBasicDouble * currentDouble;//设置获取的欢乐豆

                gameOverBGRT.GetComponent<Image>().color = new Color((255 / 255f), (224 / 255f), (0 / 255f), (255 / 255f)); //设置胜利背景颜色

                MusicControl.Instance.PLaySound(MusicControl.SOUND.WIN_MUSIC); //播放【胜利】音频
            }
        }
        
    }

    /// <summary>
    /// 卡牌【出牌】辅助函数
    /// 设置出牌的位置、移除玩家卡牌中出牌的卡牌、存储出牌信息至上一个出牌中、清空出牌集合、进入下一个
    /// </summary>
    private void OutCardOutAuxiliary()
    {
        //Debug.Log("出牌辅助函数，1、outCardsList.Count="+ outCardClass.outCardsList.Count);
        outCardClass.SetOutCardsListSort(outCardClass.outCardsList); //将出牌卡牌进行排序

        //Debug.Log("出牌辅助函数，2、outCardsList.Count=" + outCardClass.outCardsList.Count);
        Vector3 currentPlayerOutCardInitPos = new Vector3(0f,0f,0f); //存储当前玩家出牌位置
        float offset = 45f; //出牌卡牌位置偏移量
        List<GameObject> cards = new List<GameObject>(); //存储玩家卡牌引用，用于间接使用玩家卡牌
        //设置当前玩家卡牌、出牌初始位置、存储出牌卡牌至上一轮出牌集合中、设置玩家剩余卡牌数
        switch (currentPlayer)
        {
            case 0:
                cards = CardManager.instance.cardsLeftPlayer;
                Text leftRemainCountText = CardManager.instance.leftPlayerRemainCardsCountGO.GetComponent<Text>();
                leftRemainCountText.text=( int.Parse(leftRemainCountText.text) - outCardClass.outCardsList.Count ).ToString();
                currentPlayerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.LEFT,new Vector3(400f,93f,0f)); //当前玩家出牌初始位置
                //currentPlayerOutCardInitPos = new Vector3(400f, 600f, 0f); //当前玩家出牌初始位置
                //添加出牌卡牌至该玩家上一轮的卡牌集合中
                foreach (GameObject go in outCardClass.outCardsList)
                {
                    outCardClass.leftPlayerLastOutCardsList.Add(go);
                    go.GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //禁用背面
                }
                break;
            case 1:
                cards = CardManager.instance.cardsPlayer;
                //currentPlayerOutCardInitPos = new Vector3(1000f - (outCardClass.outCardsList.Count * offset) / 2, 350f, 0f); //当前玩家出牌初始位置
                currentPlayerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.BUTTOM, new Vector3(0f, 360f, 0f)) + new Vector3(-(outCardClass.outCardsList.Count * offset) / 2,0f,0f); //当前玩家出牌初始位置
                //添加出牌卡牌至该玩家上一轮的卡牌集合中，移除卡牌点击类
                foreach (GameObject go in outCardClass.outCardsList)
                {
                    outCardClass.playerLastOutCardsList.Add(go);
                    CardOnClickClass cardOnClickClass = go.GetComponent<RectTransform>().Find("Front").GetComponent<CardOnClickClass>(); //查找该卡牌前面的RT引用
                    if (cardOnClickClass != null)
                    {
                        Destroy(cardOnClickClass); //移除组件
                    }
                }
                break;
            case 2:
                cards = CardManager.instance.cardsRightPlayer;
                Text rightRemainCountText = CardManager.instance.rightPlayerRemainCardsCountGO.GetComponent<Text>();
                rightRemainCountText.text = (int.Parse(rightRemainCountText.text) - outCardClass.outCardsList.Count).ToString();
                currentPlayerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.RIGHT, new Vector3(-350f, 93f, 0f)) + new Vector3(-(outCardClass.outCardsList.Count*offset),0f,0f); //当前玩家出牌初始位置
                //currentPlayerOutCardInitPos = new Vector3(1600f-outCardClass.outCardsList.Count*offset,600f,0f); //当前玩家出牌初始位置
                //添加出牌卡牌至该玩家上一轮的卡牌集合中
                foreach (GameObject go in outCardClass.outCardsList)
                {
                    outCardClass.rightPlayerLastOutCardsList.Add(go);
                    go.GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //禁用背面
                }
                break;
        }

        //Debug.Log("currentPlayerOutCardInitPos=" + currentPlayerOutCardInitPos);
        outCardClass.SetOutCardsSitsInHierarchy(outCardClass.outCardsList); //调整出牌卡牌在Hierarchy中的位置

        //Debug.Log("出牌辅助函数，4、outCardsList.Count=" + outCardClass.outCardsList.Count);
        //设置出牌位置、玩家卡牌中移除移除卡牌
        for (int i=0;i<outCardClass.outCardsList.Count;i++)
        {
            outCardClass.outCardsList[i].GetComponent<RectTransform>().localPosition = currentPlayerOutCardInitPos + new Vector3(offset*i, 0f, 0f); //将要出的卡牌设置到出牌的位置
            //Debug.Log("outCardClass.outCardsList[i].localPosition"+ outCardClass.outCardsList[i].GetComponent<RectTransform>().localPosition);
            MusicControl.Instance.PLaySound(MusicControl.SOUND.OUT_CARD_SOUND); //播放出牌音效
            cards.Remove(outCardClass.outCardsList[i]); //玩家卡牌中移除已出的卡牌
        }

        //调整移除后的卡牌集合中各个卡牌的位置（当前仅对玩家的卡牌进行调整）
        if (currentPlayer == 1)
        {
            int cardsCount = cards.Count; //卡牌总个数
            //Debug.Log("cardsCount="+cardsCount);
            //float cardsBeginPosition = 0 - (cardsCount / 2) * 55f; //计算卡牌的起始位置
            //Debug.Log("-(cardsCount / 2) * (offset+15)" + -(cardsCount / 2) * (offset + 15));
            Vector3 cardsBeginPosition = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.BUTTOM, new Vector3(0, 140, 0)) + new Vector3(-(cardsCount / 2) * (offset+15), 0f, 0f); //计算卡牌的起始位置
            //重新调整移除后的卡牌位置
            for (int i = 0; i < cardsCount; i++)
            {
                cards[i].GetComponent<RectTransform>().localPosition = cardsBeginPosition+new Vector3(i * (offset+15), 0f, 0f);
                //cards[i].GetComponent<RectTransform>().localPosition = new Vector3(cardsBeginPosition + i * 55f, -400f, 0f);
            }
        }

        //Debug.Log("卡牌【出牌】辅助函数，出牌卡牌为：");
        //foreach (GameObject item in outCardClass.outCardsList)
        //{
        //    Debug.Log("出牌卡牌："+item.name);
        //}

        Dictionary<CardManager.ValueType, int> outCardsListValueTypeCount = new Dictionary<CardManager.ValueType, int>(); //存储出牌卡牌的值类型以及对应的数量
        outCardsListValueTypeCount = CardManager.instance.rememberCardValueCount.GetCardsValueCount(outCardClass.outCardsList); //获取出牌卡牌的值类型以及对应的数量
        CardManager.instance.rememberCardDevice.UpdateRememberCardDeviceValue(outCardsListValueTypeCount); //更新记牌器

        outCardClass.SetLastOutCardInfo(outCardClass.outCardsList, currentPlayer); //存储出牌卡牌的最大数量最大值的ValueType、出牌类型、上一个出牌玩家、清除出牌卡牌集合

        //根据出牌类型播放对应音效、并设置倍数
        switch ((OutCardClass.OUT_CARD_TYPE)outCardClass.lastOutCardType)
        {
            //顺子
            case OutCardClass.OUT_CARD_TYPE.STRAIGHT:
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.STRAIGHT_SOUND); //播放【顺子】音效
                    break;
                }
            //连对
            case OutCardClass.OUT_CARD_TYPE.PAIRS:
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.PAIRS_SOUND); //播放【连对】音效
                    break;
                }
            //飞机
            case OutCardClass.OUT_CARD_TYPE.PLANES_WITHOUT:
            case OutCardClass.OUT_CARD_TYPE.PLANES_WITH_SINGLE:
            case OutCardClass.OUT_CARD_TYPE.PLANES_WITH_DOUBLE:
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.PLANE_SOUND); //播放【飞机】音效
                    break;
                }
            //炸弹
            case OutCardClass.OUT_CARD_TYPE.BOMBS:
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.GIRL_BOMB_SOUND); //播放【炸弹】音效
                    currentDouble *= 2; //倍数翻倍
                    gamePlayManager.doubleText.text = "×" + currentDouble.ToString();   //倍数显示

                    //判断当前开始游戏背景音频是否正在播放
                    if (!MusicControl.Instance.isPlayingBeginGameMusic02)
                    {
                        MusicControl.Instance.PLaySound(MusicControl.SOUND.BEGIN_GAME_MUSIC_02); //更换开始游戏背景音乐
                        MusicControl.Instance.isPlayingBeginGameMusic02 = true;
                    }
                    break;
                }
            //王炸
            case OutCardClass.OUT_CARD_TYPE.KING_EXPLOSIONS:
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.KING_EXPLOSIONS_SOUND); //播放【王炸】音效
                    //Debug.Log("播放炸弹音效");
                    currentDouble *= 4; //倍数翻4倍
                    gamePlayManager.doubleText.text = "×" + currentDouble.ToString();   //倍数显示

                    //判断当前开始游戏背景音频是否正在播放
                    if (!MusicControl.Instance.isPlayingBeginGameMusic02)
                    {
                        MusicControl.Instance.PLaySound(MusicControl.SOUND.BEGIN_GAME_MUSIC_02); //更换开始游戏背景音乐
                        MusicControl.Instance.isPlayingBeginGameMusic02 = true;
                    }
                    break;
                }
            //其余出牌情况
            default:
                {
                    //非自由出牌
                    if (!isFreePlay)
                    {
                        MusicControl.Instance.PLaySound(MusicControl.SOUND.LARGER_SOUND); //播放【大你】音效
                    }
                    break;
                }
        }

        //进入下一个玩家
        currentPlayer = (currentPlayer + 1) % 3;
        gamePlayManager.GameControl();
    }
    /// <summary>
    /// 玩家【出牌】辅助函数（用于玩家必须出牌，但超时未出牌的情况）
    /// 将预存入的卡牌辅助给出牌卡牌，并清除预存入卡牌
    /// </summary>
    private void PlayerOutCardOutAuxiliary()
    {
        //Debug.Log("玩家【出牌】辅助函数01，playerPreOutCardsList.Count=" + outCardClass.playerPreOutCardsList.Count);
        //禁用玩家按钮
        playerButtonGO.SetActive(false);
        noPlayerButtonGO.SetActive(false);

        //Debug.Log("玩家【出牌】辅助函数，outCardClass.playerPreOutCardsList.Count="+ outCardClass.playerPreOutCardsList.Count);
        //如果当前玩家点击卡牌，即有存入玩家点击预出牌卡牌，则复位位置且清空玩家点击预出牌卡牌
        if (outCardClass.playerOnClickPreOutCardsList.Count!=0)
        {
            //取消卡牌点击效果
            foreach (GameObject item in outCardClass.playerOnClickPreOutCardsList)
            {
                item.GetComponent<RectTransform>().position -= new Vector3(0f, 20f, 0f); //被点击的卡牌取消点击效果（回到初始位置）
                item.GetComponent<RectTransform>().Find("Front").GetComponent<CardOnClickClass>().isSelect = false; //该卡牌设置为未选择
            }

            outCardClass.playerOnClickPreOutCardsList.Clear(); //清除玩家点击预出卡牌
        }
        outCardClass.outCardsList.AddRange(outCardClass.playerPreOutCardsList); //将预存入的玩家卡牌赋值给出牌卡牌
        outCardClass.playerPreOutCardsList.Clear(); //清除预存入卡牌
        Debug.Log("玩家【出牌】辅助函数02，playerPreOutCardsList.Count=" + outCardClass.playerPreOutCardsList.Count);
        //Debug.Log("玩家【出牌】辅助函数，outCardClass.outCardsList[0]=" + outCardClass.outCardsList[0]);
        OutCardOutAuxiliary(); //调用卡牌【出牌】函数
    }

    /// <summary>
    /// 卡牌【不要】辅助函数
    /// (清空outCardsList、设置text、设置进入下一个玩家)
    /// </summary>
    private void OutCardCanNotAuxiliary()
    {
        MusicControl.Instance.PLaySound(MusicControl.SOUND.GILR_CAN_NOT_SOUND); //播放【不要】音效

        afterChooseText[currentPlayer].text = "不出"; //玩家倒计时结束，默认不出

        //进入下一个玩家
        currentPlayer = (currentPlayer + 1) % 3;
        gamePlayManager.GameControl();
    }
    /// <summary>
    /// 玩家【不要】辅助函数
    /// （禁用玩家按钮、取消卡牌点击效果、调用玩家【不要】辅助函数）
    /// </summary>
    private void PlayerOutCardCanNotAuxiliary()
    {
        //禁用玩家按钮
        playerButtonGO.SetActive(false);
        noPlayerButtonGO.SetActive(false);

        //取消卡牌点击效果
        foreach (GameObject item in outCardClass.playerOnClickPreOutCardsList)
        {
            item.GetComponent<RectTransform>().localPosition -= new Vector3(0f, 20f, 0f); //被点击的卡牌取消点击效果（回到初始位置）
            item.GetComponent<RectTransform>().Find("Front").GetComponent<CardOnClickClass>().isSelect = false; //该卡牌设置为未选择
        }

        outCardClass.playerPreOutCardsList.Clear(); //清除玩家预出牌集合
        outCardClass.playerOnClickPreOutCardsList.Clear(); //清除玩家点击预出卡牌

        OutCardCanNotAuxiliary(); //调用玩家【不要】辅助函数
    }

    /// <summary>
    /// 倒计时结束出牌辅助函数（倒计时结束，调用该函数）
    /// </summary>
    public void CountDownOverOutCardAuxiliary()
    {
        //玩家
        if (currentPlayer==1)
        {
            //自由出牌，玩家超时未出牌
            if (outCardClass.playerPreOutCardsList.Count != 0)
            {
                PlayerOutCardOutAuxiliary(); //调用玩家出牌辅助函数
            }
            //非自由出牌，玩家倒计时结束，视为【不要】
            else
            {
                PlayerOutCardCanNotAuxiliary(); //调用玩家不要辅助函数
            }
        }
        //AI玩家
        else
        {
            //当前有可出的卡牌
            if (outCardClass.outCardsList.Count!=0)
            {
                OutCardOutAuxiliary(); //调用卡牌出牌辅助函数
            }
            else
            {
                OutCardCanNotAuxiliary(); //调用卡牌【不要】辅助函数
            }
        }
    }



    /// <summary>
    /// 叫地主按钮监听事件
    /// </summary>
    private void CallLandlordButtonOnClick()
    {
        //Debug.Log("叫地主按钮点击");
        afterChooseText[currentPlayer].text = "叫地主";  //设置text

        MusicControl.Instance.PLaySound(MusicControl.SOUND.CALL_LANDLORD_SOUND); //播放【叫地主】音效

        //根据textRandom中进行修改
        if(textRandom=="不叫")
        {
            firstLandlordPlayer = currentPlayer;  //存储第一个叫地主的玩家
            gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.GRAD_LANDLORD;   //设置当前游戏阶段为抢地主
            canGradLandlordPlayer.Add(currentPlayer);  //在textRandom中删除的抢地主资格重新加上
        }

        ListenEventCommonBehavior();
    } 
    /// <summary>
    /// 不叫地主按钮监听事件
    /// </summary>
    private void NotCallLandlordButtonOnClick()
    {
        afterChooseText[currentPlayer].text = "不叫";  //设置text

        MusicControl.Instance.PLaySound(MusicControl.SOUND.NOT_CALL_LANDLORD_SOUND); //播放【不叫】音效

        //根据textRandom中进行修改
        if (textRandom == "叫地主")
        {
            gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.CALL_LANDLORD;  //将当前游戏阶段修改回叫地主
            firstLandlordPlayer = -1;   //将第一个叫地主玩家修改回-1
            canGradLandlordPlayer.Remove(currentPlayer);  //不叫地主的玩家没有抢地主的资格，移除
        }
        ListenEventCommonBehavior();
    }
    /// <summary>
    /// 抢地主按钮监听事件
    /// </summary>
    private void GradLandlordButtonOnClick()
    {
        CountdownManager.Instance.StopIEnum();  //停止协程

        GameObject.Destroy(playerButtonGO);  //销毁玩家点击yes按钮
        GameObject.Destroy(noPlayerButtonGO);  //销毁玩家点击no按钮

        afterChooseText[currentPlayer].text = "抢地主";  //选择之后的文本提示

        MusicControl.Instance.PLaySound(MusicControl.SOUND.GRAD_LANDLORD_SOUND); //播放【抢地主】音效

        //如果该玩家为第一个叫地主玩家，则进入加倍阶段，并发牌
        if (currentPlayer == firstLandlordPlayer)
        {
            if (textRandom=="抢地主")
            {
                CancelInvoke("GradLandlordOver"); //取消textRandom=="抢地主"的调用
            }

            //Debug.Log("抢地主按钮点击，玩家为第一个叫地主玩家");
            GradLandlordOver(); //抢地主完毕
            currentPlayer = (currentPlayer + 1) % 3; //设置下一个玩家
            Invoke("StartGameControl", 2f); //两秒后调用GameControl()，进入下一个（等待动画播放完毕）
        }
        //该玩家不是第一个叫地主的玩家
        else
        {
            //Debug.Log("抢地主按钮点击，玩家不是第一个叫地主玩家");
            //整理text中的操作
            if (textRandom != "抢地主")
            {
                currentDouble *= 2;  //【抢地主】倍数×2
                canGradLandlordPlayer.Add(currentPlayer);  //抢地主资格在textRandom的位置已经移除，这边需要再添加回来
            }

            gamePlayManager.doubleText.text = "×" + currentDouble.ToString();   //倍数显示
            currentPlayer = (currentPlayer + 1) % 3; //设置下一个玩家
            gamePlayManager.GameControl();  //执行下一个
        }
    }
    /// <summary>
    /// 不抢地主按钮监听事件
    /// </summary>
    private void NotGradLandlordButtonOnClick()
    {
        //Debug.Log("点击【不抢】按钮");
        afterChooseText[currentPlayer].text = "不抢";  //选择之后的文本提示
        MusicControl.Instance.PLaySound(MusicControl.SOUND.NOT_GRAD_LANDLORD_SOUND); //播放【叫地主】音效
        if (textRandom=="抢地主")
        {
            if(currentPlayer==firstLandlordPlayer)
            {
                //landlordPlayer = -1; //将textRandom中添加的该玩家为地主重置
                CancelInvoke("GradLandlordOver"); //取消textRandom叫地主时调用的Invoke()
            }
            currentDouble /= 2;  //【抢地主】倍数÷2（textRandom处×2）
            canGradLandlordPlayer.Remove(currentPlayer);  //移除抢地主资格
        }
        ListenEventCommonBehavior();
    }
    /// <summary>
    /// 加倍按钮监听事件
    /// </summary>
    private void DoubleButtonOnClick()
    {
        afterChooseText[currentPlayer].text = "加倍";
        MusicControl.Instance.PLaySound(MusicControl.SOUND.DOUBLE_SOUND); //播放【加倍】音效
        if (textRandom=="不加倍")
        {
            currentDouble *= 2; //倍数加倍
        }
        GameObject.Destroy(thirdPlayerButtonGO); //删除第3个按钮
        gamePlayManager.doubleText.text = "×" + currentDouble.ToString();   //倍数显示
        ListenEventCommonBehavior();
    }
    /// <summary>
    /// 不加倍按钮监听事件
    /// </summary>
    private void NotDoubleButtonOnClick()
    {
        afterChooseText[currentPlayer].text = "不加倍";
        MusicControl.Instance.PLaySound(MusicControl.SOUND.NOT_DOUBLE_SOUND); //播放【不加倍】音效

        if (textRandom=="加倍")
        {
            currentDouble /= 2; //将textRandom处的预加倍÷2
        }
        GameObject.Destroy(thirdPlayerButtonGO); //删除第3个按钮
        ListenEventCommonBehavior();
    }
    /// <summary>
    /// 超级加倍监听事件
    /// </summary>
    private void SuperDoubleButtonOnClick()
    {
        afterChooseText[currentPlayer].text = "超级加倍";
        if(textRandom=="加倍")
        {
            currentDouble /= 2;//将textRandom处的预加倍÷2
        }
        currentDouble *= 4; //倍数加倍
        GameObject.Destroy(thirdPlayerButtonGO); //删除第3个按钮
        gamePlayManager.doubleText.text = "×" + currentDouble.ToString();   //倍数显示
        ListenEventCommonBehavior();
    }
    /// <summary>
    /// 出牌按钮监听事件
    /// </summary>
    private void OutCardButtonOnClick()
    {
        //当前玩家所选卡牌不合法
        if (!outCardClass.IsCanOutCards(outCardClass.playerOnClickPreOutCardsList,currentPlayer))
        {
            //将要出的牌重置位置
            foreach (GameObject item in outCardClass.playerOnClickPreOutCardsList)
            {
                item.GetComponent<RectTransform>().localPosition -= new Vector3(0f, 20f, 0f); //重置要出卡牌的位置
                item.GetComponent<RectTransform>().Find("Front").GetComponent<CardOnClickClass>().isSelect = false; //该卡牌设置为未选择
            }
            outCardClass.playerOnClickPreOutCardsList.Clear(); //清空所选的卡牌
            return;
        }

        CountdownManager.Instance.StopIEnum(); //停止协程

        //禁用玩家按钮
        playerButtonGO.SetActive(false);
        noPlayerButtonGO.SetActive(false);

        outCardClass.outCardsList.AddRange(outCardClass.playerOnClickPreOutCardsList); //将点击预出牌集合添加入出牌卡牌中
        outCardClass.playerOnClickPreOutCardsList.Clear(); //清空点击预出牌集合
        outCardClass.playerPreOutCardsList.Clear(); //清空与出牌集合

        //调用玩家出牌辅助函数
        OutCardOutAuxiliary();
    }
    /// <summary>
    /// 不要按钮监听事件
    /// </summary>
    private void CanNotButtonOnClick()
    {
        //如果当前玩家必须要出牌，则点击【不要】按钮无效
        if (outCardClass.lastOutCardType == -1 || outCardClass.lastOutCardPlayer == currentPlayer)
        {
            return;
        }
        CountdownManager.Instance.StopIEnum(); //停止协程
        PlayerOutCardCanNotAuxiliary(); //调用玩家【不要】辅助函数
    }
    /// <summary>
    /// 监听事件共同行为
    /// </summary>
    private void ListenEventCommonBehavior()
    {
        CountdownManager.Instance.StopIEnum();  //停止协程
        currentPlayer = (currentPlayer + 1) % 3;  //当前玩家设置为下一个玩家
        GameObject.Destroy(playerButtonGO);  //销毁玩家点击yes按钮
        GameObject.Destroy(noPlayerButtonGO);  //销毁玩家点击no按钮
        gamePlayManager.GameControl();  //执行下一个
    }

    /// <summary>
    /// 游戏结束，【再来一次】按钮点击事件
    /// </summary>
    private void GameOverOnceAgainButtonOnClick()
    {
        //如果当前玩家欢乐豆不足，则播放提示
        if (DATA.Instance.GetData()<=0)
        {
            BeginGame beginGame = thisGORT.GetComponent<BeginGame>(); //脚本BeginGame的临时变量
            //如果当前正在播放的提示动画不超过3个
            if (beginGame.currentAnimPlayingCount<=3)
            {
                beginGame.SetJoyBeanRemindTextAnim();
            }
            return;
        }

        gameOverPanelRT.gameObject.SetActive(false); //禁用游戏结束panel
        gamePlayManager.doubleGO.SetActive(false); //禁用加倍
        outCardClass.SetOutCardsSitsInHierarchy(CardManager.instance.cardsList); //调整所有卡牌在Hierarchy面板中的位置

        //删除玩家按钮
        Destroy(playerButtonGO);
        Destroy(noPlayerButtonGO);
        Destroy(thirdPlayerButtonGO);

        landlordGO.SetActive(false); //禁用地主标志

        //移除卡牌点击脚本
        foreach (GameObject item in CardManager.instance.cardsList)
        {
            CardOnClickClass cardOnClickClass = item.GetComponent<RectTransform>().Find("Front").GetComponent<CardOnClickClass>(); //查找该卡牌前面的RT引用
            if (cardOnClickClass!=null)
            {
                Destroy(cardOnClickClass); //移除组件
            }
        }

        outCardClass.Init(); //出牌类初始化

        MusicControl.Instance.isPlayingBeginGameMusic02 = false; //背景音乐02播放状态设置为false

        gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.CALL_LANDLORD; //设置当前游戏阶段为叫地主阶段
        CountdownManager.Instance.timeCountGO.SetActive(false); //禁用倒计时

        CardManager.instance.rememberCardDevice.ChangeOnClick(true); //记牌器设置为关闭状态

        GameReset(); //游戏重置（Player脚本声明初始化、卡牌重分配、开始游戏）
    }
    /// <summary>
    /// 游戏结束，【退出游戏】按钮点击事件
    /// </summary>
    private void GameOverExitButtonOnClick()
    {
        //UnityEditor.EditorApplication.isPlaying = false; //将状态设置为false
        Application.Quit(); //退出游戏
    }
}
