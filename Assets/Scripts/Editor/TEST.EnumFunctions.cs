using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using NUnit.Framework;
using System;
using System.Reflection;
using System.Collections.Generic;

public static partial class TEST
{
    public static class EnumFunctions
    {
        enum TestEnum { TEST1, TEST2, TEST3 }
        [Test]
        public static void IsDefined()
        {
            Assert.AreEqual(global::EnumFunctions.IsDefined<TestEnum>(-1), false);
            Assert.AreEqual(global::EnumFunctions.IsDefined<TestEnum>(0), true);
            Assert.AreEqual(global::EnumFunctions.IsDefined<TestEnum>(1), true);
            Assert.AreEqual(global::EnumFunctions.IsDefined<TestEnum>(2), true);
            Assert.AreEqual(global::EnumFunctions.IsDefined<TestEnum>(3), false);
        }
        [Test]
        public static void Normalize()
        {
            Assert.AreEqual(global::EnumFunctions.Normalize<TestEnum>(-1), TestEnum.TEST1);
            Assert.AreEqual(global::EnumFunctions.Normalize<TestEnum>(0), TestEnum.TEST1);
            Assert.AreEqual(global::EnumFunctions.Normalize<TestEnum>(1), TestEnum.TEST2);
            Assert.AreEqual(global::EnumFunctions.Normalize<TestEnum>(2), TestEnum.TEST3);
            Assert.AreEqual(global::EnumFunctions.Normalize<TestEnum>(3), TestEnum.TEST1);
        }
        [Test]
        public static void GetLength()
        {
            Assert.AreEqual(global::EnumFunctions.GetLength<TestEnum>(), 3);
        }
        [Test]
        public static void GetMax()
        {
            Assert.AreEqual(global::EnumFunctions.GetMax<TestEnum>(), TestEnum.TEST3);
        }
        [Test]
        public static void GetMin()
        {
            Assert.AreEqual(global::EnumFunctions.GetMin<TestEnum>(), TestEnum.TEST1);
        }
    }
}