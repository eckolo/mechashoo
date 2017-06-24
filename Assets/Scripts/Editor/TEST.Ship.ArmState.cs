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
        public static class ArmState
        {
            [Test]
            public static void Equals()
            {
                var armState1 = new global::Ship.ArmState
                {
                    rootPosition = new Vector2(2.4f, 567),
                    positionZ = 5.672f,
                    partsNum = 8,
                    tipPosition = new Vector2(548, 569f),
                    siteTweak = new Vector2(548, 569f)
                };
                var armState2 = new global::Ship.ArmState
                {
                    rootPosition = new Vector2(2.4f, 567),
                    positionZ = 5.672f,
                    partsNum = 8,
                    entity = new GameObject("test1", typeof(Arm)).GetComponent<Arm>(),
                    tipPosition = new Vector2(548, 569f),
                    siteTweak = new Vector2(548, 569f)
                };
                var armState3 = new global::Ship.ArmState
                {
                    rootPosition = new Vector2(24, 56.7f),
                    positionZ = 672,
                    partsNum = 138,
                    entity = new GameObject("test2", typeof(Arm)).GetComponent<Arm>(),
                    tipPosition = new Vector2(54.58f, 5693),
                    siteTweak = new Vector2(548, 569f)
                };
                var armState4 = new global::Ship.ArmState
                {
                    rootPosition = new Vector2(2.4f, 567),
                    positionZ = 5.672f,
                    partsNum = 8,
                    tipPosition = new Vector2(548, 569f),
                    siteTweak = new Vector2(548, 569f)
                };

                Assert.IsTrue(armState1.Equals(armState1));
                Assert.IsFalse(armState1.Equals(armState2));
                Assert.IsFalse(armState1.Equals(armState3));
                Assert.IsTrue(armState1.Equals(armState4));

                Assert.IsFalse(armState2.Equals(armState1));
                Assert.IsTrue(armState2.Equals(armState2));
                Assert.IsFalse(armState2.Equals(armState3));
                Assert.IsFalse(armState2.Equals(armState4));

                Assert.IsFalse(armState3.Equals(armState1));
                Assert.IsFalse(armState3.Equals(armState2));
                Assert.IsTrue(armState3.Equals(armState3));
                Assert.IsFalse(armState3.Equals(armState4));

                Assert.IsTrue(armState4.Equals(armState1));
                Assert.IsFalse(armState4.Equals(armState2));
                Assert.IsFalse(armState4.Equals(armState3));
                Assert.IsTrue(armState4.Equals(armState4));
            }
            [Test]
            public static void myself()
            {
                var armState1 = new global::Ship.ArmState
                {
                    rootPosition = new Vector2(2.4f, 567),
                    positionZ = 5.672f,
                    partsNum = 8,
                    entity = new GameObject("test1", typeof(Arm)).GetComponent<Arm>(),
                    tipPosition = new Vector2(548, 569f),
                    siteTweak = new Vector2(548, 569f)
                };
                var armState2 = armState1.myself;

                Assert.IsTrue(armState1.Equals(armState2));
                Assert.IsFalse(armState1 == armState2);

                armState1.positionZ = 120.544f;

                Assert.IsFalse(armState1.Equals(armState2));
                Assert.IsFalse(armState1 == armState2);
            }
        }
    }
}