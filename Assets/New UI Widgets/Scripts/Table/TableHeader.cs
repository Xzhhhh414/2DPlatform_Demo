namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Pool;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Table Header.
	/// How to change DefaultItem:
	/// var order = tableHeader.GetColumnsOrder();
	/// tableHeader.RestoreColumnsOrder();
	/// listView.DefaultItem = newDefaultItem;
	/// tableHeader.SetColumnsOrder(order);
	/// </summary>
	[RequireComponent(typeof(LayoutGroup))]
	[AddComponentMenu("UI/New UI Widgets/Collections/Table Header")]
	[DisallowMultipleComponent]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/collections/table-header.html")]
	public class TableHeader : UIBehaviourInteractable, IDropSupport<TableHeaderDragCell>, IPointerEnterHandler, IPointerExitHandler, IStylable, ILateUpdatable
	{
		/// <summary>
		/// Cell sizes.
		/// </summary>
		protected readonly struct CellSizes
		{
			/// <summary>
			/// Size of the left cell.
			/// </summary>
			public readonly float Left;

			/// <summary>
			/// Size of the right cell.
			/// </summary>
			public readonly float Right;

			/// <summary>
			/// Size of both cells.
			/// </summary>
			public readonly float Total;

			/// <summary>
			/// Initializes a new instance of the <see cref="CellSizes"/> struct.
			/// </summary>
			/// <param name="left">Size of the left cell.</param>
			/// <param name="right">Size of the right cell.</param>
			public CellSizes(float left, float right)
			{
				Total = left + right;
				Left = left;
				Right = right;
			}
		}

		/// <summary>
		/// ListView instance.
		/// </summary>
		[SerializeField]
		public ListViewBase List;

		/// <summary>
		/// Allow resize.
		/// </summary>
		[SerializeField]
		public bool AllowResize = true;

		/// <summary>
		/// Allow reorder.
		/// </summary>
		[SerializeField]
		public bool AllowReorder = true;

		/// <summary>
		/// Is now processed cell reorder?
		/// </summary>
		[NonSerialized]
		[HideInInspector]
		public bool ProcessCellReorder = false;

		/// <summary>
		/// Update ListView columns width/height on drag.
		/// </summary>
		[SerializeField]
		public bool OnDragUpdate = true;

		/// <summary>
		/// The active region in points from left or right border where resize allowed.
		/// </summary>
		[SerializeField]
		public float ActiveRegion = 5;

		/// <summary>
		/// The current camera. For Screen Space - Overlay let it empty.
		/// </summary>
		[NonSerialized]
		protected Camera CurrentCamera;

		/// <summary>
		/// Cursors.
		/// </summary>
		[SerializeField]
		public Cursors Cursors;

		/// <summary>
		/// The cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D CursorTexture;

		/// <summary>
		/// The cursor hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 CursorHotSpot = new Vector2(16, 16);

		/// <summary>
		/// The cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D AllowDropCursor;

		/// <summary>
		/// The cursor hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 AllowDropCursorHotSpot = new Vector2(4, 2);

		/// <summary>
		/// The cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D DeniedDropCursor;

		/// <summary>
		/// The cursor hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 DeniedDropCursorHotSpot = new Vector2(4, 2);

		/// <summary>
		/// The default cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D DefaultCursorTexture;

		/// <summary>
		/// The default cursor hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 DefaultCursorHotSpot;

		/// <summary>
		/// The drop indicator.
		/// </summary>
		[SerializeField]
		public LayoutDropIndicator DropIndicator;

		/// <summary>
		/// Drag button.
		/// </summary>
		[SerializeField]
		public PointerEventData.InputButton DragButton = PointerEventData.InputButton.Left;

		RectTransform rectTransform;

		/// <summary>
		/// Gets the RectTransform.
		/// </summary>
		/// <value>The RectTransform.</value>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// The cells info.
		/// </summary>
		protected List<TableHeaderCellInfo> CellsInfo = new List<TableHeaderCellInfo>();

		/// <summary>
		/// Header cells.
		/// </summary>
		/// <value>The cells.</value>
		public RectTransform[] Cells
		{
			get
			{
				var result = new RectTransform[CellsInfo.Count];

				for (int i = 0; i < CellsInfo.Count; i++)
				{
					result[i] = CellsInfo[i].Rect;
				}

				return result;
			}
		}

		/// <summary>
		/// Gets a value indicating whether mouse position in active region.
		/// </summary>
		/// <value><c>true</c> if in active region; otherwise, <c>false</c>.</value>
		public bool InActiveRegion => CheckInActiveRegion(CompatibilityInput.MousePosition, CurrentCamera);

		/// <summary>
		/// Can change cursor?
		/// </summary>
		protected bool CanChangeCursor => UICursor.CanSet(this) && CompatibilityInput.MousePresent && IsCursorOver;

		readonly List<TableHeaderCellInfo> cellsInfoOrdered = new List<TableHeaderCellInfo>();

		/// <summary>
		/// CellInfo sorted by position.
		/// </summary>
		protected List<TableHeaderCellInfo> CellsInfoOrdered
		{
			get
			{
				cellsInfoOrdered.Clear();
				cellsInfoOrdered.AddRange(CellsInfo);
				cellsInfoOrdered.Sort(CellComparison);

				return cellsInfoOrdered;
			}
		}

		/// <summary>
		/// Cell comparison.
		/// </summary>
		[DomainReloadExclude]
		protected static Comparison<TableHeaderCellInfo> CellComparison = (x, y) => x.Position.CompareTo(y.Position);

		/// <summary>
		/// Header controls width or height.
		/// </summary>
		protected bool IsWidth => !List.IsHorizontal();

		/// <summary>
		/// Is cursor over gameobject?
		/// </summary>
		protected bool IsCursorOver;

		/// <summary>
		/// Is cursor changed?
		/// </summary>
		protected bool CursorChanged;

		LayoutElement leftTarget;

		LayoutElement rightTarget;

		bool processDrag;

		LayoutGroup layout;

		/// <summary>
		/// Resize info.
		/// </summary>
		[NonSerialized]
		protected CellSizes ResizeInfo;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

#pragma warning disable 0618
			if (DefaultCursorTexture != null)
			{
				UICursor.ObsoleteWarning();
			}
#pragma warning restore 0618

			Refresh();
		}

		/// <inheritdoc/>
		protected override void OnInteractableChange(bool interactableState)
		{
			if (!interactableState && IsCursorOver)
			{
				IsCursorOver = false;

				ResetCursor();
			}
		}

		/// <summary>
		/// Process the initialize potential drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			// do nothing, left for compatibility reason
		}

		/// <summary>
		/// Shows the drop indicator.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void ShowDropIndicator(int index)
		{
			if (DropIndicator != null)
			{
				DropIndicator.Show(index, RectTransform);
			}
		}

		/// <summary>
		/// Hides the drop indicator.
		/// </summary>
		protected virtual void HideDropIndicator()
		{
			if (DropIndicator != null)
			{
				DropIndicator.Hide();
			}
		}

		/// <summary>
		/// Restore initial cells order.
		/// </summary>
		[Obsolete("Renamed to RestoreColumnOrder().")]
		public void RestoreOrder()
		{
			RestoreColumnsOrder();
		}

		/// <summary>
		/// Set cells size.
		/// </summary>
		/// <param name="sizes">Sizes.</param>
		/// <param name="withListView">Set ListView cells sizes.</param>
		public virtual void SetCellsSize(IList<float> sizes, bool withListView = true)
		{
			if (sizes.Count != CellsInfo.Count)
			{
				throw new ArgumentException("Sizes count should match with cells count.");
			}

			if (IsWidth)
			{
				for (int i = 0; i < sizes.Count; i++)
				{
					GetCell(i).SetWidth(sizes[i]);
				}
			}
			else
			{
				for (int i = 0; i < sizes.Count; i++)
				{
					GetCell(i).SetHeight(sizes[i]);
				}
			}

			if (withListView)
			{
				List.SetTableSizes(sizes, false);
			}

			LayoutUtilities.UpdateLayout(layout);

			Resize();
		}

		/// <summary>
		/// Get the columns order.
		/// </summary>
		/// <returns>Columns order.</returns>
		public virtual List<int> GetColumnsOrder()
		{
			var result = new List<int>(CellsInfo.Count);

			GetColumnsOrder(result);

			return result;
		}

		/// <summary>
		/// Get the columns order.
		/// </summary>
		/// <param name="order">Columns order.</param>
		public virtual void GetColumnsOrder(List<int> order)
		{
			order.Clear();

			foreach (var cell in CellsInfo)
			{
				order.Add(cell.Position);
			}
		}

		/// <summary>
		/// Set the columns order.
		/// </summary>
		/// <param name="order">New columns order.</param>
		public virtual void SetColumnsOrder(List<int> order)
		{
			// restore original order
			RestoreColumnsOrder();

			using var _ = ListPool<int>.Get(out var temp);

			// convert list of the new positions to list of the old positions
			for (int i = 0; i < order.Count; i++)
			{
				temp.Add(order.IndexOf(i));
			}

			// restore list components cells order
			List.Init();
			List.ForEachComponent(SetItemColumnOrder);

			for (int new_position = 0; new_position < temp.Count; new_position++)
			{
				var old_position = temp[new_position];
				CellsInfo[old_position].Position = new_position;
				CellsInfo[old_position].Rect.SetAsLastSibling();
			}
		}

		/// <summary>
		/// Set item order.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SetItemColumnOrder(ListViewItem component)
		{
			using var _ = ListPool<Transform>.Get(out var temp);

			var t = component.transform;
			for (int i = 0; i < t.childCount; i++)
			{
				temp.Add(t.GetChild(i));
			}

			for (int i = 0; i < temp.Count; i++)
			{
				temp[i].SetAsLastSibling();
			}
		}

		/// <summary>
		/// Restore column order for the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void RestoreColumnsOrder(ListViewItem component)
		{
			using var _ = ListPool<Transform>.Get(out var temp);

			var t = component.RectTransform;
			for (int i = 0; i < t.childCount; i++)
			{
				temp.Add(t.GetChild(i));
			}

			foreach (var cell in CellsInfo)
			{
				temp[cell.Position].SetAsLastSibling();
			}
		}

		/// <summary>
		/// Restore initial cells order.
		/// </summary>
		public virtual void RestoreColumnsOrder()
		{
			// restore order
			if ((List != null) && (CellsInfo != null) && (CellsInfo.Count > 1))
			{
				// restore list components cells order
				List.Init();
				List.ForEachComponent(RestoreColumnsOrder);

				// restore header cells order
				for (int i = 0; i < CellsInfo.Count; i++)
				{
					CellsInfo[i].Position = i;
					CellsInfo[i].Rect.SetSiblingIndex(i);
				}
			}
		}

		/// <summary>
		/// Re-init this instance in case if you remove or add cells manually.
		/// </summary>
		public virtual void Reinit()
		{
			RestoreColumnsOrder();

			// clear cells list
			CellsInfo.Clear();

			// clear cell settings and events
			for (int i = 0; i < RectTransform.childCount; i++)
			{
				var child = RectTransform.GetChild(i);
				child.gameObject.SetActive(true);

				var cell = Utilities.RequireComponent<TableHeaderDragCell>(child);
				cell.Position = -1;

				var events = Utilities.RequireComponent<TableHeaderCell>(child);
				events.OnInitializePotentialDragEvent.RemoveListener(OnInitializePotentialDrag);
				events.OnBeginDragEvent.RemoveListener(OnBeginDrag);
				events.OnDragEvent.RemoveListener(OnDrag);
				events.OnEndDragEvent.RemoveListener(OnEndDrag);
			}

			Refresh();
		}

		/// <summary>
		/// Init cells.
		/// </summary>
		protected virtual void InitCells()
		{
			for (int i = 0; i < RectTransform.childCount; i++)
			{
				var child = RectTransform.GetChild(i);
				var cell = Utilities.RequireComponent<TableHeaderDragCell>(child);

				if (cell.Position == -1)
				{
					cell.Position = CellsInfo.Count;
					cell.TableHeader = this;

					cell.Cursors = Cursors;
					#pragma warning disable 0618
					cell.AllowDropCursor = AllowDropCursor;
					cell.AllowDropCursorHotSpot = AllowDropCursorHotSpot;
					cell.DeniedDropCursor = DeniedDropCursor;
					cell.DeniedDropCursorHotSpot = DeniedDropCursorHotSpot;
					#pragma warning restore 0618

					var events = Utilities.RequireComponent<TableHeaderCell>(child);
					events.OnInitializePotentialDragEvent.AddListener(OnInitializePotentialDrag);
					events.OnBeginDragEvent.AddListener(OnBeginDrag);
					events.OnDragEvent.AddListener(OnDrag);
					events.OnEndDragEvent.AddListener(OnEndDrag);

					CellsInfo.Add(new TableHeaderCellInfo()
					{
						Rect = child as RectTransform,
						LayoutElement = Utilities.RequireComponent<LayoutElement>(child),
						Position = CellsInfo.Count,
					});
				}
			}
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerEnter event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}

			IsCursorOver = true;
			CurrentCamera = eventData.pressEventCamera;
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerExit event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerExit(PointerEventData eventData)
		{
			IsCursorOver = false;

			ResetCursor();
		}

		/// <summary>
		/// Process application focus event.
		/// </summary>
		/// <param name="hasFocus">Application has focus?</param>
		protected virtual void OnApplicationFocus(bool hasFocus)
		{
			if (!hasFocus)
			{
				IsCursorOver = false;
			}
		}

		/// <summary>
		/// Reset cursor.
		/// </summary>
		protected virtual void ResetCursor()
		{
			CursorChanged = false;
			UICursor.Reset(this);
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			Updater.AddLateUpdate(this);
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			Updater.RemoveLateUpdate(this);
		}

		/// <summary>
		/// Update cursors.
		/// </summary>
		public virtual void RunLateUpdate()
		{
			if (!IsActive())
			{
				return;
			}

			if (!AllowResize)
			{
				return;
			}

			if (!CanChangeCursor)
			{
				return;
			}

			if (processDrag || ProcessCellReorder)
			{
				return;
			}

			if (InActiveRegion)
			{
				UICursor.Set(this, GetCursor());
				CursorChanged = true;
			}
			else if (CursorChanged)
			{
				ResetCursor();
			}
		}

		/// <summary>
		/// Get cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetCursor()
		{
			if (Cursors != null)
			{
				return Cursors.EastWestArrow;
			}

			if (UICursor.Cursors != null)
			{
				return UICursor.Cursors.EastWestArrow;
			}

			return default;
		}

		/// <summary>
		/// Check if event happened in active region.
		/// </summary>
		/// <param name="eventData">Event data</param>
		/// <returns>True if event happened in active region; otherwise false.</returns>
		public virtual bool CheckInActiveRegion(PointerEventData eventData)
		{
			return CheckInActiveRegion(eventData.pressPosition, eventData.pressEventCamera);
		}

		/// <summary>
		/// Point with pivot.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>true if RectTransform contains point otherwise false.</returns>
		protected virtual bool PointWithPivot(ref Vector2 point)
		{
			var rect = RectTransform.rect;
			if (!rect.Contains(point))
			{
				return false;
			}

			if (IsWidth)
			{
				point += new Vector2(rect.width * RectTransform.pivot.x, 0);
			}
			else
			{
				point += new Vector2(0, rect.height * RectTransform.pivot.y);
			}

			return true;
		}

		/// <summary>
		/// Check if cursor in active region to resize.
		/// </summary>
		/// <param name="position">Cursor position.</param>
		/// <param name="camera">Camera.</param>
		/// <returns>true if cursor in active region to resize; otherwise, false.</returns>
		protected virtual bool CheckInActiveRegion(Vector2 position, Camera camera)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, position, camera, out var point);

			if (!PointWithPivot(ref point))
			{
				return false;
			}

			int i = 0;
			foreach (var cell in CellsInfoOrdered)
			{
				if (!cell.ActiveSelf)
				{
					i++;
					continue;
				}

				if (GetTargetIndex(i, -1) != -1)
				{
					if (CheckLeft(cell.Rect, point))
					{
						return true;
					}
				}

				if (GetTargetIndex(i, +1) != -1)
				{
					if (CheckRight(cell.Rect, point))
					{
						return true;
					}
				}

				i++;
			}

			return false;
		}

		/// <summary>
		/// Can drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if drag allowed; otherwise false.</returns>
		protected virtual bool CanDrag(PointerEventData eventData)
		{
			return IsActive() && (eventData.button == DragButton) && AllowResize && !ProcessCellReorder;
		}

		/// <summary>
		/// Process the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (!CanDrag(eventData))
			{
				return;
			}

			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out var point);

			PointWithPivot(ref point);

			var width = IsWidth;
			processDrag = false;

			foreach (var cell in CellsInfoOrdered)
			{
				var i = cell.Position;
				if (!cell.ActiveSelf)
				{
					continue;
				}

				if (CheckLeft(cell.Rect, point))
				{
					var left = GetTargetIndex(i, -1);
					if (left != -1)
					{
						processDrag = true;

						var left_cell = GetCell(left);
						leftTarget = left_cell.LayoutElement;
						rightTarget = cell.LayoutElement;

						ResizeInfo = new CellSizes(
							width ? left_cell.Width : left_cell.Height,
							width ? cell.Width : cell.Height);
						return;
					}
				}

				if (CheckRight(cell.Rect, point))
				{
					var right = GetTargetIndex(i, +1);
					if (right != -1)
					{
						processDrag = true;

						var right_cell = GetCell(right);

						leftTarget = cell.LayoutElement;
						rightTarget = right_cell.LayoutElement;

						ResizeInfo = new CellSizes(
							width ? cell.Width : cell.Height,
							width ? right_cell.Width : right_cell.Height);
						return;
					}
				}
			}
		}

		/// <summary>
		/// Get cell.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <returns>Cell.</returns>
		protected virtual TableHeaderCellInfo GetCell(int position)
		{
			foreach (var cell in CellsInfo)
			{
				if (cell.Position == position)
				{
					return cell;
				}
			}

			return null;
		}

		/// <summary>
		/// Get target index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="direction">Direction.</param>
		/// <returns>Cell index.</returns>
		protected virtual int GetTargetIndex(int index, int direction)
		{
			if ((index + direction) == -1)
			{
				return -1;
			}

			if ((index + direction) >= CellsInfo.Count)
			{
				return -1;
			}

			var cell = GetCell(index + direction);
			if (cell == null)
			{
				return -1;
			}

			var result = cell.ActiveSelf
				? index + direction
				: GetTargetIndex(index + direction, direction);

			return result;
		}

		/// <summary>
		/// Checks if point in the left region.
		/// </summary>
		/// <returns><c>true</c>, if point in the left region, <c>false</c> otherwise.</returns>
		/// <param name="childRectTransform">RectTransform.</param>
		/// <param name="point">Point.</param>
		protected virtual bool CheckLeft(RectTransform childRectTransform, Vector2 point)
		{
			var r = childRectTransform.rect;
			if (IsWidth)
			{
				r.position += new Vector2(childRectTransform.anchoredPosition.x, 0);
				r.width = ActiveRegion;
			}
			else
			{
				r.position += new Vector2(0, childRectTransform.anchoredPosition.y);
				r.height = ActiveRegion;
			}

			return r.Contains(point);
		}

		/// <summary>
		/// Checks if point in the right region.
		/// </summary>
		/// <returns><c>true</c>, if right was checked, <c>false</c> otherwise.</returns>
		/// <param name="childRectTransform">Child RectTransform.</param>
		/// <param name="point">Point.</param>
		protected virtual bool CheckRight(RectTransform childRectTransform, Vector2 point)
		{
			var r = childRectTransform.rect;

			if (IsWidth)
			{
				r.position += new Vector2(childRectTransform.anchoredPosition.x, 0);
				r.position = new Vector2(r.position.x + r.width - ActiveRegion - 1, r.position.y);
				r.width = ActiveRegion + 1;
			}
			else
			{
				r.position += new Vector2(0, childRectTransform.anchoredPosition.y);
				r.position = new Vector2(r.position.x, r.position.y + r.height - ActiveRegion - 1);
				r.height = ActiveRegion + 1;
			}

			return r.Contains(point);
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (!processDrag)
			{
				return;
			}

			ResetCursor();

			ResetChildren();
			if (!OnDragUpdate)
			{
				Resize();
			}

			processDrag = false;
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!processDrag)
			{
				return;
			}

			if (!CanDrag(eventData))
			{
				OnEndDrag(eventData);
				return;
			}

			CursorChanged = true;
			UICursor.Set(this, GetCursor());

			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out var current_point);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out var original_point);

			var delta = current_point - original_point;

			if (IsWidth)
			{
				if (delta.x > 0)
				{
					leftTarget.preferredWidth = Mathf.Min(ResizeInfo.Left + delta.x, ResizeInfo.Total - rightTarget.minWidth);
					rightTarget.preferredWidth = ResizeInfo.Total - leftTarget.preferredWidth;
				}
				else
				{
					rightTarget.preferredWidth = Mathf.Min(ResizeInfo.Right - delta.x, ResizeInfo.Total - leftTarget.minWidth);
					leftTarget.preferredWidth = ResizeInfo.Total - rightTarget.preferredWidth;
				}
			}
			else
			{
				if (delta.y > 0)
				{
					leftTarget.preferredHeight = Mathf.Min(ResizeInfo.Left + delta.y, ResizeInfo.Total - rightTarget.minHeight);
					rightTarget.preferredHeight = ResizeInfo.Total - leftTarget.preferredHeight;
				}
				else
				{
					rightTarget.preferredHeight = Mathf.Min(ResizeInfo.Right - delta.y, ResizeInfo.Total - leftTarget.minHeight);
					leftTarget.preferredHeight = ResizeInfo.Total - rightTarget.preferredHeight;
				}
			}

			LayoutUtilities.UpdateLayout(layout);

			if (OnDragUpdate)
			{
				Resize();
			}
		}

		/// <summary>
		/// Resets the children sizes.
		/// </summary>
		protected virtual void ResetChildren()
		{
			if (IsWidth)
			{
				foreach (var cell in CellsInfo)
				{
					cell.LayoutElement.preferredWidth = cell.Rect.rect.width;
				}
			}
			else
			{
				foreach (var cell in CellsInfo)
				{
					cell.LayoutElement.preferredHeight = cell.Rect.rect.height;
				}
			}
		}

		/// <summary>
		/// Set column width.
		/// </summary>
		/// <param name="column">Column index in the displayed order (not the original one).</param>
		/// <param name="width">Width.</param>
		/// <param name="resizeRightNeighbour">Resize column width on the right or on the left.</param>
		public virtual void SetColumnWidth(int column, float width, bool resizeRightNeighbour = true)
		{
			if (!IsWidth)
			{
				throw new InvalidOperationException("SetColumnWidth() can be used only if ListView.Direction in vertical.");
			}

			if (column == (CellsInfo.Count - 1) && resizeRightNeighbour)
			{
				resizeRightNeighbour = false;
			}
			else if (column == 0 && !resizeRightNeighbour)
			{
				resizeRightNeighbour = true;
			}

			var left = GetCell(resizeRightNeighbour ? column : column - 1);
			var right = GetCell(resizeRightNeighbour ? column + 1 : column);
			var total = left.Width + right.Width;

			if (resizeRightNeighbour)
			{
				left.LayoutElement.preferredWidth = Mathf.Min(width, total - right.LayoutElement.minWidth);
				right.LayoutElement.preferredWidth = total - left.LayoutElement.preferredWidth;
			}
			else
			{
				right.LayoutElement.preferredWidth = Mathf.Min(width, total - left.LayoutElement.minWidth);
				left.LayoutElement.preferredWidth = total - right.LayoutElement.preferredWidth;
			}

			LayoutUtilities.UpdateLayout(layout);

			Resize();
		}

		/// <summary>
		/// Set column height.
		/// </summary>
		/// <param name="column">Column index in the displayed order (not the original one).</param>
		/// <param name="height">Height.</param>
		/// <param name="resizeBottomNeighbour">Resize column height on the top or on the bottom.</param>
		public virtual void SetColumnHeight(int column, float height, bool resizeBottomNeighbour = true)
		{
			if (IsWidth)
			{
				throw new InvalidOperationException("SetColumnHeight() can be used only if ListView.Direction in horizontal.");
			}

			if (column == (CellsInfo.Count - 1) && resizeBottomNeighbour)
			{
				resizeBottomNeighbour = false;
			}
			else if (column == 0 && !resizeBottomNeighbour)
			{
				resizeBottomNeighbour = true;
			}

			var left = GetCell(resizeBottomNeighbour ? column : column - 1);
			var right = GetCell(resizeBottomNeighbour ? column + 1 : column);
			var total = left.Height + right.Height;

			if (resizeBottomNeighbour)
			{
				left.LayoutElement.preferredHeight = Mathf.Min(height, total - right.LayoutElement.minHeight);
				right.LayoutElement.preferredHeight = total - left.LayoutElement.preferredHeight;
			}
			else
			{
				right.LayoutElement.preferredHeight = Mathf.Min(height, total - left.LayoutElement.minHeight);
				left.LayoutElement.preferredHeight = total - right.LayoutElement.preferredHeight;
			}

			LayoutUtilities.UpdateLayout(layout);

			Resize();
		}

		/// <summary>
		/// Resize items in ListView.
		/// </summary>
		public virtual void Resize()
		{
			if (List == null)
			{
				return;
			}

			if (CellsInfo.Count < 2)
			{
				return;
			}

			List.Init();
			List.ForEachComponent(ResizeComponent);
		}

		/// <summary>
		/// Resizes the game object.
		/// </summary>
		/// <param name="go">Game object.</param>
		/// <param name="index">The index.</param>
		protected virtual void ResizeGameObject(GameObject go, int index)
		{
			var cell = GetCell(index);
			if (cell == null)
			{
				return;
			}

			var rt = go.transform as RectTransform;
			var layoutElement = go.GetComponent<LayoutElement>();
			if (IsWidth)
			{
				rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cell.Width);

				if (layoutElement != null)
				{
					layoutElement.minWidth = cell.LayoutElement.minWidth;
					layoutElement.flexibleWidth = cell.LayoutElement.flexibleWidth;
					layoutElement.preferredWidth = cell.Width;
				}
			}
			else
			{
				rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cell.Height);

				if (layoutElement != null)
				{
					layoutElement.minHeight = cell.LayoutElement.minHeight;
					layoutElement.flexibleHeight = cell.LayoutElement.flexibleHeight;
					layoutElement.preferredHeight = cell.Height;
				}
			}
		}

		/// <summary>
		/// Resizes the component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void ResizeComponent(ListViewItem component)
		{
			for (int i = 0; i < component.transform.childCount; i++)
			{
				ResizeGameObject(component.transform.GetChild(i).gameObject, i);
			}
		}

		static void ReorderComponent(ListViewItem component, int prevPosition, int newPosition)
		{
			var target = component.transform.GetChild(prevPosition);
			target.SetSiblingIndex(newPosition);
		}

		/// <summary>
		/// Move column from oldColumnPosition to newColumnPosition.
		/// </summary>
		/// <param name="oldColumnPosition">Old column position.</param>
		/// <param name="newColumnPosition">New column position.</param>
		public void Reorder(int oldColumnPosition, int newColumnPosition)
		{
			if (CellsInfo.Count < 2)
			{
				return;
			}

			if (List != null)
			{
				List.Init();

#if CSHARP_7_3_OR_NEWER
				void Action(ListViewItem component)
#else
				Action<ListViewItem> Action = component =>
#endif
				{
					ReorderComponent(component, CellsInfo[oldColumnPosition].Position, CellsInfo[newColumnPosition].Position);
				}
#if !CSHARP_7_3_OR_NEWER
				;
#endif

				List.ForEachComponent(Action);
			}

			var target = CellsInfo[oldColumnPosition].Rect;
			target.SetSiblingIndex(CellsInfo[newColumnPosition].Position);

			for (int i = 0; i < CellsInfo.Count; i++)
			{
				CellsInfo[i].Position = CellsInfo[i].Rect.GetSiblingIndex();
			}
		}

#region IDropSupport<TableHeaderCell>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Cell.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(TableHeaderDragCell data, PointerEventData eventData)
		{
			if (!IsActive())
			{
				return false;
			}

			if (!AllowReorder)
			{
				return false;
			}

			var target = FindTarget(eventData);
			if ((target == null) || (target.GetInstanceID() == data.GetInstanceID()))
			{
				return false;
			}

			ShowDropIndicator(CellsInfo[target.Position].Position);

			return true;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Cell.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(TableHeaderDragCell data, PointerEventData eventData)
		{
			HideDropIndicator();

			var target = FindTarget(eventData);

			Reorder(data.Position, target.Position);
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Cell.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(TableHeaderDragCell data, PointerEventData eventData)
		{
			HideDropIndicator();
		}

		/// <summary>
		/// Change value position in specified list.
		/// </summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="list">List.</param>
		/// <param name="oldPosition">Old position.</param>
		/// <param name="newPosition">New position.</param>
		protected static void ChangePosition<T>(List<T> list, int oldPosition, int newPosition)
		{
			var item = list[oldPosition];
			list.RemoveAt(oldPosition);
			list.Insert(newPosition, item);
		}

		/// <summary>
		/// Get TableHeaderDragCell in position specified with event data.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>TableHeaderDragCell if found; otherwise null.</returns>
		protected TableHeaderDragCell FindTarget(PointerEventData eventData)
		{
			using var _ = ListPool<RaycastResult>.Get(out var raycasts);

			EventSystem.current.RaycastAll(eventData, raycasts);
			TableHeaderDragCell result = null;

			foreach (var raycast in raycasts)
			{
				if (!raycast.isValid)
				{
					continue;
				}

				var target = raycast.gameObject.GetComponent<TableHeaderDragCell>();
				if ((target != null) && target.transform.IsChildOf(transform))
				{
					result = target;
					break;
				}
			}

			return result;
		}
#endregion

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			CellsInfo.Clear();
			for (int i = 0; i < RectTransform.childCount; i++)
			{
				var child = RectTransform.GetChild(i);
				if (!child.TryGetComponent<TableHeaderCell>(out var events))
				{
					continue;
				}

				events.OnInitializePotentialDragEvent.RemoveListener(OnInitializePotentialDrag);
				events.OnBeginDragEvent.RemoveListener(OnBeginDrag);
				events.OnDragEvent.RemoveListener(OnDrag);
				events.OnEndDragEvent.RemoveListener(OnEndDrag);
			}
		}

		/// <summary>
		/// Refresh header.
		/// </summary>
		public virtual void Refresh()
		{
			if (layout == null)
			{
				TryGetComponent(out layout);
			}

			if (layout != null)
			{
				LayoutUtilities.UpdateLayout(layout);
			}

			InitCells();

			ResetChildren();
			Resize();
		}

		/// <summary>
		/// Recalculate cell sizes to add get required size for the enabled column.
		/// </summary>
		/// <param name="enabledCell">Enabled cell.</param>
		protected virtual void RecalculateSizes(TableHeaderCellInfo enabledCell)
		{
			if (enabledCell.ActiveSelf)
			{
				return;
			}

			var min_flexible = enabledCell.Rect.parent.TryGetComponent<EasyLayoutNS.EasyLayout>(out var _) ? 0f : 1f;
			var required = IsWidth ? enabledCell.Width : enabledCell.Height;

			var total_actual = 0f;
			var total_min = 0f;
			var total_preferred = 0f;
			var total_flexible = 0f;

			foreach (var cell in CellsInfo)
			{
				if (!cell.ActiveSelf)
				{
					continue;
				}

				var le = cell.LayoutElement;

				total_actual += IsWidth ? cell.Width : cell.Height;
				total_min += IsWidth ? le.minWidth : le.minHeight;
				total_preferred += IsWidth ? le.preferredWidth : le.preferredHeight;
				total_flexible += Mathf.Max(IsWidth ? le.flexibleWidth : le.flexibleHeight, min_flexible);
			}

			foreach (var cell in CellsInfo)
			{
				if (!cell.ActiveSelf)
				{
					continue;
				}

				var le = cell.LayoutElement;
				var cell_size = IsWidth ? cell.Width : cell.Height;
				var cell_flexible = Mathf.Max(IsWidth ? le.flexibleWidth : le.flexibleHeight, min_flexible);
				var new_size = cell_size - (required * (cell_flexible / total_flexible));

				if (IsWidth)
				{
					le.preferredWidth = new_size;
				}
				else
				{
					le.preferredHeight = new_size;
				}
			}
		}

		/// <summary>
		/// Change column state.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="active">If set state to active.</param>
		public virtual void ColumnToggle(int index, bool active)
		{
			var target = CellsInfo[index];

			if (active)
			{
				RecalculateSizes(target);
			}

			target.Rect.gameObject.SetActive(active);

#if CSHARP_7_3_OR_NEWER
			void Action(ListViewItem component)
#else
			Action<ListViewItem> Action = component =>
#endif
			{
				var child = component.transform.GetChild(target.Position);
				child.gameObject.SetActive(active);
			}
#if !CSHARP_7_3_OR_NEWER
			;
#endif

			List.ForEachComponent(Action);

			List.ComponentsColoring();

			Refresh();
		}

		/// <summary>
		/// Disable column.
		/// </summary>
		/// <param name="index">Index.</param>
		public virtual void ColumnDisable(int index) => ColumnToggle(index, false);

		/// <summary>
		/// Enable column.
		/// </summary>
		/// <param name="index">Index.</param>
		public virtual void ColumnEnable(int index) => ColumnToggle(index, true);

		/// <summary>
		/// Add header cell.
		/// </summary>
		/// <param name="cell">Cell.</param>
		public void AddCell(GameObject cell)
		{
			cell.transform.SetParent(transform, false);
			cell.SetActive(true);

			Refresh();
		}

		/// <summary>
		/// Get cell index for the specified gameobject.
		/// </summary>
		/// <param name="go">Gameobject.</param>
		/// <returns>Index of the cell.</returns>
		protected virtual int GetCellIndex(GameObject go)
		{
			for (int i = 0; i < CellsInfo.Count; i++)
			{
				if (CellsInfo[i].Rect.gameObject == go)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Remove header cell.
		/// </summary>
		/// <param name="cell">Cell.</param>
		/// <param name="parent">Parent.</param>
		public virtual void RemoveCell(GameObject cell, RectTransform parent = null)
		{
			var index = GetCellIndex(cell);
			if (index == -1)
			{
				Debug.LogWarning("Cell not in header", cell);
				return;
			}

			cell.SetActive(false);
			cell.transform.SetParent(parent, false);
			if (parent == null)
			{
				Destroy(cell);
			}

			// remove from cells
			var removed = CellsInfo[index];
			CellsInfo.RemoveAt(index);

			// remove events
			var events = Utilities.RequireComponent<TableHeaderCell>(cell);
			events.OnInitializePotentialDragEvent.RemoveListener(OnInitializePotentialDrag);
			events.OnBeginDragEvent.RemoveListener(OnBeginDrag);
			events.OnDragEvent.RemoveListener(OnDrag);
			events.OnEndDragEvent.RemoveListener(OnEndDrag);

			// decrease position for cells where >cell_position
			for (int i = 0; i < CellsInfo.Count; i++)
			{
				if (CellsInfo[i].Position > removed.Position)
				{
					CellsInfo[i].Position -= 1;
				}
			}

			// update list layout
			LayoutUtilities.UpdateLayout(layout);
			Resize();
		}

		#if UNITY_EDITOR

		/// <summary>
		/// Reset this instance.
		/// </summary>
		protected override void Reset()
		{
			CursorsDPISelector.Require(this);

			base.Reset();
		}

		#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			// apply style for header
			style.Table.Border.ApplyTo(gameObject);

			for (int i = 0; i < RectTransform.childCount; i++)
			{
				var cell = RectTransform.GetChild(i);
				if (cell.TryGetComponent<StyleSupportHeaderCell>(out var style_support))
				{
					style_support.SetStyle(style);
				}
				else
				{
					style.Table.Background.ApplyTo(cell);
					style.Table.HeaderText.ApplyTo(cell.Find("Text"));
				}
			}

			// apply style to list
			style.Table.Border.ApplyTo(List.Container);

			#if CSHARP_7_3_OR_NEWER
			void Action(ListViewItem component)
			#else
			Action<ListViewItem> Action = component =>
			#endif
			{
				style.Table.Border.ApplyTo(component);

				for (int i = 0; i < component.RectTransform.childCount; i++)
				{
					var child = component.RectTransform.GetChild(i);
					style.Table.Background.ApplyTo(child.gameObject);
				}
			}
			#if !CSHARP_7_3_OR_NEWER
			;
			#endif

			List.ForEachComponent(Action);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			// got style from header
			style.Table.Border.GetFrom(gameObject);

			for (int i = 0; i < RectTransform.childCount; i++)
			{
				var cell = RectTransform.GetChild(i);
				if (cell.TryGetComponent<StyleSupportHeaderCell>(out var style_support))
				{
					style_support.GetStyle(style);
				}
				else
				{
					style.Table.Background.GetFrom(cell);
					style.Table.HeaderText.GetFrom(cell.Find("Text"));
				}
			}

			// got style from list
			style.Table.Border.GetFrom(List.Container);

			return true;
		}

		#endregion
	}
}