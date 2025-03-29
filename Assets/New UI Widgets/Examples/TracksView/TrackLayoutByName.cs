namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;

	/// <summary>
	/// Layout with compact order and items at any line grouped by name.
	/// </summary>
	public class TrackLayoutByName : TrackLayoutGroup<TrackData, DateTime>
	{
		/// <inheritdoc/>
		protected override bool SameGroup(TrackData x, TrackData y)
		{
			return x.Name == y.Name;
		}
	}
}