using FileHelpers;
using System;

namespace Core.Business
{
    [DelimitedRecord(",")]
    [IgnoreFirst(1)]
    [Serializable]
    public class ShipDefinition : BaseGameDefinition
    {
        public string SkinPath { get; set; }
        public float MoveSpeed { get; set; }
        public float Radius { get; set; }
    }
}
