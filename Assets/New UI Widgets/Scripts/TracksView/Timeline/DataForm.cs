namespace UIWidgets.Timeline
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Form to add/edit data.
	/// </summary>
	public class DataForm : TrackDataFormBase<TimelineData, TimeSpan>
	{
		/// <summary>
		/// DateTime picker.
		/// </summary>
		[SerializeField]
		protected TimePicker Picker;

		/// <summary>
		/// Text to display StartPoint.
		/// </summary>
		[SerializeField]
		protected TextAdapter StartPoint;

		/// <summary>
		/// Text to display EndPoint.
		/// </summary>
		[SerializeField]
		protected TextAdapter EndPoint;

		/// <summary>
		/// Button to change StartPoint.
		/// </summary>
		[SerializeField]
		protected Button StartPointChange;

		/// <summary>
		/// Button to change EndPoint.
		/// </summary>
		[SerializeField]
		protected Button EndPointChange;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		protected InputFieldAdapter Name;

		/// <summary>
		/// Date format.
		/// </summary>
		[SerializeField]
		protected string TimeFormat = @"hh\:mm\:ss";

		/// <summary>
		/// Create data.
		/// </summary>
		public override void Create()
		{
			Data = new TimelineData();
		}

		/// <summary>
		/// Create data with specified StartPoint.
		/// </summary>
		/// <param name="startPoint">SpartPoint.</param>
		public override void Create(TimeSpan startPoint)
		{
			Data = new TimelineData()
			{
				StartPoint = startPoint,
				EndPoint = startPoint.Add(new TimeSpan(0, 0, 10)),
			};

			SetValues();
		}

		/// <summary>
		/// Edit data.
		/// </summary>
		/// <param name="data">Data.</param>
		public override void Edit(TimelineData data)
		{
			Data = new TimelineData();

			data.CopyTo(Data);

			SetValues();
		}

		/// <summary>
		/// Set values.
		/// </summary>
		protected virtual void SetValues()
		{
			StartPoint.Value = Data.StartPoint.ToString(TimeFormat, UtilitiesCompare.Culture);
			EndPoint.Value = Data.EndPoint.ToString(TimeFormat, UtilitiesCompare.Culture);
			Name.Value = Data.Name;
		}

		/// <summary>
		/// Process name changed event.
		/// </summary>
		/// <param name="name">Name.</param>
		protected void NameChanged(string name)
		{
			Data.Name = name;
		}

		/// <summary>
		/// Add listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public override void AddListeners()
		{
			if (StartPointChange != null)
			{
				StartPointChange.onClick.AddListener(OpenStartPointPicker);
			}

			if (EndPointChange != null)
			{
				EndPointChange.onClick.AddListener(OpenEndPointPicker);
			}

			if (Name != null)
			{
				Name.onValueChanged.AddListener(NameChanged);
				Name.onEndEdit.AddListener(NameChanged);
			}
		}

		/// <summary>
		/// Open picker to change StartPoint.
		/// </summary>
		protected async void OpenStartPointPicker()
		{
			var dt = await Picker.Clone().ShowAsync(Data.StartPoint);

			Data.StartPoint = dt;
			StartPoint.Value = Data.StartPoint.ToString(TimeFormat);
		}

		/// <summary>
		/// Open picker to change EndPoint.
		/// </summary>
		protected async void OpenEndPointPicker()
		{
			var dt = await Picker.Clone().ShowAsync(Data.EndPoint);

			Data.EndPoint = dt;
			EndPoint.Value = Data.EndPoint.ToString(TimeFormat);
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public override void RemoveListeners()
		{
			if (StartPointChange != null)
			{
				StartPointChange.onClick.RemoveListener(OpenStartPointPicker);
			}

			if (EndPointChange != null)
			{
				EndPointChange.onClick.RemoveListener(OpenEndPointPicker);
			}

			if (Name != null)
			{
				Name.onValueChanged.RemoveListener(NameChanged);
				Name.onEndEdit.RemoveListener(NameChanged);
			}
		}
	}
}