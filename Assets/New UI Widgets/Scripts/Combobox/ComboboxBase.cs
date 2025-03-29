namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Base class for the Combobox.
	/// </summary>
	public abstract class ComboboxBase : UIBehaviourInteractable, ISubmitHandler, IStylable
	{
		/// <summary>
		/// Process the submit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public abstract void OnSubmit(BaseEventData eventData);

		/// <summary>
		/// Set widget properties from specified style.
		/// </summary>
		/// <param name="style">Style data.</param>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		public abstract bool SetStyle(Style style);

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="style">Style data.</param>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		public abstract bool GetStyle(Style style);
	}
}