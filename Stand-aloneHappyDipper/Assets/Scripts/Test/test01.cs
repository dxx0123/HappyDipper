using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test01 : MonoBehaviour
{
    public Button button;

    public GameObject GO;

    public delegate void IEnum();
    public IEnum ienum;
    // Start is called before the first frame update
    void Start()
    {
        button = this.GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        GameObject.Destroy(GO);
        //ienum = new IEnum(this.GetComponent<Test>().StopIEnum);
        //StopCoroutine(this.GetComponent<Test>().ie);
        //this.GetComponent<Test>().StopIEnum();
        //Debug.Log("关闭协程");
        Debug.Log("按钮点击");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
