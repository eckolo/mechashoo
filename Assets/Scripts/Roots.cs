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

    protected static class easing
    {
        public class BaseEaaing
        {
            public virtual float In(float max, int time, int limit)
            {
                return max;
            }
            public float Out(float max, int time, int limit)
            {
                return max - In(max, limit - time, limit);
            }
            public float InOut(float max, int time, int limit)
            {
                return time < limit / 2
                    ? In(max / 2, time, limit / 2)
                    : Out(max / 2, time - limit / 2, limit / 2);
            }
        }
        public class linear : BaseEaaing
        {
            public override float In(float max, int time, int limit)
            {
                return max * time / limit;
            }
        }
        public class quadratic : BaseEaaing
        {
            public override float In(float max, int time, int limit)
            {
                return max * (time / limit) * (time / limit);
            }
        }
        public class cubic : BaseEaaing
        {
            public override float In(float max, int time, int limit)
            {
                return max * (time / limit) * (time / limit) * (time / limit);
            }
        }
        public class quartic : BaseEaaing
        {
            public override float In(float max, int time, int limit)
            {
                return max * (time / limit) * (time / limit) * (time / limit) * (time / limit);
            }
        }
        public class quintic : BaseEaaing
        {
            public override float In(float max, int time, int limit)
            {
                return max * (time / limit) * (time / limit) * (time / limit) * (time / limit) * (time / limit);
            }
        }
        public class sinusoidal : BaseEaaing
        {
            public override float In(float max, int time, int limit)
            {
                return -max * Mathf.Cos(time / limit * (Mathf.PI / 2)) + max;
            }
        }
        public class exponential : BaseEaaing
        {
            public override float In(float max, int time, int limit)
            {
                return max * Mathf.Pow(2, 10 * (time / limit - 1));
            }
        }
        public class circular : BaseEaaing
        {
            public override float In(float max, int time, int limit)
            {
                return -max * (Mathf.Sqrt(1 - (time / limit) * (time / limit)) - 1);
            }
        }
    }
}
