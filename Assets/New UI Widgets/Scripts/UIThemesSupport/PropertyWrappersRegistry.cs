namespace UIWidgets.UIThemesSupport
{
	using UIThemes;
	using UnityEngine;
	using UnityEngine.Scripting;

	/// <summary>
	/// Theme wrappers registry.
	/// </summary>
	public static class PropertyWrappersRegistry
	{
		/// <summary>
		/// Add wrappers.
		/// </summary>
		[PropertiesRegistry]
		[Preserve]
		public static void AddWrappers()
		{
			// ListView
			PropertyWrappers<Color>.Add(new ListViewDefaultBackgroundColor());
			PropertyWrappers<Color>.Add(new ListViewDefaultEvenBackgroundColor());
			PropertyWrappers<Color>.Add(new ListViewDefaultOddBackgroundColor());
			PropertyWrappers<Color>.Add(new ListViewDefaultColor());
			PropertyWrappers<Color>.Add(new ListViewHighlightedBackgroundColor());
			PropertyWrappers<Color>.Add(new ListViewHighlightedColor());
			PropertyWrappers<Color>.Add(new ListViewSelectedBackgroundColor());
			PropertyWrappers<Color>.Add(new ListViewSelectedColor());
			PropertyWrappers<Color>.Add(new ListViewDisabledColor());
			PropertyWrappers<Color>.AddIgnore(typeof(ListViewCustom<Color>), nameof(ListViewCustom<Color>.SelectedItem));

			// TreeView
			PropertyWrappers<Sprite>.Add(new TreeViewComponentBaseNodeOpened());
			PropertyWrappers<Sprite>.Add(new TreeViewComponentBaseNodeClosed());

			// Text
			PropertyWrappers<Color>.AddIgnore(typeof(ITextProxy), nameof(ITextProxy.color));
			PropertyWrappers<Color>.AddIgnore(typeof(TextAdapter), nameof(TextAdapter.color));

			// SelectableHelper
			PropertyWrappers<Color>.Add(new SelectableHelperNormalColor());
			PropertyWrappers<Color>.Add(new SelectableHelperHighlightedColor());
			PropertyWrappers<Color>.Add(new SelectableHelperPressedColor());
			PropertyWrappers<Color>.Add(new SelectableHelperSelectedColor());
			PropertyWrappers<Color>.Add(new SelectableHelperDisabledColor());

			PropertyWrappers<Sprite>.Add(new SelectableHelperHighlightedSprite());
			PropertyWrappers<Sprite>.Add(new SelectableHelperPressedSprite());
			PropertyWrappers<Sprite>.Add(new SelectableHelperSelectedSprite());
			PropertyWrappers<Sprite>.Add(new SelectableHelperDisabledSprite());

			// SelectableHelperList
			PropertyWrappers<Color>.Add(new SelectableHelperListNormalColor());
			PropertyWrappers<Color>.Add(new SelectableHelperListHighlightedColor());
			PropertyWrappers<Color>.Add(new SelectableHelperListPressedColor());
			PropertyWrappers<Color>.Add(new SelectableHelperListSelectedColor());
			PropertyWrappers<Color>.Add(new SelectableHelperListDisabledColor());

			PropertyWrappers<Sprite>.Add(new SelectableHelperListHighlightedSprite());
			PropertyWrappers<Sprite>.Add(new SelectableHelperListPressedSprite());
			PropertyWrappers<Sprite>.Add(new SelectableHelperListSelectedSprite());
			PropertyWrappers<Sprite>.Add(new SelectableHelperListDisabledSprite());

			// Accordion
			PropertyWrappers<Color>.Add(new AccordionHighlightDefaultTextColor());
			PropertyWrappers<Color>.Add(new AccordionHighlightDefaultBackgroundColor());
			PropertyWrappers<Sprite>.Add(new AccordionHighlightDefaultBackgroundSprite());

			PropertyWrappers<Color>.Add(new AccordionHighlightActiveTextColor());
			PropertyWrappers<Color>.Add(new AccordionHighlightActiveBackgroundColor());
			PropertyWrappers<Sprite>.Add(new AccordionHighlightActiveBackgroundSprite());

			PropertyWrappers<Color>.Add(new AccordionHighlightThemesDefaultLabelColor());
			PropertyWrappers<Color>.Add(new AccordionHighlightThemesDefaultBackgroundColor());
			PropertyWrappers<Sprite>.Add(new AccordionHighlightThemesDefaultBackgroundSprite());

			PropertyWrappers<Color>.Add(new AccordionHighlightThemesActiveLabelColor());
			PropertyWrappers<Color>.Add(new AccordionHighlightThemesActiveBackgroundColor());
			PropertyWrappers<Sprite>.Add(new AccordionHighlightThemesActiveBackgroundSprite());

			// Calendar
			PropertyWrappers<Color>.Add(new CalendarDateBaseCurrentMonth());
			PropertyWrappers<Color>.Add(new CalendarDateBaseOtherMonth());
			PropertyWrappers<Color>.Add(new CalendarDateBaseOtherMonthWeekend());
			PropertyWrappers<Color>.Add(new CalendarDateBaseOutOfRangeDate());
			PropertyWrappers<Color>.Add(new CalendarDateBaseSelectedDay());
			PropertyWrappers<Color>.Add(new CalendarDateBaseWeekend());

			PropertyWrappers<Sprite>.Add(new CalendarDateBaseDefaultDayBackground());
			PropertyWrappers<Sprite>.Add(new CalendarDateBaseSelectedDayBackground());

			// Switch
			PropertyWrappers<Color>.Add(new SwitchBackgroundOffColor());
			PropertyWrappers<Color>.Add(new SwitchBackgroundOnColor());
			PropertyWrappers<Color>.Add(new SwitchMarkOffColor());
			PropertyWrappers<Color>.Add(new SwitchMarkOnColor());

			// Rating
			PropertyWrappers<Color>.Add(new RatingColorMin());
			PropertyWrappers<Color>.Add(new RatingColorMax());

			// SplitButton
			PropertyWrappers<Color>.Add(new SplitButtonModalColor());
			PropertyWrappers<Sprite>.Add(new SplitButtonModalSprite());

			// Effects
			PropertyWrappers<Color>.Add(new RingEffectRingColor());
			PropertyWrappers<Color>.Add(new LinesDrawerBaseLineColor());
			PropertyWrappers<Color>.Add(new RippleEffectStartColor());
			PropertyWrappers<Color>.Add(new RippleEffectEndColor());
			PropertyWrappers<Color>.Add(new BorderEffectColor());
			PropertyWrappers<Color>.Add(new RoundedCornersColor());
			PropertyWrappers<Color>.Add(new RoundedCornersX4Color());

			// Sidebar
			PropertyWrappers<Color>.Add(new SidebarModalColor());

			// Connector
			PropertyWrappers<Sprite>.Add(new ConnectorBaseSprite());

			// ColorPickers
			PropertyWrappers<Color>.AddIgnore(typeof(ColorPicker), nameof(ColorPicker.Color));
			PropertyWrappers<Color>.AddIgnore(typeof(ColorPickerRange), nameof(ColorPickerRange.Color));
			PropertyWrappers<Color>.AddIgnore(typeof(ColorPickerRange), nameof(ColorPickerRange.ColorLeft));
			PropertyWrappers<Color>.AddIgnore(typeof(ColorPickerRange), nameof(ColorPickerRange.ColorRight));
			PropertyWrappers<Color>.AddIgnore(typeof(ColorPickerRangeHSV), nameof(ColorPickerRangeHSV.ColorRGB));
			PropertyWrappers<Color>.AddIgnore(typeof(ColorPickerRangeHSV), nameof(ColorPickerRangeHSV.ColorLeftRGB));
			PropertyWrappers<Color>.AddIgnore(typeof(ColorPickerRangeHSV), nameof(ColorPickerRangeHSV.ColorRightRGB));
		}
	}
}