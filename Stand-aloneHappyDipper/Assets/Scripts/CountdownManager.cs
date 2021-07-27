using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownManager : MonoBehaviour
{
    public GameObject timeCountGO;
    //private RectTransform parentRT;
    public RectTransform countdownRT;  //����ʱRectTransform���
    private Text countdownText;  //����ʱʱ��Text
    Vector3[] countdownPos;  //����ʱλ��
    public int countdownTime = 5;  //����ʱʱ��

    private GamePlayManager gamePlayManager;  //�ű�GamePlayManager������
    public Coroutine countdownIEnum;  //�洢Э������

    /// <summary>
    /// �����ýű�����
    /// </summary>
    public static CountdownManager Instance { get; private set; }
    /// <summary>
    /// ��ֵ�ű�����
    /// </summary>
    private void Awake()
    {
        Instance = this;
        //��������ʱ
        timeCountGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/TimeCount"), this.GetComponent<RectTransform>()); //��������ʱ
        timeCountGO.SetActive(false);  //����ʱ��ʼΪ����״̬
    }
    private void Start()
    {
        //Debug.Log("����CountdownManager��Start");
        //��ȡ����ʱRT���
        countdownRT = timeCountGO.GetComponent<RectTransform>();
        //��ȡ����ʱ�������
        countdownText = countdownRT.Find("Text").GetComponent<Text>();
        //�ֱ�洢���С�����ҵ���ʱ��λ��
        countdownPos = new Vector3[] {
            new Vector3(440f, -346f, 0f),
            new Vector3(0f, 380f, 0f),
            new Vector3(-440f, -346f, 0f)
        };
        //��ȡGamePlayManager�ű�������
        gamePlayManager = this.GetComponent<GamePlayManager>();
    }

    /// <summary>
    /// ���õ���ʱ����������ʼ����ʱ
    /// </summary>
    public void SetCountdown(int cPos,int stopCountdownTime)
    {
        //Debug.Log("���뵹��ʱ");
        //Debug.Log("����ʱcurrentStage="+gamePlayManager.currentGameStage);

        //���õ���ʱê��
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
        countdownRT.anchoredPosition3D = countdownPos[cPos];  //���õ���ʱ�����λ��

        countdownIEnum=StartCoroutine(StartCountdown(stopCountdownTime));   //��ʼ����ʱ
    }
    /// <summary>
    /// ֹͣЭ��
    /// </summary>
    public void StopIEnum()
    {
        StopCoroutine(countdownIEnum);
    }
    /// <summary>
    /// Э�̡�����ʼ����ʱ
    /// stopCountdownTimeΪ����ʱ�������ƣ�һ��Ϊ-1����AI���õ���ʱ������
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
        //Э�̽�������
        //�ǳ��ƽ׶�
        if (gamePlayManager.currentGameStage != GamePlayManager.GAMESTAGE.OUT_CARD)
        {
            gamePlayManager.IsCountdownOver();  //����ʱ������������һ��
        }
        //���ƽ׶�
        else
        {
            Player.Instance.CountDownOverOutCardAuxiliary(); //���õ���ʱ�������Ƹ�������
        }
    }
}
