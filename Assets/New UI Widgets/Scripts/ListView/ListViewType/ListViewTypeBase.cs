namespace UIWidgets
{
	using System;
	using EasyLayoutNS;
	using UIWidgets.Attributes;
	using UIWidgets.Extensions;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <content>
	/// Base class for the custom ListViews.
	/// </content>
	public partial class ListViewCustom<TItemView, TItem> : ListViewCustom<TItem>, IUpdatable, ILateUpdatable, IListViewCallbacks<TItemView>
		where TItemView : ListViewItem
	{
		/// <summary>
		/// Base class for the ListView renderer.
		/// </summary>
		protected abstract class ListViewTypeBase
		{
			/// <summary>
			/// Visibility data.
			/// </summary>
			protected struct Visibility : IEquatable<Visibility>
			{
				/// <summary>
				/// Default instance.
				/// </summary>
				public static Visibility Default => new Visibility(0, -1);

				/// <summary>
				/// First visible index.
				/// </summary>
				public int FirstVisible;

				/// <summary>
				/// Count of the visible items.
				/// </summary>
				public int Items;

				/// <summary>
				/// Last visible index.
				/// </summary>
				public readonly int LastVisible => FirstVisible + Items;

				/// <summary>
				/// Initializes a new instance of the <see cref="Visibility"/> struct.
				/// </summary>
				/// <param name="firstVisible">First visible index.</param>
				/// <param name="items">Count of the visible items.</param>
				public Visibility(int firstVisible, int items)
				{
					FirstVisible = firstVisible;
					Items = items;
				}

				/// <summary>
				/// Determines whether the specified object is equal to the current object.
				/// </summary>
				/// <param name="obj">The object to compare with the current object.</param>
				/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
				public readonly override bool Equals(object obj) => (obj is Visibility visibility) && Equals(visibility);

				/// <summary>
				/// Determines whether the specified object is equal to the current object.
				/// </summary>
				/// <param name="other">The object to compare with the current object.</param>
				/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
				public readonly bool Equals(Visibility other) => FirstVisible == other.FirstVisible && Items == other.Items;

				/// <summary>
				/// Hash function.
				/// </summary>
				/// <returns>A hash code for the current object.</returns>
				public readonly override int GetHashCode() => FirstVisible ^ Items;

				/// <summary>
				/// Compare specified visibility data.
				/// </summary>
				/// <param name="a">First data.</param>
				/// <param name="b">Second data.</param>
				/// <returns>true if the data are equal; otherwise, false.</returns>
				public static bool operator ==(Visibility a, Visibility b) => a.Equals(b);

				/// <summary>
				/// Compare specified visibility data.
				/// </summary>
				/// <param name="a">First data.</param>
				/// <param name="b">Second data.</param>
				/// <returns>true if the data not equal; otherwise, false.</returns>
				public static bool operator !=(Visibility a, Visibility b) => !a.Equals(b);

				/// <summary>
				/// Converts this instance to string.
				/// </summary>
				/// <returns>String representation.</returns>
				public override string ToString()
				{
					return string.Format("{0}..{1}", FirstVisible.ToString(), LastVisible.ToString());
				}
			}

			/// <summary>
			/// Minimal count of the visible items.
			/// </summary>
			protected const int MinVisibleItems = 2;

			/// <summary>
			/// Maximal count of the visible items.
			/// </summary>
			public int MaxVisibleItems
			{
				get;
				protected set;
			}

			/// <summary>
			/// Visibility info.
			/// </summary>
			protected Visibility Visible;

			/// <summary>
			/// First visible index.
			/// </summary>
			public int FirstVisibleIndex => Visible.FirstVisible;

			/// <summary>
			/// Owner.
			/// </summary>
			protected ListViewCustom<TItemView, TItem> Owner;

			/// <summary>
			/// Default inertia state.
			/// </summary>
			protected bool DefaultInertia;

			/// <summary>
			/// Is inited?
			/// </summary>
			protected bool isInited;

			/// <summary>
			/// Is looped list allowed?
			/// </summary>
			/// <returns>True if looped list allowed; otherwise false.</returns>
			public virtual bool IsTileView => false;

			/// <summary>
			/// Allow owner to set ContentSizeFitter settings.
			/// </summary>
			public virtual bool AllowSetContentSizeFitter => true;

			/// <summary>
			/// Allow owner to control Container.RectTransform.
			/// </summary>
			public virtual bool AllowControlRectTransform => true;

			/// <summary>
			/// Allow looped ListView.
			/// </summary>
			public abstract bool AllowLoopedList
			{
				get;
			}

			/// <summary>
			/// Can scroll?
			/// </summary>
			public abstract bool CanScroll
			{
				get;
			}

			/// <summary>
			/// Support items of variable size.
			/// </summary>
			public virtual bool SupportVariableSize => false;

			/// <summary>
			/// List can be looped and items is enough to make looped list.
			/// </summary>
			public bool LoopedListAvailable => Owner.LoopedList && Owner.Virtualization && IsVirtualizationSupported() && AllowLoopedList;

			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewTypeBase"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			protected ListViewTypeBase(ListViewCustom<TItemView, TItem> owner)
			{
				Owner = owner;
			}

			/// <summary>
			/// Init this instance.
			/// </summary>
			public void Init()
			{
				if (isInited)
				{
					return;
				}

				isInited = true;
				InitOnce();
				EnableLoopedList();
			}

			/// <summary>
			/// Init this instance only once.
			/// </summary>
			protected virtual void InitOnce()
			{
				EnableLoopedList();
			}

			/// <summary>
			/// Enabled looped list.
			/// </summary>
			protected virtual void EnableLoopedList()
			{
				if (!Owner.LoopedList)
				{
					return;
				}

				if (Owner.LayoutBridge.GetFullMargin() != 0f)
				{
					Debug.LogWarning("Non-zero margin does not have meaning for the LoopedList. It is changed to 0.", Owner);
					Owner.LayoutBridge.ResetMargin();
				}

				Owner.ScrollRect.movementType = ScrollRect.MovementType.Unrestricted;
			}

			/// <summary>
			/// Disable this instance.
			/// </summary>
			public abstract void Disable();

			/// <summary>
			/// Reset position.
			/// </summary>
			public abstract void ResetPosition();

			/// <summary>
			/// Validate position.
			/// </summary>
			protected abstract void ValidatePosition();

			/// <summary>
			/// Validate position.
			/// </summary>
			/// <param name="position">Position.</param>
			/// <returns>Validated position.</returns>
			public abstract float ValidatePosition(float position);

			/// <summary>
			/// Toggle scroll to nearest item center.
			/// </summary>
			/// <param name="state">State.</param>
			public abstract void ToggleScrollToItemCenter(bool state);

			/// <summary>
			/// Scroll to the nearest item center.
			/// </summary>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0301:Closure Allocation Source", Justification = "Required")]
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0302:Display class allocation to capture closure", Justification = "Required")]
			public void ScrollToItemCenter()
			{
				var center_position = GetCenterPosition();
				var index_at_center = ScrollPosition2Index(center_position);
				var index = VisibleIndex2ItemIndex(index_at_center);
				var middle_position = GetItemPositionMiddle(index);
				var valid_position = ValidateScrollPosition(middle_position);

				if (!Mathf.Approximately(GetPosition(), valid_position))
				{
					Owner.ScrollToPositionAnimated(valid_position, Owner.ScrollInertia, Owner.ScrollUnscaledTime, () => Owner.ScrollCenter = ScrollCenterState.Disable);
				}
			}

			/// <summary>
			/// Validate scroll position.
			/// </summary>
			/// <param name="position">Position.</param>
			/// <returns>Validated position.</returns>
			public abstract float ValidateScrollPosition(float position);

			/// <summary>
			/// Validate position.
			/// </summary>
			/// <param name="position">Position.</param>
			/// <returns>Validated position.</returns>
			public Vector2 ValidatePosition(Vector2 position)
			{
				if (Owner.IsHorizontal())
				{
					position.x = ValidatePosition(position.x);
				}
				else
				{
					position.y = ValidatePosition(position.y);
				}

				return position;
			}

			/// <summary>
			/// Sets the scroll value.
			/// </summary>
			/// <param name="value">Value.</param>
			/// <param name="updateView">Call ScrollUpdate() if position changed.</param>
			public abstract void SetPosition(float value, bool updateView = true);

			/// <summary>
			/// Sets the scroll value.
			/// </summary>
			/// <param name="newPosition">Value.</param>
			/// <param name="updateView">Update view if position changed.</param>
			public abstract void SetPosition(Vector2 newPosition, bool updateView = true);

			/// <summary>
			/// Gets the scroll value in ListView direction.
			/// </summary>
			/// <returns>The scroll value.</returns>
			public abstract Vector2 GetPositionVector();

			/// <summary>
			/// Gets the scroll value in ListView direction.
			/// </summary>
			/// <returns>The scroll value.</returns>
			public abstract float GetPosition();

			/// <summary>
			/// Gets the center position in ListView direction.
			/// </summary>
			/// <returns>Center position.</returns>
			public abstract float GetCenterPosition();

			/// <summary>
			/// Get scroll position for the specified index.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <returns>Scroll position</returns>
			public abstract Vector2 GetPosition(int index);

			/// <summary>
			/// Get item index at scroll position.
			/// </summary>
			/// <param name="position">Scroll position.</param>
			/// <returns>Index.</returns>
			public abstract int ScrollPosition2Index(float position);

			/// <summary>
			/// Is visible item with specified index.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <param name="minVisiblePart">The minimal visible part of the item to consider item visible.</param>
			/// <returns>true if item visible; false otherwise.</returns>
			public abstract bool IsVisible(int index, float minVisiblePart);

			/// <summary>
			/// Update view.
			/// </summary>
			public virtual void UpdateView()
			{
				if (IsVirtualizationSupported())
				{
					ValidatePosition();
				}

				Owner.UpdateView(forced: false, isNewData: false);
			}

			/// <summary>
			/// Updates the layout bridge.
			/// </summary>
			/// <param name="recalculate">Recalculate layout.</param>
			public abstract void UpdateLayout(bool recalculate = true);

			/// <summary>
			/// Get the top filler size to center the items.
			/// </summary>
			/// <returns>Size.</returns>
			public abstract float CenteredFillerSize();

			/// <summary>
			/// Determines whether is required center the list items.
			/// </summary>
			/// <returns><c>true</c> if required center the list items; otherwise, <c>false</c>.</returns>
			public abstract bool IsRequiredCenterTheItems();

			/// <summary>
			/// Calculates the maximum count of the visible items.
			/// </summary>
			public abstract void CalculateMaxVisibleItems();

			/// <summary>
			/// Compare LayoutElements by layoutPriority.
			/// </summary>
			[DomainReloadExclude]
			protected static Comparison<ILayoutElement> LayoutElementsComparison = (x, y) => -x.layoutPriority.CompareTo(y.layoutPriority);

			/// <summary>
			/// Calculates the size of the item.
			/// </summary>
			/// <param name="reset">Reset item size.</param>
			/// <returns>Item size.</returns>
			[Obsolete("Renamed to CalculateDefaultInstanceSize()")]
			public virtual Vector2 GetItemSize(bool reset = false)
			{
				return CalculateDefaultInstanceSize(Owner.DefaultInstanceSize, reset);
			}

			/// <summary>
			/// Calculates the default size of the instance.
			/// </summary>
			/// <param name="currentSize">Current size.</param>
			/// <param name="reset">Reset size.</param>
			/// <returns>Item size.</returns>
			public virtual Vector2 CalculateDefaultInstanceSize(Vector2 currentSize, bool reset = false)
			{
				Owner.DefaultItem.gameObject.SetActive(true);

				var rt = Owner.DefaultItem.RectTransform;
				var layout_size = new LayoutElementData(Owner.DefaultItem);
				var size = currentSize;

				if ((size.x == 0f) || reset)
				{
					var preff_width = SupportVariableSize ? layout_size.PreferredWidth : 0f;
					size.x = Mathf.Max(Mathf.Max(preff_width, rt.rect.width), 1f);
					if (float.IsNaN(size.x))
					{
						size.x = 1f;
					}
				}

				if ((size.y == 0f) || reset)
				{
					var preff_height = SupportVariableSize ? layout_size.PreferredHeight : 0f;
					size.y = Mathf.Max(Mathf.Max(preff_height, rt.rect.height), 1f);
					if (float.IsNaN(size.y))
					{
						size.y = 1f;
					}
				}

				Owner.DefaultItem.gameObject.SetActive(false);

				return size;
			}

			/// <summary>
			/// Calculates the size of the top filler.
			/// </summary>
			/// <returns>The top filler size.</returns>
			public abstract float TopFillerSize();

			/// <summary>
			/// Calculates the size of the bottom filler.
			/// </summary>
			/// <returns>The bottom filler size.</returns>
			public abstract float BottomFillerSize();

			/// <summary>
			/// Gets the first index of the visible.
			/// </summary>
			/// <returns>The first visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public abstract int GetFirstVisibleIndex(bool strict = false);

			/// <summary>
			/// Gets the last visible index.
			/// </summary>
			/// <returns>The last visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public abstract int GetLastVisibleIndex(bool strict = false);

			/// <summary>
			/// Gets the position of the start border of the item.
			/// </summary>
			/// <returns>The position.</returns>
			/// <param name="index">Index.</param>
			public abstract float GetItemPosition(int index);

			/// <summary>
			/// Gets the position of the end border of the item.
			/// </summary>
			/// <returns>The position.</returns>
			/// <param name="index">Index.</param>
			public abstract float GetItemPositionBorderEnd(int index);

			/// <summary>
			/// Gets the position to display item at the center of the ScrollRect viewport.
			/// </summary>
			/// <returns>The position.</returns>
			/// <param name="index">Index.</param>
			public abstract float GetItemPositionMiddle(int index);

			/// <summary>
			/// Gets the position to display item at the bottom of the ScrollRect viewport.
			/// </summary>
			/// <returns>The position.</returns>
			/// <param name="index">Index.</param>
			public abstract float GetItemPositionBottom(int index);

			/// <summary>
			/// Calculate and sets the sizes of the items.
			/// </summary>
			/// <param name="items">Items.</param>
			/// <param name="forceUpdate">If set to <c>true</c> force update.</param>
			[Obsolete("Renamed to CalculateDefaultInstanceSize()")]
			public virtual void CalculateItemsSizes(ObservableList<TItem> items, bool forceUpdate = true)
			{
				CalculateInstancesSizes(items, forceUpdate);
			}

			/// <summary>
			/// Calculate sizes of the instances of the specified items.
			/// </summary>
			/// <param name="items">Items.</param>
			/// <param name="forceUpdate">If set to <c>true</c> force update.</param>
			public virtual void CalculateInstancesSizes(ObservableList<TItem> items, bool forceUpdate = true)
			{
			}

			/// <summary>
			/// Calculates the size of the instance for the specified item.
			/// </summary>
			/// <returns>The component size.</returns>
			/// <param name="item">Item.</param>
			/// <param name="template">Template.</param>
			protected virtual Vector2 CalculateInstanceSize(TItem item, Template template)
			{
				Owner.SetData(template.Template, item);

				LayoutRebuilder.ForceRebuildLayoutImmediate(Owner.Container);
				var size = ValidateSize(template.Template.RectTransform.rect.size);
				template.Template.MovedToCache();

				return size;
			}

			/// <summary>
			/// Size value is acceptable?
			/// </summary>
			/// <param name="value">Value.</param>
			/// <returns>true if value is acceptable; false otherwise.</returns>
			protected bool AcceptableSize(float value)
			{
				if (float.IsNaN(value))
				{
					return false;
				}

				if (float.IsInfinity(value))
				{
					return false;
				}

				return value >= 1f;
			}

			/// <summary>
			/// Validate size.
			/// </summary>
			/// <param name="size">Size.</param>
			/// <returns>Correct size.</returns>
			protected Vector2 ValidateSize(Vector2 size)
			{
				if (!AcceptableSize(size.x))
				{
					size.x = 1f;
				}

				if (!AcceptableSize(size.y))
				{
					size.y = 1f;
				}

				return size;
			}

			/// <summary>
			/// Get the size of the instance for the item with the specified index.
			/// </summary>
			/// <returns>The instance size.</returns>
			/// <param name="index">Index.</param>
			[Obsolete("Renamed to GetInstanceFullSize().")]
			public virtual Vector2 GetItemFullSize(int index)
			{
				return GetInstanceFullSize(index);
			}

			/// <summary>
			/// Get the size of the instance for the item with the specified index.
			/// </summary>
			/// <param name="index">Item index.</param>
			/// <returns>The instance size.</returns>
			public virtual Vector2 GetInstanceFullSize(int index)
			{
				if (!Owner.IsValid(index))
				{
					return Vector2.zero;
				}

				var template = Owner.ComponentsPool.GetTemplate(index);
				if (Owner.OverriddenTemplateSizes.TryGetValue(template.TemplateID, out var size))
				{
					return size;
				}

				return template.DefaultSize;
			}

			/// <summary>
			/// Set the size of the instance for the item with the specified index.
			/// </summary>
			/// <param name="index">Item index.</param>
			/// <param name="size">Size.</param>
			public virtual void SetInstanceFullSize(int index, Vector2 size)
			{
				throw new NotImplementedException("Not supported. Change ListType to one with variable size.");
			}

			/// <summary>
			/// Reset the size to the default for the item with the specified index.
			/// </summary>
			/// <param name="index">Item index.</param>
			public virtual void ResetInstanceFullSize(int index)
			{
			}

			/// <summary>
			/// Update size of the instance.
			/// </summary>
			/// <param name="instance">Instance.</param>
			protected virtual void UpdateInstanceSize(TItemView instance)
			{
			}

			/// <summary>
			/// Update sizes of the instances.
			/// </summary>
			public virtual void UpdateInstancesSizes()
			{
				foreach (var instance in Owner.Instances)
				{
					UpdateInstanceSize(instance);
				}
			}

			/// <summary>
			/// Gets the size of the instance for the item with the specified index.
			/// </summary>
			/// <returns>The instance size.</returns>
			/// <param name="index">Item index.</param>
			protected float GetInstanceSize(int index)
			{
				var size = GetInstanceFullSize(index);
				return Owner.IsHorizontal() ? size.x : size.y;
			}

			/// <summary>
			/// Adds the callback.
			/// </summary>
			/// <param name="item">Item.</param>
			public virtual void AddCallback(TItemView item)
			{
			}

			/// <summary>
			/// Removes the callback.
			/// </summary>
			/// <param name="item">Item.</param>
			public virtual void RemoveCallback(TItemView item)
			{
			}

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			/// <param name="point">Point.</param>
			public int GetNearestIndex(Vector2 point)
			{
				return GetNearestIndex(point, NearestType.Auto);
			}

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			/// <param name="point">Point.</param>
			/// <param name="type">Preferable nearest index.</param>
			public abstract int GetNearestIndex(Vector2 point, NearestType type);

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			public abstract int GetNearestItemIndex();

			/// <summary>
			/// Get the size of the ListView.
			/// </summary>
			/// <returns>The size.</returns>
			public abstract float ListSize();

			/// <summary>
			/// Get block index by item index.
			/// </summary>
			/// <param name="index">Item index.</param>
			/// <returns>Block index.</returns>
			protected virtual int GetBlockIndex(int index) => index;

			/// <summary>
			/// Gets the items per block count.
			/// </summary>
			/// <returns>The items per block.</returns>
			public virtual int GetItemsPerBlock() => 1;

			/// <summary>
			/// Determines is virtualization supported.
			/// </summary>
			/// <returns><c>true</c> if virtualization supported; otherwise, <c>false</c>.</returns>
			public virtual bool IsVirtualizationSupported() => IsVirtualizationPossible();

			/// <summary>
			/// Determines whether this instance can be virtualized.
			/// </summary>
			/// <returns><c>true</c> if this instance can virtualized; otherwise, <c>false</c>.</returns>
			public virtual bool IsVirtualizationPossible()
			{
				LayoutGroup layout = null;
				if (Owner.Container != null)
				{
					if (Owner.layout != null)
					{
						layout = Owner.layout;
					}
					else
					{
						Owner.Container.TryGetComponent(out layout);
					}
				}

				var valid_layout = false;
				if (layout != null)
				{
					var is_easy_layout = layout is EasyLayout;
					valid_layout = Owner.RequireEasyLayout
						? is_easy_layout
						: (is_easy_layout || (layout is HorizontalOrVerticalLayoutGroup));
				}

				var has_scroll_rect = Owner.ScrollRect != null;

				return has_scroll_rect && valid_layout;
			}

			/// <summary>
			/// Process the item move event.
			/// </summary>
			/// <param name="eventData">Event data.</param>
			/// <param name="item">Item.</param>
			/// <returns>true if was moved to the next item; otherwise false.</returns>
			public virtual bool OnItemMove(AxisEventData eventData, ListViewItem item)
			{
				var step = 0;
				switch (eventData.moveDir)
				{
					case MoveDirection.Left:
						if (Owner.IsHorizontal())
						{
							step = -1;
						}

						break;
					case MoveDirection.Right:
						if (Owner.IsHorizontal())
						{
							step = 1;
						}

						break;
					case MoveDirection.Up:
						if (!Owner.IsHorizontal())
						{
							step = -1;
						}

						break;
					case MoveDirection.Down:
						if (!Owner.IsHorizontal())
						{
							step = 1;
						}

						break;
				}

				if (step == 0)
				{
					return false;
				}

				var target = GetSelectableComponentIndex(item.Index, step, Owner.LoopedNavigation);

				return Owner.Navigate(eventData, target, target > item.Index);
			}

			/// <summary>
			/// Get index of the next selectable component.
			/// </summary>
			/// <param name="currentIndex">Current index.</param>
			/// <param name="step">Step.</param>
			/// <param name="looped">Looped.</param>
			/// <returns>Index of the component to select.</returns>
			protected int GetSelectableComponentIndex(int currentIndex, int step, bool looped = true)
			{
				if (!looped)
				{
					return currentIndex + step;
				}

				var index = VisibleIndex2ItemIndex(currentIndex + step);

				while (!Owner.CanSelect(index) && Owner.IsValid(index))
				{
					index = VisibleIndex2ItemIndex(index + step);
				}

				return index;
			}

			/// <summary>
			/// Validates the content size and item size.
			/// </summary>
			public virtual void ValidateContentSize()
			{
			}

			/// <summary>
			/// Get viewport width.
			/// </summary>
			/// <returns>Width.</returns>
			protected virtual float ViewportWidth()
			{
				return Owner.Viewport.Size.x + Owner.GetItemSpacingX();
			}

			/// <summary>
			/// Get viewport height.
			/// </summary>
			/// <returns>Height.</returns>
			protected virtual float ViewportHeight()
			{
				return Owner.Viewport.Size.y + Owner.GetItemSpacingY();
			}

			/// <summary>
			/// Get visible items count starting from specified index.
			/// </summary>
			/// <param name="start_index">Start index.</param>
			/// <returns>Visible items count.</returns>
			protected virtual int GetVisibleItems(int start_index)
			{
				return Mathf.Min(MaxVisibleItems, Owner.DataSource.Count);
			}

			/// <summary>
			/// Get visibility data.
			/// </summary>
			/// <returns>Visibility data.</returns>
			protected virtual Visibility VisibilityData()
			{
				var visible = default(Visibility);

				if (LoopedListAvailable)
				{
					visible.FirstVisible = GetFirstVisibleIndex();
					visible.Items = GetVisibleItems(visible.FirstVisible);
				}
				else if (Owner.Virtualization && IsVirtualizationSupported() && (Owner.DataSource.Count > 0))
				{
					visible.FirstVisible = GetFirstVisibleIndex();
					visible.Items = GetVisibleItems(visible.FirstVisible);

					if ((visible.FirstVisible + visible.Items) > Owner.DataSource.Count)
					{
						visible.Items = Owner.DataSource.Count - visible.FirstVisible;
						if (visible.Items < GetItemsPerBlock())
						{
							visible.Items = Mathf.Min(Owner.DataSource.Count, visible.Items + GetItemsPerBlock());
							visible.FirstVisible = Owner.DataSource.Count - visible.Items;
						}
					}
				}
				else
				{
					visible.FirstVisible = 0;
					visible.Items = Owner.DataSource.Count;
				}

				return visible;
			}

			/// <summary>
			/// Reset displayed indices.
			/// </summary>
			public virtual void ResetDisplayedIndices()
			{
				Visible = Visibility.Default;

				Owner.DisplayedIndices.Clear();
			}

			/// <summary>
			/// Update displayed indices.
			/// </summary>
			/// <returns>true if displayed indices changed; otherwise false.</returns>
			public virtual bool UpdateDisplayedIndices()
			{
				var new_visible = VisibilityData();
				if (new_visible == Visible)
				{
					return false;
				}

				Visible = new_visible;

				Owner.DisplayedIndices.Clear();

				for (int i = Visible.FirstVisible; i < Visible.LastVisible; i++)
				{
					Owner.DisplayedIndices.Add(VisibleIndex2ItemIndex(i));
				}

				return true;
			}

			/// <summary>
			/// Convert visible index to item index.
			/// </summary>
			/// <returns>The item index.</returns>
			/// <param name="index">Visible index.</param>
			public virtual int VisibleIndex2ItemIndex(int index)
			{
				if (index < 0)
				{
					index += Owner.DataSource.Count * Mathf.CeilToInt((float)-index / Owner.DataSource.Count);
				}

				return index % Owner.DataSource.Count;
			}

			/// <summary>
			/// Process ListView direction changed.
			/// </summary>
			public virtual void DirectionChanged()
			{
				if (Owner.Layout != null)
				{
					Owner.Layout.MainAxis = !Owner.IsHorizontal() ? Axis.Horizontal : Axis.Vertical;
				}
			}

			/// <summary>
			/// Get maximum width and maximum height of the items.
			/// </summary>
			/// <returns>Max size.</returns>
			public virtual Vector2 GetMaxSize() => Owner.DefaultInstanceSize;

			/// <summary>
			/// Get debug information.
			/// </summary>
			/// <param name="builder">String builder.</param>
			public virtual void GetDebugInfo(System.Text.StringBuilder builder)
			{
				builder.AppendValue("IsTileView: ", IsTileView);
				builder.AppendValue("Max Visible Items: ", MaxVisibleItems);

				builder.AppendLine("Visibility");

				builder.AppendValue("Visibility.FirstVisible: ", Visible.FirstVisible);
				builder.AppendValue("Visibility.LastVisible: ", Visible.LastVisible);
				builder.AppendValue("Visibility.Items: ", Visible.Items);
				builder.AppendValue("First Visible Index: ", GetFirstVisibleIndex());
				builder.AppendValue("Last Visible Index: ", GetLastVisibleIndex());
				builder.AppendValue("List Size: ", ListSize());
				builder.AppendValue("Items Per Block: ", GetItemsPerBlock());
				builder.AppendValue("Top Filler: ", TopFillerSize());
				builder.AppendValue("Bottom Filler: ", BottomFillerSize());

				builder.AppendLine("Items");
				for (int index = 0; index < Owner.DataSource.Count; index++)
				{
					builder.Append("\t");
					builder.Append(index);
					builder.Append(". size: ");
					builder.Append(GetInstanceFullSize(index).ToString());
					builder.Append("; position: ");
					builder.Append(GetItemPosition(index));
					builder.Append("; block: ");
					builder.Append(GetBlockIndex(index));
					builder.AppendLine();
				}
			}
		}
	}
}