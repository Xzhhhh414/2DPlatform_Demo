namespace UIWidgets.Timeline
{
	using System;
	using System.ComponentModel;
	using UIWidgets.Attributes;
	using UIWidgets.Pool;
	using UnityEngine;

	/// <summary>
	/// Schedule view.
	/// </summary>
	/// <typeparam name="TData">Data type.</typeparam>
	/// <typeparam name="TDataView">DataView type.</typeparam>
	/// <typeparam name="TTrackView">TrackView type.</typeparam>
	/// <typeparam name="TTrackBackground">TrackBackground type.</typeparam>
	/// <typeparam name="TDataDialog">TrackDataDialog type.</typeparam>
	/// <typeparam name="TDataForm">TrackDataForm type.</typeparam>
	/// <typeparam name="TDialog">TrackDialog type.</typeparam>
	/// <typeparam name="TForm">TrackForm type.</typeparam>
	public abstract class TimelineCustom<TData, TDataView, TTrackView, TTrackBackground, TDataDialog, TDataForm, TDialog, TForm>
		: TracksViewCustom<TData, TimeSpan, TDataView, TTrackView, TTrackBackground, TDataDialog, TDataForm, TDialog, TForm>
		where TData : class, ITrackData<TimeSpan>, IObservable, INotifyPropertyChanged
		where TDataView : TrackDataViewBase<TData, TimeSpan>
		where TTrackView : TrackViewBase<TData, TimeSpan>
		where TTrackBackground : TrackBackgroundBase<TData, TimeSpan>
		where TDataDialog : TrackDataDialogBase<TData, TimeSpan, TDataForm>
		where TDataForm : TrackDataFormBase<TData, TimeSpan>
		where TDialog : TrackDialogBase<TData, TimeSpan, TForm>
		where TForm : TrackFormBase<TData, TimeSpan>
	{
		[Multiline]
		[SerializeField]
		string timeFormat = @"mm\:ss";

		/// <summary>
		/// Date format.
		/// </summary>
		[DataBindField]
		public string TimeFormat
		{
			get => timeFormat;

			set
			{
				if (timeFormat != value)
				{
					timeFormat = value;
					PointsNamesView.UpdateView();
				}
			}
		}

		[SerializeField]
		SerializedTimeSpan step = new SerializedTimeSpan(0, 0, 0, 1);

		/// <summary>
		/// Position (drag) step.
		/// </summary>
		public TimeSpan Step
		{
			get;
			set;
		}

		[SerializeField]
		SerializedTimeSpan headerStep = new SerializedTimeSpan(0, 0, 5);

		/// <summary>
		/// Header step.
		/// </summary>
		public TimeSpan HeaderStep
		{
			get;
			set;
		}

		/// <summary>
		/// Init this instance only once.
		/// </summary>
		protected override void InitOnce()
		{
			Step = step;
			HeaderStep = headerStep;

			PointsNamesView.AllowDecrease = AllowDecrease;

			base.InitOnce();

			BaseValue = default;
		}

		/// <summary>
		/// Check is value can be decreased.
		/// </summary>
		/// <returns>true if value can be decreased; otherwise false.</returns>
		protected virtual bool AllowDecrease()
		{
			var min = default(TimeSpan);
			return VisibleStart > min;
		}

		/// <summary>
		/// Set highlight position for the specified date.
		/// </summary>
		/// <param name="rect">RectTransform.</param>
		/// <param name="time">Time.</param>
		protected void SetPosition(RectTransform rect, TimeSpan time)
		{
			var pos = rect.localPosition;
			pos.x = Point2Position(time);
			rect.localPosition = pos;
			rect.SetAsFirstSibling();
		}

		/// <summary>
		/// Convert point to base value.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Base value.</returns>
		protected override float Point2Base(TimeSpan point)
		{
			var delta = point - BaseValue;
			return (float)(delta.TotalMilliseconds / HeaderStep.TotalMilliseconds);
		}

		/// <summary>
		/// Convert base value to point.
		/// </summary>
		/// <param name="baseValue">Base value.</param>
		/// <returns>Point.</returns>
		protected override TimeSpan Base2Point(float baseValue)
		{
			var ms = baseValue * HeaderStep.TotalMilliseconds;
			var step = Step.TotalMilliseconds;
			ms = Math.Round(ms / step) * step;

			return BaseValue.Add(TimeSpan.FromMilliseconds(ms));
		}

		/// <summary>
		/// Get minimal width of the item.
		/// </summary>
		/// <returns>Minimal width.</returns>
		protected override float GetItemMinWidth()
		{
			return GetPointHeaderWidth() * (float)(Step.TotalMilliseconds / HeaderStep.TotalMilliseconds);
		}

		/// <summary>
		/// Get string representation of ValueAtCenter at specified distance.
		/// </summary>
		/// <param name="distance">Distance.</param>
		/// <returns>String representation of value at specified distance.</returns>
		protected override string Value2Text(int distance)
		{
			return ChangeValue(distance).ToString(TimeFormat, UtilitiesCompare.Culture);
		}

		/// <summary>
		/// Change ValueAtCenter on specified delta.
		/// </summary>
		/// <param name="delta">Delta.</param>
		/// <returns>New value.</returns>
		protected override TimeSpan ChangeValue(int delta)
		{
			var time = TimeSpan.FromMilliseconds(HeaderStep.TotalMilliseconds * delta);
			return BaseValue.Add(time);
		}

		/// <summary>
		/// Set track settings.
		/// </summary>
		/// <param name="track">Track.</param>
		protected override void SetTrackSettings(Track<TData, TimeSpan> track)
		{
			track.Layout = Layout;
			track.ItemsToTop = ItemsToTop;
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
		public override bool TrackIntersection(Track<TData, TimeSpan> track, TimeSpan start, TimeSpan end, int order, TData target)
		{
			var is_new = !track.Data.Contains(target);
			if ((!is_new) && (target.Order != order))
			{
				return false;
			}

			using var _ = ListPool<TData>.Get(out var temp);

			GetPossibleIntersections(track.Data, order, target, temp);
			var result = ListIntersection(temp, start, end, order, target);

			return result;
		}
	}
}