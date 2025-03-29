namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// ColorPicker block.
	/// </summary>
	public abstract class ColorPickerBlock : MonoBehaviourInitiable, UIThemes.ITargetOwner
	{
		/// <summary>
		/// Owner.
		/// </summary>
		[NonSerialized]
		[HideInInspector]
		public ColorPicker Owner;

		/// <summary>
		/// Is instance interactable?
		/// </summary>
		/// <returns>true if instance interactable; otherwise false</returns>
		public virtual bool IsInteractable() => (Owner == null) || Owner.IsInteractable();

		/// <summary>
		/// Set target owner.
		/// </summary>
		public virtual void SetTargetOwner()
		{
		}
	}
}