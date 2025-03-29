namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test TableList.
	/// </summary>
	[RequireComponent(typeof(TableList))]
	public class TableListTest : MonoBehaviour
	{
		/// <summary>
		/// The table.
		/// </summary>
		[NonSerialized]
		public TableList Table;

		/// <summary>
		/// The header.
		/// </summary>
		[SerializeField]
		public TableHeader Header;

		/// <summary>
		/// The header cell.
		/// </summary>
		[SerializeField]
		public TableListCell HeaderCell;

		/// <summary>
		/// The row cell.
		/// </summary>
		[SerializeField]
		public TableListCell RowCell;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			TryGetComponent(out Table);

			Table.Init();
			Table.DataSource = Generate(100, 10);

			Header.Refresh();
		}

		/// <summary>
		/// Generate test data.
		/// </summary>
		/// <param name="rows">Rows.</param>
		/// <param name="columns">Columns.</param>
		/// <returns>Test list.</returns>
		protected static ObservableList<List<int>> Generate(int rows, int columns)
		{
			var data = new ObservableList<List<int>>();
			for (int i = 0; i < rows; i++)
			{
				var row = new List<int>(columns);
				for (int j = 0; j < columns; j++)
				{
					row.Add((columns * i) + j);
				}

				data.Add(row);
			}

			return data;
		}

		/// <summary>
		/// Add the column.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		public void AddColumn(string columnName)
		{
			foreach (var component in Table.GetComponentsEnumerator(PoolEnumeratorMode.All))
			{
				component.AddCell(RowCell);
			}

			var cell = Compatibility.Instantiate(HeaderCell);
			cell.TextAdapter.text = string.Format("{0} {1}", columnName, Table.DefaultItem.TextAdapterComponents.Count.ToString());
			Header.AddCell(cell.gameObject);

			Table.ComponentsColoring();
		}

		/// <summary>
		/// Removes the column.
		/// </summary>
		/// <param name="cellIndex">Index.</param>
		public void RemoveColumn(int cellIndex)
		{
			if (cellIndex >= Table.DefaultItem.TextAdapterComponents.Count)
			{
				return;
			}

			foreach (var component in Table.GetComponentsEnumerator(PoolEnumeratorMode.All))
			{
				component.RemoveCell(cellIndex);
			}

			var cell = Header.transform.GetChild(cellIndex);
			Header.RemoveCell(cell.gameObject);
		}
	}
}