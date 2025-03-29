namespace UIWidgets
{
	using System;
	using System.Globalization;
	using UIWidgets.Attributes;
	using UIWidgets.l10n;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Calendar base class.
	/// </summary>
	[DataBindSupport]
	public abstract class DateBase : UIBehaviourInteractable, IStylable
	{
		[SerializeField]
		DateTime date = DateTime.Today;

		/// <summary>
		/// Selected date.
		/// </summary>
		[DataBindField]
		public DateTime Date
		{
			get => date;

			set => SetDate(value);
		}

		[SerializeField]
		DateTime dateMin = DateTime.MinValue;

		/// <summary>
		/// The minimum selectable date.
		/// </summary>
		/// <value>The date minimum.</value>
		public DateTime DateMin
		{
			get => dateMin;

			set
			{
				Init();
				dateMin = value;

				date = Clamp(date);
			}
		}

		[SerializeField]
		DateTime dateMax = DateTime.MaxValue;

		/// <summary>
		/// The maximum selectable date.
		/// </summary>
		/// <value>The date max.</value>
		public DateTime DateMax
		{
			get => dateMax;

			set
			{
				Init();
				dateMax = value;

				date = Clamp(date);
			}
		}

		CultureInfo culture = CultureInfo.InvariantCulture;

		/// <summary>
		/// Current culture.
		/// </summary>
		public CultureInfo Culture
		{
			get => culture;

			set
			{
				culture = value;

				UpdateCalendar();
			}
		}

		[SerializeField]
		bool currentDateAsDefault = true;

		/// <summary>
		/// Default date.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("currentDate")]
		[FormerlySerializedAs("defaultDate")]
		[EditorConditionBool(nameof(currentDateAsDefault), false)]
		protected string DefaultDate = DateTime.Today.ToString("yyyy-MM-dd");

		/// <summary>
		/// Minimal date.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("currentDateMin")]
		[FormerlySerializedAs("defaultDateMin")]
		protected string DefaultDateMin = DateTime.MinValue.ToString("yyyy-MM-dd");

		/// <summary>
		/// Maximal date.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("format")]
		[FormerlySerializedAs("defaultDateMax")]
		protected string DefaultDateMax = DateTime.MaxValue.ToString("yyyy-MM-dd");

		/// <summary>
		/// Date format.
		/// </summary>
		[SerializeField]
		protected string format = "yyyy-MM-dd";

		/// <summary>
		/// Date format.
		/// </summary>
		public string Format
		{
			get => format;

			set
			{
				format = value;

				UpdateDate();
			}
		}

		/// <summary>
		/// Event called when date changed.
		/// </summary>
		[DataBindEvent(nameof(Date))]
		[SerializeField]
		public CalendarDateEvent OnDateChanged = new CalendarDateEvent();

		/// <summary>
		/// Event called when date clicked.
		/// </summary>
		[SerializeField]
		public CalendarDateEvent OnDateClick = new CalendarDateEvent();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			culture = Localization.Culture;
			dateMin = DateTime.ParseExact(DefaultDateMin, Format, CultureInfo.InvariantCulture);
			dateMax = DateTime.ParseExact(DefaultDateMax, Format, CultureInfo.InvariantCulture);

			date = currentDateAsDefault
				? Clamp(DateTime.Now)
				: Clamp(DateTime.ParseExact(DefaultDate, Format, CultureInfo.InvariantCulture));

			InitNestedWidgets();

			UpdateCalendar();
		}

		bool localeSubscription;

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			if (!localeSubscription)
			{
				Init();

				localeSubscription = true;
				Localization.OnLocaleChanged += LocaleChanged;
				LocaleChanged();
			}
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();
			Localization.OnLocaleChanged -= LocaleChanged;
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public virtual void LocaleChanged() => Culture = Localization.Culture;

		/// <summary>
		/// Set date.
		/// </summary>
		/// <param name="value">value.</param>
		protected virtual void SetDate(DateTime value)
		{
			var is_changed = date != value;

			Init();
			date = Clamp(value);

			UpdateCalendar();

			if (is_changed)
			{
				OnDateChanged.Invoke(date);
			}

			OnDateClick.Invoke(date);
		}

		/// <summary>
		/// Nested widgets initialization.
		/// </summary>
		protected abstract void InitNestedWidgets();

		/// <summary>
		/// Clamp specified date.
		/// </summary>
		/// <param name="d">Date.</param>
		/// <returns>Date</returns>
		protected virtual DateTime Clamp(DateTime d)
		{
			if (d < dateMin)
			{
				d = dateMin;
			}

			if (d > dateMax)
			{
				d = dateMax;
			}

			return d;
		}

		/// <summary>
		/// Updates the calendar.
		/// Should be called only after changing culture settings.
		/// </summary>
		public abstract void UpdateCalendar();

		/// <summary>
		/// Update displayed date and month.
		/// </summary>
		protected virtual void UpdateDate()
		{
		}

		/// <summary>
		/// Is specified date is weekend?
		/// </summary>
		/// <param name="displayedDate">Displayed date.</param>
		/// <returns>true if specified date is weekend; otherwise, false.</returns>
		public virtual bool IsWeekend(DateTime displayedDate)
		{
			var day_of_week = culture.DateTimeFormat.Calendar.GetDayOfWeek(displayedDate);
			return day_of_week == DayOfWeek.Saturday || day_of_week == DayOfWeek.Sunday;
		}

		/// <summary>
		/// Is specified date is holiday?
		/// </summary>
		/// <param name="displayedDate">Displayed date.</param>
		/// <returns>true if specified date is holiday; otherwise, false.</returns>
		public virtual bool IsHoliday(DateTime displayedDate) => false;

		/// <summary>
		/// Is dates belongs to same month?
		/// </summary>
		/// <param name="date1">First date.</param>
		/// <param name="date2">Second date.</param>
		/// <returns>true if dates belongs to same month; otherwise, false.</returns>
		public virtual bool IsSameMonth(DateTime date1, DateTime date2)
		{
			return culture.DateTimeFormat.Calendar.GetYear(date1) == culture.DateTimeFormat.Calendar.GetYear(date2)
				&& culture.DateTimeFormat.Calendar.GetMonth(date1) == culture.DateTimeFormat.Calendar.GetMonth(date2);
		}

		/// <summary>
		/// Is dates belongs to same day?
		/// </summary>
		/// <param name="date1">First date.</param>
		/// <param name="date2">Second date.</param>
		/// <returns>true if dates is same day; otherwise, false.</returns>
		public virtual bool IsSameDay(DateTime date1, DateTime date2) => date1.Date == date2.Date;

		/// <summary>
		/// Set calendar type.
		/// </summary>
		/// <param name="calendar">Calendar type.</param>
		public void SetCalendar(System.Globalization.Calendar calendar)
		{
			culture.DateTimeFormat.Calendar = calendar;
			UpdateCalendar();
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public abstract bool SetStyle(Style style);

		/// <inheritdoc/>
		public abstract bool GetStyle(Style style);
		#endregion
	}
}