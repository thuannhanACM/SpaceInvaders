using System.Linq;

namespace Core.Framework.Utilities
{
    public static class GetEnumToString
    {
        public static string[] Get<T>(bool nonNegativeValue = false) where T : System.Enum
        {
            int[] enumValues = (int[])System.Enum.GetValues(typeof(T));

            if (nonNegativeValue)
            {
                // Remove elements smaller than 0.
                enumValues = enumValues.Where((source, index) => enumValues[index] >= 0).ToArray();
            }

            // Fix bug of "Enum.GetValues".
            System.Array.Sort(enumValues);

            int length = enumValues.Length;
            string[] result = new string[length];

            for (int i = 0; i < length; i++)
                result[i] = (System.Enum.GetName(typeof(T), enumValues[i])).ToString();

            return result;
        }
    }
}
