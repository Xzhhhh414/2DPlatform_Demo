namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// CalendarDate.
	/// Display date.
	/// </summary>
	public class CalendarDate : CalendarDateBase
	{
		/// <summary>
		/// Text component to display day.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with DayAdapter")]
		protected Text Day;

		/// <inheritdoc/>
		public override void Upgrade()
		{
			base.Upgrade();
#pragma warning disable 0618
			Utilities.RequireComponent(Day, ref dayAdapter);
#pragma warning restore 0618
		}
	}
}