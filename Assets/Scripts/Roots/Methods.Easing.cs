using UnityEngine;
using System.Collections;

public partial class Methods : MonoBehaviour
{
    /// <summary>
    ///イージング関数群
    /// </summary>
    protected static Easing easing = new Easing();

    /// <summary>
    ///ベクトルイージング関数群
    /// </summary>
    public static class MathVEasing
    {
        /// <summary>
        ///始点から終点まで円軌道を描く
        /// </summary>
        public static Vector2 elliptical(Vector2 end, float time, float limit, bool clockwise)
        {
            bool verticalIn = clockwise ^ (end.x * end.y > 0);
            float right = verticalIn
                ? easing.sinusoidal.Out(end.x, time, limit)
                : easing.sinusoidal.In(end.x, time, limit);
            float up = verticalIn
                ? easing.sinusoidal.In(end.y, time, limit)
                : easing.sinusoidal.Out(end.y, time, limit);
            return new Vector2(right, up);
        }
        /// <summary>
        ///始点から終点まで円軌道を描く
        /// </summary>
        public static Vector2 elliptical(Vector2 start, Vector2 end, float time, float limit, bool clockwise) => start + elliptical(end - start, time, limit, clockwise);
    }

    protected class Easing
    {
        /// <summary>
        ///線形変動
        /// </summary>
        public Linear liner = new Linear();
        /// <summary>
        ///二乗変動
        /// </summary>
        public Quadratic quadratic = new Quadratic();
        /// <summary>
        ///三乗変動
        /// </summary>
        public Cubic cubic = new Cubic();
        /// <summary>
        ///四乗変動
        /// </summary>
        public Quartic quartic = new Quartic();
        /// <summary>
        ///五乗変動
        /// </summary>
        public Quintic quintic = new Quintic();
        /// <summary>
        ///円形変動
        /// </summary>
        public Sinusoidal sinusoidal = new Sinusoidal();
        /// <summary>
        ///累乗変動
        /// </summary>
        public Exponential exponential = new Exponential();
        /// <summary>
        ///乗根変動
        /// </summary>
        public Circular circular = new Circular();

        public class BaseEaaing
        {
            public virtual float In(float max, float time, float limit)
            {
                Debug.Log(max);
                return max;
            }
            public float In(float time, float limit) => In(1, time, limit);
            public float SubIn(float max, float time, float limit) => max - In(max, time, limit);
            public float SubIn(float time, float limit) => SubIn(1, time, limit);

            public float Out(float max, float time, float limit) => max - In(max, limit - time, limit);
            public float Out(float time, float limit) => Out(1, time, limit);
            public float SubOut(float max, float time, float limit) => max - Out(max, time, limit);
            public float SubOut(float time, float limit) => SubOut(1, time, limit);

            public float InOut(float max, float time, float limit)
                => time < limit / 2
                ? In(max / 2, time, limit / 2)
                : Out(max / 2, time - limit / 2, limit / 2) + max / 2;
            public float InOut(float time, float limit) => InOut(1, time, limit);
            public float SubInOut(float max, float time, float limit) => max - InOut(max, time, limit);
            public float SubInOut(float time, float limit) => SubInOut(1, time, limit);
        }
        public class Linear : BaseEaaing
        {
            public override float In(float max, float time, float limit)
                => max * time / limit;
        }
        public class Quadratic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
                => max * time * time / limit / limit;
        }
        public class Cubic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
                => max * time * time * time / limit / limit / limit;
        }
        public class Quartic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
                => max * time * time * time * time / limit / limit / limit / limit;
        }
        public class Quintic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
                => max * time * time * time * time * time / limit / limit / limit / limit / limit;
        }
        public class Sinusoidal : BaseEaaing
        {
            public override float In(float max, float time, float limit)
                => max * (1 - Mathf.Cos(time * Mathf.PI / limit / 2));
        }
        public class Exponential : BaseEaaing
        {
            public override float In(float max, float time, float limit)
                => max * Mathf.Pow(2, 10 * (time - limit) / limit);
        }
        public class Circular : BaseEaaing
        {
            public override float In(float max, float time, float limit)
                => -max * (Mathf.Sqrt(1 - time * time / limit / limit) - 1);
        }
    }
}
