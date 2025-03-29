namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewCustom sample component.
	/// </summary>
	public class ListViewCustomSampleComponent : ListViewItem, IViewData<ListViewCustomSampleItemDescription>
	{
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
		/// Progressbar.
		/// </summary>
		[SerializeField]
		#pragma warning disable 0618
		public Progressbar Progressbar;
		#pragma warning restore

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				#pragma warning disable 0618
				Foreground = new Graphic[] { UtilitiesUI.GetGraphic(TextAdapter), };
				#pragma warning restore
				GraphicsForegroundVersion = 1;
			}

			base.GraphicsForegroundInit();
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ListViewCustomSampleItemDescription item)
		{
			if (item == null)
			{
				Icon.sprite = null;
				TextAdapter.text = string.Empty;
				Progressbar.Value = 0;
			}
			else
			{
				Icon.sprite = item.Icon;
				TextAdapter.text = item.Name;
				Progressbar.Value = item.Progress;
			}

			Icon.SetNativeSize();

			Icon.enabled = Icon.sprite != null;
		}

		/// <inheritdoc/>
		public override void SetThemeImagesPropertiesOwner(Component owner)
		{
			base.SetThemeImagesPropertiesOwner(owner);

			UIThemes.Utilities.SetTargetOwner(typeof(Sprite), Icon, nameof(Icon.sprite), owner);
			UIThemes.Utilities.SetTargetOwner(typeof(Color), Icon, nameof(Icon.color), owner);
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.RequireComponent(Text, ref TextAdapter);
#pragma warning restore 0612, 0618
		}
	}
}