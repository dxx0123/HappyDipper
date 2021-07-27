using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControl : MonoBehaviour
{
    public static MusicControl Instance { get; private set; } //�����ű�����

    private AudioSource bgMusic; //���������������
    private AudioSource onceShotMusic; //������һ�������������

    /// <summary>
    /// ��Ч��ö�����ͣ�����ѡ�������Ӧ��Ч��
    /// </summary>
    public enum SOUND
    {
        ENTER_GAME_MUSIC, //������Ϸ
        BEGIN_GAME_MUSIC_01, //��ʼ��Ϸbg01
        BEGIN_GAME_MUSIC_02, //��ʼ��Ϸbg02
        CALL_LANDLORD_SOUND, //�е���������Դ��
        NOT_CALL_LANDLORD_SOUND, //���У�����Դ��
        GRAD_LANDLORD_SOUND, //������������Դ��
        NOT_GRAD_LANDLORD_SOUND, //����������Դ��
        DOUBLE_SOUND, //�ӱ�������Դ��
        NOT_DOUBLE_SOUND, //���ӱ�������Դ��
        DEAL_CARDS_SOUND, //����
        OUT_CARD_SOUND, //����
        LARGER_SOUND, //���㣨����Դ��
        GILR_CAN_NOT_SOUND, //��Ҫ��Ů��
        STRAIGHT_SOUND, //˳�ӣ�����Դ��
        PAIRS_SOUND, //���ԣ�����Դ��
        PLANE_SOUND, //�ɻ�
        GIRL_BOMB_SOUND, //ը����Ů��
        KING_EXPLOSIONS_SOUND, //��ը
        ALARM_SOUND, //����
        LOSE_MUSIC, //ʧ��
        WIN_MUSIC //ʤ��
    }

    //��Ƶ����
    private AudioClip alarmSound; //������Ч�����Ƽ������꣩
    private AudioClip dealCardsSound; //������Ч
    private AudioClip enterGameMusic; //������Ϸ��������
    private AudioClip gilrBombSound; //ը����Ů����
    private AudioClip girlCanNotSound; //��Ҫ��Ů����
    private AudioClip loseMusic; //ʧ������
    private AudioClip outCardSound; //������Ч
    private AudioClip planeSound; //�ɻ���Ч
    private AudioClip winMusic; //ʤ������
    private AudioClip beginGameMusic01; //��ʼ��Ϸ����01
    private AudioClip beginGameMusic02; //��ʼ��Ϸ����02��ը����������ը֮�󲥷ţ�
    private AudioClip kingExplosionsMusic; //��ը��Ч
    private AudioClip callLandlordSound; //�е���������Դ1��
    private AudioClip notCallLandlordSound; //���У�����Դ1��
    private AudioClip gradLandlordSound; //������������Դ1��
    private AudioClip notGradLandlordSound; //����������Դ1��
    private AudioClip doubleSound;//�ӱ�������Դ1��
    private AudioClip notDoubleSound; //���ӱ�������Դ��
    private AudioClip largerSound; //���㣨����Դ1��
    private AudioClip straightSound; //˳�ӣ�����Դ1��
    private AudioClip pairsSound; //���ԣ�����Դ1��

    public bool isPlayingBeginGameMusic02; //�жϿ�ʼ��Ϸ�����Ƿ����ڲ���

    /// <summary>
    /// ����ű�����
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    void Start()
    {
        //������Ч��Դ
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

        bgMusic = this.GetComponent<Transform>().Find("BGLoopAudioSource").GetComponent<AudioSource>(); //��ӱ��������������
        onceShotMusic = this.GetComponent<Transform>().Find("OnceShotAudioSource").GetComponent<AudioSource>(); //���һ�β��������������

        //bgMusic.clip = enterGameMusic; //���ñ������ֳ�ʼ��Ƶ

        //����ѭ������
        bgMusic.loop = true;
        onceShotMusic.loop = false;

        //��������
        bgMusic.volume = 0.7f;
        onceShotMusic.volume = 1.0f;

        //�������л���
        bgMusic.playOnAwake = true;
        onceShotMusic.playOnAwake = false;

        PLaySound(SOUND.ENTER_GAME_MUSIC); //���Ž�����Ϸ��Ƶ

        isPlayingBeginGameMusic02 = false; //��ʼ��Ϸ�������ڲ���Ĭ������Ϊfalse
    }


    /// <summary>
    /// ������Ƶ
    /// </summary>
    /// <param name="selectSound">ѡ��Ҫ���ŵ���Ƶ</param>
    public void PLaySound(SOUND selectSound)
    {
        switch (selectSound)
        {
            //������Ϸ���������֣�
            case SOUND.ENTER_GAME_MUSIC:
                bgMusic.clip = enterGameMusic; //�����Ч
                bgMusic.Play(); //������Ч
                break;
            //��ʼ��Ϸ����������01��
            case SOUND.BEGIN_GAME_MUSIC_01:
                bgMusic.clip = beginGameMusic01;
                bgMusic.Play();
                break;
            //��ʼ��Ϸ����������02��
            case SOUND.BEGIN_GAME_MUSIC_02:
                bgMusic.clip = beginGameMusic02;
                bgMusic.Play();
                break;
            //�е���
            case SOUND.CALL_LANDLORD_SOUND:
                onceShotMusic.PlayOneShot(callLandlordSound);
                break;
            //����
            case SOUND.NOT_CALL_LANDLORD_SOUND:
                onceShotMusic.PlayOneShot(notCallLandlordSound);
                break;
            //������
            case SOUND.GRAD_LANDLORD_SOUND:
                onceShotMusic.PlayOneShot(gradLandlordSound);
                break;
            //����
            case SOUND.NOT_GRAD_LANDLORD_SOUND:
                onceShotMusic.PlayOneShot(notGradLandlordSound);
                break;
            //�ӱ�
            case SOUND.DOUBLE_SOUND:
                onceShotMusic.PlayOneShot(doubleSound);
                break;
            //���ӱ�
            case SOUND.NOT_DOUBLE_SOUND:
                onceShotMusic.PlayOneShot(notDoubleSound);
                break;
            //������Ч
            case SOUND.DEAL_CARDS_SOUND:
                onceShotMusic.PlayOneShot(dealCardsSound);
                break;
            //����Ҫ����Ч
            case SOUND.GILR_CAN_NOT_SOUND:
                onceShotMusic.PlayOneShot(girlCanNotSound);
                break;
            //�����ơ���Ч
            case SOUND.OUT_CARD_SOUND:
                onceShotMusic.PlayOneShot(outCardSound);
                break;
            //����
            case SOUND.LARGER_SOUND:
                onceShotMusic.PlayOneShot(largerSound);
                break;
            //˳��
            case SOUND.STRAIGHT_SOUND:
                onceShotMusic.PlayOneShot(straightSound);
                break;
            //����
            case SOUND.PAIRS_SOUND:
                onceShotMusic.PlayOneShot(pairsSound);
                break;
            //���ɻ�����Ч
            case SOUND.PLANE_SOUND:
                onceShotMusic.PlayOneShot(planeSound);
                break;
            //��ը������Ч
            case SOUND.GIRL_BOMB_SOUND:
                onceShotMusic.PlayOneShot(gilrBombSound);
                break;
            //����ը����Ч
            case SOUND.KING_EXPLOSIONS_SOUND:
                onceShotMusic.PlayOneShot(kingExplosionsMusic);
                break;
            //����������Ч
            case SOUND.ALARM_SOUND:
                onceShotMusic.PlayOneShot(alarmSound);
                break;
            //��ʧ�ܡ�����
            case SOUND.LOSE_MUSIC:
                onceShotMusic.PlayOneShot(loseMusic);
                break;
            //��ʤ��������
            case SOUND.WIN_MUSIC:
                onceShotMusic.PlayOneShot(winMusic);
                break;
        }
    }

    /// <summary>
    /// ����������ͣ
    /// </summary>
    public void StopBGMusic()
    {
        bgMusic.Stop(); //��ͣ���ű�������
    }

    /// <summary>
    /// ��Ч��ͣ
    /// </summary>
    public void StopOnceShotMusic()
    {
        onceShotMusic.Stop();
    }

    /// <summary>
    /// ���Դ���
    /// </summary>
    //public void test()
    //{
    //    //music.clip = winMusic;
    //    music.PlayOneShot(winMusic);
    //    //music.PlayOneShot(alarmSound);
    //}
}
