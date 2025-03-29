namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Schedule view.
	/// </summary>
	public class ScheduleView : TracksViewCustom<TrackData, DateTime, TrackDataView, TrackView, TrackBackground, TrackDataDialog, TrackDataForm, TrackDialog, TrackForm>
	{
		[Multiline]
		[SerializeField]
		string dateFormat = "dd MMM yyyy";

		/// <summary>
		/// Date format.
		/// </summary>
		public string DateFormat
		{
			get => dateFormat;

			set
			{
				dateFormat = value;
				PointsNamesView.UpdateView();
			}
		}

		[SerializeField]
		RectTransform todayHighlight;

		/// <summary>
		/// Today highlight.
		/// </summary>
		public RectTransform TodayHighlight
		{
			get => todayHighlight;

			set
			{
				todayHighlight = value;
				todayHighlight.pivot = new Vector2(0f, 1f);
				UpdateDateHighlights();
			}
		}

		[SerializeField]
		RectTransform weekendHighlightsDefaultItem;

		/// <summary>
		/// Default item to display weekend highlights.
		/// </summary>
		public RectTransform WeekendHighlightsDefaultItem
		{
			get => weekendHighlightsDefaultItem;

			set
			{
				weekendHighlightsDefaultItem = value;
				weekendHighlightsDefaultItem.pivot = new Vector2(0f, 1f);
				weekendHighlights.Template = value;
				UpdateDateHighlights();
			}
		}

		[SerializeField]
		[HideInInspector]
		List<RectTransform> weekendHighlightsActive = new List<RectTransform>();

		[SerializeField]
		[HideInInspector]
		List<RectTransform> weekendHighlightsCache = new List<RectTransform>();

		ListComponentPool<RectTransform> weekendHighlights;

		/// <summary>
		/// Weekend highlights.
		/// </summary>
		protected ListComponentPool<RectTransform> WeekendHighlights
		{
			get
			{
				if ((weekendHighlights == null) || (weekendHighlights.Template == null))
				{
					weekendHighlights = new ListComponentPool<RectTransform>(weekendHighlightsDefaultItem, weekendHighlightsActive, weekendHighlightsCache, TracksDataView.content);
				}

				return weekendHighlights;
			}
		}

		[SerializeField]
		Timeline.SerializedTimeSpan step = new Timeline.SerializedTimeSpan(1, 0, 0);

		TimeSpan convertedStep;

		/// <summary>
		/// Step.
		/// </summary>
		public TimeSpan Step
		{
			get
			{
				if (convertedStep.Ticks == 0)
				{
					convertedStep = step;
				}

				return convertedStep;
			}

			set => convertedStep = value;
		}

		/// <summary>
		/// Seconds in day.
		/// </summary>
		protected readonly int SecondsInDay = 24 * 60 * 60;

		DateTime Today;

		[SerializeField]
		[FormerlySerializedAs("GroupByName")]
		bool groupByName;

		/// <summary>
		/// Group tasks with same name on one line.
		/// </summary>
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

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			Today = DateTime.Today;
			baseValue = Today;

			if (TodayHighlight != null)
			{
				TodayHighlight.pivot = new Vector2(0f, 1f);
			}

			if (WeekendHighlightsDefaultItem != null)
			{
				WeekendHighlightsDefaultItem.pivot = new Vector2(0f, 1f);
			}

			base.InitOnce();
		}

		/// <summary>
		/// Update view.
		/// </summary>
		protected override void UpdateView()
		{
			base.UpdateView();

			UpdateDateHighlights();
		}

		/// <summary>
		/// Update view.
		/// </summary>
		protected override void UpdateDataViewPositions()
		{
			base.UpdateDataViewPositions();

			UpdateDateHighlights();
		}

		[NonSerialized]
		readonly List<DateTime> weekends = new List<DateTime>();

		/// <summary>
		/// Update date highlights.
		/// </summary>
		protected virtual void UpdateDateHighlights()
		{
			if (TodayHighlight != null)
			{
				SetPosition(TodayHighlight, Today);
			}

			if (weekendHighlightsDefaultItem != null)
			{
				weekends.Clear();

				var half = PointsNamesView.Count / 2;
				for (int i = -half; i <= half; i++)
				{
					var day = BaseValue.AddDays(i);
					if (IsWeekend(day))
					{
						weekends.Add(day);
					}
				}

				WeekendHighlights.Require(weekends.Count);

				for (int i = 0; i < weekends.Count; i++)
				{
					SetPosition(WeekendHighlights[i], weekends[i]);
				}
			}
		}

		/// <summary>
		/// Set highlight position for the specified date.
		/// </summary>
		/// <param name="rect">RectTransform.</param>
		/// <param name="date">Date.</param>
		protected void SetPosition(RectTransform rect, DateTime date)
		{
			var pos = rect.localPosition;
			pos.x = Point2Position(date);
			rect.localPosition = pos;
			rect.SetAsFirstSibling();
		}

		/// <summary>
		/// Check is date is weekend.
		/// </summary>
		/// <param name="date">Date.</param>
		/// <returns>true if date is weekend; otherwise false.</returns>
		protected virtual bool IsWeekend(DateTime date)
		{
			return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
		}

		/// <summary>
		/// Convert point to base value.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Base value.</returns>
		protected override float Point2Base(DateTime point)
		{
			var delta = point - BaseValue;
			return (float)delta.TotalSeconds / SecondsInDay;
		}

		/// <summary>
		/// Convert base value to point.
		/// </summary>
		/// <param name="baseValue">Base value.</param>
		/// <returns>Point.</returns>
		protected override DateTime Base2Point(float baseValue)
		{
			var seconds = baseValue * SecondsInDay;
			var step = (float)Step.TotalSeconds;
			seconds = Mathf.RoundToInt(seconds / step) * step;

			return BaseValue.AddSeconds(seconds);
		}

		/// <summary>
		/// Get minimal width of the item.
		/// </summary>
		/// <returns>Minimal width.</returns>
		protected override float GetItemMinWidth()
		{
			return GetPointHeaderWidth() * (float)Step.TotalSeconds / SecondsInDay;
		}

		/// <summary>
		/// Get string representation of ValueAtCenter at specified distance.
		/// </summary>
		/// <param name="distance">Distance.</param>
		/// <returns>String representation of value at specified distance.</returns>
		protected override string Value2Text(int distance)
		{
			return ChangeValue(distance).ToString(DateFormat, UtilitiesCompare.Culture);
		}

		/// <summary>
		/// Change ValueAtCenter on specified delta.
		/// </summary>
		/// <param name="delta">Delta.</param>
		/// <returns>New value.</returns>
		protected override DateTime ChangeValue(int delta)
		{
			return BaseValue.AddDays(delta);
		}

		/// <summary>
		/// Copy data from source to target.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="target">Target.</param>
		public override void DataCopy(TrackData source, TrackData target)
		{
			source.CopyTo(target);
		}

		/// <summary>
		/// Set track settings.
		/// </summary>
		/// <param name="track">Track.</param>
		protected override void SetTrackSettings(Track<TrackData, DateTime> track)
		{
			track.SeparateGroups = !GroupByName;
			track.Layout = Layout;
			track.ItemsToTop = ItemsToTop;
		}

		/// <summary>
		/// Get layout for the tracks.
		/// </summary>
		/// <returns>Layout function.</returns>
		protected override TrackLayout<TrackData, DateTime> GetLayout()
		{
			return GroupByName ? new TrackLayoutByName() : base.GetLayout();
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
		public override bool TrackIntersection(Track<TrackData, DateTime> track, DateTime start, DateTime end, int order, TrackData target)
		{
			var is_new = !track.Data.Contains(target);
			if ((!is_new) && (target.Order != order) && (!GroupByName))
			{
				return false;
			}

			using var _ = ListPool<TrackData>.Get(out var temp);

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
		protected override void GetPossibleIntersections(ObservableList<TrackData> items, int order, TrackData target, List<TrackData> output)
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

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			weekendHighlights = null;
		}
	}
}