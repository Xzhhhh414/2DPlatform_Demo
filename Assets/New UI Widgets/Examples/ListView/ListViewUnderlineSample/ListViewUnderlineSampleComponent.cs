namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewUnderline sample component.
	/// </summary>
	public class ListViewUnderlineSampleComponent : ListViewItem, IViewData<ListViewUnderlineSampleItemDescription>
	{
		/// <inheritdoc/>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				#pragma warning disable 0618
				Foreground = new Graphic[] { UtilitiesUI.GetGraphic(TextAdapter), Underline, };
				#pragma warning restore
				GraphicsForegroundVersion = 1;
			}

			base.GraphicsForegroundInit();
		}

		/// <inheritdoc/>
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
		/// Icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with TextAdapter.")]
		public Text Text;

		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		public TextAdapter TextAdapter;

		/// <summary>
		/// Underline.
		/// </summary>
		[SerializeField]
		public Image Underline;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ListViewUnderlineSampleItemDescription item)
		{
			Icon.sprite = item.Icon;
			TextAdapter.text = item.Name;

			Icon.SetNativeSize();

			Icon.enabled = Icon.sprite != null;
		}

		/// <inheritdoc/>
		public override void SetThemeImagesPropertiesOwner(Component owner)
		{
			base.SetThemeImagesPropertiesOwner(owner);

			UIThemes.Utilities.SetTargetOwner(typeof(Color), Icon, nameof(Icon.color), owner);
			UIThemes.Utilities.SetTargetOwner(typeof(Sprite), Icon, nameof(Icon.sprite), owner);
		}

		/// <inheritdoc/>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.RequireComponent(Text, ref TextAdapter);
#pragma warning restore 0612, 0618
		}
	}
}