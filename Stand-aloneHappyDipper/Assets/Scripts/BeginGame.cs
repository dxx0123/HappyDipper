using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BeginGame : MonoBehaviour
{
    private Transform bgRT; //BG��RT 
    private RectTransform beginGameButtonRT; //��ʼ��Ϸ��ť��RT
    private GameObject rememberCardGO; //������
    //private bool isAnimPlaying; //�жϻ��ֶ���ʾ�����Ƿ��ڲ���
    public int currentAnimPlayingCount; //�洢��ǰ���ڲ��ŵĻ��ֶ���ʾ��������
    private void Start()
    {
        bgRT =this.GetComponent<RectTransform>();
        beginGameButtonRT = (RectTransform)bgRT.Find("BeginGameButton");
        beginGameButtonRT.GetComponent<Button>().onClick.AddListener(BeginGameButtonOnClick); //��Ӱ�ť����¼�
        currentAnimPlayingCount = 0; //Ĭ�϶�������Ϊ0
    }
    
    /// <summary>
    /// ���ֶ���ʾ
    /// </summary>
    public void SetJoyBeanRemindTextAnim()
    {
        currentAnimPlayingCount++; //��ǰ���ڲ��ŵĶ�������+1
        GameObject joyBeanRemindText = (GameObject)Instantiate(Resources.Load("Prefabs/Game/JoyBeanRemindText"),bgRT); //ʵ�������ֶ���ʾGO
        RectTransform joyBeanRemindTextRT = joyBeanRemindText.GetComponent<RectTransform>(); //��ȡ���ֶ���ʾRT
        Text remindText = joyBeanRemindTextRT.GetComponent<Text>();
         
        Sequence quence = DOTween.Sequence(); //���嶯������
        float duration = 1.0f; //���嶯������ʱ��

        //����ǰ��ζ���
        quence.Append(joyBeanRemindTextRT.DOScale(new Vector3(1f,1f,1f), duration));
        quence.Join(remindText.DOFade(1, duration));
        quence.Join(joyBeanRemindTextRT.DOLocalMove(joyBeanRemindTextRT.localPosition + new Vector3(0f, 100f, 0f), duration).OnComplete(
                () => 
                    {
                        //���ź��ζ���
                        quence.Append(joyBeanRemindTextRT.DOScale(new Vector3(0f, 0f, 1f), duration));
                        quence.Join(remindText.DOFade(0,duration));
                        quence.Join(joyBeanRemindTextRT.DOLocalMove(joyBeanRemindTextRT.localPosition + new Vector3(0f, 100f, 0f), duration).OnComplete(
                                () => {
                                            Destroy(joyBeanRemindText); //�������Ž�����ɾ�����ֶ���ʾ
                                            currentAnimPlayingCount--; //��ǰ���ڲ��ŵĶ�������-1
                                }
                            ));
                    }
            ));
    }

    /// <summary>
    /// ��ʼ��Ϸ��ť��������¼�
    /// </summary>
    private void BeginGameButtonOnClick()
    {
        //���ֶ�����С�ڵ���0
        if (DATA.Instance.GetData()<=0)
        {
            //��ǰû�л��ֶ���ʾ�����ڲ���
            if (currentAnimPlayingCount<=3)
            {
                SetJoyBeanRemindTextAnim(); //���Ż��ֶ���ʾ����
            }
            return;
        }
        //����������
        rememberCardGO = (GameObject)Instantiate(Resources.Load("Prefabs/RememberCard/RememberCardDevice"), bgRT);
        rememberCardGO.name = "RememberCardDevice";

        //�������ƿ����壬��Ϊ���п��Ƶĸ�����
        //GameObject go = new GameObject("CardsListEmptyGO");
        GameObject go = (GameObject)Instantiate(Resources.Load("Prefabs/Cards/CardsListEmptyGO"),bgRT);
        go.AddComponent<CardManager>();
        go.transform.position = this.transform.parent.position;

        //�������
        CreatePlayer("LeftPlayer", true);
        CreatePlayer("Player");
        CreatePlayer("RightPlayer",false);
        
        beginGameButtonRT.gameObject.SetActive(false); //���ÿ�ʼ��Ϸ��ť

    }
    /// <summary>
    /// �����м����
    /// </summary>
    /// <param name="name">�������</param>
    private void CreatePlayer(string name)
    {
        //Ĭ�ϴ�����ҵ�ͼ��Ϊwoman��λ�������λ��
        GameObject playerGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Player"), bgRT);
        AdaptiveScript.Instance.SetAnchor(playerGO,AdaptiveScript.ANCHOR.LEFT_BUTTOM); //�����м����ê��

        RectTransform playerRT = playerGO.GetComponent<RectTransform>();
        playerRT.anchoredPosition3D = new Vector3(100f,90f,0f);
        //playerRT.localPosition = new Vector3(100f,90f,0f);

        playerGO.name = name; 
    }
    /// <summary>
    /// ������ߡ��ұ����
    /// </summary>
    /// <param name="name">�������</param>
    /// <param name="isLeftPlayer">�ж��������һ����ұ����</param>
    private void CreatePlayer(string name,bool isLeftPlayer)
    {
        //Ĭ�ϴ�����ҵ�ͼ��Ϊwoman
        GameObject playerGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Player"),bgRT);
        playerGO.name = name;
        //��ȡ���
        RectTransform playerRT = playerGO.GetComponent<RectTransform>();
        //�������ͼ����ӽű����AIPlayer
        //����������ͼ��
        int player = Random.Range(0, 2);
        if (player == 1)
        {
            playerGO.GetComponent<Image>().sprite = (Sprite)Resources.Load("UI/NPC/NPC_Man",typeof(Sprite));
        }
        //����������
        if (isLeftPlayer)
        {
            AdaptiveScript.Instance.SetAnchor(playerGO,AdaptiveScript.ANCHOR.LEFT_TOP); //����������ê��
            playerRT.anchoredPosition3D = new Vector3(240f, -390f, 0f);
            //playerRT.localPosition = new Vector3(240f, -390f, 0f);
        }
        //�����ұ����
        else
        {
            AdaptiveScript.Instance.SetAnchor(playerGO,AdaptiveScript.ANCHOR.RIGHT_TOP); //�����ұ����ê��
            playerRT.anchoredPosition3D = new Vector3(-240f, -390f, 0f);
            //playerRT.localPosition = new Vector3(-240f, -390f, 0f);
            playerRT.rotation = Quaternion.Euler(0f,180f,0f);
        }
    }
}
