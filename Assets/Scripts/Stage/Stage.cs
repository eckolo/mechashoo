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
    protected int elapsedFlame = 0;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(stageAction());
    }

    // Update is called once per frame
    void Update()
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
    protected Material setObject(Material setedObject, Vector2 coordinate)
    {
        Vector2 precisionCoordinate = Camera.main.ViewportToWorldPoint(coordinate + Vector2.right);

        var newObject = (Material)Instantiate(setedObject, precisionCoordinate, transform.rotation);

        return newObject;
    }
}
