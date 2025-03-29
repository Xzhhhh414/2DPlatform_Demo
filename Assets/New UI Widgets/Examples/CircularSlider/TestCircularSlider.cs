namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// CircularSlider with label.
	/// </summary>
	[RequireComponent(typeof(CircularSlider))]
	public class TestCircularSlider : MonoBehaviourInitiable
	{
		/// <summary>
		/// Text component to display min value.
		/// </summary>
		[SerializeField]
		protected TextAdapter Label;

		CircularSlider slider;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			if (TryGetComponent(out slider))
			{
				slider.OnValueChanged.AddListener(ValueChanged);
				ValueChanged(slider.Value);
			}
		}

		/// <summary>
		/// Callback when slider value changed.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void ValueChanged(int value)
		{
			Label.text = value.ToString();
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (slider != null)
			{
				slider.OnValueChanged.RemoveListener(ValueChanged);
			}
		}
	}
}