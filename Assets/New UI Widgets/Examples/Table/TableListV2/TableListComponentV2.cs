namespace UIWidgets.Examples.TableListDemo
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TableList component.
	/// </summary>
	public class TableListComponentV2 : ListViewItem, IViewData<TableListItemV2>
	{
		/// <summary>
		/// Name.
		/// </summary>
		public TextAdapter Name;

		/// <summary>
		/// The text adapters for values.
		/// </summary>
		[SerializeField]
		public List<TextAdapter> ValueTexts = new List<TextAdapter>();

		/// <summary>
		/// The item.
		/// </summary>
		[SerializeField]
		protected TableListItemV2 Item;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(TableListItemV2 item)
		{
			Item = item;
			UpdateView();
		}

		/// <summary>
		/// Update view.
		/// </summary>
		protected void UpdateView()
		{
			Name.text = Item.Name;

			for (int index = 0; index < ValueTexts.Count; index++)
			{
				ValueTexts[index].text = index < Item.Values.Count ? Item.Values[index] : "none";
			}
		}

		/// <summary>
		/// Add value cell.
		/// </summary>
		/// <param name="template">Template.</param>
		public void AddValueCell(TableListCellV2 template)
		{
			var cell = Compatibility.Instantiate(template);
			cell.transform.SetParent(RectTransform, false);
			cell.gameObject.SetActive(true);

			ValueTexts.Add(cell.TextAdapter);
			foregrounds.Add(cell.TextAdapter.Graphic);

			UpdateView();
		}

		/// <summary>
		/// Remove value cell.
		/// </summary>
		/// <param name="valueCellIndex">Value cell index.</param>
		public void RemoveValueCell(int valueCellIndex)
		{
			var go = RectTransform.GetChild(valueCellIndex + 1).gameObject;
			Destroy(go);

			ValueTexts.RemoveAt(valueCellIndex);
			foregrounds.RemoveAt(valueCellIndex + 1);

			UpdateView();
		}
	}
}