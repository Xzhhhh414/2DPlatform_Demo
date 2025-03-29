#if UNITY_EDITOR && UNITY_2020_3_OR_NEWER
namespace UIThemes.Editor
{
	using UnityEngine;
	using UnityEngine.UIElements;

	/// <summary>
	/// Drag&amp;Drop for EditBlockView.
	/// </summary>
	public class DragDrop : PointerManipulator
	{
		/// <summary>
		/// USS classes.
		/// </summary>
		public class UssClasses
		{
			/// <summary>
			/// Handle.
			/// </summary>
			public readonly string Handle;

			/// <summary>
			/// Handle selected.
			/// </summary>
			public readonly string HandleSelected;

			/// <summary>
			/// Drop indicator.
			/// </summary>
			public readonly string DropIndicator;

			/// <summary>
			/// Item.
			/// </summary>
			public readonly string Item;

			/// <summary>
			/// Initializes a new instance of the <see cref="UssClasses"/> class.
			/// </summary>
			/// <param name="handle">Handle class.</param>
			/// <param name="handleSelected">Selected handle class.</param>
			/// <param name="dropIndicator">Drop indicator class.</param>
			/// <param name="item">Item class.</param>
			public UssClasses(string handle, string handleSelected, string dropIndicator, string item)
			{
				Handle = handle;
				HandleSelected = handleSelected;
				DropIndicator = dropIndicator;
				Item = item;
			}
		}

		readonly struct DropIndices
		{
			public readonly int ElementIndex;

			public readonly int ItemIndex;

			public readonly bool Valid => ElementIndex != -1 && ItemIndex != -1;

			public DropIndices(int elementIndex, int itemIndex)
			{
				ElementIndex = elementIndex;
				ItemIndex = itemIndex;
			}

			public static DropIndices Invalid => new DropIndices(-1, -1);
		}

		readonly VisualElement indicator;

		VisualElement currentHandle;

		/// <summary>
		/// Current handle.
		/// </summary>
		protected VisualElement CurrentHandle
		{
			get => currentHandle;

			set
			{
				currentHandle?.RemoveFromClassList(ussClasses.HandleSelected);

				currentHandle = value;

				currentHandle?.AddToClassList(ussClasses.HandleSelected);
			}
		}

		readonly UssClasses ussClasses;

		readonly bool horizontal = true;

		DropIndices oldIndices;

		DropIndices newIndices;

		bool enabled;

		/// <summary>
		/// Drop event.
		/// </summary>
		/// <param name="oldIndex">Old index.</param>
		/// <param name="newIndex">New index.</param>
		public delegate void DropEvent(int oldIndex, int newIndex);

		/// <summary>
		/// On drop event.
		/// </summary>
		public event DropEvent OnDrop;

		/// <summary>
		/// Initializes a new instance of the <see cref="DragDrop"/> class.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="ussClasses">Uss classes.</param>
		/// <param name="horizontal">true if items in one row; otherwise in one column.</param>
		public DragDrop(VisualElement target, UssClasses ussClasses, bool horizontal)
		{
			this.target = target;
			this.ussClasses = ussClasses;
			this.horizontal = horizontal;

			indicator = new VisualElement();
			indicator.AddToClassList(ussClasses.DropIndicator);
		}

		/// <summary>
		/// Register callbacks.
		/// </summary>
		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<PointerDownEvent>(PointerDownHandler, TrickleDown.NoTrickleDown);
			target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
			target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
			target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
		}

		/// <summary>
		/// Unregister callbacks.
		/// </summary>
		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
			target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
			target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
			target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
		}

		bool HandleDown(PointerDownEvent evt)
		{
			if (evt.modifiers != EventModifiers.None)
			{
				return false;
			}

			if (evt.button != 0)
			{
				return false;
			}

			if (!(evt.target is VisualElement b))
			{
				return false;
			}

			return b.ClassListContains(ussClasses.Handle);
		}

		void PointerDownHandler(PointerDownEvent evt)
		{
			if (!HandleDown(evt))
			{
				return;
			}

			CurrentHandle = evt.target as VisualElement;
			oldIndices = FindIndices(evt, false);
			target.CapturePointer(evt.pointerId);
			enabled = true;
		}

		DropIndices FindIndices(IPointerEvent evt, bool strict)
		{
			for (var i = 0; i < target.childCount; i++)
			{
				var element = target.ElementAt(i);
				if (!element.ClassListContains(ussClasses.Item))
				{
					continue;
				}

				var local_position = element.WorldToLocal(evt.position);
				if (element.ContainsPoint(local_position))
				{
					var index = (int)element.userData;
					var delta = 0;
					if (strict)
					{
						if (horizontal)
						{
							delta = (local_position.x > (element.layout.width / 2f)) ? 1 : 0;
						}
						else
						{
							delta = (local_position.y > (element.layout.height / 2f)) ? 1 : 0;
						}
					}

					return new DropIndices(i + delta, index + delta);
				}
			}

			return DropIndices.Invalid;
		}

		void ShowIndicator(int index)
		{
			if (indicator.parent == null)
			{
				target.Add(indicator);
			}

			var end = index == target.childCount;
			var element = end ? target.ElementAt(index - 1) : target.ElementAt(index);
			Vector2 pos = Vector2.zero;
			if (horizontal)
			{
				pos.x = element.worldBound.x - target.worldBound.x - 1;
				if (end)
				{
					pos.x += element.layout.width;
				}
			}
			else
			{
				pos.y = element.worldBound.y - target.worldBound.y - 1;
				if (end)
				{
					pos.y += element.layout.height;
				}
			}

			indicator.transform.position = pos;
		}

		void HideIndicator()
		{
			indicator.parent?.Remove(indicator);
		}

		void PointerMoveHandler(PointerMoveEvent evt)
		{
			if (!enabled || !target.HasPointerCapture(evt.pointerId))
			{
				return;
			}

			newIndices = FindIndices(evt, true);
			if (newIndices.Valid)
			{
				ShowIndicator(newIndices.ElementIndex);
			}
			else
			{
				HideIndicator();
			}
		}

		void PointerUpHandler(PointerUpEvent evt)
		{
			if (!enabled || !target.HasPointerCapture(evt.pointerId))
			{
				return;
			}

			CurrentHandle = null;
			target.ReleasePointer(evt.pointerId);
			HideIndicator();
		}

		void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
		{
			if (!enabled)
			{
				return;
			}

			if (oldIndices.Valid && newIndices.Valid)
			{
				OnDrop?.Invoke(oldIndices.ItemIndex, newIndices.ItemIndex);
			}

			enabled = false;
		}
	}
}
#endif