namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the OtherMonth of CalendarDateBase.
	/// </summary>
	public class CalendarDateBaseOtherMonth : Wrapper<Color, CalendarDateBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CalendarDateBaseOtherMonth"/> class.
		/// </summary>
		public CalendarDateBaseOtherMonth()
		{
			Name = nameof(CalendarDateBase.OtherMonth);
		}

		/// <inheritdoc/>
		protected override Color Get(CalendarDateBase widget)
		{
			return widget.OtherMonth;
		}

		/// <inheritdoc/>
		protected override void Set(CalendarDateBase widget, Color value)
		{
			widget.OtherMonth = value;
		}
	}
}