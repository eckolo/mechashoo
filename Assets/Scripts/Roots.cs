using UnityEngine;
using System.Collections;

public class Roots : MonoBehaviour
//あらゆるオブジェクトの基底関数とか
{
    //発生直後を-1としての経過フレーム数
    [SerializeField]
    protected int counter = -1;

    // オブジェクトの移動
    public void setVerosity(Vector2 verosity, float speed = 0, bool inScreen = false)
    {
        // 実移動量を計算
        var innerVerosity = verosity.normalized * speed;

        if (inScreen)
        {
            // オブジェクトの座標を取得
            var self = transform.position;

            // 画面左下のワールド座標をビューポートから取得
            var lowerLeft = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));

            // 画面右上のワールド座標をビューポートから取得
            var upperRight = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

            // オブジェクトの位置が画面内に収まるように制限をかける
            innerVerosity.x = Mathf.Clamp(
                innerVerosity.x,
                (lowerLeft.x - self.x) * 100,
                (upperRight.x - self.x) * 100);
            innerVerosity.y = Mathf.Clamp(
                innerVerosity.y,
                (lowerLeft.y - self.y) * 100,
                (upperRight.y - self.y) * 100);
        }

        //速度設定
        GetComponent<Rigidbody2D>().velocity = innerVerosity;

        //移動時アクション呼び出し
        setVerosityAction(verosity, speed);
    }
    protected virtual void setVerosityAction(Vector2 verosity, float speed) { }
}
