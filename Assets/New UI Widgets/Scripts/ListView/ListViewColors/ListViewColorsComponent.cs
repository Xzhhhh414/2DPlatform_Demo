namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewColor component.
	/// </summary>
	public class ListViewColorsComponent : ListViewItem, IViewData<Color>
	{
		/// <summary>
		/// The number.
		/// </summary>
		[SerializeField]
		public Image Color;

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

		/// <inheritdoc/>
		public override void SetThemeImagesPropertiesOwner(Component owner)
		{
			base.SetThemeImagesPropertiesOwner(owner);

			UIThemes.Utilities.SetTargetOwner(typeof(Sprite), Color, nameof(Color.sprite), owner);
			UIThemes.Utilities.SetTargetOwner(typeof(Color), Color, nameof(Color.color), owner);
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(Color item)
		{
			Color.color = item;
		}
	}
}