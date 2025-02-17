using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using Spine.Unity;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class GameManager :MonoSingleton<GameManager>
{
    #region 构造函数及其变量
    public GameManager()
    {
        configMag = new ConfigManager();
    }
    public static bool isDbugLog = true;
    public PlayerData playerData = null;                            //玩家数据（本地持久化）
    public ConfigManager configMag;
    private System.Random Random;                                   //随机种子
    private int TimeNumber = 0;
    private List<UnityAction> unityActionList = new List<UnityAction>();
    public bool isBattle = true;


    public static int TI_LI_MAX_NUMBER = 100;
    public static int TI_LI_CD_NUMBER = 600;

    #endregion

    private void Update()
    {
        foreach (var item in unityActionList)
        {
            item.Invoke();
        }
    }
    #region Awake()
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;//设置帧率为60帧
        GetLocalPlayerData();
        Random = new System.Random(Guid.NewGuid().GetHashCode());
    }
    #endregion



    private void Start()
    {
        this.InvokeRepeating("CheckTime", 0, 0.1f);
        BeginGame();
    }

    void CheckTime()
    {
        TimeNumber++;

        if (TimeNumber % 10 == 0)
        {
            ChangeHouChuLi();
        }
        if (TimeNumber % 20 == 0)
        {

        }


    }


    #region OnApplicationPause(bool pause)切屏感知
    public void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            if (isDbugLog)
                Debug.Log("切屏感知");
            SaveGame();
        }
    }
    #endregion

    #region OnApplicationQuit() 退出游戏感知
    public void OnApplicationQuit()
    {
        if (isDbugLog)
            Debug.Log("退出感知");
        SaveGame();

    }
    #endregion

    #region 获取本地数据
    public void GetLocalPlayerData()
    {
        playerData = PlayerData.GetLocalData();//读取本地持久化玩家数据(包括本土化设置)
        configMag.InitGameCfg();//读取配置表
        playerData.InitData();//根据配置表和本地数据初始化游戏数据
    }
    #endregion

    #region SaveGame() 保存玩家数据
    public void SaveGame()
    {
        //if(SocketManager.instance.socket!=null)
        //SocketManager.instance.socket.Disconnect();
        playerData.Save();
    }
    #endregion

    #region OnDestroy()
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    #endregion

    /// <summary>
    /// 注册一个update在这里跑
    /// </summary>
    /// <param name="_action"></param>
    public void AddUpdateListener(UnityAction _action)
    {
        unityActionList.Add(_action);
    }

    /// <summary>
    /// 加载图片
    /// </summary>
    public void SpritPropImage(string id, Image image)
    {
        string path = "Icon/" + id;
        Sprite Tab3Img = ResourcesLoad.instance.Load<Sprite>(path);
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// 加载图片--装备图标
    /// </summary>
    public void SpritPropEquipIcon(string id, Image image)
    {
        string path = "EquipIcon/" + id;
        Sprite Tab3Img = ResourcesLoad.instance.Load<Sprite>(path);
        image.sprite = Tab3Img;
    }


    /// <summary>
    /// 加载图片
    /// </summary>
    public void SpritPropImageByPath(string path, Image image)
    {
        Sprite Tab3Img = Resources.Load(path, typeof(Sprite)) as Sprite;
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// 加载图片
    /// </summary>
    public void SpritPropImageByPath(string path, SpriteRenderer image)
    {
        Sprite Tab3Img = Resources.Load(path, typeof(Sprite)) as Sprite;
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// 添加预制体
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fatherTransform"></param>
    /// <returns></returns>
    public GameObject AddPrefab(string name, Transform fatherTransform)
    {
        string newpath = "Prefab/" + name;
        GameObject obj = ObjPool.instance.GetObj(newpath, fatherTransform);
        obj.AddComponent<DesObj>();
        obj.GetComponent<DesObj>().InitDes(newpath);
        return obj;
    }
    /// <summary>
    /// 销毁预制体
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(string name, GameObject gameObject)
    {
        string[] list = name.Split(new char[] { '(' });
        if (list.Length != 2)
        {
            string newpath = "Prefab/" + name;
            ObjPool.instance.Recycle(newpath, gameObject);
        }
        else
        {
            string newpath = "Prefab/" + list[0];
            ObjPool.instance.Recycle(newpath, gameObject);
        }
        return;
    }
    /// <summary>
    /// 销毁预制体
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(GameObject prefabObj, GameObject gameObject, string _path = null)
    {
        ObjPool.instance.Recycle(prefabObj, gameObject, "Prefab/" + _path);
        return;
    }
    /// <summary>
    /// 销毁预制体
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(GameObject gameObject)
    {
        string name = gameObject.GetComponent<DesObj>().name;
        ObjPool.instance.Recycle(name, gameObject);
        return;
    }
    /// <summary>
    /// 播放动画并重置动画到第0帧
    /// </summary>
    public void PlaySpine(SkeletonGraphic _skeletonGraphic, bool isLoop, string _spineName, bool isRest)
    {
        if (isRest)
        {
            _skeletonGraphic.AnimationState.ClearTracks();
            _skeletonGraphic.AnimationState.Update(0);
        }
        _skeletonGraphic.AnimationState.SetAnimation(0, _spineName, isLoop);

        return;
    }
    /// <summary>
    /// 播放动画并重置动画到第0帧
    /// </summary>
    public void PlaySpine(Animator _animator, string _spineName, bool isRest)
    {
        //_animator.Play(_spineName, 0 ,0f);
        if (isRest)
        {
            //_animator.Update(0);
            _animator.Play(_spineName, 0, 0f);
        }
        else
        {
            _animator.Play(_spineName);
        }
        return;
    }
    /// <summary>
    /// 获取对象池内对象数据
    /// </summary>
    /// <returns></returns>
    public ObjPool.PoolItem GetPoolItem(string name)
    {
        string newpath = "Prefab/" + name;
        return ObjPool.instance.GetPoolItem(newpath); ;
    }
    /// <summary>
    /// 网络拉取图片
    /// </summary>
    /// <param name="_url"></param>
    /// <param name="_image"></param>
    /// <returns></returns>
    public IEnumerator GetHead(string _url, Image _image)
    {
        if (_url == string.Empty || _url == "")
        {
            _url = "https://p11.douyinpic.com/aweme/100x100/aweme-avatar/mosaic-legacy_3797_2889309425.jpeg?from=3067671334";
        }

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(1f, 1f));
                _image.sprite = sprite;
                //Renderer renderer = plane.GetComponent<Renderer>();
                //renderer.material.mainTexture = texture;
            }
        }
    }

    /// <summary>
    /// 清空数据
    /// </summary>
    public void CleraPlayerData()
    {
        PlayerData.ClearLocalData();
    }

#if UNITY_EDITORt
    [UnityEditor.MenuItem("Editor/Tools/Clear")]
    static void CleraPlayerData1()
    {
        PlayerData.ClearLocalData();
    }
#endif
    private GameObject[] GetDontDestroyOnLoadGameObjects()
    {
        var allGameObjects = new List<GameObject>();
        allGameObjects.AddRange(FindObjectsOfType<GameObject>());
        //移除所有场景包含的对象
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var objs = scene.GetRootGameObjects();
            for (var j = 0; j < objs.Length; j++)
            {
                allGameObjects.Remove(objs[j]);
            }
        }
        //移除父级不为null的对象
        int k = allGameObjects.Count;
        while (--k >= 0)
        {
            if (allGameObjects[k].transform.parent != null)
            {
                allGameObjects.RemoveAt(k);
            }
        }
        return allGameObjects.ToArray();
    }

    public PlayerInfo playerInfo = new PlayerInfo();
    public GameObject men1Obj;
    public GameObject men2Obj;
    public GameObject men3Obj;
    public GameObject men4Obj;
    public GameObject BoosObj;
    public List<GameObject> bianPaoList = new List<GameObject>();

    public GameObject panelBase1;
    public GameObject panelBase2;
    public GameObject panelBase3;
    public GameObject panelMsg;
    public GameObject endPanel;
    public MusicManager musicManager;
    public GameObject beginGamePanel;
    //开始游戏
    public void BeginGame()
    {
        InitData();
        musicManager = new MusicManager();
        musicManager.PlayBkMusic("123");
        BeginPanel();
    }
    public void BeginPanel()
    {
        beginGamePanel.SetActive(true);
        beginGamePanel.transform.Find("Image").GetComponent<Image>().DOFade(1f, 10f);
        StartTypewriter("年关将至，年兽来袭，村庄哀鸿遍野。少年勇者挺身而出，誓要为乡亲除害，守护新年安宁。");
    }
    public Text textComponent; // 需要显示文字的 Text 组件
    private float duration = 10f; // 打字机效果的持续时间
    private float delayBetweenCharacters = 0.1f; // 每个字符之间的延迟时间

    private string _targetText; // 目标文字
    public void StartTypewriter(string text)
    {
        _targetText = text;
        textComponent.text = ""; // 清空初始文字

        // 使用 DOTween 实现打字机效果
        DOTween.To(
            () => textComponent.text, // 获取当前文字
            x => textComponent.text = x, // 设置当前文字
            _targetText, // 目标文字
            duration // 持续时间
        ).SetEase(Ease.Linear) // 线性变化
         .SetDelay(0.5f) // 延迟开始（可选）
         .OnUpdate(() => {
             // 每个字符之间的延迟
             if (textComponent.text.Length < _targetText.Length)
             {
                 textComponent.text += _targetText[textComponent.text.Length];
             }
         });
    }
    //初始化数据
    public void InitData()
    {
        men1Obj.SetActive(true);
        men2Obj.SetActive(false);
        men3Obj.SetActive(true);
        men4Obj.SetActive(false);
        panelBase1.SetActive(false);
        panelBase2.SetActive(false);
        panelBase3.SetActive(false);
        ptPanelObj.SetActive(false);
        panelMsg.SetActive(false);
        endPanel.SetActive(false);

        foreach (var item in bianPaoList)
        {
            item.SetActive(false);
        }
    }
    //开门
    public void OpenDoor()
    {
        if (playerInfo.haveChunLian == 1)
        {
            men1Obj.SetActive(false);
            men2Obj.SetActive(true);
            playerInfo.haveChunLian = 2;
        }
    }
    //剪纸
    public void JianZhi()
    {
        if (playerInfo.jiandao && playerInfo.hongzhi)
        {
            playerInfo.jianzhi = true;
        }
    }
    //开门2
    public void OpenDoor2()
    {
        if (playerInfo.jianzhi)
        {
            men3Obj.SetActive(false);
            men4Obj.SetActive(true);
            
        }
    }
    //获得鞭炮
    public void AddBianPao()
    {
        playerInfo.bianpao = true;

        foreach (var item in bianPaoList)
        {
            item.SetActive(true);
        }
        panelMsg.SetActive(true);
    }
    private int boomNumber = 0;
    //BOOM
    public IEnumerator Boom(GameObject _obj)
    {
        var spr = _obj.GetComponent<SpriteRenderer>().color;
        spr.a = 1f;
        _obj.GetComponent<SpriteRenderer>().color = spr;

        yield return new WaitForSeconds(3f);

        _obj.transform.Find("boom").gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        _obj.SetActive(false);
        boomNumber++;
        StartCoroutine(CheckBoom());
    }
    //检查爆炸数量
    public IEnumerator CheckBoom()
    {
        if (boomNumber >= bianPaoList.Count)
        {
            //结束
            var spriteRenderer = BoosObj.transform.GetComponent<SpriteRenderer>();
            spriteRenderer.DOColor(new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0), 2f);
        }
        yield return new WaitForSeconds(3f);
        Destroy(BoosObj);

        endPanel.SetActive(true);
        endPanel.transform.Find("Image").GetComponent<Image>().DOFade(1f, 5f);
    }
    //修改后处理
    public Volume globalVolume;
    private Vignette vignette;
    private float vNumber = 0.6f;
    public void ChangeHouChuLi()
    {
        float number = 0f;
        if (playerInfo.haveChunLian == 2)
        {
            number += 0.1f;
        }
        if (playerInfo.jianzhi)
        {
            number += 0.1f;
        }
        if (playerInfo.kaimen2)
        {
            number += 0.1f;
        }
        if (playerInfo.bianpao)
        {
            number += 0.1f;
        }

        SetVignetteIntensity(vNumber - number);
    }
    public void SetVignetteIntensity(float intensity)
    {
        globalVolume.profile.TryGet(out vignette);

        if (vignette != null)
        {
            // 确保强度在合理范围内
            intensity = Mathf.Clamp(intensity, 0f, 1f);
            vignette.intensity.value = intensity;
        }
    }
    //拼图游戏
    private Sprite[] spriteArray;
    public GameObject ptPanelObj;
    private int regNumber = 0;
    public void BeginPingTuGame()
    {
        ptPanelObj.SetActive(true);
        InitPt();
    }
    //初始化拼图数据
    public void InitPt()
    {
        //初始化拼图碎片
        spriteArray = Resources.LoadAll<Sprite>("Tex/9");
        for (int i = 0; i < spriteArray.Length; i++)
        {
            var image = ptPanelObj.transform.Find("list/Image" + i).GetComponent<Image>();
            image.sprite = spriteArray[i];
            var obj = AddPrefab("Card", ptPanelObj.transform);
            int randX = Util.randomInt(-800, 100);
            int randY = Util.randomInt(-300, 300);
            obj.GetComponent<card>().InitCard(i, new Vector2(randX, randY));
            obj.GetComponent<Image>().sprite = spriteArray[i];
        }

    }
    //检查位置是否OK
    public bool CheckCardVec(int _id, Vector3 _vec3)
    {
        GameObject obj = ptPanelObj.transform.Find("list/Image" + _id).gameObject;
        // 检查两个位置是否在指定范围内
        bool isWithinRange = IsWithinDistance(obj.transform.position, _vec3, 50f);
        if (isWithinRange)
        {
            obj.SetActive(true);
            regNumber++;
            if (regNumber >= spriteArray.Length)
            {
                //完成拼图
                ptPanelObj.SetActive(false);
                AddBianPao();
            }
        }
        return isWithinRange;
    }
    // 计算两个Vector3位置之间的距离，并判断是否在指定范围内
    bool IsWithinDistance(Vector3 position1, Vector3 position2, float threshold)
    {
        // 计算两个位置之间的距离
        float distance = Vector3.Distance(position1, position2);
        Debug.Log(distance);
        // 判断距离是否小于阈值
        return distance < threshold;
    }
}
