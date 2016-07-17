using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hit : Effect
{
    [SerializeField]
    private List<Sprite> spriteSet = new List<Sprite>();

    // Update is called once per frame
    public override void Start()
    {
        base.Start();
        StartCoroutine(annimationSprite());
    }

    private IEnumerator annimationSprite()
    {
        for (int spriteNum = 0; spriteNum < spriteSet.Count; spriteNum++)
        {
            GetComponent<SpriteRenderer>().sprite = spriteSet[spriteNum];
            yield return wait(6);
        }
        Destroy(gameObject);
    }
}
