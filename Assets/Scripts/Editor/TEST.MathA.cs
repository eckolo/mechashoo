using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using NUnit.Framework;
using System;
using System.Reflection;

public static partial class TEST
{
    public static class MathA
    {
        [Test]
        public static void Max()
        {
            var angle1 = 405;
            var angle2 = 60;
            var angle3 = -120;

            Assert.AreEqual(global::MathA.MaxAngle(angle1, angle2), 60);
            Assert.AreEqual(global::MathA.MaxAngle(angle2, angle3), -120);
            Assert.AreEqual(global::MathA.MaxAngle(angle1, angle3), -120);
        }
        [Test]
        public static void Min()
        {
            var angle1 = 405;
            var angle2 = 60;
            var angle3 = -120;

            Assert.AreEqual(global::MathA.MinAngle(angle1, angle2), 405);
            Assert.AreEqual(global::MathA.MinAngle(angle2, angle3), 60);
            Assert.AreEqual(global::MathA.MinAngle(angle1, angle3), 405);
        }
        [Test]
        public static void Acute()
        {
            var angle1 = 405;
            var angle2 = 60;
            var angle3 = -120;

            Assert.AreEqual(global::MathA.Acute(angle1), 45);
            Assert.AreEqual(global::MathA.Acute(angle2), 60);
            Assert.AreEqual(global::MathA.Acute(angle3), 120);
        }
        [Test]
        public static void Correct()
        {
            var angle1 = 405;
            var angle2 = 60;

            Assert.AreEqual(global::MathA.CorrectAngle(angle1, angle2), 52.5f);
            Assert.AreEqual(global::MathA.CorrectAngle(angle1, angle2, 1), 45);
            Assert.AreEqual(global::MathA.CorrectAngle(angle1, angle2, 0), 60);
        }
        [Test]
        public static void Compile()
        {
            var angle1 = 405;
            var angle2 = 60;
            var angle3 = -120;

            Assert.AreEqual(global::MathA.Compile(angle1), 45);
            Assert.AreEqual(global::MathA.Compile(angle2), 60);
            Assert.AreEqual(global::MathA.Compile(angle3), 240);
        }
        [Test]
        public static void ToAngle()
        {
            var vector1 = new Vector2(5, 5);
            var vector2 = new Vector2(-3, 3 * Mathf.Sqrt(3));
            var vector3 = new Vector2(-6, -2 * Mathf.Sqrt(3));
            var vector4 = new Vector2(12.3f, -12.3f);

            //どうしても有効桁数下2桁あたりで検算側に誤差が出るため手動で丸める
            Assert.AreEqual(Mathf.Round(global::MathA.ToAngle(vector1) * 1000) / 1000, 45);
            Assert.AreEqual(Mathf.Round(global::MathA.ToAngle(vector2) * 1000) / 1000, 120);
            Assert.AreEqual(Mathf.Round(global::MathA.ToAngle(vector3) * 1000) / 1000, 210);
            Assert.AreEqual(Mathf.Round(global::MathA.ToAngle(vector4) * 1000) / 1000, 315);
        }
        [Test]
        public static void ToAngle2()
        {
            var rotation = Quaternion.AngleAxis(120, Vector3.forward);

            Assert.AreEqual(Mathf.Round(global::MathA.ToAngle(rotation) * 1000) / 1000, 120);
        }
        [Test]
        public static void ToRotation()
        {
            var angle = 120f;

            Assert.AreEqual(global::MathA.ToRotation(angle), Quaternion.AngleAxis(120, Vector3.forward));
        }
        [Test]
        public static void ToRotation2()
        {
            var vector = new Vector2(5, 5);

            Assert.AreEqual(global::MathA.ToRotation(vector), Quaternion.AngleAxis(45, Vector3.forward));
        }
        [Test]
        public static void Invert()
        {
            var angle1 = 405;
            var angle2 = 60;
            var angle3 = -120;

            Assert.AreEqual(global::MathA.Invert(angle1), 135f);
            Assert.AreEqual(global::MathA.Invert(angle2), 120f);
            Assert.AreEqual(global::MathA.Invert(angle3), 300f);
        }
        [Test]
        public static void Invert2()
        {
            var rotation1 = Quaternion.AngleAxis(405, Vector3.forward);
            var rotation2 = Quaternion.AngleAxis(60, Vector3.forward);
            var rotation3 = Quaternion.AngleAxis(-120, Vector3.forward);

            Assert.AreEqual(global::MathA.Invert(rotation1), Quaternion.AngleAxis(135, Vector3.forward));
            Assert.AreEqual(global::MathA.Invert(rotation2), Quaternion.AngleAxis(120, Vector3.forward));
            Assert.AreEqual(global::MathA.Invert(rotation3), Quaternion.AngleAxis(300, Vector3.forward));
        }
    }
}