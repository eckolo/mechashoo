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

    public bool Action()
    {
        if (!canAction) return false;
        if (!canStartAction) return false;

        canStartAction = false;
        StartCoroutine(baseMotion());
        return true;
    }
    private IEnumerator baseMotion()
    {
        yield return StartCoroutine(Motion());

        yield return StartCoroutine(wait(actionDelay));
        yield return StartCoroutine(endMotion());
        canStartAction = true;

        yield break;
    }

    protected virtual IEnumerator Motion()
    {
        injection(0);
        yield break;
    }
    protected virtual IEnumerator endMotion()
    {
        yield break;
    }

    /// <summary>
    ///指定フレーム数待機する関数
    ///yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected virtual IEnumerator wait(int delay)
    {
        for (var i = 0; i < actionDelay; i++) yield return null;
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
