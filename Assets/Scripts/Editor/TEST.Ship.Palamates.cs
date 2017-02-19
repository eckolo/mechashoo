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
        public static class PalamatesTEST
        {
            [Test]
            public static void Equals()
            {
                var palamates1 = new Ship.Palamates
                {
                    maxArmor = 10,
                    maxBarrier = 12,
                    recoveryBarrier = 45,
                    maxFuel = 4.6f,
                    recoveryFuel = 938.63f,
                    baseSiteSpeed = 34.5674f
                };
                var palamates2 = new Ship.Palamates
                {
                    maxArmor = 10.3f,
                    maxBarrier = 12,
                    recoveryBarrier = 45,
                    maxFuel = 4.6f,
                    recoveryFuel = 938.63f,
                    baseSiteSpeed = 34.5674f
                };
                var palamates3 = new Ship.Palamates
                {
                    maxArmor = 10.3f,
                    maxBarrier = 1352,
                    recoveryBarrier = 4.5f,
                    maxFuel = 46,
                    recoveryFuel = 938f,
                    baseSiteSpeed = 34
                };
                var palamates4 = new Ship.Palamates
                {
                    maxArmor = 10,
                    maxBarrier = 12,
                    recoveryBarrier = 45,
                    maxFuel = 4.6f,
                    recoveryFuel = 938.63f,
                    baseSiteSpeed = 34.5674f
                };

                Assert.IsTrue(palamates1.Equals(palamates1));
                Assert.IsFalse(palamates1.Equals(palamates2));
                Assert.IsFalse(palamates1.Equals(palamates3));
                Assert.IsTrue(palamates1.Equals(palamates4));

                Assert.IsFalse(palamates2.Equals(palamates1));
                Assert.IsTrue(palamates2.Equals(palamates2));
                Assert.IsFalse(palamates2.Equals(palamates3));
                Assert.IsFalse(palamates2.Equals(palamates4));

                Assert.IsFalse(palamates3.Equals(palamates1));
                Assert.IsFalse(palamates3.Equals(palamates2));
                Assert.IsTrue(palamates3.Equals(palamates3));
                Assert.IsFalse(palamates3.Equals(palamates4));

                Assert.IsTrue(palamates4.Equals(palamates1));
                Assert.IsFalse(palamates4.Equals(palamates2));
                Assert.IsFalse(palamates4.Equals(palamates3));
                Assert.IsTrue(palamates4.Equals(palamates4));
            }
            [Test]
            public static void myself()
            {
                var palamates1 = new Ship.Palamates
                {
                    maxArmor = 10,
                    maxBarrier = 12,
                    recoveryBarrier = 45,
                    maxFuel = 4.6f,
                    recoveryFuel = 938.63f,
                    baseSiteSpeed = 34.5674f
                };
                var palamates2 = palamates1.myself;

                Assert.IsTrue(palamates1.Equals(palamates2));
                Assert.IsFalse(palamates1 == palamates2);

                palamates1.maxArmor = 120;

                Assert.IsFalse(palamates1.Equals(palamates2));
                Assert.IsFalse(palamates1 == palamates2);
            }
        }
    }
}