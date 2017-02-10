using UnityEngine;
using System.Collections;

public class Easing
{
    /// <summary>
    ///線形変動
    /// </summary>
    public static Linear liner = new Linear();
    /// <summary>
    ///二乗変動
    /// </summary>
    public static Quadratic quadratic = new Quadratic();
    /// <summary>
    ///三乗変動
    /// </summary>
    public static Cubic cubic = new Cubic();
    /// <summary>
    ///四乗変動
    /// </summary>
    public static Quartic quartic = new Quartic();
    /// <summary>
    ///五乗変動
    /// </summary>
    public static Quintic quintic = new Quintic();
    /// <summary>
    ///円形変動
    /// </summary>
    public static Sinusoidal sinusoidal = new Sinusoidal();
    /// <summary>
    ///累乗変動
    /// </summary>
    public static Exponential exponential = new Exponential();
    /// <summary>
    ///乗根変動
    /// </summary>
    public static Circular circular = new Circular();

    public abstract class BaseEaaing
    {
        public abstract float In(float max, float time, float limit);
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
