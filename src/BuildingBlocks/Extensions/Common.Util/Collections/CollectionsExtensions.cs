namespace System.Collections.Generic
{
    public static class CollectionsExtensions
    {
        /// <summary>
        ///  Adds an object to the end of the System.Collections.Generic.List if the provided value 
        ///  is not null and the List is not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        public static void AddIfNotNull<T>(this List<T> list, T value)
        {
            if (list != null && value != null)
            {
                list.Add(value);
            }
        }


        /// <summary>
        ///  Returns true and adds an object to the end of the System.Collections.Generic.List if the provided value 
        ///  is not null and the List is not null. Otherwise returns false;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryAddIfNotNull<T>(this List<T> list, T value)
        {
            if (list != null && value != null)
            {
                list.Add(value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds an object to the end of the System.Collections.Generic.List if the provided value 
        ///  is not null, the List is not null and the predicate is true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">List<typeparamref name="T"/></param>
        /// <param name="value">T</param>
        /// <param name="predicate">bool</param>
        public static void AddIfSatisfies<T>(this List<T> list, T value, bool predicate)
        {
            if (list != null && value != null && predicate)
            {
                list.Add(value);
            }
        }

        /// <summary>
        /// Returns true and adds an object to the end of the System.Collections.Generic.List if the provided value 
        ///  is not null, the List is not null and the predicate is true. Otherwise returns false;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool TryAddIfSatisfies<T>(this List<T> list, T value, bool predicate)
        {
            if (list != null && value != null && predicate)
            {
                list.Add(value);
                return true;
            }
            return false;
        }
    }
}
