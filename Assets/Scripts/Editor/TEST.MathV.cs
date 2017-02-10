using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using NUnit.Framework;
using System;
using System.Reflection;

public static partial class TEST
{
    public static class MathVTEST
    {
        [Test]
        public static void max()
        {
            var bigVector = new Vector2(5, 5);
            var smallVector = new Vector2(3, 4);

            Assert.AreEqual(MathV.max(smallVector, bigVector), new Vector2(5, 5));
        }
        [Test]
        public static void max2()
        {
            var vector = new Vector2(3, 4);
            var smallScalar = 4f;
            var bigScalar = 10f;

            Assert.AreEqual(MathV.max(vector, smallScalar), new Vector2(3, 4));
            Assert.AreEqual(MathV.max(vector, bigScalar), new Vector2(6, 8));
        }
        [Test]
        public static void min()
        {
            var bigVector = new Vector2(5, 5);
            var smallVector = new Vector2(3, 4);

            Assert.AreEqual(MathV.min(smallVector, bigVector), new Vector2(3, 4));
        }
        [Test]
        public static void min2()
        {
            var vector = new Vector2(6, 8);
            var smallScalar = 5f;
            var bigScalar = 12f;

            Assert.AreEqual(MathV.min(vector, smallScalar), new Vector2(3, 4));
            Assert.AreEqual(MathV.min(vector, bigScalar), new Vector2(6, 8));
        }
        [Test]
        public static void abs()
        {
            var vector = new Vector2(-3, -4);

            Assert.AreEqual(MathV.abs(vector), new Vector2(3, 4));
        }
        [Test]
        public static void correct()
        {
            var vector1 = new Vector2(3, 4);
            var vector2 = new Vector2(5, 5);

            Assert.AreEqual(MathV.correct(vector1, vector2), new Vector2(4, 4.5f));
            Assert.AreEqual(MathV.correct(vector1, vector2, 1), new Vector2(3, 4));
            Assert.AreEqual(MathV.correct(vector1, vector2, 0), new Vector2(5, 5));
        }
        [Test]
        public static void correct2()
        {
            var scalar1 = 3f;
            var scalar2 = 5f;

            Assert.AreEqual(MathV.correct(scalar1, scalar2), 4f);
            Assert.AreEqual(MathV.correct(scalar1, scalar2, 1), 3f);
            Assert.AreEqual(MathV.correct(scalar1, scalar2, 0), 5f);
        }
        [Test]
        public static void scaling()
        {
            var vector1 = new Vector2(3, 4);
            var vector2 = new Vector2(5, 5);

            Assert.AreEqual(MathV.scaling(vector1, vector2), new Vector2(15, 20));
        }
        [Test]
        public static void scaling2()
        {
            var vector = new Vector2(3, 4);
            var scalar1 = 3f;
            var scalar2 = 7f;

            Assert.AreEqual(MathV.scaling(vector, scalar1, scalar2), new Vector2(9, 28));
        }
        [Test]
        public static void rescaling()
        {
            var vector1 = new Vector2(3, 4);
            var vector2 = new Vector2(5, 5);

            Assert.AreEqual(MathV.rescaling(vector1, vector2), new Vector2(0.6f, 0.8f));
        }
        [Test]
        public static void rescaling2()
        {
            var vector = new Vector2(3, 4);
            var scalar1 = 3f;
            var scalar2 = 10f;

            Assert.AreEqual(MathV.rescaling(vector, scalar1, scalar2), new Vector2(1, 0.4f));
        }
        [Test]
        public static void recalculation()
        {
            var vector = new Vector2(3, 4);
            var scalar = 3f;

            Assert.AreEqual(MathV.recalculation(vector, scalar), new Vector2(1.8f, 2.4f));
            Assert.AreEqual(MathV.recalculation(Vector2.zero, scalar), Vector2.zero);
        }
        [Test]
        public static void recalculation2()
        {
            var vector1 = new Vector2(3, 4);
            var vector2 = new Vector2(5, 12);

            Assert.AreEqual(MathV.recalculation(vector1, vector2), new Vector2(7.8f, 10.4f));
        }
        [Test]
        public static void recalculation3()
        {
            var rotation = Quaternion.AngleAxis(60, Vector3.forward);
            var scalar = 3f;

            Assert.AreEqual(MathV.recalculation(rotation, scalar), new Vector2(1.5f, 1.5f * Mathf.Sqrt(3)));
        }
        [Test]
        public static void recalculation4()
        {
            var angle = 60f;
            var scalar = 3f;

            Assert.AreEqual(MathV.recalculation(angle, scalar), new Vector2(1.5f, 1.5f * Mathf.Sqrt(3)));
        }
        [Test]
        public static void recalculation5()
        {
            var angle = 60f;
            var vector = new Vector2(3, 4);

            Assert.AreEqual(MathV.recalculation(angle, vector), new Vector2(2.5f, 2.5f * Mathf.Sqrt(3)));
        }
        [Test]
        public static void within()
        {
            var vector1 = new Vector2(-2, -4);
            var vector2 = new Vector2(3, -4);
            var vector3 = new Vector2(8, -4);
            var vector4 = new Vector2(-2, 7);
            var vector5 = new Vector2(3, 7);
            var vector6 = new Vector2(8, 7);
            var vector7 = new Vector2(-2, 18);
            var vector8 = new Vector2(3, 18);
            var vector9 = new Vector2(8, 18);
            var underLeft = new Vector2(-1, -2);
            var upperRight = new Vector2(5, 12);

            Assert.AreEqual(MathV.within(vector1, underLeft, upperRight), new Vector2(-1, -2));
            Assert.AreEqual(MathV.within(vector2, underLeft, upperRight), new Vector2(3, -2));
            Assert.AreEqual(MathV.within(vector3, underLeft, upperRight), new Vector2(5, -2));
            Assert.AreEqual(MathV.within(vector4, underLeft, upperRight), new Vector2(-1, 7));
            Assert.AreEqual(MathV.within(vector5, underLeft, upperRight), new Vector2(3, 7));
            Assert.AreEqual(MathV.within(vector6, underLeft, upperRight), new Vector2(5, 7));
            Assert.AreEqual(MathV.within(vector7, underLeft, upperRight), new Vector2(-1, 12));
            Assert.AreEqual(MathV.within(vector8, underLeft, upperRight), new Vector2(3, 12));
            Assert.AreEqual(MathV.within(vector9, underLeft, upperRight), new Vector2(5, 12));
        }
        [Test]
        public static void easingElliptical()
        {
            var destination = new Vector2(6, 9);
            var now = 60f;
            var max = 90f;

            Assert.AreEqual(MathV.EasingV.elliptical(destination, now, max, true).y, 4.5f * Mathf.Sqrt(3));
            Assert.AreEqual(MathV.EasingV.elliptical(destination, now, max, false).x, 3 * Mathf.Sqrt(3));
        }
        [Test]
        public static void easingElliptical2()
        {
            var start = new Vector2(-2, -3);
            var destination = new Vector2(4, 6);
            var now = 60f;
            var max = 90f;

            Assert.AreEqual(MathV.EasingV.elliptical(start, destination, now, max, true).y, 4.5f * Mathf.Sqrt(3) - 3);
            Assert.AreEqual(MathV.EasingV.elliptical(start, destination, now, max, false).x, 3 * Mathf.Sqrt(3) - 2);
        }
    }
}
