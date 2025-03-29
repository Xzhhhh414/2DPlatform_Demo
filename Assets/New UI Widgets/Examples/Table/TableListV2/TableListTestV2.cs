namespace UIWidgets.Examples.TableListDemo
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test TableListV2.
	/// </summary>
	public class TableListTestV2 : MonoBehaviour
	{
		/// <summary>
		/// Table.
		/// </summary>
		[SerializeField]
		public TableListV2 Table;

		/// <summary>
		/// Header template.
		/// </summary>
		[SerializeField]
		public TableListCellV2 HeaderTemplate;

		/// <summary>
		/// Value template.
		/// </summary>
		[SerializeField]
		public TableListCellV2 ValueTemplate;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void Start() => Table.DataSource = Generate(100, 10);

		/// <summary>
		/// Add column.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		public void AddColumn(string columnName)
		{
			foreach (var row in Table.GetComponentsEnumerator(PoolEnumeratorMode.All))
			{
				row.AddValueCell(ValueTemplate);
			}

			// add header
			var cell = Compatibility.Instantiate(HeaderTemplate);
			var columns = Table.DefaultItem.ValueTexts.Count;
			cell.TextAdapter.text = string.Format("{0} {1}", columnName, columns.ToString());
			Table.Header.AddCell(cell.gameObject);

			Table.ComponentsColoring();
		}

		/// <summary>
		/// Remove column.
		/// </summary>
		/// <param name="columnIndex">Column index.</param>
		public void RemoveColumn(int columnIndex)
		{
			if (columnIndex >= Table.DefaultItem.ValueTexts.Count)
			{
				return;
			}

			foreach (var row in Table.GetComponentsEnumerator(PoolEnumeratorMode.All))
			{
				row.RemoveValueCell(columnIndex);
			}

			var cell = Table.Header.transform.GetChild(columnIndex + 1);
			Table.Header.RemoveCell(cell.gameObject);
		}

		ObservableList<TableListItemV2> Generate(int rows, int columns)
		{
			var data = new ObservableList<TableListItemV2>(rows);
			for (int row = 0; row < rows; row++)
			{
				var item = new TableListItemV2()
				{
					Name = "Item " + row,
					Values = CreateValue(row, columns),
				};
				data.Add(item);
			}

			return data;
		}

		List<string> CreateValue(int row, int columns)
		{
			var values = new List<string>(columns);
			for (int j = 0; j < columns; j++)
			{
				var v = (columns * row) + j;
				values.Add(v.ToString());
			}

			return values;
		}
	}
}