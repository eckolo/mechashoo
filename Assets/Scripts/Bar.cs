using UnityEngine;
using System.Collections;

public class Bar : Material
{
    public Vector2 setLanges(float now, float max, float maxPixel, Vector2? basePosition = null)
    {
        float nowWidth = maxPixel * now / max;
        float nowHeight = Mathf.Min(maxPixel * baseSize.y / baseSize.x, 0.5f);

        transform.localScale = new Vector2(nowWidth / baseSize.x, nowHeight / baseSize.y);
        Vector2 parentPosition = transform.parent != null
            ? (Vector2)transform.parent.transform.position
            : Vector2.zero;
        transform.position = parentPosition
            + (basePosition ?? Camera.main.ViewportToWorldPoint(new Vector2(0, 1)))
            + new Vector2(nowWidth / 2, -nowHeight / 2);

        setAlpha(0.47f);

        return new Vector2(nowWidth, nowHeight);
    }
}
