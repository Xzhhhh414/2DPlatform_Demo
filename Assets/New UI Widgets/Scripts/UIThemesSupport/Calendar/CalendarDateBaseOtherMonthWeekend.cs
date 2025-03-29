namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the OtherMonthWeekend of CalendarDateBase.
	/// </summary>
	public class CalendarDateBaseOtherMonthWeekend : Wrapper<Color, CalendarDateBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CalendarDateBaseOtherMonthWeekend"/> class.
		/// </summary>
		public CalendarDateBaseOtherMonthWeekend()
		{
			Name = nameof(CalendarDateBase.OtherMonthWeekend);
		}

		/// <inheritdoc/>
		protected override Color Get(CalendarDateBase widget)
		{
			return widget.OtherMonthWeekend;
		}

		/// <inheritdoc/>
		protected override void Set(CalendarDateBase widget, Color value)
		{
			widget.OtherMonthWeekend = value;
		}
	}
}