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
    ///経過時間
    /// </summary>
    protected ulong elapsedFlame = 0;

    /// <summary>
    ///初期背景
    /// </summary>
    [SerializeField]
    protected MeshRenderer initialScenery;

    /// <summary>
    ///プレイヤー機の初期位置
    /// </summary>
    [SerializeField]
    private Vector2 initialPlayerPosition = new Vector2(-3.6f, 0);

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

    // Use this for initialization
    public virtual void Start()
    {
        setScenery();
        GameObject.Find("player").transform.position = initialPlayerPosition;
        points = 0;

        StartCoroutine(stageAction());
    }

    // Update is called once per frame
    public virtual void Update()
    {
        elapsedFlame += 1;
    }

    protected virtual IEnumerator stageAction()
    {
        yield break;
    }

    /// <summary>
    ///オブジェクト配置関数
    /// </summary>
    protected Material setObject(Material obj, Vector2 coordinate)
    {
        Vector2 precisionCoordinate = Camera.main.ViewportToWorldPoint(coordinate + Vector2.right);

        var newObject = (Material)Instantiate(obj, precisionCoordinate, transform.rotation);

        return newObject;
    }
    /// <summary>
    ///NPC機体配置関数
    /// </summary>
    protected Npc setEnemy(Npc obj, Vector2 coordinate, ulong? levelCorrection = null)
    {
        if (obj == null) return null;
        var newObject = (Npc)setObject(obj, coordinate);
        newObject.shipLevel = levelCorrection ?? stageLevel;

        return newObject;
    }
    /// <summary>
    ///背景設定関数
    ///初期値はStageの初期背景
    /// </summary>
    protected void setScenery(MeshRenderer buckGround = null)
    {
        var baseScenery = GameObject.Find("SceneryRoot");
        foreach (Transform oldScenery in baseScenery.transform)
        {
            Destroy(oldScenery);
        }
        ((GameObject)Instantiate((buckGround ?? initialScenery).gameObject, new Vector2(0, 0), transform.rotation)).transform.parent = baseScenery.transform;
    }
}
