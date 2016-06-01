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
    //反動完了までの残り時間
    [SerializeField]
    private int recoilTime = 0;

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
             (transform.rotation * injectionHole[injectionNum]).x * getLossyScale(transform).x,
             (transform.rotation * injectionHole[injectionNum]).y * getLossyScale(transform).y
            );
        var instantiatedBullet = (Bullet)Instantiate(Bullet, (Vector2)transform.position + injectionHoleLocal, Quaternion.Euler(origin.rotation.eulerAngles * getLossyScale(origin).x / Mathf.Abs(getLossyScale(origin).x)));
        instantiatedBullet.gameObject.layer = gameObject.layer;
        instantiatedBullet.transform.localScale = getLossyScale(transform);
        instantiatedBullet.velocity = new Vector2(
            (transform.rotation * Vector2.right).x * getLossyScale(transform).x,
            (transform.rotation * Vector2.right).y * getLossyScale(transform).y
            );

        // ショット音を鳴らす
        //GetComponent<AudioSource>().Play();

        yield break;
    }

    //反動関数
    public Vector2 startRecoil(Vector2 setRecoil, int? remainingTime = null)
    {
        correctionVector = setRecoil;
        recoilTime = remainingTime ?? (int)setRecoil.magnitude;

        return correctionVector;
    }
    private void updateRecoil()
    {
        if (recoilTime >= 0) correctionVector *= recoilTime-- / recoilTime;
    }
}
