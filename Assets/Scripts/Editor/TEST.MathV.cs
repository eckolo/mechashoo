using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using NUnit.Framework;
using System;
using System.Reflection;

public static partial class TEST {
    public class MethodMathV : Methods {
        [Test]
        public static void max() {
            var method = new GameObject().AddComponent<Methods>();
            var maxValue = (Vector2)
                method.GetType()
                .GetMethod("MathV.max", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(method, new object[] { new Vector2(3, 4), new Vector2(5, 5) });

            Assert.AreEqual(new Vector2(5, 5), maxValue);
        }
    }
}
