using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Material : MonoBehaviour
{
    //制御下のPartsリスト
    [SerializeField]
    private List<Parts> childPartsList = new List<Parts>();

    // 機体の移動
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
    public virtual void setVerosityAction(Vector2 verosity, float speed) { }

    public int setParts(Parts setedParts)
    {
        if (setedParts == null) return -1;

        childPartsList.Add(setedParts);
        setedParts.setParent(gameObject.GetComponent<Material>());

        return childPartsList.Count - 1;
    }
    public Parts getParts(int sequenceNum)
    {
        return childPartsList[sequenceNum];
    }

    //mainのベクトルをsubに合わせて補正する
    protected Vector2 correctVector(Vector2 main, Vector2 sub, float degree = 0.5f)
    {
        var adjustedSub = sub.normalized * main.magnitude;
        return main * degree + adjustedSub * (1 - degree);
    }
}
