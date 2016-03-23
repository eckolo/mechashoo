using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    // 移動スピード
    public float speed;

    // 弾を撃つ間隔
    public float shotDelay;
    private float delayCount;

    // 弾のPrefab
    public GameObject bullet;

    // 弾を撃つかどうか
    public bool canShot;

    // 爆発のPrefab
    public GameObject explosion;

    // アニメーターコンポーネント
    private Animator animator;

    // Use this for initialization
    void Start()
    {
        delayCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // 右・左
        float x = Input.GetAxisRaw("Horizontal");

        // 上・下
        float y = Input.GetAxisRaw("Vertical");

        // 発射
        var pushedZ = Input.GetKey("z");

        // 移動する向きを求める
        Vector2 direction = new Vector2(x, y).normalized;

        // 移動の制限
        Move(direction);

        if (delayCount-- <= 0)
        {
            delayCount = shotDelay;

            if (pushedZ)
            {
                // 弾をプレイヤーと同じ位置/角度で作成
                Shot(transform);

                // ショット音を鳴らす
                GetComponent<AudioSource>().Play();
            }
        }
    }

    // 機体の移動
    void Move(Vector2 direction)
    {
        // 画面左下のワールド座標をビューポートから取得
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));

        // 画面右上のワールド座標をビューポートから取得
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        // プレイヤーの座標を取得
        Vector2 pos = transform.position;

        // 移動量を加える
        pos += direction * speed * Time.deltaTime;

        // プレイヤーの位置が画面内に収まるように制限をかける
        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);

        // 制限をかけた値をプレイヤーの位置とする
        transform.position = pos;
    }

    // 弾の作成
    public void Shot(Transform origin)
    {
        Instantiate(bullet, origin.position, origin.rotation);
    }
}
