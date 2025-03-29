namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UnityEngine;

	/// <summary>
	/// Multiple dates support for Calendar.
	/// </summary>
	[RequireComponent(typeof(CalendarBase))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/calendar-multiple-dates.html")]
	public class CalendarMultipleDates : MonoBehaviourInitiable
	{
		/// <summary>
		/// Date comparer.
		/// </summary>
		protected class DateComparer : IEquatable<DateComparer>
		{
			readonly CalendarBase Calendar;
			readonly DateTime Date;

			/// <summary>
			/// Initializes a new instance of the <see cref="DateComparer"/> class.
			/// </summary>
			/// <param name="calendar">Calender.</param>
			/// <param name="date">Date to compare with.</param>
			public DateComparer(CalendarBase calendar, DateTime date)
			{
				Calendar = calendar;
				Date = date;
			}

			/// <summary>
			/// Check is date is same day.
			/// </summary>
			/// <param name="date">Date.</param>
			/// <returns>true if date represents the same day; otherwise false.</returns>
			public bool IsSameDay(DateTime date) => Calendar.IsSameDay(Date, date);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj) => (obj is DateComparer comparer) && Equals(comparer);

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(DateComparer other)
			{
				if (other is null)
				{
					return false;
				}

				return Calendar == other.Calendar && Date == other.Date;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode() => Calendar.GetHashCode() ^ Date.GetHashCode();

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(DateComparer left, DateComparer right)
			{
				if (left is null)
				{
					return right is null;
				}

				return left.Equals(right);
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are not equal; otherwise, false.</returns>
			public static bool operator !=(DateComparer left, DateComparer right) => !(left == right);
		}

		/// <summary>
		/// Data source.
		/// </summary>
		protected ObservableList<DateTime> dataSource;

		/// <summary>
		/// Gets or sets the data source.
		/// </summary>
		/// <value>The data source.</value>
		[DataBindField]
		public virtual ObservableList<DateTime> DataSource
		{
			get
			{
				if (dataSource == null)
				{
					dataSource = new ObservableList<DateTime>();
					dataSource.OnChangeMono.AddListener(CollectionChanged);
				}

				return dataSource;
			}

			set
			{
				if (dataSource == value)
				{
					return;
				}

				dataSource?.OnChangeMono.RemoveListener(CollectionChanged);

				Init();

				dataSource = value;

				dataSource?.OnChangeMono.AddListener(CollectionChanged);

				CollectionChanged();
			}
		}

		CalendarBase currentCalendar;

		/// <summary>
		/// Current calendar.
		/// </summary>
		public CalendarBase CurrentCalendar
		{
			get
			{
				if (currentCalendar == null)
				{
					TryGetComponent(out currentCalendar);
				}

				return currentCalendar;
			}
		}

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			CurrentCalendar.OnDateClick.AddListener(ToggleDate);

			CollectionChanged();
		}

		/// <summary>
		/// Toggle date state.
		/// </summary>
		/// <param name="date">Date.</param>
		public virtual void ToggleDate(DateTime date)
		{
			if (IsSelected(date))
			{
				DataSource.RemoveAll(new DateComparer(CurrentCalendar, date).IsSameDay);
			}
			else
			{
				DataSource.Add(date);
			}
		}

		/// <summary>
		/// Select date.
		/// </summary>
		/// <param name="date">Date.</param>
		public virtual void SelectDate(DateTime date)
		{
			if (!IsSelected(date))
			{
				DataSource.Add(date);
			}
		}

		/// <summary>
		/// Toggle date state.
		/// </summary>
		/// <param name="date">Date.</param>
		public virtual void DeselectDate(DateTime date)
		{
			if (IsSelected(date))
			{
				DataSource.RemoveAll(new DateComparer(CurrentCalendar, date).IsSameDay);
			}
		}

		/// <summary>
		/// Is date selected?
		/// </summary>
		/// <param name="date">Date.</param>
		/// <returns>true if date selected; otherwise false.</returns>
		public virtual bool IsSelected(DateTime date)
		{
			foreach (var selected_date in DataSource)
			{
				if (CurrentCalendar.IsSameDay(date, selected_date))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Update calendar.
		/// </summary>
		protected virtual void CollectionChanged() => CurrentCalendar.UpdateCalendar();

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			dataSource?.OnChangeMono.RemoveListener(CollectionChanged);
		}
	}
}