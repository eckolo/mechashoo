﻿using UnityEngine;
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
    public string displayName = "";
    /// <summary>
    ///特殊ステージフラグ
    /// </summary>
    public bool isSystem = false;

    /// <summary>
    ///経過時間
    /// </summary>
    protected ulong elapsedFlame = 0;

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

    public string stageName
    {
        get
        {
            if (displayName != null && displayName != "") return displayName;
            if (gameObject != null) return gameObject.name.Replace("(Clone)", "");
            return displayName;
        }
    }
    public bool isClear
    {
        get
        {
            return Sys.getClearFlug(this);
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
        if (!isContinue) stopStageAction();
        elapsedFlame += 1;
    }

    protected bool isContinue
    {
        get
        {
            if (sysPlayer != null && sysPlayer.isExist && !sysPlayer.isAlive) return false;
            return true;
        }
    }
    public virtual void startStageAction()
    {
        visualizePlayer();
        sysPlayer.transform.localPosition = initialPlayerPosition;
        sysPlayer.deleteArmorBar();
        sysPlayer.setArmorBar();
        sysPlayer.canRecieveKey = true;

        Sys.playerHPbar = getBar(barType.HPbar, Color.red);
        Sys.playerBRbar = getBar(barType.BRbar, Color.cyan);
        Sys.playerENbar = getBar(barType.ENbar, Color.yellow);

        StartCoroutine(nowStageAction = stageAction());
    }
    protected void stopStageAction()
    {
        StopCoroutine(nowStageAction);
        nowStageAction = null;

        destroyAll();
        if (Sys.playerHPbar != null) Sys.playerHPbar.setAlpha(0);
        if (Sys.playerBRbar != null) Sys.playerBRbar.setAlpha(0);
        if (Sys.playerENbar != null) Sys.playerENbar.setAlpha(0);
        resetView();
        if (scenery != null) Destroy(scenery.gameObject);

        Sys.Start();
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
    protected Npc setEnemy(Npc obj, Vector2 coordinate, ulong? levelCorrection = null)
    {
        if (obj == null) return null;
        var newObject = (Npc)setObject(obj, coordinate + Vector2.right);
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
        if (setBuckGround == null) return null;

        var baseScenery = GameObject.Find("SceneryRoot");
        foreach (Transform oldScenery in baseScenery.transform)
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
        if (setMusic == null) return null;

        var baseMusic = GameObject.Find("MusicRoot");
        foreach (Transform oldMusic in baseMusic.transform)
        {
            Destroy(oldMusic.gameObject);
        }

        var BGM = Instantiate(Sys.BGMrootObject);
        BGM.transform.SetParent(baseMusic.transform);
        BGM.audioSource.clip = setMusic;
        BGM.audioSource.Play();

        return BGM.audioSource;
    }
}
