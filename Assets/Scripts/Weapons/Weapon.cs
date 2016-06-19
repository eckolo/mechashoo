using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : Parts
{
    //現在攻撃動作可能かどうかの判定フラグ
    public bool canAction = true;
    //持ち手の座標
    public Vector2 handlePosition = new Vector2(0, 0);
    //射出孔のリスト
    public List<Vector2> injectionHoles = new List<Vector2>();
    // アクション毎の間隔
    public int actionDelay;
    // 弾のPrefab
    public Bullet Bullet;

    //攻撃動作開始可能かどうかの内部フラグ
    [SerializeField]
    protected bool canStartAction = true;
    //ブレ補正の強度
    [SerializeField]
    protected float recoilReturn = 0;

    public override void Update()
    {
        base.Update();
        updateRecoil();
    }

    public override bool Action(int actionNum = 0)
    {
        if (!canAction) return false;
        if (!canStartAction) return false;

        canStartAction = false;
        base.Action(actionNum);

        return true;
    }
    protected override IEnumerator baseMotion(int actionNum)
    {
        yield return base.baseMotion(actionNum);

        if (actionDelay > 0) yield return wait(actionDelay);
        yield return endMotion();

        canStartAction = true;

        yield break;
    }

    protected override IEnumerator Motion(int actionNum)
    {
        injection(actionNum);
        yield break;
    }
    protected virtual IEnumerator endMotion()
    {
        yield break;
    }

    // 弾の作成
    protected Bullet injection(int injectionNum = 0, Bullet injectionBullet = null)
    {
        if ((Bullet = injectionBullet ?? Bullet) == null) return null;

        if (injectionHoles.Count <= 0) return null;
        injectionNum = injectionNum % injectionHoles.Count;

        var injectionHoleLocal = new Vector2(
          (transform.rotation * injectionHoles[injectionNum]).x * getLossyScale(transform).x,
          (transform.rotation * injectionHoles[injectionNum]).y * getLossyScale(transform).y * (heightPositive ? 1 : -1)
         );
        var instantiatedBullet = (Bullet)Instantiate(Bullet, (Vector2)transform.position + injectionHoleLocal, Quaternion.Euler(transform.rotation.eulerAngles * getLossyScale(transform).x / Mathf.Abs(getLossyScale(transform).x)));
        instantiatedBullet.gameObject.layer = gameObject.layer;
        // ショット音を鳴らす
        //GetComponent<AudioSource>().Play();

        return instantiatedBullet;
    }

    //反動関数
    public Vector2 startRecoil(Vector2 setRecoil)
    {
        correctionVector += setRecoil;

        return correctionVector;
    }
    private void updateRecoil()
    {
        correctionVector /= (recoilReturn + 1);
    }
}
