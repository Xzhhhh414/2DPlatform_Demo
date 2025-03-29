namespace UIThemes
{
	using UnityEngine;
	using UnityEngine.UIElements;

	/// <summary>
	/// Field view for the Unity object.
	/// </summary>
	/// <typeparam name="TValue">Type of value.</typeparam>
	public class ObjectFieldView<TValue> : FieldView<TValue>
		where TValue : Object
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectFieldView{TValue}"/> class.
		/// </summary>
		/// <param name="undoName">Undo name.</param>
		/// <param name="values">Theme values wrapper.</param>
		public ObjectFieldView(string undoName, Theme.ValuesWrapper<TValue> values)
			: base(undoName, values)
		{
		}

		/// <inheritdoc/>
		protected override VisualElement CreateView(VariationId variationId, OptionId optionId, TValue value)
		{
			#if UNITY_EDITOR
			var input = new UnityEditor.UIElements.ObjectField
			{
				value = value,
				objectType = typeof(TValue),
			};
			input.RegisterValueChangedCallback(x => Save(variationId, optionId, x.newValue as TValue));

			return input;
			#else
			return null;
			#endif
		}

		/// <inheritdoc/>
		public override void UpdateValue(VisualElement view, TValue value)
		{
			#if UNITY_EDITOR
			if (view is UnityEditor.UIElements.ObjectField input)
			{
				input.value = value;
				input.objectType = typeof(TValue);
			}
			#endif
		}
	}
}