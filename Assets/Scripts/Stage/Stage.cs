using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

/// <summary>
///各ステージ動作の基底クラス
/// </summary>
public class Stage : Methods
{
    /// <summary>
    ///1ステージ説明文
    /// </summary>
    [Multiline]
    public string explanation = "依頼内容";

    /// <summary>
    ///特殊ステージフラグ
    /// </summary>
    public bool isSystem = false;

    /// <summary>
    ///初期背景
    /// </summary>
    [SerializeField]
    protected MeshRenderer initialScenery;
    private MeshRenderer scenery = null;
    /// <summary>
    ///初期BGM
    /// </summary>
    [SerializeField]
    protected AudioClip initialBGM;

    /// <summary>
    ///ステージサイズ
    /// </summary>
    [SerializeField]
    public Vector2 fieldSize = Vector2.one;
    /// <summary>
    ///ビューの初期位置
    /// </summary>
    [SerializeField]
    protected Vector2 initialViewPosition = Vector2.zero;
    /// <summary>
    ///プレイヤー機の初期位置
    /// </summary>
    [SerializeField]
    protected Vector2 initialPlayerPosition = new Vector2(-3.6f, 0);

    /// <summary>
    ///ステージの難易度
    ///オプションからの難易度設定とか用
    /// </summary>
    public ulong stageLevel = 1;

    /// <summary>
    ///ステージに出てくるNPCのリスト
    ///基本的に出現対象はここから指定する
    /// </summary>
    public List<Npc> enemyList = new List<Npc>();

    /// <summary>
    ///獲得ポイント総数
    /// </summary>
    public int points = 0;

    /// <summary>
    ///ステージアクションのコルーチンを所持する変数
    /// </summary>
    public Coroutine nowStageAction = null;

    public bool isCleared
    {
        get {
            return sys.getClearFlug(this);
        }
    }
    public virtual bool ableChoice
    {
        get {
            return true;
        }
    }

    // Use this for initialization
    public virtual void Start()
    {
        setBGM();
        setScenery();

        points = 0;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if(nowStageAction != null && !isContinue) endStageProcess();
        if(!isSystem && !onPause && Input.GetKeyDown(Buttom.Esc)) StartCoroutine(pauseMenu());
    }

    /// <summary>
    ///ポーズメニューアクション
    /// </summary>
    IEnumerator pauseMenu()
    {
        switchPause(true);
        var pauseDarkTone = putDarkTone(0.8f);

        bool withdraw = false;
        yield return getChoices(new List<string> { "継続", "撤退" },
            endProcess: result => withdraw = result == 1,
            ableCancel: true,
            pibot: TextAnchor.MiddleCenter);
        deleteChoices();

        if(withdraw)
        {
            sys.nextStage = sys.mainMenu;
            isContinue = false;
        }
        pauseDarkTone.selfDestroy();
        switchPause(false);
        yield break;
    }

    bool _isContinue = true;
    protected bool isContinue
    {
        get {
            if(isSuccess || isFault) return false;
            return _isContinue;
        }
        set {
            _isContinue = value;
        }
    }
    bool _isSuccess = false;
    protected bool isSuccess
    {
        get {
            if(isFault) return false;
            return _isSuccess;
        }
        set {
            _isSuccess = value;
        }
    }
    bool _isFault = false;
    protected bool isFault
    {
        get {
            if(sysPlayer != null && sysPlayer.isExist && !sysPlayer.isAlive) return true;
            return _isFault;
        }
        set {
            _isFault = value;
        }
    }
    public virtual void startStageProcess()
    {
        Debug.Log($"{this}");
        visualizePlayer();
        sysPlayer.position = initialPlayerPosition;

        if(!isSystem)
        {
            sysPlayer.deleteArmorBar();
            sysPlayer.setArmorBar();
            sysPlayer.canRecieveKey = true;

            sys.playerHPbar = getBar(BarType.HP, Color.red);
            sys.playerBRbar = getBar(BarType.BR, Color.cyan);
            sys.playerENbar = getBar(BarType.EN, Color.yellow);
            sys.playerHPbar.nowOrder = Orders.PUBLIC_STATE;
            sys.playerBRbar.nowOrder = Orders.PUBLIC_STATE;
            sys.playerENbar.nowOrder = Orders.PUBLIC_STATE;
        }

        nowStageAction = StartCoroutine(stageAction());
    }
    protected void endStageProcess()
    {
        StopCoroutine(nowStageAction);
        nowStageAction = null;
        destroyAll(true);
        if(sys.playerHPbar != null) sys.playerHPbar.selfDestroy();
        if(sys.playerBRbar != null) sys.playerBRbar.selfDestroy();
        if(sys.playerENbar != null) sys.playerENbar.selfDestroy();
        StartCoroutine(endStageAction());
    }
    protected IEnumerator endStageAction()
    {
        if(isFault) yield return faultAction();
        if(isSuccess) yield return successAction();

        resetView();
        transparentPlayer();
        if(scenery != null) Destroy(scenery.gameObject);

        isContinue = true;
        selfDestroy();
        sys.Start();
        yield break;
    }
    protected virtual IEnumerator successAction()
    {
        var text = setWindowWithText(setSysText("Success", charSize: 24));
        yield return wait(12000, Buttom.Z);
        text.selfDestroy();
        yield break;
    }
    protected virtual IEnumerator faultAction()
    {
        var text = setWindowWithText(setSysText("Fault", charSize: 24));
        yield return wait(12000, Buttom.Z);
        text.selfDestroy();
        yield break;
    }

    public Vector2 resetView()
    {
        return viewPosition = initialViewPosition;
    }

    protected virtual IEnumerator stageAction()
    {
        yield break;
    }

    /// <summary>
    ///オブジェクト配置関数
    /// </summary>
    protected Things setObject(Things obj, Vector2 coordinate)
    {
        Vector2 precisionCoordinate = -fieldArea / 2 + MathV.scaling(fieldArea, coordinate);

        var newObject = Instantiate(obj);
        newObject.nowParent = sysPanel.transform;
        newObject.position = precisionCoordinate;

        return newObject;
    }
    /// <summary>
    ///NPC機体配置関数
    /// </summary>
    protected Npc setEnemy(Npc npc, Vector2 coordinate, ulong? levelCorrection = null, string setLayer = Layers.ENEMY)
    {
        if(npc == null) return null;
        Debug.Log($"{npc}\t: {coordinate}");
        var newObject = (Npc)setObject(npc, coordinate + Vector2.right);
        newObject.shipLevel = levelCorrection ?? stageLevel;
        newObject.layer = LayerMask.NameToLayer(setLayer);

        return newObject;
    }
    /// <summary>
    ///NPC機体配置関数
    /// </summary>
    protected Npc setEnemy(int npcIndex, Vector2 coordinate, ulong? levelCorrection = null)
    {
        if(npcIndex < 0) return null;
        if(npcIndex >= enemyList.Count) return null;

        return setEnemy(enemyList[npcIndex], coordinate, levelCorrection);
    }
    /// <summary>
    ///背景設定関数
    ///初期値はStageの初期背景
    /// </summary>
    protected MeshRenderer setScenery(MeshRenderer buckGround = null)
    {
        var setBuckGround = (buckGround ?? initialScenery);
        if(setBuckGround == null) return null;

        var baseScenery = GameObject.Find("SceneryRoot");
        foreach(Transform oldScenery in baseScenery.transform)
        {
            Destroy(oldScenery.gameObject);
        }
        scenery = Instantiate(setBuckGround.gameObject, Vector3.forward, transform.rotation).GetComponent<MeshRenderer>();
        scenery.transform.localScale = fieldArea;
        scenery.transform.parent = baseScenery.transform;

        return scenery;
    }
    /// <summary>
    ///BGM設定関数
    ///初期値はStageの初期BGM
    /// </summary>
    protected AudioSource setBGM(AudioClip setBGM = null)
    {
        var setMusic = (setBGM ?? initialBGM);
        if(setMusic == null) return null;

        var baseMusic = GameObject.Find("MusicRoot");
        foreach(Transform oldMusic in baseMusic.transform)
        {
            Destroy(oldMusic.gameObject);
        }

        var BGM = Instantiate(sys.BGMrootObject);
        BGM.transform.SetParent(baseMusic.transform);
        BGM.audioSource.clip = setMusic;
        BGM.audioSource.Play();

        return BGM.audioSource;
    }
}
