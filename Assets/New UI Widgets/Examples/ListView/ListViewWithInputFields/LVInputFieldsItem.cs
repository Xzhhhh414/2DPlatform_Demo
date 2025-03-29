namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using UnityEngine;

	/// <summary>
	/// LVInputFields item.
	/// </summary>
	[Serializable]
	public class LVInputFieldsItem : IObservable, INotifyPropertyChanged
	{
		[SerializeField]
		string text1;

		/// <summary>
		/// Text1.
		/// </summary>
		public string Text1
		{
			get
			{
				return text1;
			}

			set
			{
				Change(ref text1, value, "Text1");
			}
		}

		[SerializeField]
		string text2;

		/// <summary>
		/// Text2.
		/// </summary>
		public string Text2
		{
			get
			{
				return text2;
			}

			set
			{
				Change(ref text2, value, "Text2");
			}
		}

		[SerializeField]
		bool isOn;

		/// <summary>
		/// IsOn.
		/// </summary>
		public bool IsOn
		{
			get
			{
				return isOn;
			}

			set
			{
				Change(ref isOn, value, "IsOn");
			}
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
				Changed(propertyName);
			}
		}

		void Changed(string propertyName)
		{
			OnChange?.Invoke();

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}