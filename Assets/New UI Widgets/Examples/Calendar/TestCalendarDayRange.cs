namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Test calendar with dates range.
	/// </summary>
	public class TestCalendarDayRange : CalendarDate // or CalendarDateTMPro if need TMPro support
	{
		/// <summary>
		/// The range.
		/// </summary>
		[SerializeField]
		public TestDayRangeSource Range;

		[SerializeField]
		[FormerlySerializedAs("DateStartDefault")]
		Sprite dateStartDefault;

		/// <summary>
		/// The default background for start date.
		/// </summary>
		public Sprite DateStartDefault
		{
			get
			{
				return dateStartDefault;
			}

			set
			{
				dateStartDefault = value;
			}
		}

		[SerializeField]
		[FormerlySerializedAs("DateStartLastDayOfWeek")]
		Sprite dateStartLastDayOfWeek;

		/// <summary>
		/// The backdgound for start date if start date is last day of week.
		/// </summary>
		public Sprite DateStartLastDayOfWeek
		{
			get
			{
				return dateStartLastDayOfWeek;
			}

			set
			{
				dateStartLastDayOfWeek = value;
			}
		}

		[SerializeField]
		[FormerlySerializedAs("DateRangeDefault")]
		Sprite dateRangeDefault;

		/// <summary>
		/// The default background for date in range.
		/// </summary>
		public Sprite DateRangeDefault
		{
			get
			{
				return dateRangeDefault;
			}

			set
			{
				dateRangeDefault = value;
			}
		}

		[SerializeField]
		[FormerlySerializedAs("DateRangeFirstDayOfWeek")]
		Sprite dateRangeFirstDayOfWeek;

		/// <summary>
		/// The default background for date in range if date is first day of week.
		/// </summary>
		public Sprite DateRangeFirstDayOfWeek
		{
			get
			{
				return dateRangeFirstDayOfWeek;
			}

			set
			{
				dateRangeFirstDayOfWeek = value;
			}
		}

		[SerializeField]
		[FormerlySerializedAs("DateRangeLastDayOfWeek")]
		Sprite dateRangeLastDayOfWeek;

		/// <summary>
		/// The default background for date in range if date is last day of week.
		/// </summary>
		public Sprite DateRangeLastDayOfWeek
		{
			get
			{
				return dateRangeLastDayOfWeek;
			}

			set
			{
				dateRangeLastDayOfWeek = value;
			}
		}

		[SerializeField]
		[FormerlySerializedAs("DateEndDefault")]
		Sprite dateEndDefault;

		/// <summary>
		/// The default background for end date.
		/// </summary>
		public Sprite DateEndDefault
		{
			get
			{
				return dateEndDefault;
			}

			set
			{
				dateEndDefault = value;
			}
		}

		[SerializeField]
		[FormerlySerializedAs("DateEndLastDayOfWeek")]
		Sprite dateEndLastDayOfWeek;

		/// <summary>
		/// The backdgound for end date if end date is last day of week.
		/// </summary>
		public Sprite DateEndLastDayOfWeek
		{
			get
			{
				return dateEndLastDayOfWeek;
			}

			set
			{
				dateEndLastDayOfWeek = value;
			}
		}

		/// <summary>
		/// Update displayed date.
		/// </summary>
		public override void DateChanged()
		{
			// set default text, sprites and colors
			base.DateChanged();

			var is_last_day = CurrentDate.DayOfWeek == GetLastDayOfWeek();
			var is_first_day = CurrentDate.DayOfWeek == Calendar.FirstDayOfWeek;
			if (Calendar.IsSameDay(Range.DateStart, CurrentDate))
			{
				DayImage.sprite = is_last_day ? DateStartLastDayOfWeek : DateStartDefault;
			}
			else if (Calendar.IsSameDay(Range.DateEnd, CurrentDate))
			{
				DayImage.sprite = is_last_day ? DateEndLastDayOfWeek : DateEndDefault;
			}
			else if ((Range.DateStart < CurrentDate) && (CurrentDate < Range.DateEnd))
			{
				if (is_first_day)
				{
					DayImage.sprite = DateRangeFirstDayOfWeek;
				}
				else if (is_last_day)
				{
					DayImage.sprite = DateRangeLastDayOfWeek;
				}
				else
				{
					DayImage.sprite = DateRangeDefault;
				}
			}

			DayImage.color = Color.white;
		}

		DayOfWeek GetLastDayOfWeek()
		{
			var i = (int)Calendar.FirstDayOfWeek;
			i = i == 0 ? 6 : i - 1;
			return (DayOfWeek)i;
		}
	}
}