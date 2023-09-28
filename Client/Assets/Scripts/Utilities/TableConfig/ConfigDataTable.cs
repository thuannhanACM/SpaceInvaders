
using Core.Business;
using Core.Infrastructure.Extensions;
using FileHelpers;
using System;
using System.Collections.Generic;

namespace Core.Framework.Utilities
{
	public interface IConfigTable
    {
		public bool IsNullOrEmpty();
		public void Clear();
	}

	public interface IConfigTable<TDefinition> 
		where TDefinition : IGameDefinition
	{		
		public TDefinition[] All { get; }
		public TDefinition FindRecordById(string id);
		public IConfigTable<TDefinition> LoadTable(string text , string filePath);
	}
	public class ConfigTable<TDefinition>: IConfigTable, IConfigTable<TDefinition>
		where TDefinition : IGameDefinition
	{
		private TDefinition[] Records { get; set; }
		private Dictionary<string, TDefinition> _indices;
		public TDefinition[] All => Records;
		public bool IsNullOrEmpty() => Records.IsNullOrEmpty();
		public void Clear()
		{
			if (!Records.IsNullOrEmpty())
			{
				Records.Clear();
			}
		}
		public TDefinition FindRecordById(string id)
		{
			if (IsNullOrEmpty())
				throw new TableNotInitOrEmpty();
			if(!_indices.ContainsKey(id))
				throw new TableNotContainId($"Id: {id} not found");
			return _indices[id];
		}

		public IConfigTable<TDefinition> LoadTable(string text, string filePath)
		{
			FileHelperEngine engine = new FileHelperEngine(typeof(TDefinition));
			Records = engine.ReadString(text) as TDefinition[];

			if (IsNullOrEmpty())
				throw new TableIsEmpty($"CSV is empty {filePath}");

			RebuildIndexField<string>();
			return this;
		}
		
		private void RebuildIndexField<TIndex>()
        {
            _indices = new Dictionary<string, TDefinition>();
			for (int i = 0; i < Records.Length; i++)
				if (!_indices.ContainsKey(Records[i].Id))
					_indices.Add(Records[i].Id, Records[i]);
				else
					_indices[Records[i].Id] = Records[i];
		}

		private class TableNotInitOrEmpty: Exception { }
		private class TableIsEmpty : Exception 
		{
			public TableIsEmpty(string message) : base(message) { }
		}
		private class TableNotContainId : Exception
		{
			public TableNotContainId(string message): base(message) { }
		}
	}
}
