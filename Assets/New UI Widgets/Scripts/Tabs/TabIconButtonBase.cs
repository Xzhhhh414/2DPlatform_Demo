namespace UIWidgets
{
	using UIWidgets.l10n;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TabIconButton.
	/// </summary>
	public class TabIconButtonBase : TabButton<TabIcons>
	{
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public TextAdapter NameAdapter;

		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// The size of the set native.
		/// </summary>
		[SerializeField]
		public bool SetNativeSize;

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public override void SetData(TabIcons tab)
		{
			NameAdapter.text = LocalizationSupport ? Localization.GetTranslation(tab.Name) : tab.Name;

			SetIcon(tab.IconDefault);
		}

		/// <summary>
		/// Set icon.
		/// </summary>
		/// <param name="icon">Icon.</param>
		protected void SetIcon(Sprite icon)
		{
			if (Icon == null)
			{
				return;
			}

			Icon.sprite = icon;

			if (SetNativeSize)
			{
				Icon.SetNativeSize();
			}

			Icon.enabled = Icon.sprite != null;
		}
	}
}