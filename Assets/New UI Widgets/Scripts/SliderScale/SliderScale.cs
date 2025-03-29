namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Scale for the Slider widget.
	/// </summary>
	[RequireComponent(typeof(Slider))]
	public class SliderScale : SliderScaleBase
	{
		Slider slider;

		/// <summary>
		/// Slider.
		/// </summary>
		protected Slider Slider
		{
			get
			{
				if (slider == null)
				{
					TryGetComponent(out slider);
				}

				return slider;
			}
		}

		/// <inheritdoc/>
		protected override void AddListeners()
		{
			base.AddListeners();

			if (Slider != null)
			{
				Slider.onValueChanged.AddListener(OnSliderUpdate);
			}
		}

		/// <inheritdoc/>
		protected override void RemoveListeners()
		{
			base.RemoveListeners();

			if (slider != null)
			{
				slider.onValueChanged.AddListener(OnSliderUpdate);
			}
		}

		/// <summary>
		/// Process slider value changes.
		/// </summary>
		/// <param name="v">Value.</param>
		protected virtual void OnSliderUpdate(float v)
		{
			UpdateScale();
		}

		/// <inheritdoc/>
		public override void UpdateScale()
		{
			Scale.Set(Value2MarkDataDelegate, Slider.minValue, Slider.maxValue, Slider.value);
		}

		/// <inheritdoc/>
		protected override Vector2 Value2Anchor(float value)
		{
			var v = (value - Slider.minValue) / (Slider.maxValue - Slider.minValue);
			return Slider.direction switch
			{
				Slider.Direction.LeftToRight => new Vector2(v, 0.5f),
				Slider.Direction.RightToLeft => new Vector2(1f - v, 0.5f),
				Slider.Direction.TopToBottom => new Vector2(0.5f, 1f - v),
				Slider.Direction.BottomToTop => new Vector2(0.5f, v),
				_ => throw new NotSupportedException(string.Format("Unknown slider direction: {0}", EnumHelper<Slider.Direction>.ToString(Slider.direction))),
			};
		}
	}
}