using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DATA : MonoBehaviour
{
    public static DATA Instance; //设置单例

    private int playerJoyBeanInitCount = 1000; //设置玩家欢乐豆初始数量

    void Awake()
    {
        DATA.Instance = this;

        //PlayerPrefs.DeleteKey("PlayerJoyBeanCount"); //删除指定键

        //如果当前玩家为非新用户，则赋予1000欢乐豆
        if (!PlayerPrefs.HasKey("PlayerJoyBeanCount"))
        {
            SaveData(playerJoyBeanInitCount);
        }

        //测试
        //SaveData(1);

        GameObject.Find("PlayerJoyBeans").GetComponent<RectTransform>().Find("Count").GetComponent<Text>().text = GetData().ToString(); //显示欢乐豆

        ////欢乐豆数为0时，红色字体显示
        if (GetData() == 0)
        {
            GameObject.Find("PlayerJoyBeans").GetComponent<RectTransform>().Find("Count").GetComponent<Text>().color = Color.red;
        }
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="num">要保存的欢乐豆数</param>
    public void SaveData(int num)
    {
        PlayerPrefs.SetInt("PlayerJoyBeanCount",num);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    /// <returns>返回读取的欢乐豆数</returns>
    public int GetData()
    {
        return PlayerPrefs.GetInt("PlayerJoyBeanCount");
    }
}
