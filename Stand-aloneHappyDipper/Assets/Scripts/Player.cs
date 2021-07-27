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
    public static Player Instance { private set; get; }  //�洢�ýű�������
    private RectTransform thisGORT;  //�ű������������RT����
    public GamePlayManager gamePlayManager;  //�洢GamePlayManager�Ľű�����
    public OutCardClass outCardClass; //�洢OutCard�Ľű�����

    public GameObject[] afterChooseTextGO;   //�洢���С��������ѡ��ѡ��֮�����ʾ�ı���GameObject
    public Text[] afterChooseText;  //�洢���С��������ѡ��ѡ��֮�����ʾ�ı�
    private Vector3[] posTB;   //�洢���С������text����ť����ʾλ��

    //private Vector3[] landlordFlagPos;  //�洢������־��λ�ã�ñ�ӣ�
    private GameObject landlordGO;  //������־GO

    public GameObject playerButtonGO;  //��ҡ�Ҫ����ť
    public GameObject noPlayerButtonGO;  //��ҡ���Ҫ����ť
    public GameObject thirdPlayerButtonGO;  //��ҡ������֡���ť

    public List<int> canGradLandlordPlayer = new List<int>();  //���ϴ洢�ܹ���������������ң��洢��Һ��룬Ĭ�Ͼ��ɽ���������

    //�洢��Ұ�ť��λ�ã�0��>Yes  , 1��>No , 2��>��������ťʱ�ĵ�����ѡ��λ��
    private Vector3[] playerButtonPos = new Vector3[] { new Vector3(1100f, 290f, 0f), new Vector3(600f, 290f, 0f), new Vector3(1397f, 290f, 0f) };

    public string textRandom;  //���������ѡ��

    public int currentPlayer;  //����Ĭ�ϵ�ǰ���Ϊ������
    public int firstLandlordPlayer;  //��һ���е������
    private int landlordPlayer; //����

    public int currentDouble;  //��ǰ�ӱ���Ĭ��Ϊ1
    private int doubleStageCount; //�ӱ��׶�ѭ������

    private int joyBeansBasicDouble=1; //���ֶ�������

    //private int winPlayer; //�洢Ӯ�ң���һ�����ƴ������ң�
    private bool isLandlordWin = false; //�洢�Ƿ��ǵ���Ӯ

    private List<bool> isAlreadyAlarmList = new List<bool>(); //�洢����Ƿ��Ѿ�������
    private bool isFreePlay; //�жϵ�ǰ�����Ƿ������ɳ��ƣ�Ĭ��false��

    private RectTransform gameOverPanelRT; //��Ϸ����panel
    private RectTransform gameOverBGRT; //��Ϸ����BG��RT
    private Image gameOverTitleImage; //��Ϸ����Title��ͼƬ����
    private Text gameOverJoyBeanCountText; //��Ϸ�������ֶ���������á�ʧȥ��������
    private Text gameOverPlayerRemainJoyBeanText; //��Ϸ�������ʣ�໶�ֶ�����
    private Button gameOverOnceAgainButton; //��Ϸ����������һ�ΰ�ť
    private Button gameOverExitButton; //��Ϸ�������˳���Ϸ��ť

    /// <summary>
    /// �ű�����
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        thisGORT = this.GetComponent<RectTransform>();  //�ýű������ص������RectTransform���

        //�ֱ�洢���С������text����ť��λ��
        posTB = new Vector3[] {
            new Vector3(285f, 47f, 0f),
            new Vector3(855f, 290f, 0f),
            new Vector3(285f, 47f, 0f)
        };
        //�����ʼ����ѡ��ѡ���ı�GameObject
        afterChooseTextGO = new GameObject[] {
            (GameObject)Instantiate(Resources.Load("Prefabs/Game/Text"),thisGORT.Find("LeftPlayer")),
            (GameObject)Instantiate(Resources.Load("Prefabs/Game/Text"),thisGORT.Find("Player")),
            (GameObject)Instantiate(Resources.Load("Prefabs/Game/Text"),thisGORT.Find("RightPlayer"))
        };
        afterChooseTextGO[0].GetComponent<RectTransform>().localPosition = posTB[0];
        afterChooseTextGO[1].GetComponent<RectTransform>().localPosition = posTB[1];
        afterChooseTextGO[2].GetComponent<RectTransform>().localPosition = posTB[2];
        afterChooseTextGO[2].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 180f, 0f);
        //�����ʼ����ѡ��ѡ��֮�����ʾ�ı�
        afterChooseText = new Text[] {
            afterChooseTextGO[0].GetComponent<Text>(),
            afterChooseTextGO[1].GetComponent<Text>(),
            afterChooseTextGO[2].GetComponent<Text>(),
        };
        //����Text���ı�λ��
        afterChooseText[0].alignment = TextAnchor.MiddleLeft;
        afterChooseText[1].alignment = TextAnchor.MiddleCenter;
        afterChooseText[2].alignment = TextAnchor.MiddleRight;
        //������־λ��
        //landlordFlagPos = new Vector3[] {
        //    new Vector3(-876f,339f,0f),
        //    new Vector3(-688f,-264f,0f),
        //    new Vector3(876f,339f,0f)
        //};
        //��ȡ�ű�GamePlayerManager������
        gamePlayManager = this.GetComponent<GamePlayManager>();

        //��Ϸ��������ȡ�����������
        gameOverPanelRT = (RectTransform)this.GetComponent<RectTransform>().Find("GameOverPanel"); //panel
        gameOverBGRT = (RectTransform)gameOverPanelRT.Find("GameOverBG"); //GamePverBG��RT
        gameOverTitleImage = gameOverBGRT.Find("TitleImage").GetComponent<Image>(); //Title
        gameOverJoyBeanCountText = gameOverBGRT.Find("JoyBeansImage").Find("WinOrLoseBeansCountText").GetComponent<Text>(); //���ֶ����/�۳�����
        gameOverPlayerRemainJoyBeanText = gameOverBGRT.Find("PlayerRemainJoyBeanText").GetComponent<Text>(); //���ʣ�໶�ֶ���ʾ�ı�
        gameOverOnceAgainButton = gameOverBGRT.Find("OnceAgainButton").GetComponent<Button>(); //������һ�Ρ���ť
        gameOverExitButton = gameOverBGRT.Find("ExitButton").GetComponent<Button>(); //���˳���Ϸ����ť
        //��Ϸ������ť��ӵ���¼�
        gameOverOnceAgainButton.onClick.AddListener(GameOverOnceAgainButtonOnClick);
        gameOverExitButton.onClick.AddListener(GameOverExitButtonOnClick);

        //��ʼ��
        PlayerInit();
    }
    //Player�ű���������ʼ��
    public void PlayerInit()
    {
        //��տ�������������ҵļ���
        if (canGradLandlordPlayer.Count!=0)
        {
            canGradLandlordPlayer.Clear(); //��ռ���
        }
        //ѡ���ı�������������ʸ�
        for (int i=0;i<3;i++)
        {
            afterChooseText[i].text = null;  //ѡ��֮����ı���Ϊ��
            canGradLandlordPlayer.Add(i);  //��������������ʸ�
        }
        
        textRandom = null; //���ѡ���ʼ��
        currentPlayer = 0; //���õ�ǰ���Ĭ��Ϊ������
        firstLandlordPlayer = -1; //��һ���е�����ҳ�ʼ��
        landlordPlayer = -1; //������ҳ�ʼ��
        currentDouble = 1; //��ǰ�ӱ�����ʼ��
        doubleStageCount = 3; //�ӱ��׶�ѭ������Ĭ��Ϊ3�Σ�������Ҿ���һ�λ���ӱ���

        isFreePlay = false; //���ɳ���Ĭ��Ϊfalse
        //������ұ�����ʼ��Ϊfalse
        //Debug.Log("PlayerInit��isAlreadyAlarmList.Count="+isAlreadyAlarmList.Count);
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
    /// ��Ϸ���ã�������Ҳ��е���ʱ��
    /// </summary>
    public void GameReset()
    {
        PlayerInit(); //Player�ű��и�����������ʼ��
        CardManager.instance.GameCardReset(); //���·��ſ���
    }
    /// <summary>
    /// �е���
    /// </summary>
    public void CallLandlord()
    {
        //��ǰû�п�������������ң�������Ҷ����е�����
        if (canGradLandlordPlayer.Count == 0)
        {
            //���õ���ʱ
            CountdownManager.Instance.timeCountGO.SetActive(false);
            //���üӱ�
            gamePlayManager.doubleGO.SetActive(false);
            //1s�������Ϸ���÷���
            Invoke("GameReset", 1f);
            return;
        }
        //����ǰ��ҵ�text���óɿ�
        afterChooseText[currentPlayer].text = null;
        //����е������߲��е���
        int i = Random.Range(0, 2);
        //i = 1;  //����
        //if(currentPlayer==2) i = 0;  //����
        if (i == 0)
        {
            textRandom = "�е���";
            gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.GRAD_LANDLORD;  //����ҽе���ʱ������ǰ��Ϸ�׶�����Ϊ������
            firstLandlordPlayer = currentPlayer;  //�洢��һ���е��������
        }
        else
        {
            textRandom = "����";
            canGradLandlordPlayer.Remove(currentPlayer);  //���е��������û�����������ʸ��Ƴ�
        }
        if(currentPlayer==0 || currentPlayer==2)
        {
            CountdownManager.Instance.SetCountdown(currentPlayer,3);  //���õ���ʱ����
        }
        //��ң��м���ң�
        else
        {
            //����player�ġ��е�������ť
            playerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/CallLandlord"), thisGORT.Find("Player"));
            playerButtonGO.GetComponent<RectTransform>().localPosition = new Vector3(1100f, 290f, 0f);
            playerButtonGO.GetComponent<Button>().onClick.AddListener(CallLandlordButtonOnClick);
            //����player�ġ����С���ť
            noPlayerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/NotCallLandlordButton"), thisGORT.Find("Player"));
            noPlayerButtonGO.GetComponent<RectTransform>().localPosition = new Vector3(600f, 290f, 0f);
            noPlayerButtonGO.GetComponent<Button>().onClick.AddListener(NotCallLandlordButtonOnClick);
            //���õ���ʱ
            CountdownManager.Instance.SetCountdown(currentPlayer,-1);
        }
    }

    /// <summary>
    /// ���õ�����־
    /// </summary>
    private void SetLandlordFlag()
    {
        //û�е�����־���壬�򴴽����У��򼤻��Ϸ����������һ�Ρ���
        if (landlordGO==null)
        {
            landlordGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/LandlordFlag"), thisGORT);  //����������־
        }
        else
        {
            landlordGO.SetActive(true);
        }

        RectTransform landlordRT = landlordGO.GetComponent<RectTransform>();
        //landlordRT.localPosition = new Vector3(0f, 0f, 0f);  //����λ��
        landlordRT.localScale = Vector3.zero;  //���ó�ʼ����0
        landlordRT.DOScale(1f, 2f);  //�Ŵ�
        //landlordRT.DOLocalMove(landlordFlagPos[landlordPlayer],2f);  //�ƶ�
        switch (landlordPlayer)
        {
            case 0:
                AdaptiveScript.Instance.SetAnchor(landlordGO, AdaptiveScript.ANCHOR.LEFT_TOP); //����ê��
                landlordRT.anchoredPosition3D = AdaptiveScript.Instance.screenRight + AdaptiveScript.Instance.screenButtom; //����ê��������ʼλ��
                landlordRT.DOAnchorPos3D(new Vector3(84f, -201f, 0f), 2f);
                //landlordRT.DOLocalMove(new Vector3(84f,-201f,0f),2f);
                break;
            case 1:
                AdaptiveScript.Instance.SetAnchor(landlordGO, AdaptiveScript.ANCHOR.LEFT_BUTTOM); //����ê��
                landlordRT.anchoredPosition3D = AdaptiveScript.Instance.screenRight + AdaptiveScript.Instance.screenTop; //����ê��������ʼλ��
                landlordRT.DOAnchorPos3D(new Vector3(260f, 280f, 0f), 2f);
                //landlordRT.DOLocalMove(new Vector3(260f, 280f, 0f), 2f);
                break;
            case 2:
                AdaptiveScript.Instance.SetAnchor(landlordGO, AdaptiveScript.ANCHOR.RIGHT_TOP); //����ê��
                landlordRT.anchoredPosition3D = AdaptiveScript.Instance.screenLeft + AdaptiveScript.Instance.screenButtom; //����ê��������ʼλ��
                landlordRT.DOAnchorPos3D(new Vector3(-84f, -201f, 0f), 2f);
                //landlordRT.DOLocalMove(new Vector3(-84f, -201f, 0f), 2f);
                break;
        }
        //Debug.Log("������־�������");
    }
    /// <summary>
    /// �������׶�
    /// </summary>
    public void GradLandlord()
    {
        //�����ǰ��Invoke���ڵȴ����򷵻�
        if (IsInvoking())
        {
            return;
        }
        //Debug.Log("�����������׶Σ�firstCallPlayer="+firstLandlordPlayer);
        //�����һ���е������û���������ʸ񣬲��ҿ���������Ҵ���1������һ���������Ϊ��һ���е�������ң���һ���е�����Ҳ���������
        if (!canGradLandlordPlayer.Contains(firstLandlordPlayer) && canGradLandlordPlayer.Count>=1)
        {
            firstLandlordPlayer = (firstLandlordPlayer + 1) % 3;
        }
        //����ǰ��ҵ�text���óɿ�
        afterChooseText[currentPlayer].text = null;
        //���ֻ�е�������не����ʸ���ֱ�ӽ���ӱ��׶Σ������ŵ�����
        if (canGradLandlordPlayer.Count == 1)
        {
            currentPlayer = canGradLandlordPlayer[0];  //������������óɵ�ǰ���

            GradLandlordOver(); //��������Ϲ���

            currentPlayer = (currentPlayer + 1) % 3; //������һ�����

            Invoke("StartGameControl", 2f); //��������GameControl()��������һ�����ȴ�����������ϣ�
        }
        //��������жϣ���ǰ�����������������Ӧ����1
        //ɸѡû�не����ʸ����ҽ�����һ��
        else if (!canGradLandlordPlayer.Contains(currentPlayer))
        {
            currentPlayer = (currentPlayer + 1) % 3;  //��ǰ�������Ϊ��һ�����
            gamePlayManager.GameControl();  //������һ��
        }
        //��ǰ��ҿɽ���������
        else
        {
            //������������߲�������
            int i = Random.Range(0, 2);
            //i = 0; //����

            if (i == 0)
            {
                textRandom = "������";
                currentDouble *= 2;  //�����������ӱ�

                //�����ǰ���Ϊ��һ���е�����������������������ӱ��׶Σ������ŵ�����
                if (currentPlayer == firstLandlordPlayer)
                {
                    if (currentPlayer==0 || currentPlayer==2)
                    {
                        Invoke("GradLandlordOver", 2f);  //����֮��ȴ�����ʱ2s����ʱ��ϣ��ٽ��е����Ʒ���
                    }
                    //�ж��Ƿ����м���ң��м���ҵ���ʱΪ5��
                    else
                    {
                        Invoke("GradLandlordOver", 5f);  //����֮��ȴ�����ʱ5s����ʱ��ϣ��ٽ��е����Ʒ���
                    }
                }
            }
            else
            {
                textRandom = "����";
                canGradLandlordPlayer.Remove(currentPlayer);
            }
            //AI�ĵ���ʱ������Ϊ3�룬��ʼ����ʱ �������Һ��ұ���ң�
            if (currentPlayer == 0 || currentPlayer == 2)
            {
                CountdownManager.Instance.SetCountdown(currentPlayer, 3);  //���õ���ʱ����
            }
            //��ң��м���ң�
            else
            {
                //����������������ť
                playerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/GradLandlordButton"), thisGORT.Find("Player"));
                playerButtonGO.GetComponent<RectTransform>().localPosition = playerButtonPos[0];
                playerButtonGO.GetComponent<Button>().onClick.AddListener(GradLandlordButtonOnClick);
                //��������������ť
                noPlayerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/NotGradLandlordButton"), thisGORT.Find("Player"));
                noPlayerButtonGO.GetComponent<RectTransform>().localPosition = playerButtonPos[1];
                noPlayerButtonGO.GetComponent<Button>().onClick.AddListener(NotGradLandlordButtonOnClick);

                CountdownManager.Instance.SetCountdown(currentPlayer, -1);  //���õ���ʱ
            }
        }
    }

    /// <summary>
    /// ���������
    /// </summary>
    private void GradLandlordOver()
    {
        //Debug.Log("������������ϣ�currentPlayer="+currentPlayer);
        gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.DOUBLE;  //���üӱ��׶�
        CountdownManager.Instance.timeCountGO.SetActive(false); //�Ƚ��õ���ʱ����������Ʒ����¼�
        landlordPlayer = currentPlayer; //���õ������
        gamePlayManager.doubleText.text = "��" + currentDouble.ToString();   //������ʾ
        SetLandlordFlag();  //���õ�����־
        CardManager.instance.IssueLandlordCards(landlordPlayer);  //���ŵ�����
    }
    /// <summary>
    /// ����Invoke���ʱ������GameControl
    /// </summary>
    private void StartGameControl()
    {
        CountdownManager.Instance.timeCountGO.SetActive(true); //���õ���ʱ
        gamePlayManager.GameControl();
    }
    /// <summary>
    /// �ӱ��׶ν���������������ѡ�����ʾ�������ӳٺ������
    /// </summary>
    private void OutCardsStageOverTextSet()
    {
        //���ƽ׶�ѡ�����ʾ�����е�����������
        for (int index = 0; index < afterChooseText.Length; index++)
        {
            afterChooseText[index].text = null;
        }
    }
    /// <summary>
    /// �ӱ��׶�
    /// </summary>
    public void Double()
    {
        //Debug.Log("�ӱ��׶Σ�currentPlayer=" + currentPlayer);
        //�ӱ��׶�ѭ������-1
        doubleStageCount--;
        //Debug.Log("�ӱ��׶Σ�doubleStageCount=" + doubleStageCount);
        //�ж��Ƿ�������Ҿ������˼ӱ��׶�
        if(doubleStageCount==-1)
        {
            currentPlayer = landlordPlayer; //����������ȳ���
            gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.OUT_CARD;  //����ǰ��Ϸ�׶�����Ϊ���ƽ׶�
            //���û�нű�OutCardClass������Ӹýű�
            if (thisGORT.GetComponent<OutCardClass>() == null)
            {
                outCardClass = thisGORT.AddComponent<OutCardClass>(); //��ӽű�OutCard���������ô洢��outCard��
            }
            foreach(GameObject item in CardManager.instance.cardsPlayer)
            {
                item.GetComponent<RectTransform>().Find("Front").AddComponent<CardOnClickClass>(); //����ҿ�������ӿ��Ƶ���ű�
            }
            //Debug.Log("currentPlayer="+currentPlayer);
            //������Ұ�ť(���ơ�����)��������
            playerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/OutCardButton"), thisGORT.Find("Player"));
            playerButtonGO.GetComponent<RectTransform>().localPosition = playerButtonPos[0];
            playerButtonGO.GetComponent<Button>().onClick.AddListener(OutCardButtonOnClick);
            playerButtonGO.SetActive(false);

            noPlayerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/CanNotButton"), thisGORT.Find("Player"));
            noPlayerButtonGO.GetComponent<RectTransform>().localPosition = playerButtonPos[1];
            noPlayerButtonGO.GetComponent<Button>().onClick.AddListener(CanNotButtonOnClick);
            noPlayerButtonGO.SetActive(false);

            CountdownManager.Instance.timeCountGO.SetActive(false); //���õ���ʱ

            //1.8�������������ѡ����ı�
            Invoke("OutCardsStageOverTextSet",1f);
            //����������һ��
            Invoke("StartGameControl",1.5f);
            return;
        }
        afterChooseText[currentPlayer].text = null; //�����һ��ѡ��֮�����ʾ
        //�����ǰ����ʱΪ����״̬�������õ���ʱ
        if (!CountdownManager.Instance.timeCountGO.activeInHierarchy)
        {
            CountdownManager.Instance.timeCountGO.SetActive(true); //���õ���ʱ
        }
        //�����0----�ӱ���1----���ӱ�
        int i = Random.Range(0, 2);
        if(i==0)
        {
            textRandom = "�ӱ�";
            currentDouble *= 2;
        }
        else
        {
            textRandom = "���ӱ�";
        }
        if(currentPlayer==0 || currentPlayer==2)
        {
            CountdownManager.Instance.SetCountdown(currentPlayer, 3); //���õ���ʱ
        }
        else
        {
            playerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/DoubleButton"),thisGORT.Find("Player"));
            playerButtonGO.GetComponent<RectTransform>().localPosition = playerButtonPos[0];
            playerButtonGO.GetComponent<Button>().onClick.AddListener(DoubleButtonOnClick);

            noPlayerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/NotDoubleButton"), thisGORT.Find("Player"));
            noPlayerButtonGO.GetComponent<RectTransform>().localPosition = playerButtonPos[1]-new Vector3(-15f,0f,0f);
            noPlayerButtonGO.GetComponent<Button>().onClick.AddListener(NotDoubleButtonOnClick); //��ӵ���¼�

            thirdPlayerButtonGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Buttons/SuperDoubleButton"), thisGORT.Find("Player"));
            thirdPlayerButtonGO.GetComponent<RectTransform>().localPosition = playerButtonPos[2];
            thirdPlayerButtonGO.GetComponent<Button>().onClick.AddListener(SuperDoubleButtonOnClick);//��ӵ���¼�

            CountdownManager.Instance.SetCountdown(currentPlayer, -1); //���õ���ʱ
        }
    }
    /// <summary>
    /// ���ƽ׶�
    /// </summary>
    public void OutCard()
    {
        //Debug.Log("currentPlayer="+currentPlayer);
        //Debug.Log("������ƽ׶Σ�currentPlayer="+currentPlayer);
        //һ������ҿ��Ƴ��꣬��Ϸ����
        //�ж������ҵĿ����Ƿ�ȫ������
        if (CardManager.instance.cardsLeftPlayer.Count==0)
        {
            //�������ǵ���
            if (landlordPlayer==0)
            {
                isLandlordWin = true;
                //Debug.Log("��Ϸ������������һ�ʤ");
            }
            else
            {
                isLandlordWin = false;
                //Debug.Log("��Ϸ������ũ����һ�ʤ");
            }
            CountdownManager.Instance.timeCountGO.SetActive(false); //���õ���ʱ
            gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.GAME_OVER;  //���õ�ǰ�׶�Ϊ��Ϸ�����׶�

            //���������ҵ���һ��ѡ����ʾ
            for (int i=0;i<afterChooseText.Length;i++)
            {
                afterChooseText[i].text = null;
            }

            //�������ʣ�࿨����
            CardManager.instance.leftPlayerRemainCardsCountGO.SetActive(false);
            CardManager.instance.rightPlayerRemainCardsCountGO.SetActive(false);

            //���м���ҿ��Ƶ�ʣ�࿨������ʾ
            Vector3 playerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.BUTTOM, new Vector3(0f, 360f, 0f)) + new Vector3(-(CardManager.instance.cardsPlayer.Count * 45) / 2, 0f, 0f); //��ǰ��ҳ��Ƴ�ʼλ��
            for (int i = 0; i < CardManager.instance.cardsPlayer.Count; i++)
            {
                //outCardClass.SetOutCardsSitsInHierarchy(CardManager.instance.cardsPlayer); //�������ƿ�����Hierarchy�е�λ��
                CardManager.instance.cardsPlayer[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //���ñ���
                CardManager.instance.cardsPlayer[i].GetComponent<RectTransform>().localPosition = playerOutCardInitPos + new Vector3(45 * i, 0f, 0f); //��ʣ�࿨�����õ����Ƶ�λ��
                
            }
            //���ұ���ҿ��Ƶ�ʣ�࿨������ʾ
            Vector3 rightPlayerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.RIGHT, new Vector3(-350f, 93f, 0f)) + new Vector3(-(CardManager.instance.cardsRightPlayer.Count * 45), 0f, 0f); //��ǰ��ҳ��Ƴ�ʼλ��
            for (int i=0;i<CardManager.instance.cardsRightPlayer.Count;i++)
            {
                //outCardClass.SetOutCardsSitsInHierarchy(CardManager.instance.cardsRightPlayer); //�������ƿ�����Hierarchy�е�λ��
                CardManager.instance.cardsRightPlayer[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //���ñ���
                CardManager.instance.cardsRightPlayer[i].GetComponent<RectTransform>().localPosition= rightPlayerOutCardInitPos + new Vector3(45 * i, 0f, 0f); //��ʣ�࿨�����õ����Ƶ�λ��
            }

            Invoke("StartGameControl",1.0f); //����1s�������һ��
            return;
        }
        //�ж��м���ҵĿ����Ƿ�ȫ������
        if (CardManager.instance.cardsPlayer.Count == 0)
        {
            //winPlayer = 1;
            //Debug.Log("���ƽ׶Σ��м���ҿ���ȫ������");
            if (landlordPlayer == 1)
            {
                isLandlordWin = true;
                //Debug.Log("��Ϸ������������һ�ʤ");
            }
            else
            {
                isLandlordWin = false;
                //Debug.Log("��Ϸ������ũ����һ�ʤ");
            }
            gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.GAME_OVER;  //���õ�ǰ�׶�Ϊ��Ϸ�����׶�
            //gamePlayManager.GameControl(); //������һ��
            CountdownManager.Instance.timeCountGO.SetActive(false); //���õ���ʱ

            //���������ҵ���һ��ѡ����ʾ
            for (int i = 0; i < afterChooseText.Length; i++)
            {
                afterChooseText[i].text = null;
            }

            //�������ʣ�࿨����
            CardManager.instance.leftPlayerRemainCardsCountGO.SetActive(false);
            CardManager.instance.rightPlayerRemainCardsCountGO.SetActive(false);

            //�������ҿ��Ƶ�ʣ�࿨������ʾ
            Vector3 leftPlayerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.LEFT, new Vector3(400f, 93f, 0f)); //�����ҳ��Ƴ�ʼλ��
            for (int i = 0; i < CardManager.instance.cardsLeftPlayer.Count; i++)
            {
                //outCardClass.SetOutCardsSitsInHierarchy(CardManager.instance.cardsLeftPlayer); //�������ƿ�����Hierarchy�е�λ��
                CardManager.instance.cardsLeftPlayer[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //���ñ���
                CardManager.instance.cardsLeftPlayer[i].GetComponent<RectTransform>().localPosition = leftPlayerOutCardInitPos + new Vector3(45f * i, 0f, 0f); //��ʣ�࿨�����õ����Ƶ�λ��
            }
            //���ұ���ҿ��Ƶ�ʣ�࿨������ʾ
            Vector3 rightPlayerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.RIGHT, new Vector3(-350f, 93f, 0f)) + new Vector3(-(CardManager.instance.cardsRightPlayer.Count * 45), 0f, 0f); //��ǰ��ҳ��Ƴ�ʼλ��
            for (int i = 0; i < CardManager.instance.cardsRightPlayer.Count; i++)
            {
                //outCardClass.SetOutCardsSitsInHierarchy(CardManager.instance.cardsRightPlayer); //�������ƿ�����Hierarchy�е�λ��
                CardManager.instance.cardsRightPlayer[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //���ñ���
                CardManager.instance.cardsRightPlayer[i].GetComponent<RectTransform>().localPosition = rightPlayerOutCardInitPos + new Vector3(45 * i, 0f, 0f); //��ʣ�࿨�����õ����Ƶ�λ��

            }

            Invoke("StartGameControl", 1.0f); //����1s�������һ��
            return;
        }
        //�ж��ұ���ҵĿ����Ƿ�ȫ������
        if (CardManager.instance.cardsRightPlayer.Count == 0)
        {
            //Debug.Log("���ƽ׶Σ��ұ���ҿ���ȫ������");
            if (landlordPlayer == 2)
            {
                isLandlordWin = true;
                //Debug.Log("��Ϸ������������һ�ʤ");
            }
            else
            {
                isLandlordWin = false;
                //Debug.Log("��Ϸ������ũ����һ�ʤ");
            }
            gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.GAME_OVER;  //���õ�ǰ�׶�Ϊ��Ϸ�����׶�
            //gamePlayManager.GameControl(); //������һ��
            CountdownManager.Instance.timeCountGO.SetActive(false); //���õ���ʱ

            //���������ҵ���һ��ѡ����ʾ
            for (int i = 0; i < afterChooseText.Length; i++)
            {
                afterChooseText[i].text = null;
            }

            //�������ʣ�࿨����
            CardManager.instance.leftPlayerRemainCardsCountGO.SetActive(false);
            CardManager.instance.rightPlayerRemainCardsCountGO.SetActive(false);

            //���м���ҿ��Ƶ�ʣ�࿨������ʾ
            Vector3 playerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.BUTTOM, new Vector3(0f, 360f, 0f)) + new Vector3(-(CardManager.instance.cardsPlayer.Count * 45) / 2, 0f, 0f); //��ǰ��ҳ��Ƴ�ʼλ��
            for (int i = 0; i < CardManager.instance.cardsPlayer.Count; i++)
            {
                //outCardClass.SetOutCardsSitsInHierarchy(CardManager.instance.cardsPlayer); //�������ƿ�����Hierarchy�е�λ��
                CardManager.instance.cardsPlayer[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //���ñ���
                CardManager.instance.cardsPlayer[i].GetComponent<RectTransform>().localPosition = playerOutCardInitPos + new Vector3(45f * i, 0f, 0f); //��ʣ�࿨�����õ����Ƶ�λ��

            }
            //�������ҿ��Ƶ�ʣ�࿨������ʾ
            Vector3 leftPlayerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.LEFT, new Vector3(400f, 93f, 0f)); //�����ҳ��Ƴ�ʼλ��
            for (int i = 0; i < CardManager.instance.cardsLeftPlayer.Count; i++)
            {
                //outCardClass.SetOutCardsSitsInHierarchy(CardManager.instance.cardsLeftPlayer); //�������ƿ�����Hierarchy�е�λ��
                CardManager.instance.cardsLeftPlayer[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //���ñ���
                CardManager.instance.cardsLeftPlayer[i].GetComponent<RectTransform>().localPosition = leftPlayerOutCardInitPos + new Vector3(45f * i, 0f, 0f); //��ʣ�࿨�����õ����Ƶ�λ��
            }

            Invoke("StartGameControl", 1.0f); //����1s�������һ��
            return;
        }

        afterChooseText[currentPlayer].text = null;  //�����һ��ѡ����ʾ
        //�������ҿ���ֻʣ��2�ţ��򲥷š���������Ч
        if (CardManager.instance.cardsLeftPlayer.Count <= 2 && isAlreadyAlarmList[0] == false)
        {
            MusicControl.Instance.PLaySound(MusicControl.SOUND.ALARM_SOUND); //���š���������Ч
            isAlreadyAlarmList[0] = true; //����ǰ����ѱ���������Ϊtrue
        }
        if (CardManager.instance.cardsPlayer.Count <= 2 && isAlreadyAlarmList[1] == false)
        {
            MusicControl.Instance.PLaySound(MusicControl.SOUND.ALARM_SOUND); //���š���������Ч
            isAlreadyAlarmList[1] = true;
        }
        if (CardManager.instance.cardsRightPlayer.Count <= 2 && isAlreadyAlarmList[2] == false)
        {
            MusicControl.Instance.PLaySound(MusicControl.SOUND.ALARM_SOUND); //���š���������Ч
            isAlreadyAlarmList[2] = true;
        }

        //���ɳ����ұ�����ƣ���һ�����Ƶ���� || ��һ��������Ҽ�Ϊ��ǰ��ң�
        if (outCardClass.lastOutCardType == -1 || outCardClass.lastOutCardPlayer == currentPlayer)
        {
            //Debug.Log("���ƽ׶Σ����ɳ���");
            isFreePlay = true; //��ǰΪ���ɳ���
            //������һ�ֵ��ѳ������Ƴ�����
            //������
            if (outCardClass.leftPlayerLastOutCardsList.Count!=0)
            {
                foreach (GameObject item in outCardClass.leftPlayerLastOutCardsList)
                {
                    //GameObject.Destroy(item);
                    item.GetComponent<RectTransform>().position = new Vector3(-100f,-100f,0f); //���ѳ��Ŀ������õ�������
                }
            }
            //�м����
            if (outCardClass.playerLastOutCardsList.Count!=0)
            {
                foreach (GameObject item in outCardClass.playerLastOutCardsList)
                {
                    //GameObject.Destroy(item);
                    item.GetComponent<RectTransform>().position = new Vector3(-100f, -100f, 0f); //���ѳ��Ŀ������õ�������
                }
            }
            //�ұ����
            if (outCardClass.rightPlayerLastOutCardsList.Count!=0)
            {
                foreach (GameObject item in outCardClass.rightPlayerLastOutCardsList)
                {
                    //GameObject.Destroy(item);
                    item.GetComponent<RectTransform>().position = new Vector3(-100f, -100f, 0f); //���ѳ��Ŀ������õ�������
                }
            }

            switch (currentPlayer)
            {
                //������
                case 0:
                    outCardClass.outCardsList = outCardClass.GetMinCanOutCardsByType(CardManager.instance.cardsLeftPlayer,-1,-1); //AI��һ�ȡ���ƿ���
                    CountdownManager.Instance.SetCountdown(currentPlayer,3); //��ʼ����ʱ2s������ʱ��������õ���ʱ�������Ƹ�������
                    break;
                //�м����
                case 1:
                    outCardClass.playerPreOutCardsList = outCardClass.GetMinCanOutCardsByType(CardManager.instance.cardsPlayer,-1,-1); //Ԥ�ȴ�����ҳ��ƿ��ƣ���ҳ��Ƴ�ʱ�������

                    //������Ұ�ť
                    playerButtonGO.SetActive(true);
                    noPlayerButtonGO.SetActive(true);

                    CountdownManager.Instance.SetCountdown(currentPlayer, -1); //��ʼ����ʱ
                    break;
                //�ұ����
                case 2:
                    outCardClass.outCardsList = outCardClass.GetMinCanOutCardsByType(CardManager.instance.cardsRightPlayer, -1, -1); //AI��һ�ȡ���ƿ���
                    CountdownManager.Instance.SetCountdown(currentPlayer,3); //��ʼ����ʱ2s������ʱ��������õ���ʱ�������Ƹ�������
                    break;
            }
        }
        //�����ɳ��ƣ�ǰ������ҳ��ƣ�
        else
        {
            //Debug.Log("���ƽ׶Σ������ɳ���,outCardClass.playerPreOutCardsList.Count=" + outCardClass.playerPreOutCardsList.Count);
            //Debug.Log("�����ɳ��ƣ�(int)outCardClass.maxValueLastOutCardsMaxCount=" + outCardClass.maxValueLastOutCardsMaxCount);
            //Debug.Log("�����ɳ��ƣ�outCardClass.lastOutCardType"+ outCardClass.lastOutCardType);

            isFreePlay = false; //��ǰΪ�����ɳ���

            switch (currentPlayer)
            {
                //������
                case 0:
                    //�����ǰ���Ϊ������ң�������һ���������Ϊ������ң����߶���δ�����Ҷ��ѵ������������ֵС��10�Ҳ���ը������ը�������
                    if (landlordPlayer == 0 || outCardClass.lastOutCardPlayer==landlordPlayer || (!isAlreadyAlarmList[outCardClass.lastOutCardPlayer] && outCardClass.maxValueLastOutCardsMaxCount<10 && outCardClass.lastOutCardType<11))
                    {
                        outCardClass.outCardsList = outCardClass.GetMinCanOutCardsByType(CardManager.instance.cardsLeftPlayer, outCardClass.maxValueLastOutCardsMaxCount, outCardClass.lastOutCardType); //AI��һ�ȡ���ƿ���
                    }
                    
                    //����������һ�����������Ƴ�������
                    if (outCardClass.leftPlayerLastOutCardsList.Count!=0)
                    {
                        foreach (GameObject go in outCardClass.leftPlayerLastOutCardsList)
                        {
                            //Destroy(go);
                            go.GetComponent<RectTransform>().position = new Vector3(-100f, -100f, 0f); //���ѳ��Ŀ������õ�������
                        }
                    }
                    CountdownManager.Instance.SetCountdown(currentPlayer,3); //��ʼ����ʱ2s������ʱ��������õ���ʱ�������Ƹ�������
                    break;
                //�м����
                case 1:
                    //outCardClass.playerPreOutCardsList = outCardClass.GetMinCanOutCardsByType(CardManager.instance.cardsPlayer, -1, -1); //Ԥ�ȴ�����ҳ��ƿ��ƣ���ҳ��Ƴ�ʱ�������
                    //ɾ���м������һ����������
                    if (outCardClass.playerLastOutCardsList.Count != 0)
                    {
                        foreach (GameObject go in outCardClass.playerLastOutCardsList)
                        {
                            //Destroy(go);
                            go.GetComponent<RectTransform>().position = new Vector3(-100f, -100f, 0f); //���ѳ��Ŀ������õ�������
                        }
                    }
                    //������Ұ�ť
                    playerButtonGO.SetActive(true);
                    noPlayerButtonGO.SetActive(true);

                    CountdownManager.Instance.SetCountdown(currentPlayer, -1); //��ʼ����ʱ
                    break;
                //�ұ����
                case 2:
                    //�����ǰ���Ϊ������ң����߶���δ�����Ҷ��ѵ������������ֵС��10�Ҳ���ը������ը�������
                    if (landlordPlayer == 2 || outCardClass.lastOutCardPlayer == landlordPlayer || (!isAlreadyAlarmList[outCardClass.lastOutCardPlayer] && outCardClass.maxValueLastOutCardsMaxCount < 10 && outCardClass.lastOutCardType < 11))
                    {
                        outCardClass.outCardsList = outCardClass.GetMinCanOutCardsByType(CardManager.instance.cardsRightPlayer, outCardClass.maxValueLastOutCardsMaxCount, outCardClass.lastOutCardType); //AI��һ�ȡ���ƿ���
                    }
                    
                    //���ұ������һ�����������Ƴ�������
                    if (outCardClass.rightPlayerLastOutCardsList.Count != 0)
                    {
                        foreach (GameObject go in outCardClass.rightPlayerLastOutCardsList)
                        {
                            //Destroy(go);
                            go.GetComponent<RectTransform>().position = new Vector3(-100f, -100f, 0f); //���ѳ��Ŀ������õ�������
                        }
                    }
                    CountdownManager.Instance.SetCountdown(currentPlayer, 3); //��ʼ����ʱ2s������ʱ��������õ���ʱ�������Ƹ�������
                    break;
            }
        }
    }

    /// <summary>
    /// ��Ϸ�����׶�
    /// </summary>
    public void GameOver()
    {
        gameOverPanelRT.gameObject.SetActive(true); //������Ϸ����Panel
        gameOverPanelRT.SetAsLastSibling(); //��Hierarchy����н�Panel����ĩβ��ʹ֮�����ϲ㣩

        MusicControl.Instance.StopBGMusic(); //ֹͣ���ű�������

        Text playerJoyBeansCountText=CardManager.instance.playerJoyBeansRT.Find("Count").GetComponent<Text>(); //��ȡ��һ��ֶ�������Text���

        //����Title�����ֶ���
        //����ǵ���
        if (landlordPlayer==1)
        {
            //������ʤ
            if (isLandlordWin)
            {
                gameOverTitleImage.sprite = (Sprite)Resources.Load("UI/GameUI/LandlordWinTitle", typeof(Sprite)); //����Title

                int gainJoyBeans= joyBeansBasicDouble * currentDouble * 2; //��õĻ��ֶ���
                DATA.Instance.SaveData(DATA.Instance.GetData() + gainJoyBeans); //��������
                playerJoyBeansCountText.text = DATA.Instance.GetData().ToString(); //������һ��ֶ���ʾ
                gameOverJoyBeanCountText.text = "+" + gainJoyBeans.ToString(); //���û�ȡ�Ļ��ֶ�
                gameOverPlayerRemainJoyBeanText.text = "��ʣ�໶�ֶ���" + DATA.Instance.GetData().ToString() + "��"; //�������ʣ�໶�ֶ��ı�

                gameOverBGRT.GetComponent<Image>().color= new Color((255 / 255f), (224 / 255f), (0 / 255f), (255 / 255f)); //����ʤ��������ɫ

                MusicControl.Instance.PLaySound(MusicControl.SOUND.WIN_MUSIC); //���š�ʤ������Ƶ

            }
            //����ʧ��
            else
            {
                gameOverTitleImage.sprite = (Sprite)Resources.Load("UI/GameUI/LandlordLoseTitle", typeof(Sprite)); //����Title

                int loseJoyBeans = joyBeansBasicDouble * currentDouble * 2; //��Ļ��ֶ���
                //��ǰ���ʣ�໶�ֶ��㹻�۳�
                if ((DATA.Instance.GetData() - loseJoyBeans) > 0)
                {
                    DATA.Instance.SaveData(DATA.Instance.GetData() - loseJoyBeans); //��������
                }
                //��ǰ���ʣ�໶�ֶ������Կ۳�����ֻ�۳���ҵ�ǰ���л��ֶ�
                else
                {
                    loseJoyBeans = DATA.Instance.GetData(); //����ǰ������л��ֶ���ֵ����Ļ��ֶ���
                    DATA.Instance.SaveData(0); //��������
                    playerJoyBeansCountText.color = Color.red; //����һ��ֶ�����������Ϊ��ɫ
                }
                playerJoyBeansCountText.text = DATA.Instance.GetData().ToString(); //������һ��ֶ���ʾ
                //gameOverJoyBeanCountText.text = "-" + loseJoyBeans.ToString() + "��ʣ�໶�ֶ���" + DATA.Instance.GetData().ToString() + "��"; //���û�ȡ�Ļ��ֶ����Լ�ʣ�໶�ֶ�
                gameOverJoyBeanCountText.text = "-" + loseJoyBeans.ToString(); //���û�ȡ�Ļ��ֶ�
                gameOverPlayerRemainJoyBeanText.text = "��ʣ�໶�ֶ���" + DATA.Instance.GetData().ToString() + "��"; //�������ʣ�໶�ֶ��ı�
                //gameOverJoyBeanCountText.text = "-" + joyBeansBasicDouble * currentDouble * 2;//���û�ȡ�Ļ��ֶ�

                gameOverBGRT.GetComponent<Image>().color = new Color((66 / 255f), (114 / 255f), (140 / 255f), (255 / 255f)); //����ʧ�ܱ�����ɫ

                MusicControl.Instance.PLaySound(MusicControl.SOUND.LOSE_MUSIC); //���š�ʧ�ܡ���Ƶ
            }
        }
        //�����ũ��
        else
        {
            //ũ��ʧ��
            if (isLandlordWin)
            {
                gameOverTitleImage.sprite = (Sprite)Resources.Load("UI/GameUI/FarmerLoseTitle", typeof(Sprite)); //����Title

                int loseJoyBeans = joyBeansBasicDouble * currentDouble; //��Ļ��ֶ���
                //��ǰ���ʣ�໶�ֶ��㹻�۳�
                if ((DATA.Instance.GetData() - loseJoyBeans)>0)
                {
                    DATA.Instance.SaveData(DATA.Instance.GetData() - loseJoyBeans); //��������
                }
                //��ǰ���ʣ�໶�ֶ������Կ۳�����ֻ�۳���ҵ�ǰ���л��ֶ�
                else
                {
                    loseJoyBeans = DATA.Instance.GetData(); //����ǰ������л��ֶ���ֵ����Ļ��ֶ���
                    DATA.Instance.SaveData(0); //��������
                    playerJoyBeansCountText.color = Color.red; //����һ��ֶ�����������Ϊ��ɫ
                }
                playerJoyBeansCountText.text = DATA.Instance.GetData().ToString(); //������һ��ֶ���ʾ
                gameOverJoyBeanCountText.text = "-" + loseJoyBeans.ToString(); //���û�ȡ�Ļ��ֶ�
                gameOverPlayerRemainJoyBeanText.text = "��ʣ�໶�ֶ���" + DATA.Instance.GetData().ToString() + "��";  //�������ʣ�໶�ֶ��ı�
                //gameOverJoyBeanCountText.text = "-" + joyBeansBasicDouble * currentDouble;//���û�ȡ�Ļ��ֶ�

                gameOverBGRT.GetComponent<Image>().color = new Color((66 / 255f), (114 / 255f), (140 / 255f), (255 / 255f)); //����ʧ�ܱ�����ɫ

                MusicControl.Instance.PLaySound(MusicControl.SOUND.LOSE_MUSIC); //���š�ʧ�ܡ���Ƶ
            }
            //ũ��ʤ��
            else
            {
                gameOverTitleImage.sprite = (Sprite)Resources.Load("UI/GameUI/FarmerWinTitle", typeof(Sprite)); //����Title

                int gainJoyBeans = joyBeansBasicDouble * currentDouble ; //��õĻ��ֶ���
                DATA.Instance.SaveData(DATA.Instance.GetData() + gainJoyBeans); //��������
                playerJoyBeansCountText.text = DATA.Instance.GetData().ToString(); //������һ��ֶ���ʾ
                gameOverJoyBeanCountText.text = "+" + gainJoyBeans.ToString(); //���û�ȡ�Ļ��ֶ�
                gameOverPlayerRemainJoyBeanText.text = "��ʣ�໶�ֶ���" + DATA.Instance.GetData().ToString() + "��";  //�������ʣ�໶�ֶ��ı�
                //gameOverJoyBeanCountText.text = "+" + joyBeansBasicDouble * currentDouble;//���û�ȡ�Ļ��ֶ�

                gameOverBGRT.GetComponent<Image>().color = new Color((255 / 255f), (224 / 255f), (0 / 255f), (255 / 255f)); //����ʤ��������ɫ

                MusicControl.Instance.PLaySound(MusicControl.SOUND.WIN_MUSIC); //���š�ʤ������Ƶ
            }
        }
        
    }

    /// <summary>
    /// ���ơ����ơ���������
    /// ���ó��Ƶ�λ�á��Ƴ���ҿ����г��ƵĿ��ơ��洢������Ϣ����һ�������С���ճ��Ƽ��ϡ�������һ��
    /// </summary>
    private void OutCardOutAuxiliary()
    {
        //Debug.Log("���Ƹ���������1��outCardsList.Count="+ outCardClass.outCardsList.Count);
        outCardClass.SetOutCardsListSort(outCardClass.outCardsList); //�����ƿ��ƽ�������

        //Debug.Log("���Ƹ���������2��outCardsList.Count=" + outCardClass.outCardsList.Count);
        Vector3 currentPlayerOutCardInitPos = new Vector3(0f,0f,0f); //�洢��ǰ��ҳ���λ��
        float offset = 45f; //���ƿ���λ��ƫ����
        List<GameObject> cards = new List<GameObject>(); //�洢��ҿ������ã����ڼ��ʹ����ҿ���
        //���õ�ǰ��ҿ��ơ����Ƴ�ʼλ�á��洢���ƿ�������һ�ֳ��Ƽ����С��������ʣ�࿨����
        switch (currentPlayer)
        {
            case 0:
                cards = CardManager.instance.cardsLeftPlayer;
                Text leftRemainCountText = CardManager.instance.leftPlayerRemainCardsCountGO.GetComponent<Text>();
                leftRemainCountText.text=( int.Parse(leftRemainCountText.text) - outCardClass.outCardsList.Count ).ToString();
                currentPlayerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.LEFT,new Vector3(400f,93f,0f)); //��ǰ��ҳ��Ƴ�ʼλ��
                //currentPlayerOutCardInitPos = new Vector3(400f, 600f, 0f); //��ǰ��ҳ��Ƴ�ʼλ��
                //��ӳ��ƿ������������һ�ֵĿ��Ƽ�����
                foreach (GameObject go in outCardClass.outCardsList)
                {
                    outCardClass.leftPlayerLastOutCardsList.Add(go);
                    go.GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //���ñ���
                }
                break;
            case 1:
                cards = CardManager.instance.cardsPlayer;
                //currentPlayerOutCardInitPos = new Vector3(1000f - (outCardClass.outCardsList.Count * offset) / 2, 350f, 0f); //��ǰ��ҳ��Ƴ�ʼλ��
                currentPlayerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.BUTTOM, new Vector3(0f, 360f, 0f)) + new Vector3(-(outCardClass.outCardsList.Count * offset) / 2,0f,0f); //��ǰ��ҳ��Ƴ�ʼλ��
                //��ӳ��ƿ������������һ�ֵĿ��Ƽ����У��Ƴ����Ƶ����
                foreach (GameObject go in outCardClass.outCardsList)
                {
                    outCardClass.playerLastOutCardsList.Add(go);
                    CardOnClickClass cardOnClickClass = go.GetComponent<RectTransform>().Find("Front").GetComponent<CardOnClickClass>(); //���Ҹÿ���ǰ���RT����
                    if (cardOnClickClass != null)
                    {
                        Destroy(cardOnClickClass); //�Ƴ����
                    }
                }
                break;
            case 2:
                cards = CardManager.instance.cardsRightPlayer;
                Text rightRemainCountText = CardManager.instance.rightPlayerRemainCardsCountGO.GetComponent<Text>();
                rightRemainCountText.text = (int.Parse(rightRemainCountText.text) - outCardClass.outCardsList.Count).ToString();
                currentPlayerOutCardInitPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.RIGHT, new Vector3(-350f, 93f, 0f)) + new Vector3(-(outCardClass.outCardsList.Count*offset),0f,0f); //��ǰ��ҳ��Ƴ�ʼλ��
                //currentPlayerOutCardInitPos = new Vector3(1600f-outCardClass.outCardsList.Count*offset,600f,0f); //��ǰ��ҳ��Ƴ�ʼλ��
                //��ӳ��ƿ������������һ�ֵĿ��Ƽ�����
                foreach (GameObject go in outCardClass.outCardsList)
                {
                    outCardClass.rightPlayerLastOutCardsList.Add(go);
                    go.GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //���ñ���
                }
                break;
        }

        //Debug.Log("currentPlayerOutCardInitPos=" + currentPlayerOutCardInitPos);
        outCardClass.SetOutCardsSitsInHierarchy(outCardClass.outCardsList); //�������ƿ�����Hierarchy�е�λ��

        //Debug.Log("���Ƹ���������4��outCardsList.Count=" + outCardClass.outCardsList.Count);
        //���ó���λ�á���ҿ������Ƴ��Ƴ�����
        for (int i=0;i<outCardClass.outCardsList.Count;i++)
        {
            outCardClass.outCardsList[i].GetComponent<RectTransform>().localPosition = currentPlayerOutCardInitPos + new Vector3(offset*i, 0f, 0f); //��Ҫ���Ŀ������õ����Ƶ�λ��
            //Debug.Log("outCardClass.outCardsList[i].localPosition"+ outCardClass.outCardsList[i].GetComponent<RectTransform>().localPosition);
            MusicControl.Instance.PLaySound(MusicControl.SOUND.OUT_CARD_SOUND); //���ų�����Ч
            cards.Remove(outCardClass.outCardsList[i]); //��ҿ������Ƴ��ѳ��Ŀ���
        }

        //�����Ƴ���Ŀ��Ƽ����и������Ƶ�λ�ã���ǰ������ҵĿ��ƽ��е�����
        if (currentPlayer == 1)
        {
            int cardsCount = cards.Count; //�����ܸ���
            //Debug.Log("cardsCount="+cardsCount);
            //float cardsBeginPosition = 0 - (cardsCount / 2) * 55f; //���㿨�Ƶ���ʼλ��
            //Debug.Log("-(cardsCount / 2) * (offset+15)" + -(cardsCount / 2) * (offset + 15));
            Vector3 cardsBeginPosition = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.BUTTOM, new Vector3(0, 140, 0)) + new Vector3(-(cardsCount / 2) * (offset+15), 0f, 0f); //���㿨�Ƶ���ʼλ��
            //���µ����Ƴ���Ŀ���λ��
            for (int i = 0; i < cardsCount; i++)
            {
                cards[i].GetComponent<RectTransform>().localPosition = cardsBeginPosition+new Vector3(i * (offset+15), 0f, 0f);
                //cards[i].GetComponent<RectTransform>().localPosition = new Vector3(cardsBeginPosition + i * 55f, -400f, 0f);
            }
        }

        //Debug.Log("���ơ����ơ��������������ƿ���Ϊ��");
        //foreach (GameObject item in outCardClass.outCardsList)
        //{
        //    Debug.Log("���ƿ��ƣ�"+item.name);
        //}

        Dictionary<CardManager.ValueType, int> outCardsListValueTypeCount = new Dictionary<CardManager.ValueType, int>(); //�洢���ƿ��Ƶ�ֵ�����Լ���Ӧ������
        outCardsListValueTypeCount = CardManager.instance.rememberCardValueCount.GetCardsValueCount(outCardClass.outCardsList); //��ȡ���ƿ��Ƶ�ֵ�����Լ���Ӧ������
        CardManager.instance.rememberCardDevice.UpdateRememberCardDeviceValue(outCardsListValueTypeCount); //���¼�����

        outCardClass.SetLastOutCardInfo(outCardClass.outCardsList, currentPlayer); //�洢���ƿ��Ƶ�����������ֵ��ValueType���������͡���һ��������ҡ�������ƿ��Ƽ���

        //���ݳ������Ͳ��Ŷ�Ӧ��Ч�������ñ���
        switch ((OutCardClass.OUT_CARD_TYPE)outCardClass.lastOutCardType)
        {
            //˳��
            case OutCardClass.OUT_CARD_TYPE.STRAIGHT:
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.STRAIGHT_SOUND); //���š�˳�ӡ���Ч
                    break;
                }
            //����
            case OutCardClass.OUT_CARD_TYPE.PAIRS:
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.PAIRS_SOUND); //���š����ԡ���Ч
                    break;
                }
            //�ɻ�
            case OutCardClass.OUT_CARD_TYPE.PLANES_WITHOUT:
            case OutCardClass.OUT_CARD_TYPE.PLANES_WITH_SINGLE:
            case OutCardClass.OUT_CARD_TYPE.PLANES_WITH_DOUBLE:
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.PLANE_SOUND); //���š��ɻ�����Ч
                    break;
                }
            //ը��
            case OutCardClass.OUT_CARD_TYPE.BOMBS:
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.GIRL_BOMB_SOUND); //���š�ը������Ч
                    currentDouble *= 2; //��������
                    gamePlayManager.doubleText.text = "��" + currentDouble.ToString();   //������ʾ

                    //�жϵ�ǰ��ʼ��Ϸ������Ƶ�Ƿ����ڲ���
                    if (!MusicControl.Instance.isPlayingBeginGameMusic02)
                    {
                        MusicControl.Instance.PLaySound(MusicControl.SOUND.BEGIN_GAME_MUSIC_02); //������ʼ��Ϸ��������
                        MusicControl.Instance.isPlayingBeginGameMusic02 = true;
                    }
                    break;
                }
            //��ը
            case OutCardClass.OUT_CARD_TYPE.KING_EXPLOSIONS:
                {
                    MusicControl.Instance.PLaySound(MusicControl.SOUND.KING_EXPLOSIONS_SOUND); //���š���ը����Ч
                    //Debug.Log("����ը����Ч");
                    currentDouble *= 4; //������4��
                    gamePlayManager.doubleText.text = "��" + currentDouble.ToString();   //������ʾ

                    //�жϵ�ǰ��ʼ��Ϸ������Ƶ�Ƿ����ڲ���
                    if (!MusicControl.Instance.isPlayingBeginGameMusic02)
                    {
                        MusicControl.Instance.PLaySound(MusicControl.SOUND.BEGIN_GAME_MUSIC_02); //������ʼ��Ϸ��������
                        MusicControl.Instance.isPlayingBeginGameMusic02 = true;
                    }
                    break;
                }
            //����������
            default:
                {
                    //�����ɳ���
                    if (!isFreePlay)
                    {
                        MusicControl.Instance.PLaySound(MusicControl.SOUND.LARGER_SOUND); //���š����㡿��Ч
                    }
                    break;
                }
        }

        //������һ�����
        currentPlayer = (currentPlayer + 1) % 3;
        gamePlayManager.GameControl();
    }
    /// <summary>
    /// ��ҡ����ơ�����������������ұ�����ƣ�����ʱδ���Ƶ������
    /// ��Ԥ����Ŀ��Ƹ��������ƿ��ƣ������Ԥ���뿨��
    /// </summary>
    private void PlayerOutCardOutAuxiliary()
    {
        //Debug.Log("��ҡ����ơ���������01��playerPreOutCardsList.Count=" + outCardClass.playerPreOutCardsList.Count);
        //������Ұ�ť
        playerButtonGO.SetActive(false);
        noPlayerButtonGO.SetActive(false);

        //Debug.Log("��ҡ����ơ�����������outCardClass.playerPreOutCardsList.Count="+ outCardClass.playerPreOutCardsList.Count);
        //�����ǰ��ҵ�����ƣ����д�����ҵ��Ԥ���ƿ��ƣ���λλ���������ҵ��Ԥ���ƿ���
        if (outCardClass.playerOnClickPreOutCardsList.Count!=0)
        {
            //ȡ�����Ƶ��Ч��
            foreach (GameObject item in outCardClass.playerOnClickPreOutCardsList)
            {
                item.GetComponent<RectTransform>().position -= new Vector3(0f, 20f, 0f); //������Ŀ���ȡ�����Ч�����ص���ʼλ�ã�
                item.GetComponent<RectTransform>().Find("Front").GetComponent<CardOnClickClass>().isSelect = false; //�ÿ�������Ϊδѡ��
            }

            outCardClass.playerOnClickPreOutCardsList.Clear(); //�����ҵ��Ԥ������
        }
        outCardClass.outCardsList.AddRange(outCardClass.playerPreOutCardsList); //��Ԥ�������ҿ��Ƹ�ֵ�����ƿ���
        outCardClass.playerPreOutCardsList.Clear(); //���Ԥ���뿨��
        Debug.Log("��ҡ����ơ���������02��playerPreOutCardsList.Count=" + outCardClass.playerPreOutCardsList.Count);
        //Debug.Log("��ҡ����ơ�����������outCardClass.outCardsList[0]=" + outCardClass.outCardsList[0]);
        OutCardOutAuxiliary(); //���ÿ��ơ����ơ�����
    }

    /// <summary>
    /// ���ơ���Ҫ����������
    /// (���outCardsList������text�����ý�����һ�����)
    /// </summary>
    private void OutCardCanNotAuxiliary()
    {
        MusicControl.Instance.PLaySound(MusicControl.SOUND.GILR_CAN_NOT_SOUND); //���š���Ҫ����Ч

        afterChooseText[currentPlayer].text = "����"; //��ҵ���ʱ������Ĭ�ϲ���

        //������һ�����
        currentPlayer = (currentPlayer + 1) % 3;
        gamePlayManager.GameControl();
    }
    /// <summary>
    /// ��ҡ���Ҫ����������
    /// ��������Ұ�ť��ȡ�����Ƶ��Ч����������ҡ���Ҫ������������
    /// </summary>
    private void PlayerOutCardCanNotAuxiliary()
    {
        //������Ұ�ť
        playerButtonGO.SetActive(false);
        noPlayerButtonGO.SetActive(false);

        //ȡ�����Ƶ��Ч��
        foreach (GameObject item in outCardClass.playerOnClickPreOutCardsList)
        {
            item.GetComponent<RectTransform>().localPosition -= new Vector3(0f, 20f, 0f); //������Ŀ���ȡ�����Ч�����ص���ʼλ�ã�
            item.GetComponent<RectTransform>().Find("Front").GetComponent<CardOnClickClass>().isSelect = false; //�ÿ�������Ϊδѡ��
        }

        outCardClass.playerPreOutCardsList.Clear(); //������Ԥ���Ƽ���
        outCardClass.playerOnClickPreOutCardsList.Clear(); //�����ҵ��Ԥ������

        OutCardCanNotAuxiliary(); //������ҡ���Ҫ����������
    }

    /// <summary>
    /// ����ʱ�������Ƹ�������������ʱ���������øú�����
    /// </summary>
    public void CountDownOverOutCardAuxiliary()
    {
        //���
        if (currentPlayer==1)
        {
            //���ɳ��ƣ���ҳ�ʱδ����
            if (outCardClass.playerPreOutCardsList.Count != 0)
            {
                PlayerOutCardOutAuxiliary(); //������ҳ��Ƹ�������
            }
            //�����ɳ��ƣ���ҵ���ʱ��������Ϊ����Ҫ��
            else
            {
                PlayerOutCardCanNotAuxiliary(); //������Ҳ�Ҫ��������
            }
        }
        //AI���
        else
        {
            //��ǰ�пɳ��Ŀ���
            if (outCardClass.outCardsList.Count!=0)
            {
                OutCardOutAuxiliary(); //���ÿ��Ƴ��Ƹ�������
            }
            else
            {
                OutCardCanNotAuxiliary(); //���ÿ��ơ���Ҫ����������
            }
        }
    }



    /// <summary>
    /// �е�����ť�����¼�
    /// </summary>
    private void CallLandlordButtonOnClick()
    {
        //Debug.Log("�е�����ť���");
        afterChooseText[currentPlayer].text = "�е���";  //����text

        MusicControl.Instance.PLaySound(MusicControl.SOUND.CALL_LANDLORD_SOUND); //���š��е�������Ч

        //����textRandom�н����޸�
        if(textRandom=="����")
        {
            firstLandlordPlayer = currentPlayer;  //�洢��һ���е��������
            gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.GRAD_LANDLORD;   //���õ�ǰ��Ϸ�׶�Ϊ������
            canGradLandlordPlayer.Add(currentPlayer);  //��textRandom��ɾ�����������ʸ����¼���
        }

        ListenEventCommonBehavior();
    } 
    /// <summary>
    /// ���е�����ť�����¼�
    /// </summary>
    private void NotCallLandlordButtonOnClick()
    {
        afterChooseText[currentPlayer].text = "����";  //����text

        MusicControl.Instance.PLaySound(MusicControl.SOUND.NOT_CALL_LANDLORD_SOUND); //���š����С���Ч

        //����textRandom�н����޸�
        if (textRandom == "�е���")
        {
            gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.CALL_LANDLORD;  //����ǰ��Ϸ�׶��޸Ļؽе���
            firstLandlordPlayer = -1;   //����һ���е�������޸Ļ�-1
            canGradLandlordPlayer.Remove(currentPlayer);  //���е��������û�����������ʸ��Ƴ�
        }
        ListenEventCommonBehavior();
    }
    /// <summary>
    /// ��������ť�����¼�
    /// </summary>
    private void GradLandlordButtonOnClick()
    {
        CountdownManager.Instance.StopIEnum();  //ֹͣЭ��

        GameObject.Destroy(playerButtonGO);  //������ҵ��yes��ť
        GameObject.Destroy(noPlayerButtonGO);  //������ҵ��no��ť

        afterChooseText[currentPlayer].text = "������";  //ѡ��֮����ı���ʾ

        MusicControl.Instance.PLaySound(MusicControl.SOUND.GRAD_LANDLORD_SOUND); //���š�����������Ч

        //��������Ϊ��һ���е�����ң������ӱ��׶Σ�������
        if (currentPlayer == firstLandlordPlayer)
        {
            if (textRandom=="������")
            {
                CancelInvoke("GradLandlordOver"); //ȡ��textRandom=="������"�ĵ���
            }

            //Debug.Log("��������ť��������Ϊ��һ���е������");
            GradLandlordOver(); //���������
            currentPlayer = (currentPlayer + 1) % 3; //������һ�����
            Invoke("StartGameControl", 2f); //��������GameControl()��������һ�����ȴ�����������ϣ�
        }
        //����Ҳ��ǵ�һ���е��������
        else
        {
            //Debug.Log("��������ť�������Ҳ��ǵ�һ���е������");
            //����text�еĲ���
            if (textRandom != "������")
            {
                currentDouble *= 2;  //����������������2
                canGradLandlordPlayer.Add(currentPlayer);  //�������ʸ���textRandom��λ���Ѿ��Ƴ��������Ҫ����ӻ���
            }

            gamePlayManager.doubleText.text = "��" + currentDouble.ToString();   //������ʾ
            currentPlayer = (currentPlayer + 1) % 3; //������һ�����
            gamePlayManager.GameControl();  //ִ����һ��
        }
    }
    /// <summary>
    /// ����������ť�����¼�
    /// </summary>
    private void NotGradLandlordButtonOnClick()
    {
        //Debug.Log("�������������ť");
        afterChooseText[currentPlayer].text = "����";  //ѡ��֮����ı���ʾ
        MusicControl.Instance.PLaySound(MusicControl.SOUND.NOT_GRAD_LANDLORD_SOUND); //���š��е�������Ч
        if (textRandom=="������")
        {
            if(currentPlayer==firstLandlordPlayer)
            {
                //landlordPlayer = -1; //��textRandom����ӵĸ����Ϊ��������
                CancelInvoke("GradLandlordOver"); //ȡ��textRandom�е���ʱ���õ�Invoke()
            }
            currentDouble /= 2;  //����������������2��textRandom����2��
            canGradLandlordPlayer.Remove(currentPlayer);  //�Ƴ��������ʸ�
        }
        ListenEventCommonBehavior();
    }
    /// <summary>
    /// �ӱ���ť�����¼�
    /// </summary>
    private void DoubleButtonOnClick()
    {
        afterChooseText[currentPlayer].text = "�ӱ�";
        MusicControl.Instance.PLaySound(MusicControl.SOUND.DOUBLE_SOUND); //���š��ӱ�����Ч
        if (textRandom=="���ӱ�")
        {
            currentDouble *= 2; //�����ӱ�
        }
        GameObject.Destroy(thirdPlayerButtonGO); //ɾ����3����ť
        gamePlayManager.doubleText.text = "��" + currentDouble.ToString();   //������ʾ
        ListenEventCommonBehavior();
    }
    /// <summary>
    /// ���ӱ���ť�����¼�
    /// </summary>
    private void NotDoubleButtonOnClick()
    {
        afterChooseText[currentPlayer].text = "���ӱ�";
        MusicControl.Instance.PLaySound(MusicControl.SOUND.NOT_DOUBLE_SOUND); //���š����ӱ�����Ч

        if (textRandom=="�ӱ�")
        {
            currentDouble /= 2; //��textRandom����Ԥ�ӱ���2
        }
        GameObject.Destroy(thirdPlayerButtonGO); //ɾ����3����ť
        ListenEventCommonBehavior();
    }
    /// <summary>
    /// �����ӱ������¼�
    /// </summary>
    private void SuperDoubleButtonOnClick()
    {
        afterChooseText[currentPlayer].text = "�����ӱ�";
        if(textRandom=="�ӱ�")
        {
            currentDouble /= 2;//��textRandom����Ԥ�ӱ���2
        }
        currentDouble *= 4; //�����ӱ�
        GameObject.Destroy(thirdPlayerButtonGO); //ɾ����3����ť
        gamePlayManager.doubleText.text = "��" + currentDouble.ToString();   //������ʾ
        ListenEventCommonBehavior();
    }
    /// <summary>
    /// ���ư�ť�����¼�
    /// </summary>
    private void OutCardButtonOnClick()
    {
        //��ǰ�����ѡ���Ʋ��Ϸ�
        if (!outCardClass.IsCanOutCards(outCardClass.playerOnClickPreOutCardsList,currentPlayer))
        {
            //��Ҫ����������λ��
            foreach (GameObject item in outCardClass.playerOnClickPreOutCardsList)
            {
                item.GetComponent<RectTransform>().localPosition -= new Vector3(0f, 20f, 0f); //����Ҫ�����Ƶ�λ��
                item.GetComponent<RectTransform>().Find("Front").GetComponent<CardOnClickClass>().isSelect = false; //�ÿ�������Ϊδѡ��
            }
            outCardClass.playerOnClickPreOutCardsList.Clear(); //�����ѡ�Ŀ���
            return;
        }

        CountdownManager.Instance.StopIEnum(); //ֹͣЭ��

        //������Ұ�ť
        playerButtonGO.SetActive(false);
        noPlayerButtonGO.SetActive(false);

        outCardClass.outCardsList.AddRange(outCardClass.playerOnClickPreOutCardsList); //�����Ԥ���Ƽ����������ƿ�����
        outCardClass.playerOnClickPreOutCardsList.Clear(); //��յ��Ԥ���Ƽ���
        outCardClass.playerPreOutCardsList.Clear(); //�������Ƽ���

        //������ҳ��Ƹ�������
        OutCardOutAuxiliary();
    }
    /// <summary>
    /// ��Ҫ��ť�����¼�
    /// </summary>
    private void CanNotButtonOnClick()
    {
        //�����ǰ��ұ���Ҫ���ƣ���������Ҫ����ť��Ч
        if (outCardClass.lastOutCardType == -1 || outCardClass.lastOutCardPlayer == currentPlayer)
        {
            return;
        }
        CountdownManager.Instance.StopIEnum(); //ֹͣЭ��
        PlayerOutCardCanNotAuxiliary(); //������ҡ���Ҫ����������
    }
    /// <summary>
    /// �����¼���ͬ��Ϊ
    /// </summary>
    private void ListenEventCommonBehavior()
    {
        CountdownManager.Instance.StopIEnum();  //ֹͣЭ��
        currentPlayer = (currentPlayer + 1) % 3;  //��ǰ�������Ϊ��һ�����
        GameObject.Destroy(playerButtonGO);  //������ҵ��yes��ť
        GameObject.Destroy(noPlayerButtonGO);  //������ҵ��no��ť
        gamePlayManager.GameControl();  //ִ����һ��
    }

    /// <summary>
    /// ��Ϸ������������һ�Ρ���ť����¼�
    /// </summary>
    private void GameOverOnceAgainButtonOnClick()
    {
        //�����ǰ��һ��ֶ����㣬�򲥷���ʾ
        if (DATA.Instance.GetData()<=0)
        {
            BeginGame beginGame = thisGORT.GetComponent<BeginGame>(); //�ű�BeginGame����ʱ����
            //�����ǰ���ڲ��ŵ���ʾ����������3��
            if (beginGame.currentAnimPlayingCount<=3)
            {
                beginGame.SetJoyBeanRemindTextAnim();
            }
            return;
        }

        gameOverPanelRT.gameObject.SetActive(false); //������Ϸ����panel
        gamePlayManager.doubleGO.SetActive(false); //���üӱ�
        outCardClass.SetOutCardsSitsInHierarchy(CardManager.instance.cardsList); //�������п�����Hierarchy����е�λ��

        //ɾ����Ұ�ť
        Destroy(playerButtonGO);
        Destroy(noPlayerButtonGO);
        Destroy(thirdPlayerButtonGO);

        landlordGO.SetActive(false); //���õ�����־

        //�Ƴ����Ƶ���ű�
        foreach (GameObject item in CardManager.instance.cardsList)
        {
            CardOnClickClass cardOnClickClass = item.GetComponent<RectTransform>().Find("Front").GetComponent<CardOnClickClass>(); //���Ҹÿ���ǰ���RT����
            if (cardOnClickClass!=null)
            {
                Destroy(cardOnClickClass); //�Ƴ����
            }
        }

        outCardClass.Init(); //�������ʼ��

        MusicControl.Instance.isPlayingBeginGameMusic02 = false; //��������02����״̬����Ϊfalse

        gamePlayManager.currentGameStage = GamePlayManager.GAMESTAGE.CALL_LANDLORD; //���õ�ǰ��Ϸ�׶�Ϊ�е����׶�
        CountdownManager.Instance.timeCountGO.SetActive(false); //���õ���ʱ

        CardManager.instance.rememberCardDevice.ChangeOnClick(true); //����������Ϊ�ر�״̬

        GameReset(); //��Ϸ���ã�Player�ű�������ʼ���������ط��䡢��ʼ��Ϸ��
    }
    /// <summary>
    /// ��Ϸ���������˳���Ϸ����ť����¼�
    /// </summary>
    private void GameOverExitButtonOnClick()
    {
        //UnityEditor.EditorApplication.isPlaying = false; //��״̬����Ϊfalse
        Application.Quit(); //�˳���Ϸ
    }
}
