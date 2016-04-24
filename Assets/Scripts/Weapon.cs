using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    public bool canAction;
    public Vector2 injectionHole = new Vector2(0.25f, 0);

    private bool canStartAction = true;
    // 弾を撃つ間隔
    public float shotDelay;
    // 弾を撃つ間隔
    public int fileNum;
    // 弾を撃つ間隔
    public float actionDelay;

    // 弾のPrefab
    public GameObject Bullet;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool Action(Transform origin, int actionNumber = 0)
    {
        if (!canStartAction) return false;
        canStartAction = false;
        StartCoroutine(Barst(origin, fileNum));
        return true;
    }

    // 弾の作成
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
    private IEnumerator Shot(Transform origin)
    {
        var injectionHoleLocal = new Vector2(
             injectionHole.x * transform.lossyScale.x,
             injectionHole.y * transform.lossyScale.y
            );
        var instantiatedBullet = Instantiate(Bullet, (Vector2)transform.position + (Vector2)(transform.rotation * injectionHoleLocal), origin.rotation);
        ((GameObject)instantiatedBullet).transform.localScale = transform.lossyScale;

        // ショット音を鳴らす
        //GetComponent<AudioSource>().Play();

        yield break;
    }
}
