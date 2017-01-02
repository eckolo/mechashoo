using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public partial class Methods : MonoBehaviour
{

    protected static class Enums<enumType>
        where enumType : struct, IFormattable, IConvertible
    {
        public static bool isDefined(int value)
        {
            return Enum.IsDefined(typeof(enumType), value);
        }
        public static enumType normalize(int value)
        {
            if(!isDefined(value)) return min;
            return convert(value);
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
                return list.Max();
            }
        }
        public static enumType min
        {
            get
            {
                return list.Min();
            }
        }
        static enumType convert(int value)
        {
            return (enumType)Enum.ToObject(typeof(enumType), value);
        }
        static List<enumType> list
        {
            get
            {
                return ((IEnumerable<enumType>)Enum.GetValues(typeof(enumType))).ToList();
            }
        }
    }
}
