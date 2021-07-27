using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownManager : MonoBehaviour
{
    public GameObject timeCountGO;
    //private RectTransform parentRT;
    public RectTransform countdownRT;  //倒计时RectTransform组件
    private Text countdownText;  //倒计时时间Text
    Vector3[] countdownPos;  //倒计时位置
    public int countdownTime = 5;  //倒计时时间

    private GamePlayManager gamePlayManager;  //脚本GamePlayManager的引用
    public Coroutine countdownIEnum;  //存储协程引用

    /// <summary>
    /// 公开该脚本引用
    /// </summary>
    public static CountdownManager Instance { get; private set; }
    /// <summary>
    /// 赋值脚本引用
    /// </summary>
    private void Awake()
    {
        Instance = this;
        //创建倒计时
        timeCountGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/TimeCount"), this.GetComponent<RectTransform>()); //创建倒计时
        timeCountGO.SetActive(false);  //倒计时初始为禁用状态
    }
    private void Start()
    {
        //Debug.Log("启用CountdownManager的Start");
        //获取倒计时RT组件
        countdownRT = timeCountGO.GetComponent<RectTransform>();
        //获取倒计时组件引用
        countdownText = countdownRT.Find("Text").GetComponent<Text>();
        //分别存储左、中、右玩家倒计时的位置
        countdownPos = new Vector3[] {
            new Vector3(440f, -346f, 0f),
            new Vector3(0f, 380f, 0f),
            new Vector3(-440f, -346f, 0f)
        };
        //获取GamePlayManager脚本的引用
        gamePlayManager = this.GetComponent<GamePlayManager>();
    }

    /// <summary>
    /// 设置倒计时参数，并开始倒计时
    /// </summary>
    public void SetCountdown(int cPos,int stopCountdownTime)
    {
        //Debug.Log("进入倒计时");
        //Debug.Log("倒计时currentStage="+gamePlayManager.currentGameStage);

        //设置倒计时锚点
        switch (cPos)
        {
            case 0:
                AdaptiveScript.Instance.SetAnchor(timeCountGO,AdaptiveScript.ANCHOR.LEFT_TOP);
                break;
            case 1:
                AdaptiveScript.Instance.SetAnchor(timeCountGO, AdaptiveScript.ANCHOR.BUTTOM);
                break;
            case 2:
                AdaptiveScript.Instance.SetAnchor(timeCountGO, AdaptiveScript.ANCHOR.RIGHT_TOP);
                break;
        }
        countdownRT.anchoredPosition3D = countdownPos[cPos];  //设置倒计时物体的位置

        countdownIEnum=StartCoroutine(StartCountdown(stopCountdownTime));   //开始倒计时
    }
    /// <summary>
    /// 停止协程
    /// </summary>
    public void StopIEnum()
    {
        StopCoroutine(countdownIEnum);
    }
    /// <summary>
    /// 协程——开始倒计时
    /// stopCountdownTime为倒计时结束限制（一般为-1）（AI设置倒计时结束）
    /// </summary>
    /// <returns></returns>
    IEnumerator StartCountdown(int stopCountdownTime)
    {
        for(int i= countdownTime; i> stopCountdownTime;)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
            i--;
        }
        //协程结束调用
        //非出牌阶段
        if (gamePlayManager.currentGameStage != GamePlayManager.GAMESTAGE.OUT_CARD)
        {
            gamePlayManager.IsCountdownOver();  //倒计时结束，进入下一个
        }
        //出牌阶段
        else
        {
            Player.Instance.CountDownOverOutCardAuxiliary(); //调用倒计时结束出牌辅助函数
        }
    }
}
