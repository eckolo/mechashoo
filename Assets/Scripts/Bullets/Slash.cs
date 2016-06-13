using UnityEngine;
using System.Collections;

public class Slash : Bullet
{
    //最終的なサイズ
    [SerializeField]
    private float limitSize = 1;
    //最大化までの所要時間
    [SerializeField]
    private int maxSizeTime = 10;
    //残存時間
    [SerializeField]
    private int destroyLimit = 20;

    //パラメータのセット
    public void setParamate(float? size = null, int? maxlim = null, int? destroylim = null)
    {
        limitSize = size ?? limitSize;
        maxSizeTime = maxlim ?? maxSizeTime;
        destroyLimit = destroylim ?? destroyLimit;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        if (counter > destroyLimit) selfDestroy();
        base.Update();
        updateScale();
        updateAlpha();
        setVerosity(
            transform.rotation * Vector2.right
            , counter < maxSizeTime ? limitSize / maxSizeTime : 0
            );
    }

    private void updateScale()
    {
        var nowSizeX = counter < maxSizeTime
            ? easing.cubic.Out(limitSize, counter, maxSizeTime)
            : limitSize;
        var nowSizeY = counter < destroyLimit
            ? easing.quadratic.Out(limitSize / 3, counter, destroyLimit)
            : limitSize / 3;
        transform.localScale = new Vector2(nowSizeX, nowSizeY);
    }

    private void updateAlpha()
    {
        var color = GetComponent<SpriteRenderer>().color;

        color.a = 1 - easing.quintic.In(1, counter, destroyLimit);
        GetComponent<SpriteRenderer>().color = color;
    }
}
