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
        StartCoroutine(Barst(origin));
        return true;
    }

    // 弾の作成
    private IEnumerator Barst(Transform origin)
    {
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(Shot(origin));

            // shotDelay秒待つ
            yield return new WaitForSeconds(shotDelay);
        }
        canStartAction = true;
    }

    // 弾の作成
    private IEnumerator Shot(Transform origin)
    {
        var injectionHoleLocal = new Vector2(
             injectionHole.x * transform.lossyScale.x,
             injectionHole.y * transform.lossyScale.y
            );
        Instantiate(Bullet, (Vector2)transform.position + (Vector2)(transform.rotation * injectionHoleLocal), origin.rotation);

        // ショット音を鳴らす
        //GetComponent<AudioSource>().Play();

        yield break;
    }
}
