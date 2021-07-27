using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DATA : MonoBehaviour
{
    public static DATA Instance; //���õ���

    private int playerJoyBeanInitCount = 1000; //������һ��ֶ���ʼ����

    void Awake()
    {
        DATA.Instance = this;

        //PlayerPrefs.DeleteKey("PlayerJoyBeanCount"); //ɾ��ָ����

        //�����ǰ���Ϊ�����û�������1000���ֶ�
        if (!PlayerPrefs.HasKey("PlayerJoyBeanCount"))
        {
            SaveData(playerJoyBeanInitCount);
        }

        //����
        //SaveData(1);

        GameObject.Find("PlayerJoyBeans").GetComponent<RectTransform>().Find("Count").GetComponent<Text>().text = GetData().ToString(); //��ʾ���ֶ�

        ////���ֶ���Ϊ0ʱ����ɫ������ʾ
        if (GetData() == 0)
        {
            GameObject.Find("PlayerJoyBeans").GetComponent<RectTransform>().Find("Count").GetComponent<Text>().color = Color.red;
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="num">Ҫ����Ļ��ֶ���</param>
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("PlayerJoyBeanCount",num);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ��ȡ����
    /// </summary>
    /// <returns>���ض�ȡ�Ļ��ֶ���</returns>
    public int GetData()
    {
        return PlayerPrefs.GetInt("PlayerJoyBeanCount");
    }
}
