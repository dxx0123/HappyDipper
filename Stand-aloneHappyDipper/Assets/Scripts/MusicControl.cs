using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControl : MonoBehaviour
{
    public static MusicControl Instance { get; private set; } //公开脚本引用

    private AudioSource bgMusic; //背景声音组件引用
    private AudioSource onceShotMusic; //仅播放一次声音组件引用

    /// <summary>
    /// 音效（枚举类型，用于选择调用相应音效）
    /// </summary>
    public enum SOUND
    {
        ENTER_GAME_MUSIC, //进入游戏
        BEGIN_GAME_MUSIC_01, //开始游戏bg01
        BEGIN_GAME_MUSIC_02, //开始游戏bg02
        CALL_LANDLORD_SOUND, //叫地主（无资源）
        NOT_CALL_LANDLORD_SOUND, //不叫（无资源）
        GRAD_LANDLORD_SOUND, //抢地主（无资源）
        NOT_GRAD_LANDLORD_SOUND, //不抢（无资源）
        DOUBLE_SOUND, //加倍（无资源）
        NOT_DOUBLE_SOUND, //不加倍（无资源）
        DEAL_CARDS_SOUND, //发牌
        OUT_CARD_SOUND, //出牌
        LARGER_SOUND, //大你（无资源）
        GILR_CAN_NOT_SOUND, //不要（女）
        STRAIGHT_SOUND, //顺子（无资源）
        PAIRS_SOUND, //连对（无资源）
        PLANE_SOUND, //飞机
        GIRL_BOMB_SOUND, //炸弹（女）
        KING_EXPLOSIONS_SOUND, //王炸
        ALARM_SOUND, //报警
        LOSE_MUSIC, //失败
        WIN_MUSIC //胜利
    }

    //音频引用
    private AudioClip alarmSound; //警报音效（卡牌即将出完）
    private AudioClip dealCardsSound; //发牌音效
    private AudioClip enterGameMusic; //进入游戏背景音乐
    private AudioClip gilrBombSound; //炸弹（女声）
    private AudioClip girlCanNotSound; //不要（女声）
    private AudioClip loseMusic; //失败音乐
    private AudioClip outCardSound; //出牌音效
    private AudioClip planeSound; //飞机音效
    private AudioClip winMusic; //胜利音乐
    private AudioClip beginGameMusic01; //开始游戏音乐01
    private AudioClip beginGameMusic02; //开始游戏音乐02（炸弹、或者王炸之后播放）
    private AudioClip kingExplosionsMusic; //王炸音效
    private AudioClip callLandlordSound; //叫地主（无资源1）
    private AudioClip notCallLandlordSound; //不叫（无资源1）
    private AudioClip gradLandlordSound; //抢地主（无资源1）
    private AudioClip notGradLandlordSound; //不抢（无资源1）
    private AudioClip doubleSound;//加倍（无资源1）
    private AudioClip notDoubleSound; //不加倍（无资源）
    private AudioClip largerSound; //大你（无资源1）
    private AudioClip straightSound; //顺子（无资源1）
    private AudioClip pairsSound; //连对（无资源1）

    public bool isPlayingBeginGameMusic02; //判断开始游戏音乐是否正在播放

    /// <summary>
    /// 赋予脚本引用
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void Start()
    {
        //加载音效资源
        alarmSound = Resources.Load<AudioClip>("Music/AlarmSound");
        dealCardsSound = Resources.Load<AudioClip>("Music/DealCardsSound");
        enterGameMusic = Resources.Load<AudioClip>("Music/EnterGameMusic");
        girlCanNotSound = Resources.Load<AudioClip>("Music/GirlCanNotSound");
        loseMusic = Resources.Load<AudioClip>("Music/LoseMusic");
        outCardSound = Resources.Load<AudioClip>("Music/OutCardSound");
        planeSound = Resources.Load<AudioClip>("Music/PlaneSound");
        winMusic = Resources.Load<AudioClip>("Music/WinMusic");
        beginGameMusic01 = Resources.Load<AudioClip>("Music/BeginGameMusic01");
        beginGameMusic02 = Resources.Load<AudioClip>("Music/BeginGameMusic02");
        kingExplosionsMusic = Resources.Load<AudioClip>("Music/KingExplosionsSound");
        callLandlordSound = Resources.Load<AudioClip>("Music/CallLandlordSound");
        notCallLandlordSound = Resources.Load<AudioClip>("Music/NotCallLandlordSound");
        gradLandlordSound = Resources.Load<AudioClip>("Music/GradLandlordSound");
        notGradLandlordSound = Resources.Load<AudioClip>("Music/NotGradLandlordSound");
        doubleSound = Resources.Load<AudioClip>("Music/DoubleSound");
        notDoubleSound = Resources.Load<AudioClip>("Music/NotDoubleSound");
        largerSound = Resources.Load<AudioClip>("Music/LargerSound");
        straightSound = Resources.Load<AudioClip>("Music/StraightSound");
        pairsSound = Resources.Load<AudioClip>("Music/PairsSound");
        gilrBombSound = Resources.Load<AudioClip>("Music/GilrBombSound");

        bgMusic = this.GetComponent<Transform>().Find("BGLoopAudioSource").GetComponent<AudioSource>(); //添加背景声音组件引用
        onceShotMusic = this.GetComponent<Transform>().Find("OnceShotAudioSource").GetComponent<AudioSource>(); //添加一次播放声音组件引用

        //bgMusic.clip = enterGameMusic; //设置背景音乐初始音频

        //设置循环播放
        bgMusic.loop = true;
        onceShotMusic.loop = false;

        //调整音量
        bgMusic.volume = 0.7f;
        onceShotMusic.volume = 1.0f;

        //设置运行唤醒
        bgMusic.playOnAwake = true;
        onceShotMusic.playOnAwake = false;

        PLaySound(SOUND.ENTER_GAME_MUSIC); //播放进入游戏音频

        isPlayingBeginGameMusic02 = false; //开始游戏音乐正在播放默认设置为false
    }


    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="selectSound">选择要播放的音频</param>
    public void PLaySound(SOUND selectSound)
    {
        switch (selectSound)
        {
            //进入游戏（背景音乐）
            case SOUND.ENTER_GAME_MUSIC:
                bgMusic.clip = enterGameMusic; //添加音效
                bgMusic.Play(); //播放音效
                break;
            //开始游戏（背景音乐01）
            case SOUND.BEGIN_GAME_MUSIC_01:
                bgMusic.clip = beginGameMusic01;
                bgMusic.Play();
                break;
            //开始游戏（背景音乐02）
            case SOUND.BEGIN_GAME_MUSIC_02:
                bgMusic.clip = beginGameMusic02;
                bgMusic.Play();
                break;
            //叫地主
            case SOUND.CALL_LANDLORD_SOUND:
                onceShotMusic.PlayOneShot(callLandlordSound);
                break;
            //不叫
            case SOUND.NOT_CALL_LANDLORD_SOUND:
                onceShotMusic.PlayOneShot(notCallLandlordSound);
                break;
            //抢地主
            case SOUND.GRAD_LANDLORD_SOUND:
                onceShotMusic.PlayOneShot(gradLandlordSound);
                break;
            //不抢
            case SOUND.NOT_GRAD_LANDLORD_SOUND:
                onceShotMusic.PlayOneShot(notGradLandlordSound);
                break;
            //加倍
            case SOUND.DOUBLE_SOUND:
                onceShotMusic.PlayOneShot(doubleSound);
                break;
            //不加倍
            case SOUND.NOT_DOUBLE_SOUND:
                onceShotMusic.PlayOneShot(notDoubleSound);
                break;
            //发牌音效
            case SOUND.DEAL_CARDS_SOUND:
                onceShotMusic.PlayOneShot(dealCardsSound);
                break;
            //【不要】音效
            case SOUND.GILR_CAN_NOT_SOUND:
                onceShotMusic.PlayOneShot(girlCanNotSound);
                break;
            //【出牌】音效
            case SOUND.OUT_CARD_SOUND:
                onceShotMusic.PlayOneShot(outCardSound);
                break;
            //大你
            case SOUND.LARGER_SOUND:
                onceShotMusic.PlayOneShot(largerSound);
                break;
            //顺子
            case SOUND.STRAIGHT_SOUND:
                onceShotMusic.PlayOneShot(straightSound);
                break;
            //连对
            case SOUND.PAIRS_SOUND:
                onceShotMusic.PlayOneShot(pairsSound);
                break;
            //【飞机】音效
            case SOUND.PLANE_SOUND:
                onceShotMusic.PlayOneShot(planeSound);
                break;
            //【炸弹】音效
            case SOUND.GIRL_BOMB_SOUND:
                onceShotMusic.PlayOneShot(gilrBombSound);
                break;
            //【王炸】音效
            case SOUND.KING_EXPLOSIONS_SOUND:
                onceShotMusic.PlayOneShot(kingExplosionsMusic);
                break;
            //【警报】音效
            case SOUND.ALARM_SOUND:
                onceShotMusic.PlayOneShot(alarmSound);
                break;
            //【失败】音乐
            case SOUND.LOSE_MUSIC:
                onceShotMusic.PlayOneShot(loseMusic);
                break;
            //【胜利】音乐
            case SOUND.WIN_MUSIC:
                onceShotMusic.PlayOneShot(winMusic);
                break;
        }
    }

    /// <summary>
    /// 背景音乐暂停
    /// </summary>
    public void StopBGMusic()
    {
        bgMusic.Stop(); //暂停播放背景音乐
    }

    /// <summary>
    /// 音效暂停
    /// </summary>
    public void StopOnceShotMusic()
    {
        onceShotMusic.Stop();
    }

    /// <summary>
    /// 测试代码
    /// </summary>
    //public void test()
    //{
    //    //music.clip = winMusic;
    //    music.PlayOneShot(winMusic);
    //    //music.PlayOneShot(alarmSound);
    //}
}
