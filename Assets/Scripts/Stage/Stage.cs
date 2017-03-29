﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

/// <summary>
///各ステージ動作の基底クラス
/// </summary>
public abstract class Stage : Methods
{
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
    /// ステージアクションのコルーチンを所持する変数
    /// </summary>
    public Coroutine nowStageAction = null;
    /// <summary>
    /// 各部のステージアクションのコルーチンを所持する変数
    /// </summary>
    public Coroutine nowStageActionMain = null;

    /// <summary>
    /// 通信文章のデフォルト表示位置
    /// </summary>
    protected static Vector2 mainTextPosition => Vector2.down * viewSize.y * baseMas.y / 4;

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
    public override void Start()
    {
        base.Start();

        setBGM();
        setScenery();

        points = 0;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if(nowStageAction != null && !isContinue) endStageProcess();
        if(Input.GetKeyDown(Configs.Buttom.Esc)) StartCoroutine(pauseMenu());
    }

    /// <summary>
    ///ポーズメニューアクション
    /// </summary>
    IEnumerator pauseMenu()
    {
        if(onPause) yield break;
        if(isSystem) yield break;
        if(!sysPlayer.canRecieveKey) yield break;

        switchPause(true);
        var pauseDarkTone = putDarkTone(0.8f);

        bool withdraw = false;
        yield return getChoices(new List<string> { "継続", "撤退" },
            endProcess: result => withdraw = result == 1,
            ableCancel: true,
            pivot: TextAnchor.MiddleCenter);
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
        Debug.Log($"Start Stage {this}.");
        visualizePlayer();
        sysPlayer.position = initialPlayerPosition;
        if(!isSystem) sysPlayer.deleteArmorBar();

        nowStageAction = StartCoroutine(baseStageAction());
    }
    protected void endStageProcess()
    {
        StopCoroutine(nowStageActionMain);
        StopCoroutine(nowStageAction);
        sysPlayer.canRecieveKey = false;
        nowStageAction = null;
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
        destroyAll(true);
        if(scenery != null) Destroy(scenery.gameObject);

        isContinue = true;
        selfDestroy();
        sys.Start();
        yield break;
    }
    protected virtual IEnumerator successAction()
    {
        var text = setWindowWithText(setSysText("Success", charSize: 24));
        yield return wait(12000, Configs.Buttom.Z);
        text.selfDestroy();
        yield break;
    }
    protected virtual IEnumerator faultAction()
    {
        var text = setWindowWithText(setSysText("Fault", charSize: 24));
        yield return wait(12000, Configs.Buttom.Z);
        text.selfDestroy();
        yield break;
    }

    public Vector2 resetView()
    {
        return viewPosition = initialViewPosition;
    }

    private IEnumerator baseStageAction()
    {
        yield return openingAction();

        if(!isSystem)
        {
            sysPlayer.deleteArmorBar();
            sysPlayer.setArmorBar();
            sysPlayer.canRecieveKey = true;
            sysPlayer.resetAllAlignment(viewPosition - sysPlayer.position - correctWidthVector(sysPlayer.armRoot));

            sys.playerHPbar = getBar(BarType.HP, Color.red);
            sys.playerBRbar = getBar(BarType.BR, Color.cyan);
            sys.playerENbar = getBar(BarType.EN, Color.yellow);
            sys.playerHPbar.nowSort = Configs.SortLayers.SYSTEM_STATE;
            sys.playerBRbar.nowSort = Configs.SortLayers.SYSTEM_STATE;
            sys.playerENbar.nowSort = Configs.SortLayers.SYSTEM_STATE;
        }

        nowStageActionMain = StartCoroutine(stageAction());
        yield return nowStageActionMain;

        yield return wait(() => isComplete);
        isSuccess = true;
    }

    protected virtual IEnumerator openingAction()
    {
        yield break;
    }
    protected virtual IEnumerator stageAction()
    {
        yield break;
    }
    protected virtual bool isComplete { get { return !allEnemies.Any(); } }

    protected static List<Npc> allEnemies => getAllObject<Npc>(target => target.nowLayer != sysPlayer.nowLayer);
    protected static List<Npc> allEnemiesInField => getAllObject<Npc>(target => target.nowLayer != sysPlayer.nowLayer && target.inField);

    /// <summary>
    /// 1波クリアを待つ
    /// </summary>
    /// <param name="interval">1波の時間制限</param>
    /// <returns>イテレータ</returns>
    protected IEnumerator waitWave(int interval = 0)
    {
        yield return wait(() => allEnemiesInField.Any());
        if(interval > 0) yield return wait(interval, () => !allEnemiesInField.Any());
        else yield return wait(() => !allEnemiesInField.Any());
        yield break;
    }
    /// <summary>
    /// ステージ中でのメッセージ表示と待機
    /// </summary>
    /// <param name="message">表示メッセージ</param>
    /// <returns>イテレータ</returns>
    protected static IEnumerator waitMessages(string speaker, IEnumerable<string> messages)
    {
        foreach(var message in messages) yield return waitMessage(message, speaker);
        yield break;
    }
    /// <summary>
    /// ステージ中でのメッセージ表示と待機
    /// </summary>
    /// <param name="message">表示メッセージ</param>
    /// <returns>イテレータ</returns>
    protected static IEnumerator waitMessage(string message, string speaker = null)
    {
        sysPlayer.canRecieveKey = false;
        var window = setWindowWithText(setSysText(message, mainTextPosition, charSize: Configs.DEFAULT_TEXT_SIZE + 1));
        var nameWindow = speaker != null
            ? setWindowWithText(setSysText(speaker,
            new Vector2(window.underLeft.x, window.upperRight.y),
            TextAnchor.LowerLeft,
            Configs.DEFAULT_TEXT_SIZE - 1), 0)
            : null;
        yield return waitKey(Configs.Buttom.Z);
        window.selfDestroy();
        yield return wait(() => Input.GetKeyUp(Configs.Buttom.Z));
        nameWindow?.selfDestroy(false);
        sysPlayer.canRecieveKey = true;
    }

    /// <summary>
    ///オブジェクト配置関数
    /// </summary>
    protected Things setObject(Things obj, Vector2 coordinate)
    {
        if(nextDestroy) return null;
        if(sys.nowStage != this) return null;
        Debug.Log($"{this}\t: {obj}\t: {coordinate}");

        Vector2 precisionCoordinate = fieldArea.scaling(coordinate) / 2;

        var newObject = Instantiate(obj);
        newObject.nowParent = sysPanel.transform;
        newObject.position = precisionCoordinate;

        return newObject;
    }
    /// <summary>
    ///NPC機体配置関数
    /// </summary>
    protected Npc setEnemy(Npc npc,
        Vector2 coordinate,
        float? normalCourseAngle = null,
        ulong? levelCorrection = null,
        int? activityLimit = null,
        bool onTheWay = true,
        string setLayer = Configs.Layers.ENEMY)
    {
        if(npc == null) return null;
        var setedNpc = (Npc)setObject(npc, coordinate);
        if(setedNpc == null) return null;

        setedNpc.normalCourse = normalCourseAngle?.recalculation() ?? Vector2.left;
        setedNpc.shipLevel = levelCorrection ?? stageLevel;
        setedNpc.activityLimit = activityLimit ?? 0;
        setedNpc.nowLayer = setLayer;
        setedNpc.onTheWay = onTheWay;

        return setedNpc;
    }
    /// <summary>
    ///NPC機体配置関数
    /// </summary>
    protected Npc setEnemy(int npcIndex,
        Vector2 coordinate,
        float? normalCourseAngle = null,
        ulong? levelCorrection = null,
        int? activityLimit = null,
        bool onTheWay = true,
        string setLayer = Configs.Layers.ENEMY)
    {
        if(npcIndex < 0) return null;
        if(npcIndex >= enemyList.Count) return null;

        var setedNpc = setEnemy(enemyList[npcIndex], coordinate, normalCourseAngle, levelCorrection, activityLimit, onTheWay, setLayer);
        if(setedNpc?.privateBgm != null) setBGM(setedNpc.privateBgm);
        return setedNpc;
    }
    /// <summary>
    ///NPC機体配置関数
    /// </summary>
    protected Npc setEnemy(int npcIndex,
        float coordinateX,
        float coordinateY,
        float? normalCourseAngle = null,
        ulong? levelCorrection = null,
        int? activityLimit = null,
        bool onTheWay = true,
        string setLayer = Configs.Layers.ENEMY)
        => setEnemy(npcIndex, new Vector2(coordinateX, coordinateY), normalCourseAngle, levelCorrection, activityLimit, onTheWay, setLayer);
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
