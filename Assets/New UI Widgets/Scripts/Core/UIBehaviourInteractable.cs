namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;

	/// <summary>
	/// Base class for the interactable widgets.
	/// </summary>
	public abstract class UIBehaviourInteractable : UIBehaviourConditional
	{
		/// <summary>
		/// Interactable.
		/// </summary>
		[SerializeField]
		protected bool interactable = true;

		/// <summary>
		/// Is widget interactable?
		/// </summary>
		[DataBindField]
		public virtual bool Interactable
		{
			get => interactable;

			set
			{
				if (interactable != value)
				{
					interactable = value;
					InteractableChanged();
				}
			}
		}

		/// <summary>
		/// If the canvas groups allow interaction.
		/// </summary>
		protected bool GroupsAllowInteraction = true;

		/// <summary>
		/// The CanvasGroup cache.
		/// </summary>
		protected List<CanvasGroup> CanvasGroupCache = new List<CanvasGroup>();

		/// <summary>
		/// Process the CanvasGroupChanged event.
		/// </summary>
		protected override void OnCanvasGroupChanged()
		{
			base.OnCanvasGroupChanged();

			var groupAllowInteraction = true;
			var t = transform;
			while (t != null)
			{
				t.GetComponents(CanvasGroupCache);
				var shouldBreak = false;
				foreach (var canvas_group in CanvasGroupCache)
				{
					if (!canvas_group.interactable)
					{
						groupAllowInteraction = false;
						shouldBreak = true;
					}

					shouldBreak |= canvas_group.ignoreParentGroups;
				}

				if (shouldBreak)
				{
					break;
				}

				t = t.parent;
			}

			if (groupAllowInteraction != GroupsAllowInteraction)
			{
				GroupsAllowInteraction = groupAllowInteraction;
				InteractableChanged();
			}
		}

		/// <summary>
		/// Returns true if the GameObject and the Component are active.
		/// </summary>
		/// <returns>true if the GameObject and the Component are active; otherwise false.</returns>
		public override bool IsActive() => base.IsActive() && IsInteractable() && IsInited;

		/// <inheritdoc/>
		protected override void OnEnable()
		{
			base.OnEnable();

			InteractableChanged();
		}

		/// <summary>
		/// Is instance interactable?
		/// </summary>
		/// <returns>true if instance interactable; otherwise false</returns>
		public virtual bool IsInteractable() => GroupsAllowInteraction && Interactable;

		/// <summary>
		/// Process interactable change.
		/// </summary>
		protected void InteractableChanged() => OnInteractableChange(IsInteractable());

		/// <summary>
		/// Process interactable change.
		/// </summary>
		/// <param name="interactableState">Current interactable state.</param>
		protected virtual void OnInteractableChange(bool interactableState)
		{
		}
	}
}