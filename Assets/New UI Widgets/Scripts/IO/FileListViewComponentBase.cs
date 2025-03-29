namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// FileListViewComponentBase.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/collections/filelistview.html")]
	public class FileListViewComponentBase : ListViewItem, IViewData<FileSystemEntry>
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		protected TextAdapter NameAdapter;

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				#pragma warning disable 0618
				Foreground = new Graphic[] { UtilitiesUI.GetGraphic(NameAdapter), };
				#pragma warning restore
				GraphicsForegroundVersion = 1;
			}

			base.GraphicsForegroundInit();
		}

		/// <summary>
		/// Icon.
		/// </summary>
		[SerializeField]
		protected Image Icon;

		/// <summary>
		/// Directory icon.
		/// </summary>
		[SerializeField]
		protected Sprite DirectoryIcon;

		/// <summary>
		/// Current item.
		/// </summary>
		protected FileSystemEntry Item;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(FileSystemEntry item)
		{
			Item = item;

			Icon.sprite = GetIcon(item);
			Icon.enabled = Icon.sprite != null;

			NameAdapter.text = item.DisplayName;
		}

		/// <inheritdoc/>
		public override void SetThemeImagesPropertiesOwner(Component owner)
		{
			base.SetThemeImagesPropertiesOwner(owner);

			UIThemes.Utilities.SetTargetOwner(typeof(Sprite), Icon, nameof(Icon.sprite), owner);
			UIThemes.Utilities.SetTargetOwner(typeof(Color), Icon, nameof(Icon.color), owner);
		}

		/// <summary>
		/// Get icon for specified FileSystemEntry.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Icon for specified FileSystemEntry.</returns>
		public virtual Sprite GetIcon(FileSystemEntry item)
		{
			if (item.IsDirectory)
			{
				return DirectoryIcon;
			}

			return null;
		}
	}
}