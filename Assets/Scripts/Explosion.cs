using UnityEngine;
using System.Collections;

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
        transform.localScale *= 1.05f;
        color.a *= 0.9f;
        GetComponent<SpriteRenderer>().color = color;
    }
}
