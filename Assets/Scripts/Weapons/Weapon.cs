using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 武装クラス
/// </summary>
public class Weapon : Parts
{
    /// <summary>
    ///現在攻撃動作可能かどうかの判定フラグ
    /// </summary>
    public bool canAction = true;
    /// <summary>
    ///持ち手の座標
    /// </summary>
    public Vector2 handlePosition = new Vector2(0, 0);
    /// <summary>
    ///射出孔のリスト
    /// </summary>
    public List<Vector2> injectionHoles = new List<Vector2>();
    /// <summary>
    /// アクション毎の間隔
    /// </summary>
    public int actionDelay;
    /// <summary>
    /// 弾のPrefab
    /// </summary>
    public Bullet Bullet;

    /// <summary>
    ///攻撃動作開始可能かどうか(つまり動作中か否か)の内部フラグ
    /// </summary>
    [SerializeField]
    protected bool notInAction = true;
    /// <summary>
    ///ブレ補正の強度
    /// </summary>
    [SerializeField]
    protected float recoilReturn = 0;

    public override void Update()
    {
        base.Update();
        updateRecoil();
        if (inAction())
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 0.6f, 0.8f, 1);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
        }
    }

    public bool inAction()
    {
        if (parentMaterial == null) return !notInAction;
        if (parentMaterial.GetComponent<Weapon>() == null) return !notInAction;
        return parentMaterial.GetComponent<Weapon>().inAction();
    }

    public override bool Action(int actionNum = 0)
    {
        if (!canAction) return false;
        if (!notInAction) return false;

        notInAction = false;
        base.Action(actionNum);

        return true;
    }
    protected override IEnumerator baseMotion(int actionNum)
    {
        yield return base.baseMotion(actionNum);

        if (actionDelay > 0) yield return wait(actionDelay);
        yield return endMotion();

        notInAction = true;

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

    /// <summary>
    /// 弾の作成
    /// </summary>
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
        instantiatedBullet.transform.localScale = getLossyScale();
        // ショット音を鳴らす
        //GetComponent<AudioSource>().Play();

        return instantiatedBullet;
    }

    /// <summary>
    ///反動関数
    /// </summary>
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
