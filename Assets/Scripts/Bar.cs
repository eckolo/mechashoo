using UnityEngine;
using System.Collections;

public class Bar : Roots
{
    private static int spriteWidth = 160;
    private static int spriteHeight = 16;

    public Vector2 setLanges(float now, float max, float maxPixel, Vector2? basePosition = null)
    {
        float widthBasePixel = spriteWidth / GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        var nowWidth = maxPixel * now / max;

        transform.localScale = new Vector2(nowWidth / widthBasePixel, transform.localScale.y);
        transform.position = (basePosition ?? Camera.main.ViewportToWorldPoint(new Vector2(0, 1)))
            + new Vector2(nowWidth / 2, -widthBasePixel * spriteHeight / spriteWidth / 2);

        return new Vector2(nowWidth, widthBasePixel * spriteHeight / spriteWidth);
    }
}
