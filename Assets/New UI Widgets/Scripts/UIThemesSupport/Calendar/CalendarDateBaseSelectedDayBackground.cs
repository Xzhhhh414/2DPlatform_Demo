namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the SelectedDayBackground of CalendarDateBase.
	/// </summary>
	public class CalendarDateBaseSelectedDayBackground : Wrapper<Sprite, CalendarDateBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CalendarDateBaseSelectedDayBackground"/> class.
		/// </summary>
		public CalendarDateBaseSelectedDayBackground()
		{
			Name = nameof(CalendarDateBase.SelectedDayBackground);
		}

		/// <inheritdoc/>
		protected override Sprite Get(CalendarDateBase widget)
		{
			return widget.SelectedDayBackground;
		}

		/// <inheritdoc/>
		protected override void Set(CalendarDateBase widget, Sprite value)
		{
			widget.SelectedDayBackground = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(CalendarDateBase widget)
		{
			return widget.SelectedDayBackground != null;
		}
	}
}