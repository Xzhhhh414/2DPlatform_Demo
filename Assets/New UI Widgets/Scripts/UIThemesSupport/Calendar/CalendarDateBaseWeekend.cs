namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the Weekend of CalendarDateBase.
	/// </summary>
	public class CalendarDateBaseWeekend : Wrapper<Color, CalendarDateBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CalendarDateBaseWeekend"/> class.
		/// </summary>
		public CalendarDateBaseWeekend()
		{
			Name = nameof(CalendarDateBase.Weekend);
		}

		/// <inheritdoc/>
		protected override Color Get(CalendarDateBase widget)
		{
			return widget.Weekend;
		}

		/// <inheritdoc/>
		protected override void Set(CalendarDateBase widget, Color value)
		{
			widget.Weekend = value;
		}
	}
}