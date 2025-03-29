namespace UIWidgets.Timeline
{
	using System;
	using UIWidgets;

	/// <summary>
	/// Layout with compact order and items at any line grouped by name.
	/// </summary>
	public class LayoutByName : TrackLayoutGroup<TimelineData, TimeSpan>
	{
		/// <inheritdoc/>
		protected override bool SameGroup(TimelineData x, TimelineData y)
		{
			return x.Name == y.Name;
		}
	}
}