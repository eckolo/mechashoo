using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using NUnit.Framework;
using System;
using System.Reflection;
using static UnityEngine.TextAnchor;

public static partial class TEST
{
    public static class MathV
    {
        [Test]
        public static void max()
        {
            var bigVector = new Vector2(5, 5);
            var smallVector = new Vector2(3, 4);

            Assert.AreEqual(global::MathV.max(smallVector, bigVector), new Vector2(5, 5));
        }
        [Test]
        public static void max2()
        {
            var vector = new Vector2(3, 4);
            var smallScalar = 4f;
            var bigScalar = 10f;

            Assert.AreEqual(global::MathV.max(vector, smallScalar), new Vector2(3, 4));
            Assert.AreEqual(global::MathV.max(vector, bigScalar), new Vector2(6, 8));
        }
        [Test]
        public static void min()
        {
            var bigVector = new Vector2(5, 5);
            var smallVector = new Vector2(3, 4);

            Assert.AreEqual(global::MathV.min(smallVector, bigVector), new Vector2(3, 4));
        }
        [Test]
        public static void min2()
        {
            var vector = new Vector2(6, 8);
            var smallScalar = 5f;
            var bigScalar = 12f;

            Assert.AreEqual(global::MathV.min(vector, smallScalar), new Vector2(3, 4));
            Assert.AreEqual(global::MathV.min(vector, bigScalar), new Vector2(6, 8));
        }
        [Test]
        public static void abs()
        {
            var vector = new Vector2(-3, -4);

            Assert.AreEqual(global::MathV.abs(vector), new Vector2(3, 4));
        }
        [Test]
        public static void correct()
        {
            var vector1 = new Vector2(3, 4);
            var vector2 = new Vector2(5, 5);

            Assert.AreEqual(global::MathV.correct(vector1, vector2), new Vector2(4, 4.5f));
            Assert.AreEqual(global::MathV.correct(vector1, vector2, 1), new Vector2(3, 4));
            Assert.AreEqual(global::MathV.correct(vector1, vector2, 0), new Vector2(5, 5));
        }
        [Test]
        public static void correct2()
        {
            var scalar1 = 3f;
            var scalar2 = 5f;

            Assert.AreEqual(global::MathV.correct(scalar1, scalar2), 4f);
            Assert.AreEqual(global::MathV.correct(scalar1, scalar2, 1), 3f);
            Assert.AreEqual(global::MathV.correct(scalar1, scalar2, 0), 5f);
        }
        [Test]
        public static void scaling()
        {
            var vector1 = new Vector2(3, 4);
            var vector2 = new Vector2(5, 5);

            Assert.AreEqual(global::MathV.scaling(vector1, vector2), new Vector2(15, 20));
        }
        [Test]
        public static void scaling2()
        {
            var vector = new Vector2(3, 4);
            var scalar1 = 3f;
            var scalar2 = 7f;

            Assert.AreEqual(global::MathV.scaling(vector, scalar1, scalar2), new Vector2(9, 28));
        }
        [Test]
        public static void rescaling()
        {
            var vector1 = new Vector2(3, 4);
            var vector2 = new Vector2(5, 5);

            Assert.AreEqual(global::MathV.rescaling(vector1, vector2), new Vector2(0.6f, 0.8f));
        }
        [Test]
        public static void rescaling2()
        {
            var vector = new Vector2(3, 4);
            var scalar1 = 3f;
            var scalar2 = 10f;

            Assert.AreEqual(global::MathV.rescaling(vector, scalar1, scalar2), new Vector2(1, 0.4f));
        }
        [Test]
        public static void recalculation1()
        {
            var vector = new Vector2(3, 4);
            var scalar = 3f;

            Assert.AreEqual(global::MathV.toVector(vector), new Vector2(0.6f, 0.8f));
            Assert.AreEqual(global::MathV.toVector(vector, scalar), new Vector2(1.8f, 2.4f));
            Assert.AreEqual(global::MathV.toVector(Vector2.zero, scalar), Vector2.zero);
        }
        [Test]
        public static void recalculation2()
        {
            var vector1 = new Vector2(3, 4);
            var vector2 = new Vector2(5, 12);

            Assert.AreEqual(global::MathV.toVector(vector1, vector2), new Vector2(7.8f, 10.4f));
        }
        [Test]
        public static void recalculation3()
        {
            var rotation = Quaternion.AngleAxis(60, Vector3.forward);
            var scalar = 3f;

            Assert.AreEqual(global::MathV.toVector(rotation), new Vector2(0.5f, 0.5f * Mathf.Sqrt(3)));
            Assert.AreEqual(global::MathV.toVector(rotation, scalar), new Vector2(1.5f, 1.5f * Mathf.Sqrt(3)));
        }
        [Test]
        public static void recalculation4()
        {
            var angle = 60f;
            var scalar = 3f;

            Assert.AreEqual(global::MathV.toVector(angle), new Vector2(0.5f, 0.5f * Mathf.Sqrt(3)));
            Assert.AreEqual(global::MathV.toVector(angle, scalar), new Vector2(1.5f, 1.5f * Mathf.Sqrt(3)));
        }
        [Test]
        public static void recalculation5()
        {
            var angle = 60f;
            var vector = new Vector2(3, 4);

            Assert.AreEqual(global::MathV.toVector(angle, vector), new Vector2(2.5f, 2.5f * Mathf.Sqrt(3)));
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

            Assert.AreEqual(global::MathV.within(vector1, underLeft, upperRight), new Vector2(-1, -2));
            Assert.AreEqual(global::MathV.within(vector2, underLeft, upperRight), new Vector2(3, -2));
            Assert.AreEqual(global::MathV.within(vector3, underLeft, upperRight), new Vector2(5, -2));
            Assert.AreEqual(global::MathV.within(vector4, underLeft, upperRight), new Vector2(-1, 7));
            Assert.AreEqual(global::MathV.within(vector5, underLeft, upperRight), new Vector2(3, 7));
            Assert.AreEqual(global::MathV.within(vector6, underLeft, upperRight), new Vector2(5, 7));
            Assert.AreEqual(global::MathV.within(vector7, underLeft, upperRight), new Vector2(-1, 12));
            Assert.AreEqual(global::MathV.within(vector8, underLeft, upperRight), new Vector2(3, 12));
            Assert.AreEqual(global::MathV.within(vector9, underLeft, upperRight), new Vector2(5, 12));
        }
        [Test]
        public static void getAxis()
        {
            Assert.AreEqual(global::MathV.getAxis(LowerLeft), new Vector2(0, 0));
            Assert.AreEqual(global::MathV.getAxis(LowerCenter), new Vector2(0.5f, 0));
            Assert.AreEqual(global::MathV.getAxis(LowerRight), new Vector2(1, 0));
            Assert.AreEqual(global::MathV.getAxis(MiddleLeft), new Vector2(0, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleCenter), new Vector2(0.5f, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleRight), new Vector2(1, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(UpperLeft), new Vector2(0, 1));
            Assert.AreEqual(global::MathV.getAxis(UpperCenter), new Vector2(0.5f, 1));
            Assert.AreEqual(global::MathV.getAxis(UpperRight), new Vector2(1, 1));

            Assert.AreEqual(global::MathV.getAxis(LowerLeft, LowerLeft), new Vector2(0, 0));
            Assert.AreEqual(global::MathV.getAxis(LowerCenter, LowerLeft), new Vector2(0.5f, 0));
            Assert.AreEqual(global::MathV.getAxis(LowerRight, LowerLeft), new Vector2(1, 0));
            Assert.AreEqual(global::MathV.getAxis(MiddleLeft, LowerLeft), new Vector2(0, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleCenter, LowerLeft), new Vector2(0.5f, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleRight, LowerLeft), new Vector2(1, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(UpperLeft, LowerLeft), new Vector2(0, 1));
            Assert.AreEqual(global::MathV.getAxis(UpperCenter, LowerLeft), new Vector2(0.5f, 1));
            Assert.AreEqual(global::MathV.getAxis(UpperRight, LowerLeft), new Vector2(1, 1));

            Assert.AreEqual(global::MathV.getAxis(LowerLeft, LowerCenter), new Vector2(-0.5f, 0));
            Assert.AreEqual(global::MathV.getAxis(LowerCenter, LowerCenter), new Vector2(0, 0));
            Assert.AreEqual(global::MathV.getAxis(LowerRight, LowerCenter), new Vector2(0.5f, 0));
            Assert.AreEqual(global::MathV.getAxis(MiddleLeft, LowerCenter), new Vector2(-0.5f, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleCenter, LowerCenter), new Vector2(0, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleRight, LowerCenter), new Vector2(0.5f, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(UpperLeft, LowerCenter), new Vector2(-0.5f, 1));
            Assert.AreEqual(global::MathV.getAxis(UpperCenter, LowerCenter), new Vector2(0, 1));
            Assert.AreEqual(global::MathV.getAxis(UpperRight, LowerCenter), new Vector2(0.5f, 1));

            Assert.AreEqual(global::MathV.getAxis(LowerLeft, LowerRight), new Vector2(-1, 0));
            Assert.AreEqual(global::MathV.getAxis(LowerCenter, LowerRight), new Vector2(-0.5f, 0));
            Assert.AreEqual(global::MathV.getAxis(LowerRight, LowerRight), new Vector2(0, 0));
            Assert.AreEqual(global::MathV.getAxis(MiddleLeft, LowerRight), new Vector2(-1, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleCenter, LowerRight), new Vector2(-0.5f, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleRight, LowerRight), new Vector2(0, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(UpperLeft, LowerRight), new Vector2(-1, 1));
            Assert.AreEqual(global::MathV.getAxis(UpperCenter, LowerRight), new Vector2(-0.5f, 1));
            Assert.AreEqual(global::MathV.getAxis(UpperRight, LowerRight), new Vector2(0, 1));

            Assert.AreEqual(global::MathV.getAxis(LowerLeft, MiddleLeft), new Vector2(0, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(LowerCenter, MiddleLeft), new Vector2(0.5f, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(LowerRight, MiddleLeft), new Vector2(1, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleLeft, MiddleLeft), new Vector2(0, 0));
            Assert.AreEqual(global::MathV.getAxis(MiddleCenter, MiddleLeft), new Vector2(0.5f, 0));
            Assert.AreEqual(global::MathV.getAxis(MiddleRight, MiddleLeft), new Vector2(1, 0));
            Assert.AreEqual(global::MathV.getAxis(UpperLeft, MiddleLeft), new Vector2(0, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(UpperCenter, MiddleLeft), new Vector2(0.5f, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(UpperRight, MiddleLeft), new Vector2(1, 0.5f));

            Assert.AreEqual(global::MathV.getAxis(LowerLeft, MiddleCenter), new Vector2(-0.5f, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(LowerCenter, MiddleCenter), new Vector2(0, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(LowerRight, MiddleCenter), new Vector2(0.5f, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleLeft, MiddleCenter), new Vector2(-0.5f, 0));
            Assert.AreEqual(global::MathV.getAxis(MiddleCenter, MiddleCenter), new Vector2(0, 0));
            Assert.AreEqual(global::MathV.getAxis(MiddleRight, MiddleCenter), new Vector2(0.5f, 0));
            Assert.AreEqual(global::MathV.getAxis(UpperLeft, MiddleCenter), new Vector2(-0.5f, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(UpperCenter, MiddleCenter), new Vector2(0, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(UpperRight, MiddleCenter), new Vector2(0.5f, 0.5f));

            Assert.AreEqual(global::MathV.getAxis(LowerLeft, MiddleRight), new Vector2(-1, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(LowerCenter, MiddleRight), new Vector2(-0.5f, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(LowerRight, MiddleRight), new Vector2(0, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleLeft, MiddleRight), new Vector2(-1, 0));
            Assert.AreEqual(global::MathV.getAxis(MiddleCenter, MiddleRight), new Vector2(-0.5f, 0));
            Assert.AreEqual(global::MathV.getAxis(MiddleRight, MiddleRight), new Vector2(0, 0));
            Assert.AreEqual(global::MathV.getAxis(UpperLeft, MiddleRight), new Vector2(-1, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(UpperCenter, MiddleRight), new Vector2(-0.5f, 0.5f));
            Assert.AreEqual(global::MathV.getAxis(UpperRight, MiddleRight), new Vector2(0, 0.5f));

            Assert.AreEqual(global::MathV.getAxis(LowerLeft, UpperLeft), new Vector2(0, -1));
            Assert.AreEqual(global::MathV.getAxis(LowerCenter, UpperLeft), new Vector2(0.5f, -1));
            Assert.AreEqual(global::MathV.getAxis(LowerRight, UpperLeft), new Vector2(1, -1));
            Assert.AreEqual(global::MathV.getAxis(MiddleLeft, UpperLeft), new Vector2(0, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleCenter, UpperLeft), new Vector2(0.5f, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleRight, UpperLeft), new Vector2(1, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(UpperLeft, UpperLeft), new Vector2(0, 0));
            Assert.AreEqual(global::MathV.getAxis(UpperCenter, UpperLeft), new Vector2(0.5f, 0));
            Assert.AreEqual(global::MathV.getAxis(UpperRight, UpperLeft), new Vector2(1, 0));

            Assert.AreEqual(global::MathV.getAxis(LowerLeft, UpperCenter), new Vector2(-0.5f, -1));
            Assert.AreEqual(global::MathV.getAxis(LowerCenter, UpperCenter), new Vector2(0, -1));
            Assert.AreEqual(global::MathV.getAxis(LowerRight, UpperCenter), new Vector2(0.5f, -1));
            Assert.AreEqual(global::MathV.getAxis(MiddleLeft, UpperCenter), new Vector2(-0.5f, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleCenter, UpperCenter), new Vector2(0, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleRight, UpperCenter), new Vector2(0.5f, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(UpperLeft, UpperCenter), new Vector2(-0.5f, 0));
            Assert.AreEqual(global::MathV.getAxis(UpperCenter, UpperCenter), new Vector2(0, 0));
            Assert.AreEqual(global::MathV.getAxis(UpperRight, UpperCenter), new Vector2(0.5f, 0));

            Assert.AreEqual(global::MathV.getAxis(LowerLeft, UpperRight), new Vector2(-1, -1));
            Assert.AreEqual(global::MathV.getAxis(LowerCenter, UpperRight), new Vector2(-0.5f, -1));
            Assert.AreEqual(global::MathV.getAxis(LowerRight, UpperRight), new Vector2(0, -1));
            Assert.AreEqual(global::MathV.getAxis(MiddleLeft, UpperRight), new Vector2(-1, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleCenter, UpperRight), new Vector2(-0.5f, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(MiddleRight, UpperRight), new Vector2(0, -0.5f));
            Assert.AreEqual(global::MathV.getAxis(UpperLeft, UpperRight), new Vector2(-1, 0));
            Assert.AreEqual(global::MathV.getAxis(UpperCenter, UpperRight), new Vector2(-0.5f, 0));
            Assert.AreEqual(global::MathV.getAxis(UpperRight, UpperRight), new Vector2(0, 0));
        }
        [Test]
        public static void log1()
        {
            var vector1 = Vector2.one.normalized * (Mathf.Exp(11.3f) - 1);
            var vector2 = new Vector2(-Mathf.Exp(26.43f) + 1, 0);

            Assert.AreEqual(vector1.log().magnitude, 11.3f);
            Assert.AreEqual(vector1.log().normalized, Vector2.one.normalized);
            Assert.AreEqual(vector2.log(), new Vector2(-26.43f, 0));
        }
        [Test]
        public static void log2()
        {
            var vector1 = Vector2.one.normalized * (Mathf.Pow(2.6f, 11.3f) - 1);
            var vector2 = new Vector2(-Mathf.Pow(4.63f, 26.43f) + 1, 0);

            Assert.AreEqual(vector1.log(2.6f), Vector2.one.normalized * 11.3f);
            Assert.AreEqual(vector2.log(4.63f), new Vector2(-26.43f, 0));
        }
        [Test]
        public static void easingElliptical()
        {
            var destination = new Vector2(6, 9);
            var now = 60f;
            var max = 90f;

            Assert.AreEqual(global::MathV.EasingV.elliptical(destination, now, max, true).y, 4.5f * Mathf.Sqrt(3));
            Assert.AreEqual(global::MathV.EasingV.elliptical(destination, now, max, false).x, 3 * Mathf.Sqrt(3));
        }
        [Test]
        public static void easingElliptical2()
        {
            var start = new Vector2(-2, -3);
            var destination = new Vector2(4, 6);
            var now = 60f;
            var max = 90f;

            Assert.AreEqual(global::MathV.EasingV.elliptical(start, destination, now, max, true).y, 4.5f * Mathf.Sqrt(3) - 3);
            Assert.AreEqual(global::MathV.EasingV.elliptical(start, destination, now, max, false).x, 3 * Mathf.Sqrt(3) - 2);
        }
    }
}
