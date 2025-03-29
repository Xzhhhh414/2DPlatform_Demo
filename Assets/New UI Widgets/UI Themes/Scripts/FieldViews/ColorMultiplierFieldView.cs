namespace UIThemes
{
	using UnityEngine.UIElements;
	#if UNITY_2022_1_OR_NEWER
	using FloatField = UnityEngine.UIElements.FloatField;
	#elif UNITY_EDITOR
	using FloatField = UnityEditor.UIElements.FloatField;
	#endif

	/// <summary>
	/// Field view for the ColorMultiplier.
	/// </summary>
	public class ColorMultiplierFieldView : FieldView<ColorMultiplierValue>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorMultiplierFieldView"/> class.
		/// </summary>
		/// <param name="undoName">Undo name.</param>
		/// <param name="values">Theme values wrapper.</param>
		public ColorMultiplierFieldView(string undoName, Theme.ValuesWrapper<ColorMultiplierValue> values)
			: base(undoName, values)
		{
		}

		/// <inheritdoc/>
		protected override VisualElement CreateView(VariationId variationId, OptionId optionId, ColorMultiplierValue value)
		{
#if UNITY_EDITOR
			var input = new FloatField
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
		public override void UpdateValue(VisualElement view, ColorMultiplierValue value)
		{
#if UNITY_EDITOR
			if (view is FloatField input)
			{
				input.value = value;
			}
#endif
		}
	}
}