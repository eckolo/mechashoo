using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public static partial class TEST
{
    public static class Myself
    {
        [Test]
        public static void testBase()
        {
            //Arrange
            var gameObject = new GameObject();

            //Act
            //Try to rename the GameObject
            var newGameObjectName = "testBase";
            gameObject.name = newGameObjectName;

            //Assert
            //The object has a new name
            Assert.AreEqual(newGameObjectName, gameObject.name);
        }
    }
}
