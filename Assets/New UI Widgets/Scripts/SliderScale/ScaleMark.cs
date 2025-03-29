namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using UnityEngine;

	/// <summary>
	/// Scale mark.
	/// </summary>
	[Serializable]
	public class ScaleMark : IObservable, INotifyPropertyChanged
	{
		[SerializeField]
		float step;

		/// <summary>
		/// Step.
		/// </summary>
		public float Step
		{
			get => step;

			set => Change(ref step, value, nameof(Step));
		}

		[SerializeField]
		ScaleMarkTemplate template;

		/// <summary>
		/// Template.
		/// </summary>
		public ScaleMarkTemplate Template
		{
			get => template;

			set => Change(ref template, value, nameof(Template));
		}

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Change value.
		/// </summary>
		/// <typeparam name="T">Type of field.</typeparam>
		/// <param name="field">Field value.</param>
		/// <param name="value">New value.</param>
		/// <param name="propertyName">Property name.</param>
		protected void Change<T>(ref T field, T value, string propertyName)
		{
			if (!EqualityComparer<T>.Default.Equals(field, value))
			{
				field = value;
				NotifyPropertyChanged(propertyName);
			}
		}

		/// <summary>
		/// Raise PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected void NotifyPropertyChanged(string propertyName)
		{
			OnChange?.Invoke();

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}