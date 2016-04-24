using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ship : MonoBehaviour
{
    // 移動スピード
    public float speed;
    public List<GameObject> weapons = new List<GameObject>();

    // 爆発のPrefab
    public GameObject explosion;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 機体の移動
    public void Move(Vector2 direction, float inputSpeed)
    {
        // プレイヤーの座標を取得
        Vector2 pos = transform.position;

        // 移動量を加える
        pos += direction * inputSpeed * Time.deltaTime;

        // 制限をかけた値をプレイヤーの位置とする
        transform.position = pos;
    }

    // ぶつかった瞬間に呼び出される
    void OnTriggerEnter2D(Collider2D c)
    {
        // 弾の削除
        Destroy(c.gameObject);

        // 爆発する
        Explosion();

        // プレイヤーを削除
        Destroy(gameObject);
    }

    // 爆発の作成
    public void Explosion()
    {
        Instantiate(explosion, transform.position, transform.rotation);
    }
}
