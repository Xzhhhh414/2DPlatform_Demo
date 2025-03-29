namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base generic class for the ScrollBlock.
	/// </summary>
	/// <typeparam name="TItemView">Type of the component.</typeparam>
	public class ScrollBlockCustom<TItemView> : ScrollBlockBase
		where TItemView : ScrollBlockItem
	{
		/// <summary>
		/// Item changed delegate.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item instance.</param>
		public delegate void ItemChanged(int index, TItemView item);

		/// <summary>
		/// Do nothing.
		/// </summary>
		static void DoNothing(int index, TItemView item)
		{
		}

		[SerializeField]
		TItemView defaultItem;

		/// <summary>
		/// DefaultItem.
		/// </summary>
		public TItemView DefaultItem
		{
			get
			{
				return defaultItem;
			}

			set
			{
				if (defaultItem != value)
				{
					defaultItem = value;

					ComponentsPool.Template = defaultItem;

					UpdateLayout();
					Resize();
				}
			}
		}

		/// <summary>
		/// Used instances of the DefaultItem.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<TItemView> Components = new List<TItemView>();

		/// <summary>
		/// Unused instances of the DefaultItem.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<TItemView> ComponentsCache = new List<TItemView>();

		ListComponentPool<TItemView> componentsPool;

		/// <summary>
		/// Components pool.
		/// </summary>
		protected ListComponentPool<TItemView> ComponentsPool
		{
			get
			{
				if ((componentsPool == null) || (componentsPool.Template == null))
				{
					componentsPool = new ListComponentPool<TItemView>(DefaultItem, Components, ComponentsCache, RectTransform);
				}

				return componentsPool;
			}
		}

		/// <summary>
		/// Instances pool.
		/// </summary>
		public ListComponentPool<TItemView> Pool => ComponentsPool;

		/// <inheritdoc/>
		public override int Count => ComponentsPool.Count;

		/// <inheritdoc/>
		protected override int ComponentsBaseIndex => BasePosition switch
		{
			Position.Start => 0,
			Position.Center => ComponentsPool.Count / 2,
			_ => 0,
		};

		/// <summary>
		/// Item changed event.
		/// </summary>
		public event ItemChanged OnItemChanged = DoNothing;

		/// <inheritdoc/>
		protected override void UpdateLayout()
		{
			TryGetComponent<EasyLayoutNS.EasyLayout>(out var layout);
			Layout = new EasyLayoutBridge(layout, DefaultItem.transform as RectTransform, false, false)
			{
				IsHorizontal = IsHorizontal,
			};

			DefaultItemSize = Layout.GetItemSize();
			DefaultItem.gameObject.SetActive(false);

			LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform.parent as RectTransform);
		}

		/// <inheritdoc/>
		public override void SetDefaultItemSize(Vector2 size)
		{
			Init();

			DefaultItemSize = size;

			foreach (var instance in ComponentsPool.GetEnumerator(PoolEnumeratorMode.All))
			{
				instance.TryGetComponent<RectTransform>(out var rt);
				rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
			}

			Resize();
		}

		/// <inheritdoc/>
		public override void SetVisibleItems(int count)
		{
			Init();

			var size = IsHorizontal ? RectTransform.rect.width : RectTransform.rect.height;
			var spacing = Layout.GetSpacing() * (count - 1);
			var item_size = (size - spacing) / count;

			var new_size = IsHorizontal
				? new Vector2(item_size, DefaultItemSize.y)
				: new Vector2(DefaultItemSize.x, item_size);

			SetDefaultItemSize(new_size);
		}

		/// <inheritdoc/>
		protected override void Resize()
		{
			var max = CalculateMax();

			if (max == ComponentsPool.Count)
			{
				return;
			}

			ComponentsPool.Require(max);

			var median = ComponentsBaseIndex;
			for (int i = 0; i < ComponentsPool.Count; i++)
			{
				ComponentsPool[i].Index = i - median;
				ComponentsPool[i].Owner = this;
				ComponentsPool[i].transform.SetAsLastSibling();
			}

			UpdateView();
			AlignComponents();
		}

		/// <summary>
		/// Set text of the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SetComponentText(TItemView component)
		{
			component.Text.Value = Value(component.Index);
			component.DataChanged();
			OnItemChanged(component.Index, component);
		}

		/// <summary>
		/// Set text.
		/// </summary>
		[Obsolete("Replaced with UpdateView().")]
		public void SetText()
		{
			UpdateView();
		}

		/// <inheritdoc/>
		public override void UpdateView()
		{
			Init();

			foreach (var component in ComponentsPool)
			{
				SetComponentText(component);
			}
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			componentsPool = null;
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			if (DefaultItem.Text != null)
			{
				style.ScrollBlock.Text.ApplyTo(DefaultItem.Text.GameObject);

				if (IsInited)
				{
					for (int i = 0; i < Components.Count; i++)
					{
						style.ScrollBlock.Text.ApplyTo(Components[i].Text.GameObject);
					}

					for (int i = 0; i < ComponentsCache.Count; i++)
					{
						style.ScrollBlock.Text.ApplyTo(ComponentsCache[i].Text.GameObject);
					}
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			if (DefaultItem.Text != null)
			{
				style.ScrollBlock.Text.GetFrom(DefaultItem.Text.GameObject);
			}

			return true;
		}
		#endregion
	}
}