namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.l10n;
	using UnityEngine;

	/// <summary>
	/// ListViewIcons.
	/// </summary>
	public class ListViewIcons : ListViewCustom<ListViewIconsItemComponent, ListViewIconsItemDescription>, ILocalizationSupport
	{
		[SerializeField]
		[Tooltip("If enabled translates items names using Localization.GetTranslation().")]
		bool localizationSupport = true;

		/// <summary>
		/// Localization support.
		/// </summary>
		public bool LocalizationSupport
		{
			get => localizationSupport;

			set => localizationSupport = value;
		}

		/// <summary>
		/// Sort items.
		/// Deprecated. Replaced with DataSource.Comparison.
		/// </summary>
		[Obsolete("Replaced with DataSource.Comparison.")]
		public override bool Sort
		{
			get => DataSource.Comparison == ItemsComparison;

			set => DataSource.Comparison = value ? (LocalizationSupport ? LocalizedItemsComparison : ItemsComparison) : null;
		}

		static string GetLocalizedItemName(ListViewIconsItemDescription item)
		{
			if (item == null)
			{
				return string.Empty;
			}

			return item.LocalizedName ?? Localization.GetTranslation(item.Name);
		}

		static string GetItemName(ListViewIconsItemDescription item)
		{
			if (item == null)
			{
				return string.Empty;
			}

			return item.LocalizedName ?? item.Name;
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected override void InitOnce()
		{
			base.InitOnce();

#pragma warning disable 0618
			if (base.Sort)
			{
				DataSource.Comparison = LocalizationSupport ? LocalizedItemsComparison : ItemsComparison;
			}
#pragma warning restore 0618
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public override void LocaleChanged()
		{
			base.LocaleChanged();

			if (DataSource.Comparison != null)
			{
				DataSource.CollectionChanged();
			}
		}

		/// <summary>
		/// Items comparison by localized names.
		/// </summary>
		[DomainReloadExclude]
		public static readonly Comparison<ListViewIconsItemDescription> LocalizedItemsComparison = (x, y) => UtilitiesCompare.Compare(GetLocalizedItemName(x), GetLocalizedItemName(y));

		/// <summary>
		/// Items comparison.
		/// </summary>
		[DomainReloadExclude]
		public static readonly Comparison<ListViewIconsItemDescription> ItemsComparison = (x, y) => UtilitiesCompare.Compare(GetItemName(x), GetItemName(y));
	}
}