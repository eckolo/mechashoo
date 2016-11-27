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
            var bigVector = new Vector2(5, 5);
            var smallVector = new Vector2(3, 4);

            Assert.AreEqual(MathV.max(smallVector, bigVector), new Vector2(5, 5));
        }
    }
}
