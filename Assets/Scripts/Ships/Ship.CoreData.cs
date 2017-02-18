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
        public string name = "";
        public Sprite image = null;

        public float armorBarHeight = 0.5f;
        public Vector2 defaultAlignment = new Vector2(1, -0.5f);

        public float weight = 1;

        public Palamates palamates = new Palamates();
        public List<ArmState> armStates = new List<ArmState>();
        public List<AccessoryState> accessoryStates = new List<AccessoryState>();
        public List<WeaponSlot> weaponSlots = new List<WeaponSlot>();

        public bool Equals(CoreData other)
        {
            if(other == null || GetType() != other.GetType()) return false;

            if(name != other.name) return false;
            if(image != other.image) return false;
            if(armorBarHeight != other.armorBarHeight) return false;
            if(defaultAlignment != other.defaultAlignment) return false;
            if(weight != other.weight) return false;

            if(!palamates.Equals(other.palamates)) return false;
            if(!armStates.listEquals(other.armStates)) return false;
            if(!accessoryStates.listEquals(other.accessoryStates)) return false;
            if(!weaponSlots.listEquals(other.weaponSlots)) return false;

            return true;
        }

        public List<Weapon> weapons
        {
            get {
                return weaponSlots.Select(slot => slot.entity).ToList();
            }
            set {
                for(int index = 0; index < weaponSlots.Count; index++)
                    weaponSlots[index].entity = index < value.Count ? value[index] : null;
            }
        }
        public Explosion explosion;

        public bool isCorrect
        {
            get {
                if(weapons.Where(weapon => weapon != null).ToList().Count <= 0) return false;
                return true;
            }
        }

        public CoreData setWeapon(List<Weapon> setWeapons = null)
        {
            weapons = setWeapons ?? new List<Weapon>();
            return myself;
        }
        public CoreData setWeapon(int index, Weapon setWeapon = null)
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
                    name = name,
                    image = image,
                    armorBarHeight = armorBarHeight,
                    defaultAlignment = defaultAlignment,
                    explosion = explosion,
                    weight = weight,

                    palamates = palamates.myself,
                    armStates = copyStateList(armStates),
                    accessoryStates = copyStateList(accessoryStates),
                    weaponSlots = copyStateList(weaponSlots)
                };
            }
        }
    }

    public CoreData coreData
    {
        get {
            return new CoreData
            {
                name = gameObject.name,
                image = GetComponent<SpriteRenderer>().sprite,
                armorBarHeight = armorBarHeight,
                defaultAlignment = defaultAlignment,
                explosion = explosion,
                weight = weight,

                palamates = palamates.myself,
                armStates = copyStateList(armStates),
                accessoryStates = copyStateList(accessoryStates),
                weaponSlots = copyStateList(weaponSlots)
            };
        }
        set {
            value = value ?? new CoreData();

            GetComponent<SpriteRenderer>().sprite = value.image;
            armorBarHeight = value.armorBarHeight;
            defaultAlignment = value.defaultAlignment;
            explosion = value.explosion;
            weight = value.weight;

            palamates = value.palamates.myself;
            armStates = copyStateList(value.armStates);
            accessoryStates = copyStateList(value.accessoryStates);
            weaponSlots = copyStateList(value.weaponSlots);

            setParamate();
        }
    }
}
