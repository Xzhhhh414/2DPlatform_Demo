namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the SelectedDay of CalendarDateBase.
	/// </summary>
	public class CalendarDateBaseSelectedDay : Wrapper<Color, CalendarDateBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CalendarDateBaseSelectedDay"/> class.
		/// </summary>
		public CalendarDateBaseSelectedDay()
		{
			Name = nameof(CalendarDateBase.SelectedDay);
		}

		/// <inheritdoc/>
		protected override Color Get(CalendarDateBase widget)
		{
			return widget.SelectedDay;
		}

		/// <inheritdoc/>
		protected override void Set(CalendarDateBase widget, Color value)
		{
			widget.SelectedDay = value;
		}
	}
}