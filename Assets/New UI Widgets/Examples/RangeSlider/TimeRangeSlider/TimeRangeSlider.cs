namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TimeRangeSlider.
	/// </summary>
	[RequireComponent(typeof(RangeSlider))]
	public class TimeRangeSlider : MonoBehaviourInitiable
	{
		[SerializeField]
		DateTime minTime = new DateTime(2017, 1, 1, 0, 0, 1);

		/// <summary>
		/// Min time.
		/// </summary>
		public DateTime MinTime
		{
			get => minTime;

			set
			{
				minTime = value;

				Init();
			}
		}

		[SerializeField]
		DateTime maxTime = new DateTime(2017, 1, 1, 23, 50, 0);

		/// <summary>
		/// Max time.
		/// </summary>
		public DateTime MaxTime
		{
			get => maxTime;

			set
			{
				maxTime = value;

				Init();
			}
		}

		[SerializeField]
		[Tooltip("Minutes")]
		int interval = 10;

		/// <summary>
		/// Interval.
		/// </summary>
		public int Interval
		{
			get => Mathf.Max(1, interval);

			set
			{
				interval = Mathf.Max(1, value);

				Init();
			}
		}

		/// <summary>
		/// Start time.
		/// </summary>
		public DateTime StartTime
		{
			get => Value2Time(CurrentRangeSlider.ValueMin);

			set => CurrentRangeSlider.ValueMin = Time2Value(value);
		}

		/// <summary>
		/// End time.
		/// </summary>
		public DateTime EndTime
		{
			get => Value2Time(CurrentRangeSlider.ValueMax);

			set => CurrentRangeSlider.ValueMax = Time2Value(value);
		}

		RangeSlider currentRangeSlider;

		/// <summary>
		/// Current RangeSlider.
		/// </summary>
		public RangeSlider CurrentRangeSlider
		{
			get
			{
				if (currentRangeSlider == null)
				{
					TryGetComponent(out currentRangeSlider);
				}

				return currentRangeSlider;
			}
		}

		/// <summary>
		/// Change time event.
		/// </summary>
		public TimeRangeSliderEvent OnChange = new TimeRangeSliderEvent();

		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void InitOnce()
		{
			base.InitOnce();

			CurrentRangeSlider.WholeNumberOfSteps = true;
			CurrentRangeSlider.LimitMin = Time2Value(MinTime);
			CurrentRangeSlider.LimitMax = Time2Value(MaxTime);
			CurrentRangeSlider.Step = Interval;

			CurrentRangeSlider.OnValuesChanged.AddListener(SliderChanged);

			CurrentRangeSlider.ValueMin = CurrentRangeSlider.LimitMin;
			CurrentRangeSlider.ValueMax = CurrentRangeSlider.LimitMax;
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (currentRangeSlider != null)
			{
				currentRangeSlider.OnValuesChanged.RemoveListener(SliderChanged);
			}
		}

		/// <summary>
		/// Handle slider values changed event.
		/// </summary>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		protected virtual void SliderChanged(int min, int max)
		{
			OnChange.Invoke(StartTime, EndTime);
		}

		DateTime Value2Time(int value)
		{
			value -= Time2Value(MinTime);
			return MinTime.AddMinutes(value);
		}

		static int Time2Value(DateTime time)
		{
			return time.Minute + (time.Hour * 60);
		}
	}
}