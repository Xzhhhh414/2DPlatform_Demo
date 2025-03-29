namespace UIThemes
{
	using UnityEngine.UIElements;
	#if UNITY_2022_1_OR_NEWER
	using FloatField = UnityEngine.UIElements.FloatField;
	#elif UNITY_EDITOR
	using FloatField = UnityEditor.UIElements.FloatField;
	#endif

	/// <summary>
	/// Field view for the FontSize.
	/// </summary>
	public class FontSizeFieldView : FieldView<FontSizeValue>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FontSizeFieldView"/> class.
		/// </summary>
		/// <param name="undoName">Undo name.</param>
		/// <param name="values">Theme values wrapper.</param>
		public FontSizeFieldView(string undoName, Theme.ValuesWrapper<FontSizeValue> values)
			: base(undoName, values)
		{
		}

		/// <inheritdoc/>
		protected override VisualElement CreateView(VariationId variationId, OptionId optionId, FontSizeValue value)
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
		public override void UpdateValue(VisualElement view, FontSizeValue value)
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