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

    private RectTransform thisParentRT; //�ýű����������RT����
    private GamePlayManager gamePlayManager; //�ű�GamePlayManager������

    public Dictionary<GameObject, ValueType> cardsVDict = new Dictionary<GameObject, ValueType>();  //�洢���п����Լ����ǵ�Value
    Dictionary<GameObject, PictureType> cardsPDict = new Dictionary<GameObject, PictureType>();   //�洢���п����Լ����ǵ�Picture
    
    public List<GameObject> cardsList = new List<GameObject>();   //�洢���п���
    public Dictionary<ValueType, int> cardsListValueCount = new Dictionary<ValueType, int>();  //�洢���п��Ƶĸ���ֵ����

    public List<GameObject> cardsLeftPlayer = new List<GameObject>();   //���������ֿ���
    public Dictionary<ValueType, int> cardsLeftPlayerValueCount = new Dictionary<ValueType, int>();   //�洢�����ҿ��Ƶĸ���ֵ����

    public List<GameObject> cardsPlayer = new List<GameObject>();   //������ֿ���
    public Dictionary<ValueType, int> cardsPlayerValueCount = new Dictionary<ValueType, int>();   //�洢��ҿ��Ƶĸ���ֵ����

    public List<GameObject> cardsRightPlayer = new List<GameObject>();   //�ұ�������ֿ���
    public Dictionary<ValueType, int> cardsRightPlayerValueCount = new Dictionary<ValueType, int>();   //�洢�ұ���ҿ��Ƶĸ���ֵ����

    List<GameObject> cardsLandlord = new List<GameObject>();   //������
    public List<GameObject> cardsLandlordCopy=new List<GameObject>();  //���Ƶ����Ʒ�������Ļ�Ϸ�
    public Dictionary<ValueType, int> cardsLandlordValueCount = new Dictionary<ValueType, int>();   //�洢�������Ƶĸ���ֵ����

    public RememberCardValueCount rememberCardValueCount;   //���㿨�Ƹ���ֵ������������
    public RememberCardDevice rememberCardDevice; //�������ű���������

    public GameObject leftPlayerRemainCardsCountGO; //������ʣ�࿨����
    public GameObject rightPlayerRemainCardsCountGO; //�ұ����ʣ�࿨����

    private Vector3[] rememberCardDevicePos; //�洢������λ�ã�0��>���⣬1��>���ڣ�

    public RectTransform playerJoyBeansRT; //��һ��ֶ���RT

    /// <summary>
    /// ����Value����
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
    /// ����picture����
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
    /// ����ű�����
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// �����п��ƶ����뼯��cardsList��
    /// </summary>
    /// <param name="cards"></param>
    private void Start()
    {
        thisParentRT = (RectTransform)this.GetComponent<Transform>().parent; //���ø�����RT���õ���ʱ����

        rememberCardValueCount = this.GetComponent<RememberCardValueCount>(); //��ȡ������ֵ������������
        //Debug.Log(thisParentRT.Find("RememberCardDevice"));
        rememberCardDevice = thisParentRT.Find("RememberCardDevice").GetComponent<RememberCardDevice>(); //��ȡ�������ű�������

        gamePlayManager=thisParentRT.AddComponent<GamePlayManager>(); //������BG����ӽű�GamePlayManager���������ȡ������

        //������ʣ�࿨������Ĭ�Ͻ��ã�
        leftPlayerRemainCardsCountGO = (GameObject)Instantiate(Resources.Load("Prefabs/Cards/RemainCardsCountPre"), this.transform);
        leftPlayerRemainCardsCountGO.name = "leftPlayerRemainCardsCountGO";
        //leftPlayerRemainCardsCountGO.GetComponent<RectTransform>().localPosition = new Vector3(-880f, 35f, 0f);
        leftPlayerRemainCardsCountGO.SetActive(false);
        //�ұ����ʣ�࿨������Ĭ�Ͻ��ã�
        rightPlayerRemainCardsCountGO = (GameObject)Instantiate(Resources.Load("Prefabs/Cards/RemainCardsCountPre"), this.transform);
        leftPlayerRemainCardsCountGO.name = "rightPlayerRemainCardsCountGO";
        //rightPlayerRemainCardsCountGO.GetComponent<RectTransform>().localPosition = new Vector3(880f, 35f, 0f);
        rightPlayerRemainCardsCountGO.SetActive(false);

        GainAllCard(cardsList, cardsVDict, cardsPDict);   //�����п��ƴ��뼯��cardsList��
        cardsListValueCount = rememberCardValueCount.GetCardsValueCount(cardsList);   //��ȡ���п����и���ֵ������

        rememberCardDevicePos = new Vector3[2] { new Vector3(5000f, 5000f, 0f) , new Vector3(99f, -101f, 0f) }; //������λ�ó�ʼ��

        playerJoyBeansRT = (RectTransform)thisParentRT.Find("PlayerJoyBeans"); //��ȡ��һ��ֶ���RT

        GameCardInit();  //��Ϸ���Ƴ�ʼ��
    }
    /// <summary>
    /// ��Ϸ���Ƴ�ʼ��
    /// ���п�������
    /// ���俨��
    /// ��ȡ�����ҵ�����ҿ��Ƹ���ֵ������
    /// ��������ҿ�������
    /// ���Ʒ��䶯��
    /// </summary>
    public void GameCardInit()
    {
        //������һ��ֶ�
        playerJoyBeansRT.parent = thisParentRT.Find("Player"); //���ø�����
        AdaptiveScript.Instance.SetAnchor(playerJoyBeansRT.gameObject,AdaptiveScript.ANCHOR.MIDDLE); //������һ��ֶ�ê��λ��
        playerJoyBeansRT.localPosition = new Vector3(118f, -64f, 0f); //����λ��
        playerJoyBeansRT.localScale = new Vector3(0.3f, 0.3f, 1f); //��С

        CardsDistribution(CardsOutOfOrder(cardsList));     //�������Ŀ��ƽ��з���

        cardsLeftPlayerValueCount = rememberCardValueCount.GetCardsValueCount(cardsLeftPlayer);   //��ȡ�����ҿ����и���ֵ������
        cardsPlayerValueCount = rememberCardValueCount.GetCardsValueCount(cardsPlayer);   //��ȡ��ҿ����и���ֵ������
        cardsRightPlayerValueCount = rememberCardValueCount.GetCardsValueCount(cardsRightPlayer);   //��ȡ�ұ���ҿ����и���ֵ������
        cardsLandlordValueCount = rememberCardValueCount.GetCardsValueCount(cardsLandlord);   //��ȡ���������и���ֵ������

        //�����ҡ���ҡ��ұ���ҵĿ��ƽ�������
        CardsSort(cardsLeftPlayer, cardsLeftPlayerValueCount);
        CardsSort(cardsPlayer, cardsPlayerValueCount);
        CardsSort(cardsRightPlayer, cardsRightPlayerValueCount);

        rememberCardDevice.SetRememberCardDeviceValue(cardsListValueCount); //���ü������и���ֵ
        rememberCardDevice.GetComponent<RectTransform>().localPosition = rememberCardDevicePos[0]; //����������λ�����õ���Ļ��

        //���Ʒ��䶯����������������ʼ��Ϸ��
        CardsMove();
    }
    /// <summary>
    /// ��������
    /// </summary>
    public void GameCardReset()
    {
        for(int i=0;i<cardsList.Count;i++)
        {
            RectTransform cardsListRT = cardsList[i].GetComponent<RectTransform>();
            cardsListRT.Find("Back").gameObject.SetActive(true);  //�������п��Ƶı���
            cardsListRT.position = this.transform.position; //���п��ƾ����õ���ʼλ��
            cardsListRT.rotation = Quaternion.Euler(0f, 0f, 0f);  //ȡ�����Ƶ���ת
        }

        if (cardsLandlordCopy.Count!=0)
        {
            for (int i = 0;i<cardsLandlordCopy.Count;i++)
            {
                Destroy(cardsLandlordCopy[i]); //ɾ�����Ƶ����Ƶ��п���
            }
            cardsLandlordCopy.Clear(); //��ո��Ƶ����Ƽ���
        }

        ///�������������洢�Ŀ���Ԫ��
        //������
        cardsLeftPlayer.Clear();  //��������ҿ��Ƽ�������Ԫ��
        cardsLeftPlayerValueCount.Clear();  //��������ҿ���ֵ�������ֵ�����Ԫ��
        //�м����
        cardsPlayer.Clear();
        cardsPlayerValueCount.Clear();
        //�ұ����
        cardsRightPlayer.Clear();
        cardsRightPlayerValueCount.Clear();
        //������
        cardsLandlord.Clear();
        cardsLandlordValueCount.Clear();
        //��Ϸ���Ƴ�ʼ��
        GameCardInit();
    }
    /// <summary>
    /// ���ŵ�����
    /// </summary>
    public void IssueLandlordCards(int landlordPlayer)
    {
        //Debug.Log("���ŵ����ƣ���������ǣ�"+landlordPlayer);
        //������ʱ��������
        DG.Tweening.Sequence quence = DOTween.Sequence();
        //���ƴ�����ƶ����յ�����ʱ��
        float duration = 0.2f;
        //����֮���ƫ����
        float offset = 50f;

        //�����������ӵ����ơ�ֵ����
        switch (landlordPlayer)
        {
            case 0:
                //��ӵ����Ƹ�������ҵļ�����
                for (int i = 0; i < cardsLandlord.Count; i++)
                {
                    cardsLandlord[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(true); //���ñ���
                    cardsLeftPlayer.Add(cardsLandlord[i]);
                }
                //��ӵ�����ֵ������������ҵ�ֵ������
                foreach (KeyValuePair<ValueType, int> item in cardsLandlordValueCount)
                {
                    if (cardsLeftPlayerValueCount.ContainsKey(item.Key))
                    {
                        cardsLeftPlayerValueCount[item.Key] += cardsLandlordValueCount[item.Key];  //�����ֵͬ������
                    }
                    else
                    {
                        cardsLeftPlayerValueCount.Add(item.Key,item.Value);
                    }
                }
                //��������
                CardsSort(cardsLeftPlayer,cardsLeftPlayerValueCount);    
                //���ÿ���λ��
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
                //��ӵ����Ƹ�������ҵļ�����
                for (int i = 0; i < cardsLandlord.Count; i++)
                {
                    cardsLandlord[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(false); //���ñ���
                    cardsPlayer.Add(cardsLandlord[i]);
                }
                //��ӵ�����ֵ������������ҵ�ֵ������
                foreach (KeyValuePair<ValueType, int> item in cardsLandlordValueCount)
                {
                    if (cardsPlayerValueCount.ContainsKey(item.Key))
                    {
                        cardsPlayerValueCount[item.Key] += cardsLandlordValueCount[item.Key];  //�����ֵͬ������
                    }
                    else
                    {
                        cardsPlayerValueCount.Add(item.Key, item.Value);
                    }
                }
                //��������
                CardsSort(cardsPlayer, cardsPlayerValueCount);
                Vector3 cardsPlayerTargetPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.BUTTOM, new Vector3(0, 140, 0)) + new Vector3(-(cardsPlayer.Count / 2f) * offset, 0f, 0f);
                //���ÿ���λ��
                RectTransform cardsPlayerRT;
                for (int i = 0; i < cardsPlayer.Count; i++)
                {
                    cardsPlayerRT = cardsPlayer[i].GetComponent<RectTransform>();
                    
                    cardsPlayerRT.localPosition = cardsPlayerTargetPos+new Vector3(i * offset, 0f, 0f);
                    //cardsPlayerRT.localPosition = new Vector3(-540f + (i * (offset + 10f)), -400f, 0f);
                }
                break;
            case 2:
                //��ӵ����Ƹ�������ҵļ�����
                for (int i = 0; i < cardsLandlord.Count; i++)
                {
                    cardsLandlord[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(true); //���ñ���
                    cardsRightPlayer.Add(cardsLandlord[i]);
                }
                //��ӵ�����ֵ������������ҵ�ֵ������
                foreach (KeyValuePair<ValueType, int> item in cardsLandlordValueCount)
                {
                    //cardsRightPlayerValueCount[item.Key] += cardsLandlordValueCount[item.Key];  //�����ֵͬ������

                    if (cardsRightPlayerValueCount.ContainsKey(item.Key))
                    {
                        cardsRightPlayerValueCount[item.Key] += cardsLandlordValueCount[item.Key];  //�����ֵͬ������
                    }
                    else
                    {
                        cardsRightPlayerValueCount.Add(item.Key, item.Value);
                    }
                }
                //��������
                CardsSort(cardsRightPlayer, cardsRightPlayerValueCount);
                //���ÿ���λ��
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

        //���õ����ƶ���
        for (int i = 0; i < cardsLandlord.Count; i++)
        {
            cardsLandlordCopy.Add(Instantiate(cardsLandlord[i], thisParentRT));  //���Ƶ�����
            cardsLandlordCopy[i].GetComponent<RectTransform>().Find("Back").gameObject.SetActive(true);  //���ø��Ƶ����Ƶı���
            AdaptiveScript.Instance.SetAnchor(cardsLandlordCopy[i], AdaptiveScript.ANCHOR.TOP); //���ø��Ƶ�������ê��
            cardsLandlordCopy[i].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-200f + (i * 200f), -140f, 0f);  //���������Ƶĵ����Ƶ�λ��
            //Debug.Log("�����ƣ�"+cardsLandlord[i]);
            //��ȡ���Ƶ�RT�������
            RectTransform cardsLandlordCopyRT = cardsLandlordCopy[i].GetComponent<RectTransform>();
            //�����м�RT���ã�����OnComplete()
            RectTransform itemRT = cardsLandlordCopyRT;
            //������ת��90��λ�ã���ת�����󣬽���Back��ʾ��������
            quence.Append(cardsLandlordCopyRT.DOBlendableLocalRotateBy(new Vector3(0f, 90f, 0f), duration)
                .OnComplete(() => { itemRT.Find("Back").gameObject.SetActive(false); })
                );
            //���Ʒ�����ת��0��
            quence.Append(cardsLandlordCopyRT.DOBlendableLocalRotateBy(new Vector3(0f, -90f, 0f), duration));
        }

        rememberCardDevice.GetComponent<RectTransform>().anchoredPosition3D = rememberCardDevicePos[1]; //����������λ�����õ���Ļ��
    }

    /// <summary>
    /// ���Ʒ��䶯��
    /// </summary>
    //private bool isMoveComplete = false;   //�����жϿ����Ƿ񷢷����
    //public bool IsMoveComplete
    //{
    //    get { return isMoveComplete; }
    //}
    private void CardsMove()
    {
        MusicControl.Instance.PLaySound(MusicControl.SOUND.BEGIN_GAME_MUSIC_01); //���ſ�ʼ��Ϸ��������
        MusicControl.Instance.PLaySound(MusicControl.SOUND.DEAL_CARDS_SOUND); //���ſ��Ʒ�����Ч

        float duration = 0.02f;  //���ƴ�����ƶ����յ�����ʱ��
        float offset = 60f;  //����֮���ƫ����

        RectTransform cardsLeftRT;  //������RectTransform�������
        Vector3 leftPlayerStartCardPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.LEFT, new Vector3(80f, 35f, 0f)); //���������ҿ��Ƶ���ʼλ��

        RectTransform cardsPlayerRT;  //�м����RectTransform�������
        Vector3 playerStartCardPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.BUTTOM,new Vector3(0,140,0)) + new Vector3(-(cardsPlayer.Count/2)*offset,0f,0f); //�����м���ҿ��Ƶ���ʼλ��

        RectTransform cardsRightRT;  //�ұ����RectTransform�������
        Vector3 rightPlayerStartCardPos = AdaptiveScript.Instance.SetPositionByScreenOffset(AdaptiveScript.SCREEN.RIGHT, new Vector3(-80f, 35f, 0f)); //�����ұ���ҿ��Ƶ���ʼλ��

        //Debug.LogFormat("leftPlayerStartCardPos={0},playerStartCardPos={1},rightPlayerStartCardPos={2}", leftPlayerStartCardPos, playerStartCardPos, rightPlayerStartCardPos);

        DG.Tweening.Sequence quence = DOTween.Sequence();    //���嶯������

        //�������С�����ҵĿ���
        for (int i = 0; i < cardsLeftPlayer.Count; i++)
        {
            //������17��ѭ��������Ϊ��������ҷ��俨��
            cardsLeftRT = cardsLeftPlayer[i].GetComponent<RectTransform>();  //���������ҵ�i�ſ��Ƶ�RectTransform�������
            cardsPlayerRT = cardsPlayer[i].GetComponent<RectTransform>();  //�����м���ҵ�i�ſ��Ƶ�RectTransform�������
            cardsRightRT = cardsRightPlayer[i].GetComponent<RectTransform>();  //�����ұ���ҵ�i�ſ��Ƶ�RectTransform�������

            //�����ҿ�������
            quence.Append(cardsLeftRT.DOLocalMove(leftPlayerStartCardPos, duration));  //���������붯��������
            //quence.Append(cardsLeftRT.DOLocalMove(new Vector3(-880f, 35f, 0f), duration));  //���������붯��������
            //quence.Append(cardsLeftRT.DOLocalMove(new Vector3(-830f, 410f - (i * offset), 0f), duration));  //���������붯��������
            //RectTransform itemLeftRT=cardsLeftRT;  //�����м��RectTransform���ã����ڶ���ί��
            //quence.Join(itemLeftRT.DOLocalRotate(new Vector3(0f, 0f, -90f), duration).OnComplete(() => { itemLeftRT.Find("Back").gameObject.SetActive(false); }));  //��������ת������ӵ����������ƶ�����һ��ִ�У�������ִ����Ϻ���á�Back��

            //�м���ҿ�������
            RectTransform itemPlayerRT = cardsPlayerRT;
            quence.Append(itemPlayerRT.DOLocalMove(playerStartCardPos+new Vector3(i * offset, 0f, 0f), duration).OnComplete(() => { itemPlayerRT.Find("Back").gameObject.SetActive(false); }));
            //quence.Append(itemPlayerRT.DOLocalMove(new Vector3(-540f + (i * (offset + 10f)), -400f, 0f), duration).OnComplete(() => { itemPlayerRT.Find("Back").gameObject.SetActive(false); }));

            //�ұ���ҿ�������
            RectTransform itemRightRT = cardsRightRT;
            quence.Append(cardsRightRT.DOLocalMove(rightPlayerStartCardPos, duration));
            //quence.Append(cardsRightRT.DOLocalMove(new Vector3(880f, 35f, 0f), duration));
            //quence.Append(cardsRightRT.DOLocalMove(new Vector3(830f, -390f + (i * offset), 0f), duration));
            //quence.Join(itemRightRT.DOLocalRotate(new Vector3(0f, 0f, 90f), duration).OnComplete(() => { itemRightRT.Find("Back").gameObject.SetActive(false); }));

            //ÿ4�ſ���ͣ��һ��
            //if (i % 1 == 0)
            //{
            //    quence.AppendInterval(0.1f);   //�ӳ�ͣ�ٸ�
            //}
        }

        //�������ʣ�࿨����
        RectTransform leftPlayerRemainCardsCountGORT = leftPlayerRemainCardsCountGO.GetComponent<RectTransform>();
        leftPlayerRemainCardsCountGORT.localPosition = leftPlayerStartCardPos;
        leftPlayerRemainCardsCountGORT.SetAsLastSibling();
        leftPlayerRemainCardsCountGO.GetComponent<Text>().text = cardsLeftPlayer.Count.ToString();
        leftPlayerRemainCardsCountGO.SetActive(true);

        RectTransform rightPlayerRemainCardsCountGORT = rightPlayerRemainCardsCountGO.GetComponent<RectTransform>();
        rightPlayerRemainCardsCountGORT.localPosition = rightPlayerStartCardPos; //����λ��
        rightPlayerRemainCardsCountGORT.SetAsLastSibling(); //������Hierarchy����е�λ��
        rightPlayerRemainCardsCountGO.GetComponent<Text>().text = cardsRightPlayer.Count.ToString(); //�ı���ʾ
        rightPlayerRemainCardsCountGO.SetActive(true); //����

        //���õ�������
        quence.AppendInterval(0.1f);  //�����ӳ٣�������������ҿ���δ������ϣ��������Ƽ��ȷ���
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
                            MusicControl.Instance.StopOnceShotMusic(); //��ͣ������Ч
                            gamePlayManager.Init(); //��ʼ��GamePlayManager ����ʼ��Ϸ
                              }
                        ));
                    break;
            }
            //ǰ���ŵ������ƶ�
            //if(i<cardsLandlord.Count-1)
            //{
            //    quence.Append(itemLandlordRT.DOLocalMove(new Vector3(-200f + (i * 200f), 140f, 0f), 20*duration));
            //}
            ////���һ�ŵ������ƶ�
            //else
            //{
            //    quence.Append(itemLandlordRT.DOLocalMove(new Vector3(-200f + (i * 200f), 140f, 0f), 20*duration).OnComplete(
            //        ()=> {
            //                MusicControl.Instance.StopOnceShotMusic(); //��ͣ������Ч
            //                gamePlayManager.Init(); //��ʼ��GamePlayManager ����ʼ��Ϸ
            //        }
            //        ));
            //}
        }
    }
    /// <summary>
    /// Э�̣��������Ʒ��䶯��
    /// </summary>
    /// <param name="rtf">����RT���</param>
    /// <param name="waitTime">�ӳ�ִ��ʱ��</param>
    /// <returns></returns>
    //IEnumerator RTWaitIE(RectTransform rtf, float waitTime)
    //{
    //    yield return new WaitForSeconds(waitTime);
    //    rtf.Find("Back").gameObject.SetActive(false);
    //}
    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="cards"></param>
    private void CardsSort(List<GameObject> cards,Dictionary<ValueType,int> cardsValueCountDict)
    {
        ///******ͨ���ȶԿ��Ƶ�valueֵ��������******///
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
        ///****�ڿ��Ƶ�valueֵ��������֮���ٶ���ͬvalue�Ŀ���ͨ���ȶ�picture�ٽ�������****///
        GameObject pictureMinCard;
        foreach(KeyValuePair<ValueType,int> valueCountItem in cardsValueCountDict)
        {
            //����cardsValueCountDict�������β鿴cards����ֵͬ�Ŀ�����
            //�ж���ֵͬ�Ŀ������Ƿ����1����ѡ�����������1����ֵͬ����������
            if (valueCountItem.Value > 1)
            {
                //ͨ��iѭ��ȷ����ֵͬ�ĵ�һ��λ��
                for (int i=0;i<cards.Count;i++)
                {
                    if(cardsVDict[cards[i]]==valueCountItem.Key)
                    {
                        //ͨ��j��kѭ������ֵͬ���бȶ�picture����
                        //i+valueCountItem������ֵͬ�����һ��λ��+1�����Բ���<=��
                        for (int j=i;j<i+valueCountItem.Value;j++)
                        {
                            pictureMinCard = cards[j];
                            for (int k=j;k<i+valueCountItem.Value;k++)
                            {
                                //�ȶ�picture
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
    /// ���Ʒ���
    /// </summary>
    /// <param name="cardsOutOfOrder">���Ƽ���</param>
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

            //����
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
    /// ���п�������
    /// </summary>
    /// <param name="cardsList"></param>
    /// <returns>��������֮��Ŀ��Ƽ���</returns>
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
    /// �����п��ƾ����뼯���У������п��Ƽ�Valueֵ�����ֵ���
    /// </summary>
    /// <param name="cards"></param>
    private void GainAllCard(List<GameObject> cards,Dictionary<GameObject,ValueType> cardsVDict,Dictionary<GameObject,PictureType> cardsPDict)
    {
        UnityEngine.Object[] cardAtlas = Resources.LoadAll("Cards");  //��ȡͼ��
        int cardsCount = 0;
        //�������3~2����
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
        //С��
        cards.Add(GainOneCard(ValueType.BLACKJOKER, PictureType.BLACK_JOKER,cardAtlas)); //���Ƽ��������С������
        cardsVDict.Add(cards[cardsCount], ValueType.BLACKJOKER);  //����Value�ֵ������С������
        cardsPDict.Add(cards[cardsCount++], PictureType.BLACK_JOKER);  //����Picture�ֵ������С������
        //����
        cards.Add(GainOneCard(ValueType.REDJOKER,PictureType.RED_JOKER,cardAtlas));  
        cardsVDict.Add(cards[cardsCount],ValueType.REDJOKER);
        cardsPDict.Add(cards[cardsCount++], PictureType.RED_JOKER);
    }
    /// <summary>
    /// ���һ��ָ������
    /// </summary>
    /// <param name="valueItem"></param>
    /// <param name="pictureItem"></param>
    /// <param name="cardAtlas"></param>
    /// <returns></returns>
    private GameObject GainOneCard(ValueType valueItem,PictureType pictureItem,UnityEngine.Object[] cardAtlas)
    {
        GameObject card = Instantiate((GameObject)Resources.Load("Prefabs/Cards/card"), this.transform);  //����ʵ����
        card.name = valueItem.ToString() + "-" + pictureItem.ToString();  //���ÿ�������
        card.GetComponent<RectTransform>().position = this.transform.position;    //���ÿ�Ƭ��ʼλ��
        RectTransform cardValueRT = card.GetComponent<RectTransform>().Find("Front").Find("Value").GetComponent<RectTransform>();  //��ȡ����Value��RT����
        RectTransform cardPictureRT = card.GetComponent<RectTransform>().Find("Front").Find("Picture").GetComponent<RectTransform>();  //��ȡ���Ƶ�Picture��RT����
        SetCardFormat(cardValueRT, cardPictureRT, pictureItem);  //���ÿ���Value��Picture��λ��
        SetCardSprite(cardValueRT, cardPictureRT, valueItem, pictureItem, cardAtlas);  //���ÿ���Value��Picture��Sprite
        return card;  
    }
    /// <summary>
    /// ���ÿ��Ƶĸ�ʽ��value��picture��
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
    /// ���ÿ��Ƶ�Sprite��value��picture��
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

