namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Tree view item.
	/// </summary>
	[Serializable]
	public class TreeViewItem : IObservable, INotifyPropertyChanged
	{
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		Sprite icon;

		/// <summary>
		/// Gets or sets the icon.
		/// </summary>
		/// <value>The icon.</value>
		public Sprite Icon
		{
			get => icon;

			set => Change(ref icon, value, nameof(Icon));
		}

		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		string name;

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
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
		[FormerlySerializedAs("_value")]
		int itemValue;

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public int Value
		{
			get => itemValue;

			set => Change(ref itemValue, value, nameof(Value));
		}

		[SerializeField]
		object tag;

		/// <summary>
		/// Gets or sets the tag.
		/// </summary>
		/// <value>The tag.</value>
		public object Tag
		{
			get => tag;

			set => Change(ref tag, value, nameof(Tag));
		}

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
		/// Initializes a new instance of the <see cref="UIWidgets.TreeViewItem"/> class.
		/// </summary>
		/// <param name="itemName">Item name.</param>
		/// <param name="itemIcon">Item icon.</param>
		public TreeViewItem(string itemName, Sprite itemIcon = null)
		{
			name = itemName;
			icon = itemIcon;
		}

		/// <summary>
		/// Raise OnChange event.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected void NotifyPropertyChanged(string propertyName)
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