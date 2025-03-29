namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewIconsItem extended component.
	/// </summary>
	public class ListViewComponentCustomColoring : ListViewIconsItemComponent
	{
		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				#pragma warning disable 0618
				Foreground = Compatibility.EmptyArray<Graphic>();
				#pragma warning restore
				GraphicsForegroundVersion = 1;
			}

			base.GraphicsForegroundInit();
		}

		[SerializeField]
		[FormerlySerializedAs("ColorEven")]
		Color colorEven = Color.black;

		/// <summary>
		/// Color for the Text if Index is even.
		/// </summary>
		public Color ColorEven
		{
			get
			{
				return colorEven;
			}

			set
			{
				colorEven = value;
			}
		}

		[SerializeField]
		[FormerlySerializedAs("ColorOdd")]
		Color colorOdd = Color.white;

		/// <summary>
		/// Color for the Text if Index is odd.
		/// </summary>
		public Color ColorOdd
		{
			get
			{
				return colorOdd;
			}

			set
			{
				colorOdd = value;
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public override void SetData(ListViewIconsItemDescription item)
		{
			base.SetData(item);

			TextAdapter.color = (Index % 2) == 0 ? ColorEven : ColorOdd;
		}
	}
}