namespace UIWidgets.Timeline
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Schedule view.
	/// </summary>
	public class Timeline : TimelineCustom<TimelineData, DataView, TrackView, TrackBackground, DataDialog, DataForm, TrackDialog, TrackForm>
	{
		[SerializeField]
		[FormerlySerializedAs("GroupByName")]
		bool groupByName;

		/// <summary>
		/// Group tasks with same name on one line.
		/// </summary>
		[DataBindField]
		public bool GroupByName
		{
			get => groupByName;

			set
			{
				if (groupByName != value)
				{
					groupByName = value;

					if (IsInited)
					{
						Layout = null;
						UpdateView();
					}
				}
			}
		}

		/// <summary>
		/// Copy data from source to target.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="target">Target.</param>
		public override void DataCopy(TimelineData source, TimelineData target)
		{
			source.CopyTo(target);
		}

		/// <summary>
		/// Set track settings.
		/// </summary>
		/// <param name="track">Track.</param>
		protected override void SetTrackSettings(Track<TimelineData, TimeSpan> track)
		{
			track.SeparateGroups = !GroupByName;
			base.SetTrackSettings(track);
		}

		/// <summary>
		/// Get layout for the tracks.
		/// </summary>
		/// <returns>Layout function.</returns>
		protected override TrackLayout<TimelineData, TimeSpan> GetLayout()
		{
			return GroupByName ? new LayoutByName() : base.GetLayout();
		}

		/// <summary>
		/// Check if target item has intersection with items in the track within range and order.
		/// </summary>
		/// <param name="track">Track.</param>
		/// <param name="start">Start point.</param>
		/// <param name="end">End item.</param>
		/// <param name="order">New order of the target item.</param>
		/// <param name="target">Target item. Will be ignored if presents in the items list.</param>
		/// <returns>true if any items has intersection; otherwise false.</returns>
		public override bool TrackIntersection(Track<TimelineData, TimeSpan> track, TimeSpan start, TimeSpan end, int order, TimelineData target)
		{
			var is_new = !track.Data.Contains(target);
			if ((!is_new) && (target.Order != order) && (!GroupByName))
			{
				return false;
			}

			using var _ = ListPool<TimelineData>.Get(out var temp);

			GetPossibleIntersections(track.Data, order, target, temp);
			var result = ListIntersection(temp, start, end, order, target);

			return result;
		}

		/// <summary>
		/// Get possible intersections with the target.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="order">Order.</param>
		/// <param name="target">Target.</param>
		/// <param name="output">List of the possible intersections,</param>
		protected override void GetPossibleIntersections(ObservableList<TimelineData> items, int order, TimelineData target, List<TimelineData> output)
		{
			if (GroupByName)
			{
				foreach (var item in items)
				{
					if ((item.Name == target.Name) && !ReferenceEquals(item, target))
					{
						output.Add(item);
					}
				}
			}
			else
			{
				base.GetPossibleIntersections(items, order, target, output);
			}
		}
	}
}