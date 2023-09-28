using FileHelpers;
using System;

namespace Core.Business
{
    [DelimitedRecord(",")]
    [IgnoreFirst(1)]
    [Serializable]
    public class BulletDefinition : BaseGameDefinition
    {
        public string SkinPath { get; set; }
        public float MoveSpd { get; set; }
        public float Radius { get; set; }
    }
}
