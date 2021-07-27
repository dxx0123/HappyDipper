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
    private RectTransform thisGORT;   //�ýű����������RectTransform���

    public GameObject doubleGO;  //��Ϸ�е��ܱ���
    public Text doubleText;  //��Ϸ�е��ܱ���ֵ

    //public GameObject rememberCardGO; //����������

    public GAMESTAGE currentGameStage;  //�жϵ�ǰ����Ϸ�׶Σ�Ĭ��Ϊ�е����׶�

    /// <summary>
    /// ��Ϸ�׶�
    /// </summary>
    public enum GAMESTAGE
    {
        CALL_LANDLORD,  //�е����׶�
        GRAD_LANDLORD,  //�������׶�
        DOUBLE,   //�ӱ��׶�
        OUT_CARD,   //���ƽ׶�
        GAME_OVER //��Ϸ����
    }
    
    private void Start()
    {
        thisGORT = this.gameObject.GetComponent<RectTransform>(); //��ȡBG��RectTransform�������

        //�ӱ�����
        doubleGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/DoubleImage"),thisGORT); //�����ӱ�����
        doubleGO.SetActive(false);  //��ʼΪ����״̬
        doubleText=doubleGO.GetComponent<RectTransform>().Find("Text").GetComponent<Text>();  //�ӱ�Text����

    }
    /// <summary>
    /// ��Ϸ��ʼ��
    /// ���üӱ�
    /// ���ó�ʼ�ӱ�Text
    /// ���ó�ʼ��Ϸ�׶�
    /// ��ʼ��Ϸ
    /// </summary>
    public void Init()
    {
        //���üӱ�����
        doubleGO.SetActive(true);
        //���ó�ʼ������ʾΪ ��1
        doubleText.text = "��" + (1).ToString();
        //���õ�ǰ��Ϸ�׶�Ϊ�е����׶�
        currentGameStage = GAMESTAGE.CALL_LANDLORD;
        //���õ���ʱ
        CountdownManager.Instance.timeCountGO.SetActive(true);
        //ִ��GameControl
        GameControl();
    }

    /// <summary>
    /// ��Ϸ�ܿ���
    /// </summary>
    /// <param name="cGameStage">��ǰ��Ϸ�׶�</param>
    public void GameControl()
    {
        //Debug.Log("������Ϸ�ܿ��ƣ���ǰ��Ϸ�׶�Ϊ��"+currentGameStage);
        //Debug.Log("��Ϸ�ܿ��ƣ�currentPlayer = " + Player.Instance.currentPlayer);
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
    /// ����ʱ����
    /// </summary>
    public void IsCountdownOver()
    {
        //count++;
        //Debug.Log("����ʱ����");
        //�����ǰ���Ϊplayer����ɾ������ҵĵ����ť
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
        //����ʱ�������������ѡ��text
        Player.Instance.afterChooseText[Player.Instance.currentPlayer].text = Player.Instance.textRandom;
        //������Ч
        switch (Player.Instance.textRandom)
        {
            case "�е���":
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.CALL_LANDLORD_SOUND); //���š�����������Ч
                    break;
                }
            case "����":
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.NOT_CALL_LANDLORD_SOUND); //���š���������Ч
                    break;
                }
            case "������":
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.GRAD_LANDLORD_SOUND); //���š�����������Ч
                    break;
                }
            case "����":
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.NOT_GRAD_LANDLORD_SOUND); //���š���������Ч
                    break;
                }
            case "�ӱ�":
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.DOUBLE_SOUND); //���š��ӱ�����Ч
                    break;
                }
            case "���ӱ�":
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.NOT_DOUBLE_SOUND); //���š����ӱ�����Ч
                    break;
                }

        }
        //��������ó���һ��
        Player.Instance.currentPlayer = (Player.Instance.currentPlayer + 1) % 3;
        //��ʾ�ӱ�����
        doubleText.text = "��"+Player.Instance.currentDouble.ToString();
        //������һ��ѭ��
        GameControl();
    }

}
