using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using NUnit.Framework;
using System;
using System.Reflection;
using System.Collections.Generic;

public static partial class TEST
{
    public static partial class Ship
    {
        public static class AccessoryState
        {
            [Test]
            public static void Equals()
            {
                var accessoryState1 = new global::Ship.AccessoryState
                {
                    rootPosition = new Vector2(2.4f, 567),
                    positionZ = 5.672f,
                    partsNum = 8
                };
                var accessoryState2 = new global::Ship.AccessoryState
                {
                    rootPosition = new Vector2(2.4f, 567),
                    positionZ = 5.672f,
                    partsNum = 8,
                    entity = new GameObject("test1", typeof(Accessory)).GetComponent<Accessory>()
                };
                var accessoryState3 = new global::Ship.AccessoryState
                {
                    rootPosition = new Vector2(24, 56.7f),
                    positionZ = 672,
                    partsNum = 138,
                    entity = new GameObject("test2", typeof(Accessory)).GetComponent<Accessory>()
                };
                var accessoryState4 = new global::Ship.AccessoryState
                {
                    rootPosition = new Vector2(2.4f, 567),
                    positionZ = 5.672f,
                    partsNum = 8
                };

                Assert.IsTrue(accessoryState1.Equals(accessoryState1));
                Assert.IsFalse(accessoryState1.Equals(accessoryState2));
                Assert.IsFalse(accessoryState1.Equals(accessoryState3));
                Assert.IsTrue(accessoryState1.Equals(accessoryState4));

                Assert.IsFalse(accessoryState2.Equals(accessoryState1));
                Assert.IsTrue(accessoryState2.Equals(accessoryState2));
                Assert.IsFalse(accessoryState2.Equals(accessoryState3));
                Assert.IsFalse(accessoryState2.Equals(accessoryState4));

                Assert.IsFalse(accessoryState3.Equals(accessoryState1));
                Assert.IsFalse(accessoryState3.Equals(accessoryState2));
                Assert.IsTrue(accessoryState3.Equals(accessoryState3));
                Assert.IsFalse(accessoryState3.Equals(accessoryState4));

                Assert.IsTrue(accessoryState4.Equals(accessoryState1));
                Assert.IsFalse(accessoryState4.Equals(accessoryState2));
                Assert.IsFalse(accessoryState4.Equals(accessoryState3));
                Assert.IsTrue(accessoryState4.Equals(accessoryState4));
            }
            [Test]
            public static void myself()
            {
                var accessoryState1 = new global::Ship.AccessoryState
                {
                    rootPosition = new Vector2(2.4f, 567),
                    positionZ = 5.672f,
                    partsNum = 8,
                    entity = new GameObject("test1", typeof(Accessory)).GetComponent<Accessory>()
                };
                var accessoryState2 = accessoryState1.myself;

                Assert.IsTrue(accessoryState1.Equals(accessoryState2));
                Assert.IsFalse(accessoryState1 == accessoryState2);

                accessoryState1.positionZ = 120.544f;

                Assert.IsFalse(accessoryState1.Equals(accessoryState2));
                Assert.IsFalse(accessoryState1 == accessoryState2);
            }
        }
    }
}