namespace UIThemes
{
	using UnityEngine.UIElements;

	/// <summary>
	/// Base class for the field view to edit value of the specified type.
	/// </summary>
	/// <typeparam name="TValue">Type of value.</typeparam>
	public abstract class FieldView<TValue> : FieldViewBase
	{
		/// <summary>
		/// Theme values wrapper.
		/// </summary>
		public readonly Theme.ValuesWrapper<TValue> Values;

		/// <summary>
		/// Initializes a new instance of the <see cref="FieldView{TValue}"/> class.
		/// </summary>
		/// <param name="undoName">Undo name.</param>
		/// <param name="values">Theme values wrapper.</param>
		public FieldView(string undoName, Theme.ValuesWrapper<TValue> values)
			: base(undoName)
		{
			Values = values;
			Wrapper = values;
		}

		/// <inheritdoc/>
		public override VisualElement Create(VariationId variationId, OptionId optionId)
		{
			return CreateView(variationId, optionId, Values.Get(variationId, optionId));
		}

		/// <inheritdoc/>
		public override void UpdateValue(VisualElement view, VariationId variationId, OptionId optionId)
		{
			UpdateValue(view, Values.Get(variationId, optionId));
		}

		/// <summary>
		/// Update value.
		/// </summary>
		/// <param name="view">View.</param>
		/// <param name="value">Value.</param>
		public abstract void UpdateValue(VisualElement view, TValue value);

		/// <summary>
		/// Create visual element to edit value.
		/// </summary>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="optionId">Option ID.</param>
		/// <param name="value">Value.</param>
		/// <returns>Visual element.</returns>
		protected abstract VisualElement CreateView(VariationId variationId, OptionId optionId, TValue value);

		/// <summary>
		/// Set a new value and save theme.
		/// Editor only.
		/// </summary>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="optionId">Option ID.</param>
		/// <param name="value">Value.</param>
		protected virtual void Save(VariationId variationId, OptionId optionId, TValue value)
		{
			ValueChanged = true;

			#if UNITY_EDITOR
			UnityEditor.Undo.RecordObject(Values.Theme, UndoName);
			Values.Set(variationId, optionId, value);
			UnityEditor.EditorUtility.SetDirty(Values.Theme);
			#else
			Values.Set(variationId, optionId, value);
			#endif

			ValueChanged = false;
		}
	}
}