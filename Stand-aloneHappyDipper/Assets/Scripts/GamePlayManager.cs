using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(CountdownManager))]
public class GamePlayManager : MonoBehaviour
{
    private RectTransform thisGORT;   //该脚本所在物体的RectTransform组件

    public GameObject doubleGO;  //游戏中的总倍数
    public Text doubleText;  //游戏中的总倍数值

    //public GameObject rememberCardGO; //创建记牌器

    public GAMESTAGE currentGameStage;  //判断当前的游戏阶段，默认为叫地主阶段

    /// <summary>
    /// 游戏阶段
    /// </summary>
    public enum GAMESTAGE
    {
        CALL_LANDLORD,  //叫地主阶段
        GRAD_LANDLORD,  //抢地主阶段
        DOUBLE,   //加倍阶段
        OUT_CARD,   //出牌阶段
        GAME_OVER //游戏结束
    }
    
    private void Start()
    {
        thisGORT = this.gameObject.GetComponent<RectTransform>(); //获取BG的RectTransform组件引用

        //加倍物体
        doubleGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/DoubleImage"),thisGORT); //创建加倍物体
        doubleGO.SetActive(false);  //初始为禁用状态
        doubleText=doubleGO.GetComponent<RectTransform>().Find("Text").GetComponent<Text>();  //加倍Text引用

    }
    /// <summary>
    /// 游戏初始化
    /// 启用加倍
    /// 设置初始加倍Text
    /// 设置初始游戏阶段
    /// 开始游戏
    /// </summary>
    public void Init()
    {
        //启用加倍物体
        doubleGO.SetActive(true);
        //设置初始倍数显示为 ×1
        doubleText.text = "×" + (1).ToString();
        //设置当前游戏阶段为叫地主阶段
        currentGameStage = GAMESTAGE.CALL_LANDLORD;
        //启用倒计时
        CountdownManager.Instance.timeCountGO.SetActive(true);
        //执行GameControl
        GameControl();
    }

    /// <summary>
    /// 游戏总控制
    /// </summary>
    /// <param name="cGameStage">当前游戏阶段</param>
    public void GameControl()
    {
        //Debug.Log("进入游戏总控制，当前游戏阶段为："+currentGameStage);
        //Debug.Log("游戏总控制，currentPlayer = " + Player.Instance.currentPlayer);
        switch (currentGameStage)
        {
            case GAMESTAGE.CALL_LANDLORD:
                Player.Instance.CallLandlord();
                break;
            case GAMESTAGE.GRAD_LANDLORD:
                Player.Instance.GradLandlord();
                break;
            case GAMESTAGE.DOUBLE:
                //Debug.Log("firstCallLandlordPlayer="+Player.Instance.firstLandlordPlayer);
                Player.Instance.Double();
                break;
            case GAMESTAGE.OUT_CARD:
                Player.Instance.OutCard();
                break;
            case GAMESTAGE.GAME_OVER:
                Player.Instance.GameOver();
                break;
        }
    }

    /// <summary>
    /// 倒计时结束
    /// </summary>
    public void IsCountdownOver()
    {
        //count++;
        //Debug.Log("倒计时结束");
        //如果当前玩家为player，则删除该玩家的点击按钮
        if (Player.Instance.currentPlayer==1)
        {
            if(Player.Instance.playerButtonGO!=null)
            {
                GameObject.Destroy(Player.Instance.playerButtonGO);
            }
            if(Player.Instance.noPlayerButtonGO!=null)
            {
                GameObject.Destroy(Player.Instance.noPlayerButtonGO);
            }
            if(Player.Instance.thirdPlayerButtonGO!=null)
            {
                GameObject.Destroy(Player.Instance.thirdPlayerButtonGO);
            }
        }
        //倒计时结束设置随机的选项text
        Player.Instance.afterChooseText[Player.Instance.currentPlayer].text = Player.Instance.textRandom;
        //播放音效
        switch (Player.Instance.textRandom)
        {
            case "叫地主":
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.CALL_LANDLORD_SOUND); //播放【抢地主】音效
                    break;
                }
            case "不叫":
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.NOT_CALL_LANDLORD_SOUND); //播放【不抢】音效
                    break;
                }
            case "抢地主":
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.GRAD_LANDLORD_SOUND); //播放【抢地主】音效
                    break;
                }
            case "不抢":
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.NOT_GRAD_LANDLORD_SOUND); //播放【不抢】音效
                    break;
                }
            case "加倍":
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.DOUBLE_SOUND); //播放【加倍】音效
                    break;
                }
            case "不加倍":
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.NOT_DOUBLE_SOUND); //播放【不加倍】音效
                    break;
                }

        }
        //将玩家设置成下一个
        Player.Instance.currentPlayer = (Player.Instance.currentPlayer + 1) % 3;
        //显示加倍倍数
        doubleText.text = "×"+Player.Instance.currentDouble.ToString();
        //进入下一个循环
        GameControl();
    }

}
