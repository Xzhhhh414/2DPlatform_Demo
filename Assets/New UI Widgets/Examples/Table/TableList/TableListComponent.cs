namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TableList component.
	/// </summary>
	public class TableListComponent : ListViewItem, IViewData<List<int>>
	{
		/// <summary>
		/// The text components.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with TextAdapterComponents.")]
		public List<Text> TextComponents = new List<Text>();

		/// <summary>
		/// The text components.
		/// </summary>
		[SerializeField]
		public List<TextAdapter> TextAdapterComponents = new List<TextAdapter>();

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			#pragma warning disable 0618
			if (Foreground.Length != TextAdapterComponents.Count)
			{
				Foreground = new Graphic[TextAdapterComponents.Count];

				for (int i = 0; i < TextAdapterComponents.Count; i++)
				{
					Foreground[i] = TextAdapterComponents[i].Graphic;
				}
			}
			#pragma warning restore

			base.GraphicsForegroundInit();
		}

		/// <summary>
		/// Init graphics background.
		/// </summary>
		protected override void GraphicsBackgroundInit()
		{
			if (GraphicsBackgroundVersion == 0)
			{
				#pragma warning disable 0618
				graphicsBackground = Compatibility.EmptyArray<Graphic>();
				#pragma warning restore
				GraphicsBackgroundVersion = 1;
			}

			base.GraphicsBackgroundInit();
		}

		/// <summary>
		/// The item.
		/// </summary>
		[SerializeField]
		protected List<int> Item;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(List<int> item)
		{
			Item = item;
			UpdateView();
		}

		/// <summary>
		/// Update text components text.
		/// </summary>
		public void UpdateView()
		{
			for (int index = 0; index < TextAdapterComponents.Count; index++)
			{
				TextAdapterComponents[index].text = index < Item.Count ? Item[index].ToString() : "none";
			}
		}

		/// <summary>
		/// Add cell.
		/// </summary>
		/// <param name="template">Template.</param>
		public void AddCell(TableListCell template)
		{
			var cell = Compatibility.Instantiate(template);
			cell.transform.SetParent(RectTransform, false);
			cell.gameObject.SetActive(true);

			TextAdapterComponents.Add(cell.TextAdapter);
			foregrounds.Add(cell.TextAdapter.Graphic);

			UpdateView();
		}

		/// <summary>
		/// Remove cell.
		/// </summary>
		/// <param name="cellIndex">Cell index.</param>
		public void RemoveCell(int cellIndex)
		{
			var go = RectTransform.GetChild(cellIndex).gameObject;
			Destroy(go);

			TextAdapterComponents.RemoveAt(cellIndex);
			foregrounds.RemoveAt(cellIndex);

			UpdateView();
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			if (TextAdapterComponents.Count == 0)
			{
				foreach (var t in TextComponents)
				{
					if (t != null)
					{
						TextAdapterComponents.Add(Utilities.RequireComponent<TextAdapter>(t));
					}
				}
			}
#pragma warning restore 0612, 0618
		}
	}
}