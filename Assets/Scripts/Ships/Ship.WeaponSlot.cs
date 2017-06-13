public partial class Ship : Things
{
    /// <summary>
    /// 武装スロットパラメータ
    /// </summary>
    [System.Serializable]
    public class WeaponSlot : PartsState, ICopyAble<WeaponSlot>, System.IEquatable<WeaponSlot>
    {
        public Weapon entity = null;
        public float baseAngle = 0;
        public bool unique = false;

        public new WeaponSlot myself
        {
            get {
                return new WeaponSlot
                {
                    rootPosition = rootPosition,
                    positionZ = positionZ,
                    partsNum = partsNum,

                    entity = entity,
                    baseAngle = baseAngle,
                    unique = unique
                };
            }
        }

        public bool Equals(WeaponSlot other)
        {
            if(other == null || GetType() != other.GetType()) return false;

            if(rootPosition != other.rootPosition) return false;
            if(positionZ != other.positionZ) return false;
            if(partsNum != other.partsNum) return false;

            if(entity != other.entity) return false;
            if(baseAngle != other.baseAngle) return false;
            if(unique != other.unique) return false;

            return true;
        }
    }
}
