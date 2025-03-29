namespace UIWidgets.UIThemesSupport
{
	using UIThemes.Wrappers;
	using UnityEngine;

	/// <summary>
	/// Theme property for the OutOfRangeDate of CalendarDateBase.
	/// </summary>
	public class CalendarDateBaseOutOfRangeDate : Wrapper<Color, CalendarDateBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CalendarDateBaseOutOfRangeDate"/> class.
		/// </summary>
		public CalendarDateBaseOutOfRangeDate()
		{
			Name = nameof(CalendarDateBase.OutOfRangeDate);
		}

		/// <inheritdoc/>
		protected override Color Get(CalendarDateBase widget) => widget.OutOfRangeDate;

		/// <inheritdoc/>
		protected override void Set(CalendarDateBase widget, Color value) => widget.OutOfRangeDate = value;
	}
}