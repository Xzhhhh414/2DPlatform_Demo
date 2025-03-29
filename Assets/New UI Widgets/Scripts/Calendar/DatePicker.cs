namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// DatePicker.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/dialogs/datepicker.html")]
	public class DatePicker : PickerOptionalOK<DateTime, DatePicker>
	{
		/// <summary>
		/// If true select date only when date changes; otherwise select date on click.
		/// </summary>
		[SerializeField]
		[EditorConditionEnum(nameof(Mode), (int)PickerMode.CloseOnSelect)]
		public bool DateChangeOnly = false;

		/// <summary>
		/// Calendar.
		/// </summary>
		[SerializeField]
		public DateBase Calendar;

		/// <inheritdoc/>
		protected override void AddListeners()
		{
			base.AddListeners();
			Calendar.OnDateChanged.AddListener(DateChange);
			Calendar.OnDateClick.AddListener(DateClick);
		}

		/// <inheritdoc/>
		protected override void RemoveListeners()
		{
			base.RemoveListeners();
			Calendar.OnDateChanged.RemoveListener(DateChange);
			Calendar.OnDateClick.RemoveListener(DateClick);
		}

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(DateTime defaultValue)
		{
			base.BeforeOpen(defaultValue);
			Calendar.Date = defaultValue;
		}

		/// <summary>
		/// Process date time change.
		/// </summary>
		/// <param name="dt">Selected value.</param>
		protected void DateChange(DateTime dt)
		{
			Value = dt;

			if ((Mode == PickerMode.CloseOnSelect) && DateChangeOnly)
			{
				Selected(Value);
			}
		}

		/// <summary>
		/// Process date time click.
		/// </summary>
		/// <param name="dt">Selected value.</param>
		protected void DateClick(DateTime dt)
		{
			Value = dt;

			if ((Mode == PickerMode.CloseOnSelect) && !DateChangeOnly)
			{
				Selected(Value);
			}
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);
			Calendar.SetStyle(style);

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			base.GetStyle(style);
			Calendar.GetStyle(style);

			return true;
		}
		#endregion
	}
}