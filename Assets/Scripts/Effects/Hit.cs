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
        StartCoroutine(AnnimationSprite());
    }

    private IEnumerator AnnimationSprite()
    {
        for(int spriteNum = 0; spriteNum < spriteSet.Count; spriteNum++)
        {
            nowSprite = spriteSet[spriteNum];
            yield return Wait(6);
        }
        DestroyMyself();
    }
}
