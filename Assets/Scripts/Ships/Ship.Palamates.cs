public partial class Ship : Things
{
    /// <summary>
    /// 基礎パラメータ
    /// </summary>
    [System.Serializable]
    public class Palamates : ICopyAble<Palamates>, System.IEquatable<Palamates>
    {
        /// <summary>
        /// 装甲関係
        /// </summary>
        public float maxArmor = 1;
        public float nowArmor { get; set; }
        /// <summary>
        /// 障壁関係
        /// </summary>
        public float maxBarrier = 1;
        public float recoveryBarrier = 0.1f;
        public float nowBarrier { get; set; }
        /// <summary>
        /// 燃料関係
        /// </summary>
        public float maxFuel = 1;
        public float recoveryFuel = 0.1f;
        public float nowFuel { get; set; }
        /// <summary>
        /// 照準移動速度
        /// </summary>
        public float baseSiteSpeed = 0.005f;

        public bool Equals(Palamates other)
        {
            if(other == null || GetType() != other.GetType()) return false;

            if(maxArmor != other.maxArmor) return false;
            if(maxBarrier != other.maxBarrier) return false;
            if(recoveryBarrier != other.recoveryBarrier) return false;
            if(maxFuel != other.maxFuel) return false;
            if(recoveryFuel != other.recoveryFuel) return false;
            if(baseSiteSpeed != other.baseSiteSpeed) return false;

            return true;
        }

        public Palamates myself
        {
            get {
                return new Palamates
                {
                    maxArmor = maxArmor,
                    maxBarrier = maxBarrier,
                    recoveryBarrier = recoveryBarrier,
                    maxFuel = maxFuel,
                    recoveryFuel = recoveryFuel,
                    baseSiteSpeed = baseSiteSpeed
                };
            }
        }
    }
}
