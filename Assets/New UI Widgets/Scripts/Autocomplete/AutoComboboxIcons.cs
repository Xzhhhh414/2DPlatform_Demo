namespace UIWidgets
{
	using UIWidgets.l10n;
	using UnityEngine;

	/// <summary>
	/// AutoComboboxIcons.
	/// </summary>
	public partial class AutoComboboxIcons : AutoCombobox<ListViewIconsItemDescription, ListViewIcons, ListViewIconsItemComponent, AutocompleteIcons, ComboboxIcons>, ILocalizationSupport
	{
		[SerializeField]
		[Tooltip("If enabled translates item name using Localization.GetTranslation().")]
		bool localizationSupport = true;

		/// <summary>
		/// Localization support.
		/// </summary>
		public bool LocalizationSupport
		{
			get => localizationSupport;

			set => localizationSupport = value;
		}

		/// <inheritdoc/>
		protected override string GetStringValue(ListViewIconsItemDescription item)
		{
			return item.LocalizedName ?? (LocalizationSupport ? Localization.GetTranslation(item.Name) : item.Name);
		}

		/// <inheritdoc/>
		protected override ListViewIconsItemDescription Input2Item(string input)
		{
			return new ListViewIconsItemDescription()
			{
				Name = input,
			};
		}
	}
}