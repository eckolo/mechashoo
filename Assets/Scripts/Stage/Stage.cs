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
    ///表示ステージ名称
    /// </summary>
    [SerializeField]
    private string _displayName = "";

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
    public List<Ship> enemyList = new List<Ship>();

    /// <summary>
    ///獲得ポイント総数
    /// </summary>
    public int points = 0;

    /// <summary>
    ///ステージアクションのコルーチンを所持する変数
    /// </summary>
    public IEnumerator nowStageAction = null;

    public string displayName
    {
        get
        {
            if(_displayName != null && _displayName != "") return _displayName;
            if(gameObject != null) return gameObject.name.Replace("(Clone)", "");
            return _displayName;
        }
    }
    public bool isClear
    {
        get
        {
            return sys.getClearFlug(this);
        }
    }
    public virtual bool ableChoice
    {
        get
        {
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
    public virtual void Update()
    {
        if(!isContinue) stopStageAction();
        if(!isSystem && !onPause && Input.GetKeyDown(Buttom.Esc)) StartCoroutine(pauseMenu());
    }

    /// <summary>
    ///ポーズメニューアクション
    /// </summary>
    IEnumerator pauseMenu()
    {
        switchPause(true);
        var pauseDarkTone = putDarkTone(0.3f);

        bool withdraw = false;
        yield return getChoices(new List<string> { "継続", "撤退" },
            endProcess: result => withdraw = result == 1,
            ableCancel: true,
            pibot: TextAnchor.MiddleCenter);
        deleteChoices();

        if(withdraw)
        {
            sys.nextStageNum = 0;
            isContinue = false;
        }
        pauseDarkTone.selfDestroy();
        switchPause(false);
        yield break;
    }

    bool internalContinue = true;
    protected bool isContinue
    {
        get
        {
            if(!internalContinue) return false;
            if(sysPlayer != null && sysPlayer.isExist && !sysPlayer.isAlive) return false;
            return true;
        }
        set
        {
            internalContinue = value;
        }
    }
    public virtual void startStageAction()
    {
        visualizePlayer();
        sysPlayer.transform.localPosition = initialPlayerPosition;

        if(!isSystem)
        {
            sysPlayer.deleteArmorBar();
            sysPlayer.setArmorBar();
            sysPlayer.canRecieveKey = true;

            sys.playerHPbar = getBar(BarType.HP, Color.red);
            sys.playerBRbar = getBar(BarType.BR, Color.cyan);
            sys.playerENbar = getBar(BarType.EN, Color.yellow);
            sys.playerHPbar.nowOrder = Order.PUBLIC_STATE;
            sys.playerBRbar.nowOrder = Order.PUBLIC_STATE;
            sys.playerENbar.nowOrder = Order.PUBLIC_STATE;
        }

        StartCoroutine(nowStageAction = stageAction());
    }
    protected void stopStageAction()
    {
        StopCoroutine(nowStageAction);
        nowStageAction = null;

        destroyAll();
        if(sys.playerHPbar != null) sys.playerHPbar.selfDestroy();
        if(sys.playerBRbar != null) sys.playerBRbar.selfDestroy();
        if(sys.playerENbar != null) sys.playerENbar.selfDestroy();
        resetView();
        if(scenery != null) Destroy(scenery.gameObject);

        sys.Start();
        return;
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
        Vector2 precisionCoordinate = -fieldSize / 2 + MathV.scaling(fieldSize, coordinate);

        var newObject = Instantiate(obj);
        newObject.transform.parent = sysPanel.transform;
        newObject.transform.localPosition = precisionCoordinate;

        return newObject;
    }
    /// <summary>
    ///NPC機体配置関数
    /// </summary>
    protected Npc setEnemy(Npc npc, Vector2 coordinate, ulong? levelCorrection = null)
    {
        if(npc == null) return null;
        var newObject = (Npc)setObject(npc, coordinate + Vector2.right);
        newObject.shipLevel = levelCorrection ?? stageLevel;

        return newObject;
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
        scenery = ((GameObject)Instantiate(setBuckGround.gameObject, Vector3.forward, transform.rotation)).GetComponent<MeshRenderer>();
        scenery.transform.localScale = new Vector3(13.3f * fieldSize.x / viewSize.x, 10 * fieldSize.y / viewSize.y, 1);
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
