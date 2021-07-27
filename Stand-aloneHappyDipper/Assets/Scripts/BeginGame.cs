using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BeginGame : MonoBehaviour
{
    private Transform bgRT; //BG的RT 
    private RectTransform beginGameButtonRT; //开始游戏按钮的RT
    private GameObject rememberCardGO; //记牌器
    //private bool isAnimPlaying; //判断欢乐豆提示动画是否在播放
    public int currentAnimPlayingCount; //存储当前正在播放的欢乐豆提示动画个数
    private void Start()
    {
        bgRT =this.GetComponent<RectTransform>();
        beginGameButtonRT = (RectTransform)bgRT.Find("BeginGameButton");
        beginGameButtonRT.GetComponent<Button>().onClick.AddListener(BeginGameButtonOnClick); //添加按钮点击事件
        currentAnimPlayingCount = 0; //默认动画个数为0
    }
    
    /// <summary>
    /// 欢乐豆提示
    /// </summary>
    public void SetJoyBeanRemindTextAnim()
    {
        currentAnimPlayingCount++; //当前正在播放的动画个数+1
        GameObject joyBeanRemindText = (GameObject)Instantiate(Resources.Load("Prefabs/Game/JoyBeanRemindText"),bgRT); //实例化欢乐豆提示GO
        RectTransform joyBeanRemindTextRT = joyBeanRemindText.GetComponent<RectTransform>(); //获取欢乐豆提示RT
        Text remindText = joyBeanRemindTextRT.GetComponent<Text>();
         
        Sequence quence = DOTween.Sequence(); //定义动画序列
        float duration = 1.0f; //定义动画播放时间

        //播放前半段动画
        quence.Append(joyBeanRemindTextRT.DOScale(new Vector3(1f,1f,1f), duration));
        quence.Join(remindText.DOFade(1, duration));
        quence.Join(joyBeanRemindTextRT.DOLocalMove(joyBeanRemindTextRT.localPosition + new Vector3(0f, 100f, 0f), duration).OnComplete(
                () => 
                    {
                        //播放后半段动画
                        quence.Append(joyBeanRemindTextRT.DOScale(new Vector3(0f, 0f, 1f), duration));
                        quence.Join(remindText.DOFade(0,duration));
                        quence.Join(joyBeanRemindTextRT.DOLocalMove(joyBeanRemindTextRT.localPosition + new Vector3(0f, 100f, 0f), duration).OnComplete(
                                () => {
                                            Destroy(joyBeanRemindText); //动画播放结束后，删除欢乐豆提示
                                            currentAnimPlayingCount--; //当前正在播放的动画个数-1
                                }
                            ));
                    }
            ));
    }

    /// <summary>
    /// 开始游戏按钮点击监听事件
    /// </summary>
    private void BeginGameButtonOnClick()
    {
        //欢乐豆数量小于等于0
        if (DATA.Instance.GetData()<=0)
        {
            //当前没有欢乐豆提示动画在播放
            if (currentAnimPlayingCount<=3)
            {
                SetJoyBeanRemindTextAnim(); //播放欢乐豆提示动画
            }
            return;
        }
        //创建记牌器
        rememberCardGO = (GameObject)Instantiate(Resources.Load("Prefabs/RememberCard/RememberCardDevice"), bgRT);
        rememberCardGO.name = "RememberCardDevice";

        //创建卡牌空物体，作为所有卡牌的父物体
        //GameObject go = new GameObject("CardsListEmptyGO");
        GameObject go = (GameObject)Instantiate(Resources.Load("Prefabs/Cards/CardsListEmptyGO"),bgRT);
        go.AddComponent<CardManager>();
        go.transform.position = this.transform.parent.position;

        //创建玩家
        CreatePlayer("LeftPlayer", true);
        CreatePlayer("Player");
        CreatePlayer("RightPlayer",false);
        
        beginGameButtonRT.gameObject.SetActive(false); //禁用开始游戏按钮

    }
    /// <summary>
    /// 创建中间玩家
    /// </summary>
    /// <param name="name">玩家名称</param>
    private void CreatePlayer(string name)
    {
        //默认创建玩家的图像为woman，位置在玩家位置
        GameObject playerGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Player"), bgRT);
        AdaptiveScript.Instance.SetAnchor(playerGO,AdaptiveScript.ANCHOR.LEFT_BUTTOM); //设置中间玩家锚点

        RectTransform playerRT = playerGO.GetComponent<RectTransform>();
        playerRT.anchoredPosition3D = new Vector3(100f,90f,0f);
        //playerRT.localPosition = new Vector3(100f,90f,0f);

        playerGO.name = name; 
    }
    /// <summary>
    /// 创建左边、右边玩家
    /// </summary>
    /// <param name="name">玩家名称</param>
    /// <param name="isLeftPlayer">判断是左边玩家还是右边玩家</param>
    private void CreatePlayer(string name,bool isLeftPlayer)
    {
        //默认创建玩家的图像为woman
        GameObject playerGO = (GameObject)Instantiate(Resources.Load("Prefabs/Game/Player"),bgRT);
        playerGO.name = name;
        //获取组件
        RectTransform playerRT = playerGO.GetComponent<RectTransform>();
        //设置随机图像、添加脚本组件AIPlayer
        //设置玩家随机图像
        int player = Random.Range(0, 2);
        if (player == 1)
        {
            playerGO.GetComponent<Image>().sprite = (Sprite)Resources.Load("UI/NPC/NPC_Man",typeof(Sprite));
        }
        //设置左边玩家
        if (isLeftPlayer)
        {
            AdaptiveScript.Instance.SetAnchor(playerGO,AdaptiveScript.ANCHOR.LEFT_TOP); //设置左边玩家锚点
            playerRT.anchoredPosition3D = new Vector3(240f, -390f, 0f);
            //playerRT.localPosition = new Vector3(240f, -390f, 0f);
        }
        //设置右边玩家
        else
        {
            AdaptiveScript.Instance.SetAnchor(playerGO,AdaptiveScript.ANCHOR.RIGHT_TOP); //设置右边玩家锚点
            playerRT.anchoredPosition3D = new Vector3(-240f, -390f, 0f);
            //playerRT.localPosition = new Vector3(-240f, -390f, 0f);
            playerRT.rotation = Quaternion.Euler(0f,180f,0f);
        }
    }
}
