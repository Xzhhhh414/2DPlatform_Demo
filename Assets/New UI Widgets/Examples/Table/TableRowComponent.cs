namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TableRow component.
	/// </summary>
	public class TableRowComponent : ListViewItem, IViewData<TableRow>
	{
		/// <summary>
		/// Cell01Text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with Cell01TextAdapter.")]
		public Text Cell01Text;

		/// <summary>
		/// Cell02Text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with Cell02TextAdapter.")]
		public Text Cell02Text;

		/// <summary>
		/// Cell03Image.
		/// </summary>
		[SerializeField]
		public Image Cell03Image;

		/// <summary>
		/// Cell04Text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with Cell04TextAdapter.")]
		public Text Cell04Text;

		/// <summary>
		/// Cell01Text.
		/// </summary>
		[SerializeField]
		public TextAdapter Cell01TextAdapter;

		/// <summary>
		/// Cell02Text.
		/// </summary>
		[SerializeField]
		public TextAdapter Cell02TextAdapter;

		/// <summary>
		/// Cell04Text.
		/// </summary>
		[SerializeField]
		public TextAdapter Cell04TextAdapter;

		TableRow Item;

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				#pragma warning disable 0618
				Foreground = new Graphic[]
				{
					UtilitiesUI.GetGraphic(Cell01TextAdapter),
					UtilitiesUI.GetGraphic(Cell02TextAdapter),
					UtilitiesUI.GetGraphic(Cell04TextAdapter),
				};
				#pragma warning restore
				GraphicsForegroundVersion = 1;
			}

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
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(TableRow item)
		{
			Item = item;

			Cell01TextAdapter.text = Item.Cell01;
			Cell02TextAdapter.text = Item.Cell02.ToString();
			Cell03Image.sprite = Item.Cell03;
			Cell04TextAdapter.text = Item.Cell04.ToString();

			Cell03Image.enabled = Cell03Image.sprite != null;
		}

		/// <summary>
		/// Handle cell clicked event.
		/// </summary>
		/// <param name="cellName">Cell name.</param>
		public void CellClicked(string cellName)
		{
			Debug.Log(string.Format("clicked row {0}, cell {1}", Index.ToString(), cellName));
			switch (cellName)
			{
				case "Cell01":
					Debug.Log(string.Format("cell value: {0}", Item.Cell01));
					break;
				case "Cell02":
					Debug.Log(string.Format("cell value: {0}", Item.Cell02.ToString()));
					break;
				case "Cell03":
					Debug.Log(string.Format("cell value: {0}", Item.Cell03));
					break;
				case "Cell04":
					Debug.Log(string.Format("cell value: {0}", Item.Cell04.ToString()));
					break;
				default:
					Debug.Log("cell value: <unknown cell>");
					break;
			}
		}

		/// <inheritdoc/>
		public override void SetThemeImagesPropertiesOwner(Component owner)
		{
			base.SetThemeImagesPropertiesOwner(owner);

			UIThemes.Utilities.SetTargetOwner(typeof(Sprite), Cell03Image, nameof(Cell03Image.sprite), owner);
			UIThemes.Utilities.SetTargetOwner(typeof(Color), Cell03Image, nameof(Cell03Image.color), owner);
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.RequireComponent(Cell01Text, ref Cell01TextAdapter);
			Utilities.RequireComponent(Cell02Text, ref Cell02TextAdapter);
			Utilities.RequireComponent(Cell04Text, ref Cell04TextAdapter);
#pragma warning restore 0612, 0618
		}
	}
}