namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// DatePicker.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/dialogs/datepicker.html")]
	public class DateTimePicker : Picker<DateTime, DateTimePicker>
	{
		/// <summary>
		/// DateTime widget.
		/// </summary>
		[SerializeField]
		public DateTimeWidget DateTimeWidget;

		/// <inheritdoc/>
		protected override void AddListeners()
		{
			base.AddListeners();
			DateTimeWidget.OnDateTimeChanged.AddListener(DateTimeSelected);
		}

		/// <inheritdoc/>
		protected override void RemoveListeners()
		{
			base.RemoveListeners();
			DateTimeWidget.OnDateTimeChanged.RemoveListener(DateTimeSelected);
		}

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(DateTime defaultValue)
		{
			base.BeforeOpen(defaultValue);
			DateTimeWidget.DateTime = defaultValue;
		}

		/// <summary>
		/// Process selected value.
		/// </summary>
		/// <param name="dt">Selected value.</param>
		protected void DateTimeSelected(DateTime dt)
		{
			Value = dt;
		}

		/// <summary>
		/// Pick the selected value.
		/// </summary>
		public void Ok()
		{
			Selected(Value);
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);

			DateTimeWidget.Calendar.SetStyle(style);
			DateTimeWidget.Time.SetStyle(style);

			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Cancel"));
			style.Dialog.Button.ApplyTo(transform.Find("Buttons/OK"));

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			base.GetStyle(style);

			DateTimeWidget.Calendar.GetStyle(style);
			DateTimeWidget.Time.GetStyle(style);

			style.Dialog.Button.GetFrom(transform.Find("Buttons/Cancel"));
			style.Dialog.Button.GetFrom(transform.Find("Buttons/OK"));

			return true;
		}
		#endregion
	}
}