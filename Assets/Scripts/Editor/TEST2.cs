using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public static partial class TEST {
    [Test]
    public static void UnitTestTest2() {
        //Arrange
        var gameObject = new GameObject();

        //Act
        //Try to rename the GameObject
        var newGameObjectName = "UnitTestTest2";
        gameObject.name = newGameObjectName;

        //Assert
        //The object has a new name
        Assert.AreEqual(newGameObjectName, gameObject.name);
    }
}
