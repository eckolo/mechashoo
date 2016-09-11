using UnityEngine;
using System.Collections;

public class Bar : Material
{
    public Vector2 setLanges(float now, float max, float maxPixel, Vector2? basePosition = null)
    {
        float nowWidth = maxPixel * now / max;
        float nowHeight = Mathf.Min(maxPixel * baseSize.y / baseSize.x, 0.5f);

        transform.localScale = new Vector2(nowWidth / baseSize.x, nowHeight / baseSize.y);
        transform.localPosition = (basePosition ?? new Vector2(-viewSize.x, viewSize.y) / 2)
            + new Vector2(nowWidth / 2, -nowHeight / 2);

        setAlpha(0.47f);

        return new Vector2(nowWidth, nowHeight);
    }
}
