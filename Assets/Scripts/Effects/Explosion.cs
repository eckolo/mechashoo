using UnityEngine;
using System.Collections;

/// <summary>
/// 爆破エフェクトクラス
/// </summary>
public class Explosion : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var color = GetComponent<SpriteRenderer>().color;

        if (color.a < 0.01f) Destroy(gameObject);
        transform.localScale += new Vector3(1, 1, 1) * 0.02f / transform.localScale.magnitude;
        color.a *= 0.9f;
        GetComponent<SpriteRenderer>().color = color;
    }
}
