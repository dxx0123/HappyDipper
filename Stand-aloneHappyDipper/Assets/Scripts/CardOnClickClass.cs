using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardOnClickClass : MonoBehaviour
{
    private OutCardClass outcard; //脚本OutCard的引用
    private RectTransform thisRT; //卡牌正面的RT
    private RectTransform parentRT; //卡牌正面父物体的RT（即卡牌RT）
    /// <summary>
    /// 变量初始化
    /// </summary>
    private void Start()
    {
        outcard = Player.Instance.outCardClass; //获取脚本OutCard的引用
        thisRT = this.GetComponent<RectTransform>(); //卡牌正面RT
        parentRT = (RectTransform)thisRT.parent; //卡牌的RT
        this.GetComponent<Button>().onClick.AddListener(CardOnClick); //添加监听事件
    }
    //卡牌是否点击，默认未被点击
    public bool isSelect = false;
    /// <summary>
    /// 卡牌监听事件（玩家1）
    /// </summary>
    public void CardOnClick()
    {
        isSelect = !isSelect;
        if(isSelect)
        {
            parentRT.localPosition+=new Vector3(0f,20f,0f); //卡牌向上移动20f

            outcard.playerOnClickPreOutCardsList.Add(parentRT.gameObject); //将选中的卡牌存入点击预出牌集合中
        }
        else
        {
            parentRT.localPosition -= new Vector3(0f, 20f, 0f); //卡牌向下移动20f

            outcard.playerOnClickPreOutCardsList.Remove(parentRT.gameObject); //将选中的卡片移出点击预出牌集合中
        }
    }
}
