using Core.Infrastructure.Extensions;
using FileHelpers;

namespace Core.Business
{
    public class SubClassConvert<TDefinition> : ConverterBase where TDefinition : class
	{
		public override object StringToField(string text)
		{
			FileHelperEngine engine = new FileHelperEngine(typeof(TDefinition));
			TDefinition[] results = engine.ReadString(text) as TDefinition[];
			if (results.IsNullOrEmpty())
				return null;
			return results[0];
		}
	}
}
