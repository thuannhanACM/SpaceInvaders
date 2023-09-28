
using System.Collections.Generic;

namespace Core
{
    public static class ArrayExtension
    {
        public static T[] AppendNewElement<T>(this T[] source, T newOne)
        {
            int length = source.Length;
            T[] destination = new T[length + 1];
            source.CopyTo(destination, 0);
            destination[length] = newOne;

            return destination;
        }

        public static bool DeepEquals(this object obj, object another)
        {

            if (ReferenceEquals(obj, another)) return true;
            if ((obj == null) || (another == null)) return false;
            if (obj.GetType() != another.GetType()) return false;

            var result = true;
            foreach (var property in obj.GetType().GetProperties())
            {
                var objValue = property.GetValue(obj);
                var anotherValue = property.GetValue(another);
                if (!objValue.Equals(anotherValue)) result = false;
            }

            return result;
        }

        public static bool DeepEquals<T>(this IEnumerable<T> obj, IEnumerable<T> another)
        {
            if (ReferenceEquals(obj, another)) return true;
            if ((obj == null) || (another == null)) return false;

            bool result = true;
            using (IEnumerator<T> enumerator1 = obj.GetEnumerator())
            using (IEnumerator<T> enumerator2 = another.GetEnumerator())
            {
                while (true)
                {
                    bool hasNext1 = enumerator1.MoveNext();
                    bool hasNext2 = enumerator2.MoveNext();

                    if (hasNext1 != hasNext2 || !enumerator1.Current.DeepEquals(enumerator2.Current))
                    {
                        result = false;
                        break;
                    }

                    if (!hasNext1) break;
                }
            }

            return result;
        }
    }
}
