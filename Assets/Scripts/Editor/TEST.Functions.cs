using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using NUnit.Framework;
using System;
using System.Reflection;
using System.Collections.Generic;

public static partial class TEST
{
    public static class Functions
    {
        [Test]
        public static void ToInt()
        {
            Assert.AreEqual(global::Functions.ToInt(true), 1);
            Assert.AreEqual(global::Functions.ToInt(false), 0);
        }
        [Test]
        public static void ToSign1()
        {
            Assert.AreEqual(global::Functions.ToSign(true), 1);
            Assert.AreEqual(global::Functions.ToSign(false), -1);
        }
        [Test]
        public static void ToSign2()
        {
            Assert.AreEqual(global::Functions.ToSign(108), 1);
            Assert.AreEqual(global::Functions.ToSign(-62), -1);
            Assert.AreEqual(global::Functions.ToSign(0), 0);
        }
        [Test]
        public static void ToComponents1()
        {
            var list = new List<Materials>
            {
                new GameObject("test1",typeof(Materials)).GetComponent<Materials>(),
                new GameObject("test2",typeof(Materials)).GetComponent<Materials>()
            };
            var results = global::Functions.ToComponents<Methods, Materials>(list);

            Assert.IsInstanceOf<Methods>(results[0]);
            Assert.IsInstanceOf<Methods>(results[1]);
        }
        [Test]
        public static void ToComponents2()
        {
            var list = new List<Methods>
            {
                new GameObject("test1",typeof(Materials)).GetComponent<Materials>(),
                new GameObject("test2",typeof(Materials)).GetComponent<Materials>()
            };
            var results = global::Functions.ToComponents<Materials>(list);

            Assert.IsInstanceOf<Materials>(results[0]);
            Assert.IsInstanceOf<Materials>(results[1]);
        }
        [Test]
        public static void EqualsValue()
        {
            var value1 = new global::Ship.Palamates { maxArmor = 10 };
            var value2 = new global::Ship.Palamates { maxFuel = 10 };
            var value3 = new global::Ship.Palamates { maxBarrier = 10 };
            var value4 = new global::Ship.Palamates { maxArmor = 10 };
            global::Ship.Palamates value5 = null;

            Assert.IsTrue(global::Functions.EqualsValue(value1, value1));
            Assert.IsFalse(global::Functions.EqualsValue(value1, value2));
            Assert.IsFalse(global::Functions.EqualsValue(value1, value3));
            Assert.IsTrue(global::Functions.EqualsValue(value1, value4));
            Assert.IsFalse(global::Functions.EqualsValue(value1, value5));

            Assert.IsFalse(global::Functions.EqualsValue(value2, value1));
            Assert.IsTrue(global::Functions.EqualsValue(value2, value2));
            Assert.IsFalse(global::Functions.EqualsValue(value2, value3));
            Assert.IsFalse(global::Functions.EqualsValue(value2, value4));
            Assert.IsFalse(global::Functions.EqualsValue(value2, value5));

            Assert.IsFalse(global::Functions.EqualsValue(value3, value1));
            Assert.IsFalse(global::Functions.EqualsValue(value3, value2));
            Assert.IsTrue(global::Functions.EqualsValue(value3, value3));
            Assert.IsFalse(global::Functions.EqualsValue(value3, value4));
            Assert.IsFalse(global::Functions.EqualsValue(value3, value5));

            Assert.IsTrue(global::Functions.EqualsValue(value4, value1));
            Assert.IsFalse(global::Functions.EqualsValue(value4, value2));
            Assert.IsFalse(global::Functions.EqualsValue(value4, value3));
            Assert.IsTrue(global::Functions.EqualsValue(value4, value4));
            Assert.IsFalse(global::Functions.EqualsValue(value4, value5));

            Assert.IsFalse(global::Functions.EqualsValue(value5, value1));
            Assert.IsFalse(global::Functions.EqualsValue(value5, value2));
            Assert.IsFalse(global::Functions.EqualsValue(value5, value3));
            Assert.IsFalse(global::Functions.EqualsValue(value5, value4));
            Assert.IsTrue(global::Functions.EqualsValue(value5, value5));
        }
        [Test]
        public static void EqualsList()
        {
            var list1 = new List<global::Ship.Palamates>
            {
                new global::Ship.Palamates { maxArmor = 10 },
                new global::Ship.Palamates { maxFuel = 10 },
                new global::Ship.Palamates { maxBarrier = 10 }
            };
            var list2 = new List<global::Ship.Palamates>
            {
                new global::Ship.Palamates { maxFuel = 10 },
                new global::Ship.Palamates { maxArmor = 10 },
                new global::Ship.Palamates { maxBarrier = 10 }
            };
            var list3 = new List<global::Ship.Palamates>
            {
                new global::Ship.Palamates { maxArmor = 10 },
                new global::Ship.Palamates { maxFuel = 10 }
            };
            var list4 = new List<global::Ship.Palamates>
            {
                new global::Ship.Palamates { maxArmor = 10 },
                new global::Ship.Palamates { maxFuel = 10 },
                new global::Ship.Palamates { maxBarrier = 10 }
            };
            var list5 = new List<global::Ship.Palamates> { };
            List<global::Ship.Palamates> list6 = null;

            Assert.IsTrue(global::Functions.EqualsList(list1, list1));
            Assert.IsFalse(global::Functions.EqualsList(list1, list2));
            Assert.IsFalse(global::Functions.EqualsList(list1, list3));
            Assert.IsTrue(global::Functions.EqualsList(list1, list4));
            Assert.IsFalse(global::Functions.EqualsList(list1, list5));
            Assert.IsFalse(global::Functions.EqualsList(list1, list6));

            Assert.IsFalse(global::Functions.EqualsList(list2, list1));
            Assert.IsTrue(global::Functions.EqualsList(list2, list2));
            Assert.IsFalse(global::Functions.EqualsList(list2, list3));
            Assert.IsFalse(global::Functions.EqualsList(list2, list4));
            Assert.IsFalse(global::Functions.EqualsList(list2, list5));
            Assert.IsFalse(global::Functions.EqualsList(list2, list6));

            Assert.IsFalse(global::Functions.EqualsList(list3, list1));
            Assert.IsFalse(global::Functions.EqualsList(list3, list2));
            Assert.IsTrue(global::Functions.EqualsList(list3, list3));
            Assert.IsFalse(global::Functions.EqualsList(list3, list4));
            Assert.IsFalse(global::Functions.EqualsList(list3, list5));
            Assert.IsFalse(global::Functions.EqualsList(list3, list6));

            Assert.IsTrue(global::Functions.EqualsList(list4, list1));
            Assert.IsFalse(global::Functions.EqualsList(list4, list2));
            Assert.IsFalse(global::Functions.EqualsList(list4, list3));
            Assert.IsTrue(global::Functions.EqualsList(list4, list4));
            Assert.IsFalse(global::Functions.EqualsList(list4, list5));
            Assert.IsFalse(global::Functions.EqualsList(list4, list6));

            Assert.IsFalse(global::Functions.EqualsList(list5, list1));
            Assert.IsFalse(global::Functions.EqualsList(list5, list2));
            Assert.IsFalse(global::Functions.EqualsList(list5, list3));
            Assert.IsFalse(global::Functions.EqualsList(list5, list4));
            Assert.IsTrue(global::Functions.EqualsList(list5, list5));
            Assert.IsFalse(global::Functions.EqualsList(list5, list6));

            Assert.IsFalse(global::Functions.EqualsList(list6, list1));
            Assert.IsFalse(global::Functions.EqualsList(list6, list2));
            Assert.IsFalse(global::Functions.EqualsList(list6, list3));
            Assert.IsFalse(global::Functions.EqualsList(list6, list4));
            Assert.IsFalse(global::Functions.EqualsList(list6, list5));
            Assert.IsTrue(global::Functions.EqualsList(list6, list6));
        }
        [Test]
        public static void SelectRandom()
        {
            var list1 = new List<float> { 0.5f, 7.8f, 832 };
            var list2 = new List<int> { 1, 2 };

            for(int i = 0; i < 120; i++)
            {
                var result = global::Functions.SelectRandom(list1);
                Assert.IsTrue(result == 0.5f || result == 7.8f || result == 832);
            }
            for(int i = 0; i < 120; i++)
            {
                var result = global::Functions.SelectRandom(list1, list2);
                Assert.IsTrue(result == 0.5f || result == 7.8f || result == 832);
            }
        }
        [Test]
        public static void Log1()
        {
            var value1 = Mathf.Exp(11.3f) - 1;
            var value2 = -Mathf.Exp(26.43f) + 1;
            var value3 = 0f;

            Assert.AreEqual(value1.Log(), 11.3f);
            Assert.AreEqual(value2.Log(), -26.43f);
            Assert.AreEqual(value3.Log(), 0);
        }
        [Test]
        public static void Log2()
        {
            var value1 = Mathf.Pow(2.6f, 11.3f) - 1;
            var value2 = -Mathf.Pow(4.63f, 26.43f) + 1;
            var value3 = 0f;

            Assert.AreEqual(value1.Log(2.6f), 11.3f);
            Assert.AreEqual(value2.Log(4.63f), -26.43f);
            Assert.AreEqual(value3.Log(2.6f), 0);
        }
        [Test]
        public static void SetAlpha()
        {
            var obj = new GameObject();
            var text = obj.AddComponent<Text>();
            text.setAlpha(0.5f);

            Assert.AreEqual(text.color.a, 0.5f);
        }
        [Test]
        public static void SetPosition()
        {
            var obj = new GameObject();
            var text = obj.AddComponent<Text>();
            text.setPosition(new Vector2(3, 5));

            Assert.AreEqual(obj.GetComponent<RectTransform>().localPosition.x, 3);
            Assert.AreEqual(obj.GetComponent<RectTransform>().localPosition.y, 5);
        }
        [Test]
        public static void Copy()
        {
            var objects1 = new List<MockClass>{
                new MockClass
                {
                    text1 = "test1",
                    text2 = "test2",
                    number = 3
                },
                new MockClass
                {
                    text1 = "test3",
                    text2 = "test4",
                    number = 5
                }
            };
            var objects2 = new List<MockClass>{
                new MockClass
                {
                    text1 = "test5",
                    text2 = "test6",
                    number = 4
                },
                new MockClass
                {
                    text1 = "test7",
                    text2 = "test8",
                    number = 9
                }
            };
            var objects3 = new List<MockClass> { };
            List<MockClass> objects4 = null;

            Assert.AreEqual(objects1, objects1.Copy());
            Assert.AreNotSame(objects1, objects1.Copy());
            Assert.AreEqual(objects1[0], objects1.Copy()[0]);
            Assert.AreEqual(objects1[1], objects1.Copy()[1]);
            Assert.AreSame(objects1[0], objects1.Copy()[0]);
            Assert.AreSame(objects1[1], objects1.Copy()[1]);

            Assert.AreNotEqual(objects2[0], objects1.Copy()[0]);
            Assert.AreNotEqual(objects2[1], objects1.Copy()[1]);

            Assert.AreEqual(objects3, objects3.Copy());
            Assert.AreEqual(objects4, objects4.Copy());
        }
        class MockClass
        {
            public string text1;
            public string text2;
            public int number;
        }
        [Test]
        public static void ToPercentage()
        {
            float? number1 = 1.31f;
            float? number2 = 0.013f;
            float? number3 = 5f;
            float? number4 = null;

            Assert.AreEqual(number1.ToPercentage(), "1.31");
            Assert.AreEqual(number2.ToPercentage(), "0.01");
            Assert.AreEqual(number3.ToPercentage(), "5.00");
            Assert.AreEqual(number4.ToPercentage(), "-");
        }
    }
}