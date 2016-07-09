using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Mthods : MonoBehaviour
{
    /// <summary>
    ///各種カウンター
    /// </summary>
    [SerializeField]
    protected Dictionary<string, int> counterList = new Dictionary<string, int>();
    /// <summary>
    ///イージング関数群
    /// </summary>
    protected Easing easing = new Easing();

    protected delegate bool Terms(Roots target);
    protected delegate float Rank(Roots target);

    protected List<Roots> getAllObject(Terms map = null)
    {
        var returnList = new List<Roots>();
        foreach (Roots value in FindObjectsOfType(typeof(Roots)))
        {
            if (map == null || map(value)) returnList.Add(value);
        }
        return returnList;
    }
    protected List<Roots> searchMaxObject(Rank refine, Terms map = null)
    {
        List<Roots> returnList = new List<Roots>();
        foreach (var value in getAllObject(map))
        {
            if (returnList.Count <= 0)
            {
                returnList.Add(value);
            }
            else if (refine(value) > refine(returnList[0]))
            {
                returnList = new List<Roots> { value };
            }
            else if (refine(value) == refine(returnList[0]))
            {
                returnList.Add(value);
            }
        }

        return returnList;
    }

    /// <summary>
    ///システムテキストへの文字設定
    /// </summary>
    protected void setSysText(string setText)
    {
        GameObject.Find("SystemText").GetComponent<Text>().text = setText;
    }
    /// <summary>
    ///システムテキストの取得
    /// </summary>
    protected string getSysText()
    {
        return GameObject.Find("SystemText").GetComponent<Text>().text;
    }

    /// <summary>
    ///指定フレーム数待機する関数
    ///yield returnで呼び出さないと意味をなさない
    /// </summary>
    protected IEnumerator wait(int delay)
    {
        for (var i = 0; i < delay; i++) yield return null;
        yield break;
    }
    /// <summary>
    /// 自身の削除関数
    /// </summary>
    public virtual void selfDestroy()
    {
        Destroy(gameObject);
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
            public float Out(float max, float time, float limit)
            {
                return max - In(max, limit - time, limit);
            }
            public float InOut(float max, float time, float limit)
            {
                return time < limit / 2
                    ? In(max / 2, time, limit / 2)
                    : Out(max / 2, time - limit / 2, limit / 2) + max / 2;
            }
        }
        public class Linear : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time / limit;
            }
        }
        public class Quadratic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time * time / limit / limit;
            }
        }
        public class Cubic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time * time * time / limit / limit / limit;
            }
        }
        public class Quartic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time * time * time * time / limit / limit / limit / limit;
            }
        }
        public class Quintic : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * time * time * time * time * time / limit / limit / limit / limit / limit;
            }
        }
        public class Sinusoidal : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return -max * Mathf.Cos(time * Mathf.PI / limit / 2) + max;
            }
        }
        public class Exponential : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return max * Mathf.Pow(2, 10 * (time - limit) / limit);
            }
        }
        public class Circular : BaseEaaing
        {
            public override float In(float max, float time, float limit)
            {
                return -max * (Mathf.Sqrt(1 - time * time / limit / limit) - 1);
            }
        }
    }
}
