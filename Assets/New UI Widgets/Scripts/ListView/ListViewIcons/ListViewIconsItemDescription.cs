namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// ListViewIcons item description.
	/// </summary>
	[Serializable]
	public class ListViewIconsItemDescription : IObservable, INotifyPropertyChanged
	{
		[SerializeField]
		[FormerlySerializedAs("Icon")]
		Sprite icon;

		/// <summary>
		/// The icon.
		/// </summary>
		public Sprite Icon
		{
			get => icon;

			set => Change(ref icon, value, nameof(Icon));
		}

		[SerializeField]
		[FormerlySerializedAs("Name")]
		string name;

		/// <summary>
		/// The name.
		/// </summary>
		public string Name
		{
			get => name;

			set => Change(ref name, value, nameof(Name));
		}

		[NonSerialized]
		string localizedName;

		/// <summary>
		/// The localized name.
		/// </summary>
		public string LocalizedName
		{
			get => localizedName;

			set => Change(ref localizedName, value, nameof(LocalizedName));
		}

		[SerializeField]
		[FormerlySerializedAs("Value")]
		int val;

		/// <summary>
		/// The value.
		/// </summary>
		public int Value
		{
			get => val;

			set => Change(ref val, value, nameof(Value));
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

		/// <summary>
		/// Raise PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected void Changed(string propertyName)
		{
			OnChange?.Invoke();

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Convert this instance to string.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString() => LocalizedName ?? Name;
	}
}