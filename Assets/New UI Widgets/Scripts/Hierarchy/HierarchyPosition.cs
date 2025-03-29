namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Hierarchy position.
	/// </summary>
	public struct HierarchyPosition
	{
		/// <summary>
		/// Target.
		/// </summary>
		public readonly Transform Target;

		/// <summary>
		/// Original parent of the target.
		/// </summary>
		public readonly Transform OriginalParent;

		/// <summary>
		/// New parent of the target.
		/// </summary>
		public readonly Transform NewParent;

		/// <summary>
		/// Original sibling index of the target.
		/// </summary>
		public readonly int SiblingIndex;

		/// <summary>
		/// World position stays.
		/// </summary>
		public readonly bool WorldPositionStays;

		/// <summary>
		/// Original anchored position.
		/// </summary>
		public readonly Vector2 AnchoredPosition;

		/// <summary>
		/// Original size delta.
		/// </summary>
		public readonly Vector2 SizeDelta;

		/// <summary>
		/// Is parent changed?
		/// </summary>
		public bool Changed
		{
			get;
			private set;
		}

		bool parentDestroyed;

		private HierarchyPosition(Transform target, Transform parent, Transform newParent, int siblingIndex, bool worldPositionStays)
		{
			Target = target;
			if (Target is RectTransform rt)
			{
				AnchoredPosition = rt.anchoredPosition;
				SizeDelta = rt.sizeDelta;
			}
			else
			{
				AnchoredPosition = Vector2.zero;
				SizeDelta = Vector2.zero;
			}

			OriginalParent = parent;
			NewParent = newParent;

			SiblingIndex = siblingIndex;
			WorldPositionStays = worldPositionStays;
			Changed = true;
			parentDestroyed = false;

			SetNewParent();
		}

		readonly void SetNewParent()
		{
			Target.SetParent(NewParent, WorldPositionStays);
			Target.SetAsLastSibling();
		}

		/// <summary>
		/// Refresh.
		/// Used when canvas size changed.
		/// </summary>
		public void Refresh()
		{
			if (!CanRestore())
			{
				return;
			}

			Restore();
			SetNewParent();

			Changed = true;
		}

		readonly bool CanRestore() => Changed && (OriginalParent != null) && (Target != null);

		/// <summary>
		/// Restore parent with sibling index.
		/// </summary>
		public void Restore()
		{
			if (!CanRestore())
			{
				return;
			}

			if (!parentDestroyed)
			{
				Target.SetParent(OriginalParent);
				Target.SetSiblingIndex(SiblingIndex);

				if (Target is RectTransform rt)
				{
					rt.anchoredPosition = AnchoredPosition;
					rt.sizeDelta = SizeDelta;
				}
			}

			Changed = false;
		}

		/// <summary>
		/// Mark parent as destroyed.
		/// </summary>
		public void ParentDestroyed() => parentDestroyed = true;

		/// <summary>
		/// Set parent and return object to restore original position in hierarchy.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="newParent">New parent/</param>
		/// <param name="worldPositionStays">World position stays.</param>
		/// <returns>Object to restore original position in hierarchy.</returns>
		public static HierarchyPosition SetParent(Transform target, Transform newParent, bool worldPositionStays = true)
		{
			return new HierarchyPosition(target, target.parent, newParent, target.GetSiblingIndex(), worldPositionStays);
		}
	}
}