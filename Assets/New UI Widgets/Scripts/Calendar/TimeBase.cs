namespace UIWidgets
{
	using System;
	using System.Globalization;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Base class for Time widget.
	/// </summary>
	[DataBindSupport]
	public abstract class TimeBase : UIBehaviourInteractable, IStylable
	{
		[SerializeField]
		bool currentTimeAsDefault = false;

		/// <summary>
		/// The time as text.
		/// </summary>
		[SerializeField]
		[EditorConditionBool(nameof(currentTimeAsDefault), false)]
		protected string timeText = DateTime.Now.TimeOfDay.ToString();

		/// <summary>
		/// The minimum time as text.
		/// </summary>
		[SerializeField]
		protected string timeMinText = "00:00:00";

		/// <summary>
		/// The maximum time as text.
		/// </summary>
		[SerializeField]
		protected string timeMaxText = "23:59:59";

		/// <summary>
		/// The time.
		/// </summary>
		[SerializeField]
		protected TimeSpan time = DateTime.Now.TimeOfDay;

		/// <summary>
		/// The time.
		/// </summary>
		[DataBindField]
		public virtual TimeSpan Time
		{
			get => time;

			set
			{
				Init();

				if (time == value)
				{
					return;
				}

				time = value;

				if (time.Ticks < 0)
				{
					time += new TimeSpan(1, 0, 0, 0);
				}

				if (time.Days > 0)
				{
					time -= new TimeSpan(time.Days, 0, 0, 0);
				}

				time = Clamp(time);

				UpdateInputs();
				OnTimeChanged.Invoke(time);
			}
		}

		/// <summary>
		/// The minimum time.
		/// </summary>
		[SerializeField]
		protected TimeSpan timeMin = new TimeSpan(0, 0, 0);

		/// <summary>
		/// The minimum time.
		/// </summary>
		/// <value>The time minimum.</value>
		public TimeSpan TimeMin
		{
			get => timeMin;

			set
			{
				timeMin = value;
				Time = Clamp(Time);
			}
		}

		/// <summary>
		/// The maximum time.
		/// </summary>
		[SerializeField]
		protected TimeSpan timeMax = new TimeSpan(23, 59, 59);

		/// <summary>
		/// The maximum time.
		/// </summary>
		/// <value>The time max.</value>
		public TimeSpan TimeMax
		{
			get => timeMax;

			set
			{
				timeMax = value;
				Time = Clamp(Time);
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

				UpdateInputs();
			}
		}

		/// <summary>
		/// The OnTimeChanged event.
		/// </summary>
		[DataBindEvent(nameof(Time))]
		public TimeEvent OnTimeChanged = new TimeEvent();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (currentTimeAsDefault)
			{
				time = DateTime.Now.TimeOfDay;
			}
			else
			{
				if (!TimeSpan.TryParse(timeText, out time))
				{
					Debug.LogWarning("TimeText cannot be parsed.");
				}
			}

			if (!TimeSpan.TryParse(timeMinText, out timeMin))
			{
				Debug.LogWarning("TimeMinText cannot be parsed.");
			}

			if (!TimeSpan.TryParse(timeMaxText, out timeMax))
			{
				Debug.LogWarning("TimeMaxText cannot be parsed.");
			}

			InitInput();

			UpdateInputs();

			AddListeners();

			InteractableChanged();

			Time = Clamp(Time);
		}

		/// <summary>
		/// Init the input.
		/// </summary>
		protected abstract void InitInput();

		/// <summary>
		/// Clamp the specified time.
		/// </summary>
		/// <param name="t">Time.</param>
		/// <returns>Clamped time.</returns>
		protected virtual TimeSpan Clamp(TimeSpan t)
		{
			if (t < timeMin)
			{
				t = timeMin;
			}

			if (t > timeMax)
			{
				t = timeMax;
			}

			return t;
		}

		/// <summary>
		/// Add the listeners.
		/// </summary>
		protected abstract void AddListeners();

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected abstract void RemoveListeners();

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();
			RemoveListeners();
		}

		/// <summary>
		/// Updates the inputs.
		/// </summary>
		public abstract void UpdateInputs();

		#region IStylable implementation

		/// <inheritdoc/>
		public abstract bool SetStyle(Style style);

		/// <inheritdoc/>
		public abstract bool GetStyle(Style style);
		#endregion
	}
}