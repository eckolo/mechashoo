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
        public static void isDefined()
        {
            Assert.AreEqual(global::EnumFunctions.isDefined<TestEnum>(-1), false);
            Assert.AreEqual(global::EnumFunctions.isDefined<TestEnum>(0), true);
            Assert.AreEqual(global::EnumFunctions.isDefined<TestEnum>(1), true);
            Assert.AreEqual(global::EnumFunctions.isDefined<TestEnum>(2), true);
            Assert.AreEqual(global::EnumFunctions.isDefined<TestEnum>(3), false);
        }
        [Test]
        public static void normalize()
        {
            Assert.AreEqual(global::EnumFunctions.normalize<TestEnum>(-1), TestEnum.TEST1);
            Assert.AreEqual(global::EnumFunctions.normalize<TestEnum>(0), TestEnum.TEST1);
            Assert.AreEqual(global::EnumFunctions.normalize<TestEnum>(1), TestEnum.TEST2);
            Assert.AreEqual(global::EnumFunctions.normalize<TestEnum>(2), TestEnum.TEST3);
            Assert.AreEqual(global::EnumFunctions.normalize<TestEnum>(3), TestEnum.TEST1);
        }
        [Test]
        public static void length()
        {
            Assert.AreEqual(global::EnumFunctions.length<TestEnum>(), 3);
        }
        [Test]
        public static void max()
        {
            Assert.AreEqual(global::EnumFunctions.max<TestEnum>(), TestEnum.TEST3);
        }
        [Test]
        public static void min()
        {
            Assert.AreEqual(global::EnumFunctions.min<TestEnum>(), TestEnum.TEST1);
        }
    }
}