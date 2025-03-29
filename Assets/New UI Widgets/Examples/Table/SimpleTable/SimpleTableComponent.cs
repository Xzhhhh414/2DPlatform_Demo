namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// SimpleTable component.
	/// </summary>
	public class SimpleTableComponent : ListViewItem, IViewData<SimpleTableItem>, IUpgradeable
	{
		/// <summary>
		/// Field1.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Field1Adapter.")]
		public Text Field1;

		/// <summary>
		/// Field2.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Field2Adapter.")]
		public Text Field2;

		/// <summary>
		/// Field3.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Field3Adapter.")]
		public Text Field3;

		/// <summary>
		/// Field1.
		/// </summary>
		[SerializeField]
		public TextAdapter Field1Adapter;

		/// <summary>
		/// Field2.
		/// </summary>
		[SerializeField]
		public TextAdapter Field2Adapter;

		/// <summary>
		/// Field3.
		/// </summary>
		[SerializeField]
		public TextAdapter Field3Adapter;

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
					UtilitiesUI.GetGraphic(Field1Adapter),
					UtilitiesUI.GetGraphic(Field2Adapter),
					UtilitiesUI.GetGraphic(Field3Adapter),
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
		public virtual void SetData(SimpleTableItem item)
		{
			Field1Adapter.text = item.Field1;
			Field2Adapter.text = item.Field2;
			Field3Adapter.text = item.Field3;
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.RequireComponent(Field1, ref Field1Adapter);
			Utilities.RequireComponent(Field2, ref Field2Adapter);
			Utilities.RequireComponent(Field3, ref Field3Adapter);
#pragma warning restore 0612, 0618
		}
	}
}