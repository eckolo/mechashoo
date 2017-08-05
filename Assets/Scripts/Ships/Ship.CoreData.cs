using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 機体クラス
/// </summary>
public partial class Ship : Things
{
    public class CoreData : ICopyAble<CoreData>, System.IEquatable<CoreData>
    {
        [SerializeField]
        public string displayName = "";
        [SerializeField]
        public string explanation = "";
        [SerializeField]
        public Sprite image = null;

        [SerializeField]
        public float armorBarHeight = 0.5f;
        [SerializeField]
        public Vector2 defaultAlignment = new Vector2(1, -0.5f);
        [SerializeField]
        public float _turningBoundaryPoint = 0;

        [SerializeField]
        public float weight = 1;

        [SerializeField]
        public Palamates palamates = new Palamates();
        [SerializeField]
        public List<ArmState> armStates = new List<ArmState>();
        [SerializeField]
        public List<AccessoryState> accessoryStates = new List<AccessoryState>();
        [SerializeField]
        public List<WeaponSlot> weaponSlots = new List<WeaponSlot>();
        [SerializeField]
        public List<WeaponSlot> subWeaponSlots = new List<WeaponSlot>();

        public bool Equals(CoreData other)
        {
            if(other == null || GetType() != other.GetType()) return false;

            if(displayName != other.displayName) return false;
            if(image != other.image) return false;
            if(armorBarHeight != other.armorBarHeight) return false;
            if(defaultAlignment != other.defaultAlignment) return false;
            if(_turningBoundaryPoint != other._turningBoundaryPoint) return false;
            if(explosionEffects != other.explosionEffects) return false;
            if(weight != other.weight) return false;

            if(!palamates.EqualsValue(other.palamates)) return false;
            if(!armStates.EqualsList(other.armStates)) return false;
            if(!accessoryStates.EqualsList(other.accessoryStates)) return false;
            if(!weaponSlots.EqualsList(other.weaponSlots)) return false;

            return true;
        }

        public List<WeaponSlot> allWeaponSlots => weaponSlots.Concat(subWeaponSlots).ToList();
        public List<Weapon> weapons
        {
            get {
                return allWeaponSlots.Select(slot => slot.entity).ToList();
            }
            set {
                for(int index = 0; index < allWeaponSlots.Count; index++)
                {
                    if(allWeaponSlots[index].unique) continue;
                    var setedWeapon = index < value.Count ? value[index] : null;
                    allWeaponSlots[index].entity = setedWeapon;

                    var _index = index - weaponSlots.Count;
                    if(0 <= _index && _index < subWeaponSlots.Count) subWeaponSlots[_index].entity = setedWeapon;
                }
            }
        }
        public List<Explosion> explosionEffects;

        public bool isCorrect
        {
            get {
                if(weapons.Where(weapon => weapon != null).ToList().Count <= 0) return false;
                return true;
            }
        }

        public CoreData SetWeapon(List<Weapon> setWeapons = null)
        {
            weapons = setWeapons ?? new List<Weapon>();
            return myself;
        }
        public CoreData SetWeapon(int index, Weapon setWeapon = null)
        {
            if(index < 0) return this;
            if(index >= weapons.Count) return this;

            var setWeapons = weapons;
            setWeapons[index] = setWeapon;
            weapons = setWeapons;
            return myself;
        }

        public CoreData myself
        {
            get {
                return new CoreData
                {
                    displayName = displayName,
                    explanation = explanation,
                    image = image,
                    armorBarHeight = armorBarHeight,
                    defaultAlignment = defaultAlignment,
                    _turningBoundaryPoint = _turningBoundaryPoint,
                    explosionEffects = explosionEffects,
                    weight = weight,

                    palamates = palamates.myself,
                    armStates = CopyStateList(armStates),
                    accessoryStates = CopyStateList(accessoryStates),
                    weaponSlots = CopyStateList(weaponSlots),

                    subWeaponSlots = CopyStateList(subWeaponSlots)
                };
            }
        }
    }

    public CoreData coreData
    {
        get {
            return new CoreData
            {
                displayName = displayName,
                explanation = explanation,
                image = nowSprite,
                armorBarHeight = armorBarHeight,
                defaultAlignment = defaultAlignment,
                _turningBoundaryPoint = _turningBoundaryPoint,
                explosionEffects = explosionEffects.Copy(),
                weight = weight,

                palamates = palamates.myself,
                armStates = CopyStateList(armStates),
                accessoryStates = CopyStateList(accessoryStates),
                weaponSlots = CopyStateList(weaponSlots),

                subWeaponSlots = CopyStateList(subWeaponSlots)
            };
        }
        set {
            value = value ?? new CoreData();
            foreach(var child in nowChildren) child.DestroyMyself(true);

            displayName = value.displayName;
            explanation = value.explanation;
            nowSprite = value.image;
            armorBarHeight = value.armorBarHeight;
            defaultAlignment = value.defaultAlignment;
            _turningBoundaryPoint = value._turningBoundaryPoint;
            explosionEffects = value.explosionEffects.Copy();
            weight = value.weight;

            palamates = value.palamates.myself;
            armStates = CopyStateList(value.armStates);
            accessoryStates = CopyStateList(value.accessoryStates);
            weaponSlots = CopyStateList(value.weaponSlots);

            subWeaponSlots = CopyStateList(value.subWeaponSlots);

            SetParamate();
        }
    }
}
