namespace UIThemes
{
	using UnityEngine.UIElements;

	/// <summary>
	/// Base class for the field view.
	/// </summary>
	public abstract class FieldViewBase
	{
		/// <summary>
		/// Undo name.
		/// </summary>
		protected string UndoName;

		/// <summary>
		/// Theme values wrapper.
		/// </summary>
		public Theme.IValuesWrapper Wrapper
		{
			get;
			protected set;
		}

		/// <summary>
		/// Value was changed.
		/// Used to prevent unnecessary updates.
		/// </summary>
		public bool ValueChanged
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FieldViewBase"/> class.
		/// </summary>
		/// <param name="undoName">Undo name.</param>
		public FieldViewBase(string undoName)
		{
			UndoName = undoName;
		}

		/// <summary>
		/// Create visual element to edit value.
		/// </summary>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="optionId">Option ID.</param>
		/// <returns>Visual Element.</returns>
		public abstract VisualElement Create(VariationId variationId, OptionId optionId);

		/// <summary>
		/// Update value of the visual element.
		/// </summary>
		/// <param name="view">View.</param>
		/// <param name="variationId">Variation ID.</param>
		/// <param name="optionId">Option ID.</param>
		public abstract void UpdateValue(VisualElement view, VariationId variationId, OptionId optionId);
	}
}