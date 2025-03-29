namespace UIWidgets
{
	using System;
	using UIThemes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for Calendar date.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/input/calendar.html")]
	public class CalendarDateBase : UIBehaviourInteractable, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, ISubmitHandler, IUpgradeable, ITargetOwner
	{
		/// <summary>
		/// Text component to display day.
		/// </summary>
		[SerializeField]
		protected TextAdapter dayAdapter;

		/// <summary>
		/// Text component to display day.
		/// </summary>
		public TextAdapter DayAdapter
		{
			get => dayAdapter;

			set
			{
				dayAdapter = value;
				DateChanged();
			}
		}

		/// <summary>
		/// Image component to display day background.
		/// </summary>
		[SerializeField]
		protected Image DayImage;

		[SerializeField]
		[FormerlySerializedAs("SelectedDayBackground")]
		Sprite selectedDayBackground;

		/// <summary>
		/// Selected date background.
		/// </summary>
		public Sprite SelectedDayBackground
		{
			get => selectedDayBackground;

			set
			{
				selectedDayBackground = value;
				UpdateView();
			}
		}

		[SerializeField]
		[FormerlySerializedAs("SelectedDay")]
		Color selectedDay = Color.white;

		/// <summary>
		/// Selected date color.
		/// </summary>
		public Color SelectedDay
		{
			get => selectedDay;

			set
			{
				selectedDay = value;
				UpdateView();
			}
		}

		[SerializeField]
		[FormerlySerializedAs("SelectedDayBold")]
		bool selectedDayBold = false;

		/// <summary>
		/// Make selected day bold.
		/// </summary>
		public bool SelectedDayBold
		{
			get => selectedDayBold;

			set
			{
				selectedDayBold = value;
				UpdateView();
			}
		}

		[SerializeField]
		[FormerlySerializedAs("DefaultDayBackground")]
		Sprite defaultDayBackground;

		/// <summary>
		/// Default date background.
		/// </summary>
		public Sprite DefaultDayBackground
		{
			get => defaultDayBackground;

			set
			{
				defaultDayBackground = value;
				UpdateView();
			}
		}

		[SerializeField]
		[FormerlySerializedAs("CurrentMonth")]
		Color currentMonth = Color.white;

		/// <summary>
		/// Color for date in current month.
		/// </summary>
		public Color CurrentMonth
		{
			get => currentMonth;

			set
			{
				currentMonth = value;
				UpdateView();
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Weekend")]
		Color weekend = Color.red;

		/// <summary>
		/// Weekend date color.
		/// </summary>
		public Color Weekend
		{
			get => weekend;

			set
			{
				weekend = value;
				UpdateView();
			}
		}

		[SerializeField]
		[FormerlySerializedAs("OtherMonth")]
		Color otherMonth = Color.gray;

		/// <summary>
		/// Color for date not in current month.
		/// </summary>
		public Color OtherMonth
		{
			get => otherMonth;

			set
			{
				otherMonth = value;
				UpdateView();
			}
		}

		[SerializeField]
		[FormerlySerializedAs("OtherMonthWeekend")]
		Color otherMonthWeekend = Color.gray * Color.red;

		/// <summary>
		/// Color for weekend date not in current month.
		/// </summary>
		public Color OtherMonthWeekend
		{
			get => otherMonthWeekend;

			set
			{
				otherMonthWeekend = value;
				UpdateView();
			}
		}

		[SerializeField]
		[FormerlySerializedAs("OutOfRangeDate")]
		Color outOfRangeDate = Color.gray * Color.gray;

		/// <summary>
		/// Color for date out of Calendar.DateMin..Calendar.DateMax range.
		/// </summary>
		public Color OutOfRangeDate
		{
			get => outOfRangeDate;

			set
			{
				outOfRangeDate = value;
				UpdateView();
			}
		}

		[NonSerialized]
		Selectable selectable;

		/// <summary>
		/// Selectable.
		/// </summary>
		public Selectable Selectable
		{
			get
			{
				if (selectable == null)
				{
					TryGetComponent(out selectable);
				}

				return selectable;
			}
		}

		/// <summary>
		/// Current date to display.
		/// </summary>
		protected DateTime CurrentDate;

		/// <summary>
		/// Date belongs to this calendar.
		/// </summary>
		[HideInInspector]
		public CalendarBase Calendar;

		/// <summary>
		/// Version.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		int version;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected override void Start()
		{
			base.Start();
			SetTargetOwner();
		}

		/// <summary>
		/// Set theme target owner.
		/// </summary>
		public virtual void SetTargetOwner()
		{
			UIThemes.Utilities.SetTargetOwner(typeof(Color), DayAdapter.Graphic, nameof(DayAdapter.Graphic.color), this);
		}

		/// <summary>
		/// Set current date.
		/// </summary>
		/// <param name="currentDate">Current date.</param>
		public virtual void SetDate(DateTime currentDate)
		{
			CurrentDate = currentDate;

			DateChanged();
		}

		/// <summary>
		/// Update displayed date.
		/// </summary>
		public virtual void DateChanged()
		{
			if (Calendar == null)
			{
				return;
			}

			DayAdapter.text = CurrentDate.ToString("dd", Calendar.Culture);

			UpdateView();
		}

		/// <summary>
		/// Update view.
		/// </summary>
		protected virtual void UpdateView()
		{
			if (Calendar == null)
			{
				return;
			}

			var interactable = true;
			if (Calendar.IsSameDay(Calendar.Date, CurrentDate))
			{
				DayAdapter.Bold = SelectedDayBold;
				DayAdapter.color = SelectedDay;
				DayImage.sprite = SelectedDayBackground;
			}
			else
			{
				DayAdapter.Bold = false;
				DayImage.sprite = DefaultDayBackground;

				if (Calendar.IsSameMonth(Calendar.DateDisplay, CurrentDate))
				{
					if (Calendar.IsWeekend(CurrentDate) ||
						Calendar.IsHoliday(CurrentDate))
					{
						DayAdapter.color = Weekend;
					}
					else
					{
						DayAdapter.color = CurrentMonth;
					}
				}
				else
				{
					if (Calendar.IsWeekend(CurrentDate) ||
						Calendar.IsHoliday(CurrentDate))
					{
						DayAdapter.color = OtherMonthWeekend;
					}
					else
					{
						DayAdapter.color = OtherMonth;
					}
				}

				if ((CurrentDate < Calendar.DateMin) || (CurrentDate > Calendar.DateMax))
				{
					DayAdapter.color *= OutOfRangeDate;
					interactable = false;
				}
			}

			if (Selectable != null)
			{
				Selectable.interactable = interactable;
			}
		}

		/// <summary>
		/// Process the pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Process the pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Change calendar date.
		/// </summary>
		protected virtual void ChangeDate()
		{
			if (!IsActive() || !Calendar.IsActive())
			{
				return;
			}

			if ((Calendar.DateMin > CurrentDate) || (CurrentDate > Calendar.DateMax))
			{
				return;
			}

			Calendar.Date = CurrentDate;
		}

		/// <summary>
		/// Process the click event.
		/// Change calendar date to clicked date.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData) => ChangeDate();

		/// <summary>
		/// Process the submit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSubmit(BaseEventData eventData) => ChangeDate();

		/// <summary>
		/// Apply specified style.
		/// </summary>
		/// <param name="styleCalendar">Style for the calendar.</param>
		/// <param name="style">Full style data.</param>
		public virtual void SetStyle(StyleCalendar styleCalendar, Style style)
		{
			if (DayAdapter != null)
			{
				styleCalendar.DayText.ApplyTo(DayAdapter.gameObject);
			}

			styleCalendar.DayBackground.ApplyTo(DayImage);

			DefaultDayBackground = styleCalendar.DayBackground.Sprite;
			SelectedDayBackground = styleCalendar.SelectedDayBackground;

			SelectedDay = styleCalendar.ColorSelectedDay;
			Weekend = styleCalendar.ColorWeekend;

			CurrentMonth = styleCalendar.ColorCurrentMonth;
			OtherMonth = styleCalendar.ColorOtherMonth;

			if (Calendar != null)
			{
				UpdateView();
			}
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleCalendar">Style for the calendar.</param>
		/// <param name="style">Full style data.</param>
		public virtual void GetStyle(StyleCalendar styleCalendar, Style style)
		{
			if (DayAdapter != null)
			{
				styleCalendar.DayText.GetFrom(DayAdapter.gameObject);
			}

			styleCalendar.DayBackground.GetFrom(DayImage);

			styleCalendar.DayBackground.Sprite = DefaultDayBackground;
			styleCalendar.SelectedDayBackground = SelectedDayBackground;

			styleCalendar.ColorSelectedDay = SelectedDay;
			styleCalendar.ColorWeekend = Weekend;

			styleCalendar.ColorCurrentMonth = CurrentMonth;
			styleCalendar.ColorOtherMonth = OtherMonth;
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
			if (version == 0)
			{
				OtherMonthWeekend = OtherMonth * Weekend;
				OutOfRangeDate = Color.gray * Color.gray;
				version = 1;
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();
			Compatibility.Upgrade(this);
		}
#endif
	}
}