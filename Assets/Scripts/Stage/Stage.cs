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
    private MeshRenderer scenery = null;

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

    /// <summary>
    ///ステージアクションのコルーチンを所持する変数
    /// </summary>
    public IEnumerator nowStageAction = null;

    // Use this for initialization
    public virtual void Start()
    {
        setScenery();
        getPlayer().transform.position = initialPlayerPosition;
        points = 0;

        StartCoroutine(nowStageAction = stageAction());
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (!judgeContinue())
        {
            StopCoroutine(nowStageAction);
            stopStageAction();
        }
        elapsedFlame += 1;
    }

    private bool judgeContinue()
    {
        if (!getPlayer().isAlive()) return false;
        return true;
    }
    protected void stopStageAction()
    {
        Terms term = T => T.GetComponent<Ship>() != null || T.GetComponent<Bullet>() != null;
        foreach (var target in getAllObject(term))
        {
            Destroy(target.gameObject);
        }

        Destroy(scenery.gameObject);
        Destroy(gameObject);

        getSystem().Start();
        return;
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
            Destroy(oldScenery.gameObject);
        }
        scenery = ((GameObject)Instantiate((buckGround ?? initialScenery).gameObject, new Vector2(0, 0), transform.rotation)).GetComponent<MeshRenderer>();
        scenery.transform.parent = baseScenery.transform;
    }
}
