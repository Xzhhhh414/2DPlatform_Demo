namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Test TimePicker.
	/// </summary>
	public class TestTimePicker : MonoBehaviour, IUpgradeable
	{
		/// <summary>
		/// DatePicker template.
		/// </summary>
		[SerializeField]
		protected TimePicker PickerTemplate;

		/// <summary>
		/// Text component to display selected value.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with InfoAdapter.")]
		protected Text Info;

		/// <summary>
		/// Text component to display selected value.
		/// </summary>
		[SerializeField]
		protected TextAdapter InfoAdapter;

		TimeSpan currentValue = DateTime.Now.TimeOfDay;

		/// <summary>
		/// Open picker and log selected value.
		/// </summary>
		public async void Test()
		{
			// create picker from template
			var picker = PickerTemplate.Clone();

			// show picker
			var value = await picker.ShowAsync(currentValue);
			if (value.Success)
			{
				currentValue = value;
				Debug.Log(string.Format("value: {0}", value.ToString()));
			}
			else
			{
				Debug.Log("canceled");
			}
		}

		/// <summary>
		/// Open picker and display selected value.
		/// </summary>
		public async void TestShow()
		{
			// create picker from template
			var picker = PickerTemplate.Clone();

			// show picker
			var value = await picker.ShowAsync(currentValue);
			if (value.Success)
			{
				currentValue = value;
				InfoAdapter.text = string.Format("Value: {0}", value.ToString());
			}
			else
			{
				InfoAdapter.text = "Canceled";
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.RequireComponent(Info, ref InfoAdapter);
#pragma warning restore 0612, 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif
	}
}