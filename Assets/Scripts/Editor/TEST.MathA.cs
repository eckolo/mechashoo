using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using NUnit.Framework;
using System;
using System.Reflection;

public static partial class TEST
{
    public static class MathATEST
    {
        [Test]
        public static void max()
        {
            var angle1 = 405;
            var angle2 = 60;
            var angle3 = -120;

            Assert.AreEqual(MathA.maxAngle(angle1, angle2), 60);
            Assert.AreEqual(MathA.maxAngle(angle2, angle3), -120);
            Assert.AreEqual(MathA.maxAngle(angle1, angle3), -120);
        }
        [Test]
        public static void min()
        {
            var angle1 = 405;
            var angle2 = 60;
            var angle3 = -120;

            Assert.AreEqual(MathA.minAngle(angle1, angle2), 405);
            Assert.AreEqual(MathA.minAngle(angle2, angle3), 60);
            Assert.AreEqual(MathA.minAngle(angle1, angle3), 405);
        }
        [Test]
        public static void acute()
        {
            var angle1 = 405;
            var angle2 = 60;
            var angle3 = -120;

            Assert.AreEqual(MathA.acute(angle1), 45);
            Assert.AreEqual(MathA.acute(angle2), 60);
            Assert.AreEqual(MathA.acute(angle3), 120);
        }
        [Test]
        public static void correct()
        {
            var angle1 = 405;
            var angle2 = 60;

            Assert.AreEqual(MathA.correctAngle(angle1, angle2), 52.5f);
            Assert.AreEqual(MathA.correctAngle(angle1, angle2, 1), 45);
            Assert.AreEqual(MathA.correctAngle(angle1, angle2, 0), 60);
        }
        [Test]
        public static void compile()
        {
            var angle1 = 405;
            var angle2 = 60;
            var angle3 = -120;

            Assert.AreEqual(MathA.compile(angle1), 45);
            Assert.AreEqual(MathA.compile(angle2), 60);
            Assert.AreEqual(MathA.compile(angle3), 240);
        }
        [Test]
        public static void toAngle()
        {
            var vector1 = new Vector2(5, 5);
            var vector2 = new Vector2(-3, 3 * Mathf.Sqrt(3));
            var vector3 = new Vector2(-6, -2 * Mathf.Sqrt(3));
            var vector4 = new Vector2(12.3f, -12.3f);

            //どうしても有効桁数下2桁あたりで誤差が出るため手動で丸める
            Assert.AreEqual(Mathf.Round(MathA.toAngle(vector1) * 1000) / 1000, 45);
            Assert.AreEqual(Mathf.Round(MathA.toAngle(vector2) * 1000) / 1000, 120);
            Assert.AreEqual(Mathf.Round(MathA.toAngle(vector3) * 1000) / 1000, 210);
            Assert.AreEqual(Mathf.Round(MathA.toAngle(vector4) * 1000) / 1000, 315);
        }
        [Test]
        public static void toAngle2()
        {
            var rotation = Quaternion.AngleAxis(120, Vector3.forward);

            Assert.AreEqual(Mathf.Round(MathA.toAngle(rotation) * 1000) / 1000, 120);
        }
        [Test]
        public static void toRotation()
        {
            var angle = 120f;

            Assert.AreEqual(MathA.toRotation(angle), Quaternion.AngleAxis(120, Vector3.forward));
        }
        [Test]
        public static void toRotation2()
        {
            var vector = new Vector2(5, 5);

            Assert.AreEqual(MathA.toRotation(vector), Quaternion.AngleAxis(45, Vector3.forward));
        }
        [Test]
        public static void invert()
        {
            var angle1 = 405;
            var angle2 = 60;
            var angle3 = -120;

            Assert.AreEqual(MathA.invert(angle1), 135f);
            Assert.AreEqual(MathA.invert(angle2), 120f);
            Assert.AreEqual(MathA.invert(angle3), 300f);
        }
        [Test]
        public static void invert2()
        {
            var rotation1 = Quaternion.AngleAxis(405, Vector3.forward);
            var rotation2 = Quaternion.AngleAxis(60, Vector3.forward);
            var rotation3 = Quaternion.AngleAxis(-120, Vector3.forward);

            Assert.AreEqual(MathA.invert(rotation1), Quaternion.AngleAxis(135, Vector3.forward));
            Assert.AreEqual(MathA.invert(rotation2), Quaternion.AngleAxis(120, Vector3.forward));
            Assert.AreEqual(MathA.invert(rotation3), Quaternion.AngleAxis(300, Vector3.forward));
        }
    }
}