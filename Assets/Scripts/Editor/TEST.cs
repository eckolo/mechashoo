using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public static partial class TEST {
    [Test]
    public static void UnitTestTest1() {
        //Arrange
        var gameObject = new GameObject();

        //Act
        //Try to rename the GameObject
        var newGameObjectName = "UnitTestTest1";
        gameObject.name = newGameObjectName;

        //Assert
        //The object has a new name
        Assert.AreEqual(newGameObjectName, gameObject.name);
    }
    public static class InnerTEST {
        [Test]
        public static void InnerUnitTestTest1() {
            //Arrange
            var gameObject = new GameObject();

            //Act
            //Try to rename the GameObject
            var newGameObjectName = "InnerUnitTestTest1";
            gameObject.name = newGameObjectName;

            //Assert
            //The object has a new name
            Assert.AreEqual(newGameObjectName, gameObject.name);
        }
    }
}
