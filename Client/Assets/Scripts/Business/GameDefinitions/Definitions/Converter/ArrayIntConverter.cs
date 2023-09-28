using FileHelpers;

namespace Core.Business
{
    public class ArrayIntConverter : ConverterBase
	{
		public override object StringToField(string from)
		{
			string[] str = from.Split(',');
			int[] r = new int[str.Length];
			for (int i = 0; i < str.Length; i++)
			{
				if (int.TryParse(str[i], out int cost))
				{
					r[i] = cost;
				}
			}
			return r;
		}

	}
}
