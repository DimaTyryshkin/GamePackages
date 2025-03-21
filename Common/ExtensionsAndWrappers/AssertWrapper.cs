using System.Collections.Generic;
using UnityEngine.Assertions;

namespace GamePackages.Core
{
    public static class AssertWrapper
    {
        public static void IsAllNotNull<T>(T[] array, string msg = null) where T : class
        {
            Assert.IsNotNull(array, msg);

            foreach (var e in array)
            {
                Assert.IsNotNull(e, msg);
            }
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