using UnityEngine;
using System.Collections;

public class Bar : Roots
{
    public float setLanges(float now, float max, float maxPixel, Vector2? basePosition = null)
    {
        float widthBasePixel = 160 / GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        var nowPixel = maxPixel * now / max;

        transform.localScale = new Vector2(nowPixel / widthBasePixel, transform.localScale.y);
        transform.position = (basePosition ?? Camera.main.ViewportToWorldPoint(new Vector2(0, 1)))
            + new Vector2(nowPixel / 2, -widthBasePixel / 10 / 2);

        return nowPixel;
    }
}
