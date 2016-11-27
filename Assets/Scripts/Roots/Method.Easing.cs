﻿using UnityEngine;
using System.Collections;

public partial class Methods : MonoBehaviour {
    /// <summary>
    ///イージング関数群
    /// </summary>
    protected static Easing easing = new Easing();

    protected class Easing {
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

        public class BaseEaaing {
            public virtual float inner(float max, float time, float limit) {
                Debug.Log(max);
                return max;
            }
            public float inner(float time, float limit) {
                return inner(1, time, limit);
            }
            public float subInner(float max, float time, float limit) {
                return max - inner(max, time, limit);
            }
            public float subInner(float time, float limit) {
                return 1 - inner(1, time, limit);
            }

            public float outer(float max, float time, float limit) {
                return max - inner(max, limit - time, limit);
            }
            public float outer(float time, float limit) {
                return outer(1, time, limit);
            }
            public float subOuter(float max, float time, float limit) {
                return max - outer(max, time, limit);
            }
            public float subOuter(float time, float limit) {
                return 1 - outer(1, time, limit);
            }

            public float inOut(float max, float time, float limit) {
                return time < limit / 2
                    ? inner(max / 2, time, limit / 2)
                    : outer(max / 2, time - limit / 2, limit / 2) + max / 2;
            }
            public float inOut(float time, float limit) {
                return inOut(1, time, limit);
            }
            public float subInOut(float max, float time, float limit) {
                return max - inOut(max, time, limit);
            }
            public float subInOut(float time, float limit) {
                return 1 - inOut(1, time, limit);
            }
        }
        public class Linear : BaseEaaing {
            public override float inner(float max, float time, float limit) {
                return max * time / limit;
            }
        }
        public class Quadratic : BaseEaaing {
            public override float inner(float max, float time, float limit) {
                return max * time * time / limit / limit;
            }
        }
        public class Cubic : BaseEaaing {
            public override float inner(float max, float time, float limit) {
                return max * time * time * time / limit / limit / limit;
            }
        }
        public class Quartic : BaseEaaing {
            public override float inner(float max, float time, float limit) {
                return max * time * time * time * time / limit / limit / limit / limit;
            }
        }
        public class Quintic : BaseEaaing {
            public override float inner(float max, float time, float limit) {
                return max * time * time * time * time * time / limit / limit / limit / limit / limit;
            }
        }
        public class Sinusoidal : BaseEaaing {
            public override float inner(float max, float time, float limit) {
                return -max * Mathf.Cos(time * Mathf.PI / limit / 2) + max;
            }
        }
        public class Exponential : BaseEaaing {
            public override float inner(float max, float time, float limit) {
                return max * Mathf.Pow(2, 10 * (time - limit) / limit);
            }
        }
        public class Circular : BaseEaaing {
            public override float inner(float max, float time, float limit) {
                return -max * (Mathf.Sqrt(1 - time * time / limit / limit) - 1);
            }
        }
    }
}
