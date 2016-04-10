using UnityEngine;
using System.Collections;

public class Barst : Motion
{
    // 弾を撃つ間隔
    public float shotDelay;

    // 弾のPrefab
    public GameObject Bullet;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    public new bool actionroot(Transform origin)
    {
        return Shot(origin);
    }

    // 弾の作成
    private bool Shot(Transform origin)
    {
        Instantiate(Bullet, origin.position, origin.rotation);

        return true;

        // ショット音を鳴らす
        //GetComponent<AudioSource>().Play();
    }
}
