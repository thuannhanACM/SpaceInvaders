using System;
using System.Collections.Generic;

namespace Core.Infrastructure
{
    public class Query
    {
        public static string Create(string queryType, params string[] parrams)
        {
            string s = string.Join("&", parrams);
            return string.Format("{0}?{1}", queryType, s);
        }

        public static int GetInt(string key, string query)
        {
            string p = GetString(key, query);
            bool found = int.TryParse(p, out int result);
            if (!found)
            {
                throw new Exception("Parram not a number please fix your bug");
            }
            return result;
        }

        public static float GetFloat(string key, string query)
        {
            string p = GetString(key, query);
            bool found = float.TryParse(p, out float result);
            if (!found)
            {
                throw new Exception("Parram not a number please fix your bug");
            }
            return result;
        }

        public static bool GetBool(string key, string query)
        {
            string p = GetString(key, query);
            return p.ToLower() == "true" || p == "1";
        }

        public static string GetString(string key, string query)
        {
            string parramsStr = GetParamsStr(query);

            foreach (string s in parramsStr.Split('&'))
            {
                string[] ss = s.Split('=');
                if (ss[0] == key)
                {
                    return ss[1];
                }
            }

            throw new Exception(string.Format(
                "Parram not found please fix your bug key: {0}, query {1}",
                key,
                query));
        }

        public static Dictionary<string, object> GetParamsDictionary(string query)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            string parramsStr = GetParamsStr(query);

            foreach (string s in parramsStr.Split('&'))
            {
                string[] ss = s.Split('=');
                result.Add(ss[0], ss[1]);
            }

            return result;
        }

        private static string GetParamsStr(string query)
        {
            int questionMarkIndex = query.IndexOf("?");
            string parramsStr =
                query.Substring(
                    questionMarkIndex + 1,
                    query.Length - 1 - questionMarkIndex);
            return parramsStr;
        }

        public static string GetQueryType(string query)
        {
            int questionMarkIndex = query.IndexOf("?");
            return query.Substring(0, questionMarkIndex);
        }

        public static T GetQueryTypeEnum<T>(
            string query)
            where T : struct, IConvertible
        {
            string type = GetQueryType(query);
            return ToEnum<T>(type);
        }

        public static T GetEnum<T>(
            string key,
            string query)
            where T : struct, IConvertible
        {
            string p = GetString(key, query);
            return ToEnum<T>(p);
        }

        public static T ToEnum<T>(
            string value)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception(string.Format(
                    "T must be an enumerated type: {0}",
                    typeof(T)));
            }

            if (!Enum.TryParse(value, true, out T result))
            {
                throw new Exception(string.Format(
                    "Parram not a enum please fix your bug: {0}",
                    typeof(T)));
            }
            return result;
        }

    }
}