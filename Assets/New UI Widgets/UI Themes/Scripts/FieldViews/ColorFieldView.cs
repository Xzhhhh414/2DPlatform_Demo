namespace UIThemes
{
	using UnityEngine;
	using UnityEngine.UIElements;

	/// <summary>
	/// Field view for the colors.
	/// </summary>
	public class ColorFieldView : FieldView<Color>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorFieldView"/> class.
		/// </summary>
		/// <param name="undoName">Undo name.</param>
		/// <param name="values">Theme values wrapper.</param>
		public ColorFieldView(string undoName, Theme.ValuesWrapper<Color> values)
			: base(undoName, values)
		{
		}

		/// <inheritdoc/>
		protected override VisualElement CreateView(VariationId variationId, OptionId optionId, Color value)
		{
			#if UNITY_EDITOR
			var input = new UnityEditor.UIElements.ColorField
			{
				value = value,
			};
			input.RegisterValueChangedCallback(x => Save(variationId, optionId, x.newValue));

			return input;
			#else
			return null;
			#endif
		}

		/// <inheritdoc/>
		public override void UpdateValue(VisualElement view, Color value)
		{
			#if UNITY_EDITOR
			if (view is UnityEditor.UIElements.ColorField input)
			{
				input.value = value;
			}
			#endif
		}
	}
}