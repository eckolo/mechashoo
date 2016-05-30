using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : Parts
{
    //現在攻撃動作可能かどうかの判定フラグ
    public bool canAction;
    //持ち手の座標
    public Vector2 handlePosition = new Vector2(0, 0);
    //射出孔のリスト
    public List<Vector2> injectionHole = new List<Vector2>();
    // 弾を撃つ間隔
    public float shotDelay;
    // 弾を撃つ間隔
    public int fileNum;
    // 弾を撃つ間隔
    public float actionDelay;
    // 弾のPrefab
    public Bullet Bullet;

    //攻撃動作開始可能かどうかの内部フラグ
    [SerializeField]
    private bool canStartAction = true;
    
    public override void Update()
    {
        base.Update();
    }

    public bool Action(Transform origin, int actionNumber = 0)
    {
        if (!canAction) return false;
        if (!canStartAction) return false;
        canStartAction = false;
        StartCoroutine(Barst(origin, fileNum));
        return true;
    }

    // 発射システム
    private IEnumerator Barst(Transform origin, int burstNum = 1)
    {
        for (int i = 0; i < burstNum; i++)
        {
            StartCoroutine(Shot(origin));

            // shotDelay秒待つ
            yield return new WaitForSeconds(shotDelay);
        }

        // actionDelay秒待つ
        yield return new WaitForSeconds(actionDelay);
        canStartAction = true;
    }

    // 弾の作成
    private IEnumerator Shot(Transform origin, int injectionNum = 0)
    {
        var injectionHoleLocal = new Vector2(
             (transform.rotation * injectionHole[injectionNum]).x * getLssyScale(transform).x,
             (transform.rotation * injectionHole[injectionNum]).y * getLssyScale(transform).y
            );
        var instantiatedBullet = (Bullet)Instantiate(Bullet, (Vector2)transform.position + injectionHoleLocal, Quaternion.Euler(origin.rotation.eulerAngles * getLssyScale(origin).x / Mathf.Abs(getLssyScale(origin).x)));
        instantiatedBullet.gameObject.layer = gameObject.layer;
        instantiatedBullet.transform.localScale = getLssyScale(transform);
        instantiatedBullet.velocity = new Vector2(
            (transform.rotation * Vector2.right).x * getLssyScale(transform).x,
            (transform.rotation * Vector2.right).y * getLssyScale(transform).y
            );

        // ショット音を鳴らす
        //GetComponent<AudioSource>().Play();

        yield break;
    }

    public Vector2 getLssyScale(Transform origin)
    {
        var next = origin.parent != null ? getLssyScale(origin.parent) : new Vector2(1, 1);
        return new Vector2(origin.localScale.x * next.x, origin.localScale.y * next.y);
    }
}
