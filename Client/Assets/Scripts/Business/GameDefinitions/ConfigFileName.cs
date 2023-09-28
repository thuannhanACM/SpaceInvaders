using System;
using System.Collections.Generic;

namespace Core.Business
{
    public static class ConfigFileName
    {
        public static Dictionary<Type, string> Mapper = new Dictionary<Type, string>()
        {
            { typeof(ShipDefinition), "Assets/Bundles/Csv/Ships.csv" },
            { typeof(BulletDefinition), "Assets/Bundles/Csv/Bullets.csv" },
            { typeof(AlienDefinition), "Assets/Bundles/Csv/Aliens.csv" },
        };

        public static Dictionary<Type, string> ProductionMapper = new Dictionary<Type, string>()
        {
        };

        public static string GetFileName<T>(string fileId)
        {
            string suffix = string.IsNullOrEmpty(fileId) ? "" : $"_{fileId}";
            return string.Format(Mapper[typeof(T)], suffix);
        }

        public static string GetProdFileName<T>()
        {
            return ProductionMapper[typeof(T)];
        }
    }
}
