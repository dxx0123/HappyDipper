using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public GameObject joyBeanCountGO;
    private Text joyBeanCountText;
    int count = 100;

    private void Start()
    {
        joyBeanCountText = joyBeanCountGO.GetComponent<RectTransform>().Find("Count").GetComponent<Text>();

        if (!PlayerPrefs.HasKey("PlayerJoyBeanCount"))
        {
            DATA.Instance.SaveData(5000);
            joyBeanCountText.text = DATA.Instance.GetData().ToString();
        }
        else
        {
            joyBeanCountText.text = DATA.Instance.GetData().ToString();
        }
    }

    public void SaveDataButtonOnClick()
    {
        DATA.Instance.SaveData(DATA.Instance.GetData()+count);
        //count += 100;
        joyBeanCountText.text = DATA.Instance.GetData().ToString();
    }

    ////public delegate void IEnum();
    //public Coroutine ie;
    //public 
    //// Start is called before the first frame update
    //void Start()
    //{
    //    ie=StartCoroutine(StartIEnum());
    //    //IEnum ienum = new IEnum(StopIEnum);
    //}

    //IEnumerator StartIEnum()
    //{
    //    for(int i=0;i<5;i++)
    //    {
    //        Debug.Log(i);
    //        yield return new WaitForSeconds(1);
    //    }
    //}
    //public void StopIEnum()
    //{
    //    StopCoroutine(ie);
    //}
    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
