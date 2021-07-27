using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardOnClickClass : MonoBehaviour
{
    private OutCardClass outcard; //�ű�OutCard������
    private RectTransform thisRT; //���������RT
    private RectTransform parentRT; //�������游�����RT��������RT��
    /// <summary>
    /// ������ʼ��
    /// </summary>
    private void Start()
    {
        outcard = Player.Instance.outCardClass; //��ȡ�ű�OutCard������
        thisRT = this.GetComponent<RectTransform>(); //��������RT
        parentRT = (RectTransform)thisRT.parent; //���Ƶ�RT
        this.GetComponent<Button>().onClick.AddListener(CardOnClick); //��Ӽ����¼�
    }
    //�����Ƿ�����Ĭ��δ�����
    public bool isSelect = false;
    /// <summary>
    /// ���Ƽ����¼������1��
    /// </summary>
    public void CardOnClick()
    {
        isSelect = !isSelect;
        if(isSelect)
        {
            parentRT.localPosition+=new Vector3(0f,20f,0f); //���������ƶ�20f

            outcard.playerOnClickPreOutCardsList.Add(parentRT.gameObject); //��ѡ�еĿ��ƴ�����Ԥ���Ƽ�����
        }
        else
        {
            parentRT.localPosition -= new Vector3(0f, 20f, 0f); //���������ƶ�20f

            outcard.playerOnClickPreOutCardsList.Remove(parentRT.gameObject); //��ѡ�еĿ�Ƭ�Ƴ����Ԥ���Ƽ�����
        }
    }
}
