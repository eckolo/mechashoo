public partial class Ship : Things
{
    /// <summary>
    /// 付属パーツパラメータ
    /// </summary>
    [System.Serializable]
    public class AccessoryState : PartsState,
        ICopyAble<AccessoryState>,
        System.IEquatable<AccessoryState>
    {
        public Accessory entity = null;
        public float baseAngle = 0;

        public new AccessoryState myself
        {
            get {
                return new AccessoryState
                {
                    rootPosition = rootPosition,
                    positionZ = positionZ,
                    partsNum = partsNum,

                    entity = entity,
                    baseAngle = baseAngle
                };
            }
        }

        public bool Equals(AccessoryState other)
        {
            if(other == null || GetType() != other.GetType()) return false;

            if(rootPosition != other.rootPosition) return false;
            if(positionZ != other.positionZ) return false;
            if(partsNum != other.partsNum) return false;
            if(entity != other.entity) return false;
            if(baseAngle != other.baseAngle) return false;

            return true;
        }
    }
}
