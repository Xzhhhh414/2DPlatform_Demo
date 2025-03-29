namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the DefaultDayBackground of CalendarDateBase.
	/// </summary>
	public class CalendarDateBaseDefaultDayBackground : Wrapper<Sprite, CalendarDateBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CalendarDateBaseDefaultDayBackground"/> class.
		/// </summary>
		public CalendarDateBaseDefaultDayBackground()
		{
			Name = nameof(CalendarDateBase.DefaultDayBackground);
		}

		/// <inheritdoc/>
		protected override Sprite Get(CalendarDateBase widget)
		{
			return widget.DefaultDayBackground;
		}

		/// <inheritdoc/>
		protected override void Set(CalendarDateBase widget, Sprite value)
		{
			widget.DefaultDayBackground = value;
		}

		/// <inheritdoc/>
		protected override bool ShouldAttachValue(CalendarDateBase widget)
		{
			return widget.DefaultDayBackground != null;
		}
	}
}