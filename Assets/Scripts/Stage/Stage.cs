using UnityEngine;
using System.Collections;

/// <summary>
///各ステージ動作の基底クラス
/// </summary>
public class Stage : MonoBehaviour
{
    /// <summary>
    ///経過時間
    /// </summary>
    protected ulong elapsedFlame = 0;

    /// <summary>
    ///ステージの難易度
    ///オプションからの難易度設定とか用
    /// </summary>
    protected ulong stageLevel = 1;

    // Use this for initialization
    public virtual void Start()
    {
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
    ///ＮＰＣ機体配置関数
    /// </summary>
    protected Npc setEnemy(Npc obj, Vector2 coordinate, ulong? levelCorrection = null)
    {
        if (obj == null) return null;
        var newObject = (Npc)setObject(obj, coordinate);
        newObject.shipLevel = levelCorrection ?? stageLevel;

        return newObject;
    }
}
