using UnityEngine;
using System.Collections;

public class Bar : Materials
{
    public Vector2 setLanges(float now, float max, float maxPixel, Vector2? basePosition = null, bool pibotView = false)
    {
        float nowWidth = max != 0 ? maxPixel * now / max : 0;
        float nowHeight = Mathf.Min(maxPixel * spriteSize.y / spriteSize.x, 0.5f);

        transform.localScale = new Vector2(nowWidth / spriteSize.x, nowHeight / spriteSize.y);
        position = (pibotView ? new Vector2(-viewSize.x, viewSize.y) / 2 : Vector2.zero)
            + (basePosition ?? Vector2.zero)
            + new Vector2(nowWidth / 2, -nowHeight / 2);

        nowAlpha = 0.47f;

        return new Vector2(nowWidth, nowHeight);
    }
}
