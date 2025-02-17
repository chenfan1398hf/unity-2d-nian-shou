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
    #region ���캯���������
    public GameManager()
    {
        configMag = new ConfigManager();
    }
    public static bool isDbugLog = true;
    public PlayerData playerData = null;                            //������ݣ����س־û���
    public ConfigManager configMag;
    private System.Random Random;                                   //�������
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
        Application.targetFrameRate = 60;//����֡��Ϊ60֡
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


    #region OnApplicationPause(bool pause)������֪
    public void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            if (isDbugLog)
                Debug.Log("������֪");
            SaveGame();
        }
    }
    #endregion

    #region OnApplicationQuit() �˳���Ϸ��֪
    public void OnApplicationQuit()
    {
        if (isDbugLog)
            Debug.Log("�˳���֪");
        SaveGame();

    }
    #endregion

    #region ��ȡ��������
    public void GetLocalPlayerData()
    {
        playerData = PlayerData.GetLocalData();//��ȡ���س־û��������(��������������)
        configMag.InitGameCfg();//��ȡ���ñ�
        playerData.InitData();//�������ñ�ͱ������ݳ�ʼ����Ϸ����
    }
    #endregion

    #region SaveGame() �����������
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
    /// ע��һ��update��������
    /// </summary>
    /// <param name="_action"></param>
    public void AddUpdateListener(UnityAction _action)
    {
        unityActionList.Add(_action);
    }

    /// <summary>
    /// ����ͼƬ
    /// </summary>
    public void SpritPropImage(string id, Image image)
    {
        string path = "Icon/" + id;
        Sprite Tab3Img = ResourcesLoad.instance.Load<Sprite>(path);
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// ����ͼƬ--װ��ͼ��
    /// </summary>
    public void SpritPropEquipIcon(string id, Image image)
    {
        string path = "EquipIcon/" + id;
        Sprite Tab3Img = ResourcesLoad.instance.Load<Sprite>(path);
        image.sprite = Tab3Img;
    }


    /// <summary>
    /// ����ͼƬ
    /// </summary>
    public void SpritPropImageByPath(string path, Image image)
    {
        Sprite Tab3Img = Resources.Load(path, typeof(Sprite)) as Sprite;
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// ����ͼƬ
    /// </summary>
    public void SpritPropImageByPath(string path, SpriteRenderer image)
    {
        Sprite Tab3Img = Resources.Load(path, typeof(Sprite)) as Sprite;
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// ���Ԥ����
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
    /// ����Ԥ����
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
    /// ����Ԥ����
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(GameObject prefabObj, GameObject gameObject, string _path = null)
    {
        ObjPool.instance.Recycle(prefabObj, gameObject, "Prefab/" + _path);
        return;
    }
    /// <summary>
    /// ����Ԥ����
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(GameObject gameObject)
    {
        string name = gameObject.GetComponent<DesObj>().name;
        ObjPool.instance.Recycle(name, gameObject);
        return;
    }
    /// <summary>
    /// ���Ŷ��������ö�������0֡
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
    /// ���Ŷ��������ö�������0֡
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
    /// ��ȡ������ڶ�������
    /// </summary>
    /// <returns></returns>
    public ObjPool.PoolItem GetPoolItem(string name)
    {
        string newpath = "Prefab/" + name;
        return ObjPool.instance.GetPoolItem(newpath); ;
    }
    /// <summary>
    /// ������ȡͼƬ
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
    /// �������
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
        //�Ƴ����г��������Ķ���
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var objs = scene.GetRootGameObjects();
            for (var j = 0; j < objs.Length; j++)
            {
                allGameObjects.Remove(objs[j]);
            }
        }
        //�Ƴ�������Ϊnull�Ķ���
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
    //��ʼ��Ϸ
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
        StartTypewriter("��ؽ�����������Ϯ����ׯ�����Ұ����������ͦ���������ҪΪ���׳������ػ����갲����");
    }
    public Text textComponent; // ��Ҫ��ʾ���ֵ� Text ���
    private float duration = 10f; // ���ֻ�Ч���ĳ���ʱ��
    private float delayBetweenCharacters = 0.1f; // ÿ���ַ�֮����ӳ�ʱ��

    private string _targetText; // Ŀ������
    public void StartTypewriter(string text)
    {
        _targetText = text;
        textComponent.text = ""; // ��ճ�ʼ����

        // ʹ�� DOTween ʵ�ִ��ֻ�Ч��
        DOTween.To(
            () => textComponent.text, // ��ȡ��ǰ����
            x => textComponent.text = x, // ���õ�ǰ����
            _targetText, // Ŀ������
            duration // ����ʱ��
        ).SetEase(Ease.Linear) // ���Ա仯
         .SetDelay(0.5f) // �ӳٿ�ʼ����ѡ��
         .OnUpdate(() => {
             // ÿ���ַ�֮����ӳ�
             if (textComponent.text.Length < _targetText.Length)
             {
                 textComponent.text += _targetText[textComponent.text.Length];
             }
         });
    }
    //��ʼ������
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
    //����
    public void OpenDoor()
    {
        if (playerInfo.haveChunLian == 1)
        {
            men1Obj.SetActive(false);
            men2Obj.SetActive(true);
            playerInfo.haveChunLian = 2;
        }
    }
    //��ֽ
    public void JianZhi()
    {
        if (playerInfo.jiandao && playerInfo.hongzhi)
        {
            playerInfo.jianzhi = true;
        }
    }
    //����2
    public void OpenDoor2()
    {
        if (playerInfo.jianzhi)
        {
            men3Obj.SetActive(false);
            men4Obj.SetActive(true);
            
        }
    }
    //��ñ���
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
    //��鱬ը����
    public IEnumerator CheckBoom()
    {
        if (boomNumber >= bianPaoList.Count)
        {
            //����
            var spriteRenderer = BoosObj.transform.GetComponent<SpriteRenderer>();
            spriteRenderer.DOColor(new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0), 2f);
        }
        yield return new WaitForSeconds(3f);
        Destroy(BoosObj);

        endPanel.SetActive(true);
        endPanel.transform.Find("Image").GetComponent<Image>().DOFade(1f, 5f);
    }
    //�޸ĺ���
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
            // ȷ��ǿ���ں���Χ��
            intensity = Mathf.Clamp(intensity, 0f, 1f);
            vignette.intensity.value = intensity;
        }
    }
    //ƴͼ��Ϸ
    private Sprite[] spriteArray;
    public GameObject ptPanelObj;
    private int regNumber = 0;
    public void BeginPingTuGame()
    {
        ptPanelObj.SetActive(true);
        InitPt();
    }
    //��ʼ��ƴͼ����
    public void InitPt()
    {
        //��ʼ��ƴͼ��Ƭ
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
    //���λ���Ƿ�OK
    public bool CheckCardVec(int _id, Vector3 _vec3)
    {
        GameObject obj = ptPanelObj.transform.Find("list/Image" + _id).gameObject;
        // �������λ���Ƿ���ָ����Χ��
        bool isWithinRange = IsWithinDistance(obj.transform.position, _vec3, 50f);
        if (isWithinRange)
        {
            obj.SetActive(true);
            regNumber++;
            if (regNumber >= spriteArray.Length)
            {
                //���ƴͼ
                ptPanelObj.SetActive(false);
                AddBianPao();
            }
        }
        return isWithinRange;
    }
    // ��������Vector3λ��֮��ľ��룬���ж��Ƿ���ָ����Χ��
    bool IsWithinDistance(Vector3 position1, Vector3 position2, float threshold)
    {
        // ��������λ��֮��ľ���
        float distance = Vector3.Distance(position1, position2);
        Debug.Log(distance);
        // �жϾ����Ƿ�С����ֵ
        return distance < threshold;
    }
}
