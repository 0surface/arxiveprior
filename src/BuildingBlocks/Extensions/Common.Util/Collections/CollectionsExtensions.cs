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
    }
}
