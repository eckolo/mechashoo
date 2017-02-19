using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using NUnit.Framework;
using System;
using System.Reflection;
using System.Collections.Generic;

public static partial class TEST
{
    public static partial class ShipTEST
    {
        public static class WeaponSlotTEST
        {
            [Test]
            public static void Equals()
            {
                var weaponSlot1 = new Ship.WeaponSlot
                {
                    rootPosition = new Vector2(2.4f, 567),
                    positionZ = 5.672f,
                    partsNum = 8,
                    baseAngle = 548
                };
                var weaponSlot2 = new Ship.WeaponSlot
                {
                    rootPosition = new Vector2(2.4f, 567),
                    positionZ = 5.672f,
                    partsNum = 8,
                    entity = new GameObject("test1", typeof(Weapon)).GetComponent<Weapon>(),
                    baseAngle = 548
                };
                var weaponSlot3 = new Ship.WeaponSlot
                {
                    rootPosition = new Vector2(24, 56.7f),
                    positionZ = 672,
                    partsNum = 138,
                    entity = new GameObject("test2", typeof(Weapon)).GetComponent<Weapon>(),
                    baseAngle = 54.8f
                };
                var weaponSlot4 = new Ship.WeaponSlot
                {
                    rootPosition = new Vector2(2.4f, 567),
                    positionZ = 5.672f,
                    partsNum = 8,
                    baseAngle = 548
                };

                Assert.IsTrue(weaponSlot1.Equals(weaponSlot1));
                Assert.IsFalse(weaponSlot1.Equals(weaponSlot2));
                Assert.IsFalse(weaponSlot1.Equals(weaponSlot3));
                Assert.IsTrue(weaponSlot1.Equals(weaponSlot4));

                Assert.IsFalse(weaponSlot2.Equals(weaponSlot1));
                Assert.IsTrue(weaponSlot2.Equals(weaponSlot2));
                Assert.IsFalse(weaponSlot2.Equals(weaponSlot3));
                Assert.IsFalse(weaponSlot2.Equals(weaponSlot4));

                Assert.IsFalse(weaponSlot3.Equals(weaponSlot1));
                Assert.IsFalse(weaponSlot3.Equals(weaponSlot2));
                Assert.IsTrue(weaponSlot3.Equals(weaponSlot3));
                Assert.IsFalse(weaponSlot3.Equals(weaponSlot4));

                Assert.IsTrue(weaponSlot4.Equals(weaponSlot1));
                Assert.IsFalse(weaponSlot4.Equals(weaponSlot2));
                Assert.IsFalse(weaponSlot4.Equals(weaponSlot3));
                Assert.IsTrue(weaponSlot4.Equals(weaponSlot4));
            }
            [Test]
            public static void myself()
            {
                var weaponSlot1 = new Ship.WeaponSlot
                {
                    rootPosition = new Vector2(2.4f, 567),
                    positionZ = 5.672f,
                    partsNum = 8,
                    entity = new GameObject("test1", typeof(Weapon)).GetComponent<Weapon>(),
                    baseAngle = 548
                };
                var weaponSlot2 = weaponSlot1.myself;

                Assert.IsTrue(weaponSlot1.Equals(weaponSlot2));
                Assert.IsFalse(weaponSlot1 == weaponSlot2);

                weaponSlot1.positionZ = 120.544f;

                Assert.IsFalse(weaponSlot1.Equals(weaponSlot2));
                Assert.IsFalse(weaponSlot1 == weaponSlot2);
            }
        }
    }
}