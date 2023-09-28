using System;
using Zenject;

namespace Core.Framework.Helpers
{
    public static class InjectValidatableHelper
    {
        public static void ValidateByEnumName<TEnumName, TClass>(DiContainer container) where TEnumName : Enum
        {
            Array values = Enum.GetValues(typeof(TEnumName));

            foreach (TEnumName e in values)
            {
                container.ResolveId<TClass>(e);
            }
        }
    }
}
