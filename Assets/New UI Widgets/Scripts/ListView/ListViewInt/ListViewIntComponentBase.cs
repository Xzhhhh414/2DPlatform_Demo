namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewInt base component.
	/// </summary>
	public class ListViewIntComponentBase : ListViewItem, IViewData<int>
	{
		/// <summary>
		/// The number.
		/// </summary>
		[SerializeField]
		public TextAdapter NumberAdapter;

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				#pragma warning disable 0618
				Foreground = new Graphic[] { UtilitiesUI.GetGraphic(NumberAdapter), };
				#pragma warning restore
				GraphicsForegroundVersion = 1;
			}

			base.GraphicsForegroundInit();
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(int item)
		{
			NumberAdapter.text = item.ToString();
		}
	}
}