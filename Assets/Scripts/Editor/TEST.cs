using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public static partial class TEST {
    public static class Myself {
        [Test]
        public static void UnitTestTest() {
            //Arrange
            var gameObject = new GameObject();

            //Act
            //Try to rename the GameObject
            var newGameObjectName = "UnitTestTest";
            gameObject.name = newGameObjectName;

            //Assert
            //The object has a new name
            Assert.AreEqual(newGameObjectName, gameObject.name);
        }
    }
}
