using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace GamePackages.Core
{
    public static class AssertWrapper
    {
        [System.Diagnostics.DebuggerHidden]
        public static void AssertSorted<T>(this T[] array, Func<T, int> getIndex, string msg = null)
        {
            Assert.IsTrue(array.IsSorted(getIndex), msg ?? nameof(AssertSorted));
        }

        public static bool IsSorted<T>(this T[] array, Func<T, int> getIndex)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (i != getIndex(array[i]))
                    return false;
            }

            return true;
        }

        public static void IsAllNotNull<T>(T[] array, string msg = null) where T : class
        {
            Assert.IsNotNull(array, msg);

            foreach (var e in array)
            {
                Assert.IsNotNull(e, msg);
            }
        }

        public static bool IsAllNotNullBool<T>(T[] array) where T : class
        {
            if (array == null)
                return false;

            foreach (var e in array)
            {
                if (e == null)
                    return false;
            }

            return true;
        }

        public static void IsAllNotNull<T>(List<T> array, string msg = null) where T : class
        {
            Assert.IsNotNull(array, msg);

            foreach (var e in array)
            {
                Assert.IsNotNull(e, msg);
            }
        }

        public static void IsAllNotNull<T>(IEnumerable<T> collection, string msg = null) where T : class
        {
            Assert.IsNotNull(collection, msg);

            foreach (var e in collection)
                Assert.IsNotNull(e, msg);
        }
    }
}