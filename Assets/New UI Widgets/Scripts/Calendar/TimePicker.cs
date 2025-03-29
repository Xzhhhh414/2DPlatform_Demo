namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// DatePicker.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/dialogs/datepicker.html")]
	public class TimePicker : Picker<TimeSpan, TimePicker>
	{
		/// <summary>
		/// Time widget.
		/// </summary>
		[SerializeField]
		public TimeBase Time;

		/// <inheritdoc/>
		protected override void AddListeners()
		{
			base.AddListeners();
			Time.OnTimeChanged.AddListener(TimeSelected);
		}

		/// <inheritdoc/>
		protected override void RemoveListeners()
		{
			base.RemoveListeners();
			Time.OnTimeChanged.RemoveListener(TimeSelected);
		}

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(TimeSpan defaultValue)
		{
			base.BeforeOpen(defaultValue);
			Time.Time = defaultValue;
		}

		/// <summary>
		/// Process selected time.
		/// </summary>
		/// <param name="time">Time.</param>
		protected void TimeSelected(TimeSpan time)
		{
			Value = time;
		}

		/// <summary>
		/// Pick the selected time.
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

			Time.SetStyle(style);

			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Cancel"));
			style.Dialog.Button.ApplyTo(transform.Find("Buttons/OK"));

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			base.GetStyle(style);

			Time.GetStyle(style);

			style.Dialog.Button.GetFrom(transform.Find("Buttons/Cancel"));
			style.Dialog.Button.GetFrom(transform.Find("Buttons/OK"));

			return true;
		}
		#endregion
	}
}