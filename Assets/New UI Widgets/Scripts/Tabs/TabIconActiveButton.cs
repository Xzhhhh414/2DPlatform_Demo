namespace UIWidgets
{
	using UIWidgets.l10n;
	using UnityEngine;

	/// <summary>
	/// TabIconActiveButton.
	/// </summary>
	public class TabIconActiveButton : TabIconButton
	{
		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public override void SetData(TabIcons tab)
		{
			NameAdapter.text = LocalizationSupport ? Localization.GetTranslation(tab.Name) : tab.Name;

			SetIcon(tab.IconActive);
		}
	}
}