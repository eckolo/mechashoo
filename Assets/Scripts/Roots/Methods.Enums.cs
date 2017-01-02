using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public partial class Methods : MonoBehaviour
{

    protected static class Enums<enumType>
        where enumType : struct, IComparable, IFormattable, IConvertible
    {
        public static bool isDefined(int value)
        {
            return Enum.IsDefined(typeof(enumType), value);
        }
        public static int normalize(int value)
        {
            if(!isDefined(value)) return minNum;
            return value;
        }
        public static int length
        {
            get
            {
                return Enum.GetValues(typeof(enumType)).Length;
            }
        }
        public static enumType max
        {
            get
            {
                return enumList.Max();
            }
        }
        public static int maxNum
        {
            get
            {
                return intList.Max();
            }
        }
        public static enumType min
        {
            get
            {
                return enumList.Min();
            }
        }
        public static int minNum
        {
            get
            {
                return intList.Min();
            }
        }
        static IEnumerable<enumType> enumList
        {
            get
            {
                return (IEnumerable<enumType>)Enum.GetValues(typeof(enumType));
            }
        }
        static IEnumerable<int> intList
        {
            get
            {
                return (IEnumerable<int>)Enum.GetValues(typeof(enumType));
            }
        }
    }
}
