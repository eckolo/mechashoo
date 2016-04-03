using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    // 弾を撃つ間隔
    public float shotDelay;

    // 弾のPrefab
    public GameObject Bullet;

    // 弾を撃つかどうか
    public bool canShot;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            // 弾をプレイヤーと同じ位置/角度で作成
            Shot(transform);

            // ショット音を鳴らす
            //GetComponent<AudioSource>().Play();
        }

    }

    // 弾の作成
    public void Shot(Transform origin)
    {
        Instantiate(Bullet, origin.position, origin.rotation);
    }
}
