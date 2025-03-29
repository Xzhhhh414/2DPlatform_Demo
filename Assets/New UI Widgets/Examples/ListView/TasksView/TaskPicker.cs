namespace UIWidgets.Examples.Tasks
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Task Picker.
	/// </summary>
	public class TaskPicker : PickerOptionalOK<Task, TaskPicker>
	{
		/// <summary>
		/// TaskView.
		/// </summary>
		[SerializeField]
		public TaskView TaskView;

		/// <inheritdoc/>
		protected override void AddListeners()
		{
			base.AddListeners();
			TaskView.OnSelectObject.AddListener(Select);
		}

		/// <inheritdoc/>
		protected override void RemoveListeners()
		{
			base.RemoveListeners();
			TaskView.OnSelectObject.RemoveListener(Select);
		}

		/// <summary>
		/// Set initial value and add callback.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(Task defaultValue)
		{
			base.BeforeOpen(defaultValue);
			TaskView.SelectedIndex = TaskView.DataSource.IndexOf(defaultValue);
		}

		/// <summary>
		/// Callback when value selected.
		/// </summary>
		/// <param name="index">Selected item index.</param>
		void Select(int index)
		{
			Value = TaskView.DataSource[index];

			// apply selected value and close picker
			if (Mode == PickerMode.CloseOnSelect)
			{
				Selected(Value);
			}
		}
	}
}