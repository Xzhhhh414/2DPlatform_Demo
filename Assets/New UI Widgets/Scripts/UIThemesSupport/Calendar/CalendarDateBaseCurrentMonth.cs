namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the CurrentMonth of CalendarDateBase.
	/// </summary>
	public class CalendarDateBaseCurrentMonth : Wrapper<Color, CalendarDateBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CalendarDateBaseCurrentMonth"/> class.
		/// </summary>
		public CalendarDateBaseCurrentMonth()
		{
			Name = nameof(CalendarDateBase.CurrentMonth);
		}

		/// <inheritdoc/>
		protected override Color Get(CalendarDateBase widget)
		{
			return widget.CurrentMonth;
		}

		/// <inheritdoc/>
		protected override void Set(CalendarDateBase widget, Color value)
		{
			widget.CurrentMonth = value;
		}
	}
}