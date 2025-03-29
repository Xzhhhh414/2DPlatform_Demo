namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Extensions;
	using UIWidgets.Pool;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewBase.
	/// You can use it for creating custom ListViews.
	/// </summary>
	public abstract partial class ListViewBase : UIBehaviourInteractable,
			ISelectHandler, IDeselectHandler,
			ISubmitHandler, ICancelHandler,
			IStylable, IUpgradeable
	{
		/// <summary>
		/// Component template.
		/// </summary>
		/// <typeparam name="TItemView">Component type.</typeparam>
		[Serializable]
		public class ListViewItemTemplate<TItemView>
			where TItemView : ListViewItem
		{
			[SerializeField]
			private TItemView template;

			/// <summary>
			/// Template.
			/// </summary>
			public TItemView Template
			{
				get => template;

				protected set => template = value;
			}

			[SerializeField]
			private InstanceID templateID;

			/// <summary>
			/// Template ID.
			/// </summary>
			public InstanceID TemplateID
			{
				get => templateID;

				protected set => templateID = value;
			}

			[SerializeField]
			List<TItemView> instancesList = new List<TItemView>();

			InstancesCollection<TItemView> instances;

			/// <summary>
			/// Instances.
			/// </summary>
			public InstancesCollection<TItemView> Instances
			{
				get
				{
					instances ??= new InstancesCollection<TItemView>(instancesList);

					return instances;
				}
			}

			[SerializeField]
			private List<TItemView> requested = new List<TItemView>();

			/// <summary>
			/// Requested components.
			/// </summary>
			public List<TItemView> Requested
			{
				get => requested;

				protected set => requested = value;
			}

			[SerializeField]
			private List<TItemView> cache = new List<TItemView>();

			/// <summary>
			/// Cache.
			/// </summary>
			public List<TItemView> Cache
			{
				get => cache;

				protected set => cache = value;
			}

			/// <summary>
			/// Required instances.
			/// </summary>
			[NonSerialized]
			public int RequiredInstances;

			/// <summary>
			/// Callbacks.
			/// </summary>
			protected Dictionary<InstanceID, IListViewCallbacks<TItemView>> Callbacks = new Dictionary<InstanceID, IListViewCallbacks<TItemView>>();

			private Vector2 defaultSize;

			/// <summary>
			/// Template size.
			/// </summary>
			public Vector2 DefaultSize
			{
				get => defaultSize;

				set => defaultSize = value;
			}

			/// <summary>
			/// Check if instance is null.
			/// </summary>
			[DomainReloadExclude]
			static readonly Predicate<TItemView> InstanceIsNull = x => x == null;

			/// <summary>
			/// Compare LayoutElements by layoutPriority.
			/// </summary>
			[DomainReloadExclude]
			protected static Comparison<ILayoutElement> LayoutElementsComparison = (x, y) => -x.layoutPriority.CompareTo(y.layoutPriority);

			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewItemTemplate{TItemView}"/> class.
			/// </summary>
			protected ListViewItemTemplate()
			{
			}

			/// <summary>
			/// Create template.
			/// </summary>
			/// <typeparam name="TTemplate">Type of template.</typeparam>
			/// <param name="template">template.</param>
			/// <returns>Template.</returns>
			public static TTemplate Create<TTemplate>(TItemView template)
				where TTemplate : ListViewItemTemplate<TItemView>, new()
			{
				var result = new TTemplate()
				{
					Template = template,
					TemplateID = new InstanceID(template),
				};

				result.Template.Init();
				result.defaultSize = result.GetSize();

				return result;
			}

			/// <summary>
			/// Restore to valid state.
			/// </summary>
			public void Restore()
			{
				RemoveNull();
				RemoveDuplicates();
			}

			/// <summary>
			/// Remove duplicates.
			/// </summary>
			protected void RemoveDuplicates()
			{
				instancesList.RemoveUnityDuplicates();
				Requested.RemoveUnityDuplicates();
				Cache.RemoveUnityDuplicates();
			}

			/// <summary>
			/// Remove null instances.
			/// </summary>
			protected void RemoveNull()
			{
				instancesList.RemoveAll(InstanceIsNull);
				Requested.RemoveAll(InstanceIsNull);
				Cache.RemoveAll(InstanceIsNull);
			}

			/// <summary>
			/// Returns an enumerator that iterates through the <see cref="ListViewItemTemplate{TItemView}" />.
			/// </summary>
			/// <param name="ownerID">ID.</param>
			/// <param name="mode">Mode.</param>
			/// <returns>A <see cref="PoolEnumerator{TItemView}" /> for the <see cref="ListViewItemTemplate{TItemView}" />.</returns>
			public PoolEnumerator<TItemView> GetEnumerator(InstanceID ownerID, PoolEnumeratorMode mode)
			{
				return new PoolEnumerator<TItemView>(mode, Template, Instances.Of(ownerID), Cache);
			}

			/// <summary>
			/// Update Id.
			/// </summary>
			public void UpdateId()
			{
				TemplateID = new InstanceID(Template);
			}

			/// <summary>
			/// Add callbacks.
			/// </summary>
			/// <typeparam name="T">Type of callbacks.</typeparam>
			/// <param name="ownerID">Owner ID.</param>
			/// <param name="callbacks">Callbacks.</param>
			public void AddCallbacks<T>(InstanceID ownerID, T callbacks)
				where T : IListViewCallbacks<TItemView>
			{
				if (Callbacks.ContainsKey(ownerID))
				{
					return;
				}

				Callbacks[ownerID] = callbacks;

				foreach (var instance in Cache)
				{
					callbacks.ComponentCreated(instance);
				}

				foreach (var instance in Instances.Of(ownerID))
				{
					callbacks.ComponentCreated(instance);
					callbacks.ComponentActivated(instance);
				}
			}

			/// <summary>
			/// Get size.
			/// </summary>
			/// <returns>Size.</returns>
			protected virtual Vector2 GetSize()
			{
				Template.gameObject.SetActive(true);

				var rt = Template.transform as RectTransform;

				using var _ = ListPool<ILayoutElement>.Get(out var layout_elements);
				Template.GetComponents(layout_elements);
				layout_elements.Sort(LayoutElementsComparison);

				var size = Vector2.zero;

				size.x = Mathf.Max(Mathf.Max(PreferredWidth(layout_elements), rt.rect.width), 1f);
				if (float.IsNaN(size.x))
				{
					size.x = 1f;
				}

				size.y = Mathf.Max(Mathf.Max(PreferredHeight(layout_elements), rt.rect.height), 1f);
				if (float.IsNaN(size.y))
				{
					size.y = 1f;
				}

				Template.gameObject.SetActive(false);

				return size;
			}

			static float PreferredHeight(List<ILayoutElement> elems)
			{
				if (elems.Count == 0)
				{
					return 0f;
				}

				var priority = elems[0].layoutPriority;
				var result = -1f;
				for (int i = 0; i < elems.Count; i++)
				{
					if ((result > -1f) && (elems[i].layoutPriority < priority))
					{
						break;
					}

					result = Mathf.Max(Mathf.Max(result, elems[i].minHeight), elems[i].preferredHeight);
					priority = elems[i].layoutPriority;
				}

				return result;
			}

			static float PreferredWidth(List<ILayoutElement> elems)
			{
				if (elems.Count == 0)
				{
					return 0f;
				}

				var priority = elems[0].layoutPriority;
				var result = -1f;
				for (int i = 0; i < elems.Count; i++)
				{
					if ((result > -1f) && (elems[i].layoutPriority < priority))
					{
						break;
					}

					result = Mathf.Max(Mathf.Max(result, elems[i].minWidth), elems[i].preferredWidth);
					priority = elems[i].layoutPriority;
				}

				return result;
			}

			/// <summary>
			/// Process locale changes.
			/// </summary>
			/// <param name="ownerID">Owner ID.</param>
			public virtual void LocaleChanged(InstanceID ownerID)
			{
				var instances = Instances.Of(ownerID);
				for (int i = 0; i < instances.Count; i++)
				{
					instances[i].LocaleChanged();
				}
			}

			/// <summary>
			/// Disable instance.
			/// </summary>
			/// <param name="instance">Instance.</param>
			protected virtual void Disable(TItemView instance)
			{
				if (instance == null)
				{
					return;
				}

				var owner = instance.Owner;
				var has_owner = owner != null;
				if (has_owner)
				{
					var key = new InstanceID(owner);
					if (Callbacks.ContainsKey(key))
					{
						Callbacks[key].ComponentCached(instance);
					}
				}

				instance.MovedToCache();
				instance.Index = -1;
				instance.gameObject.SetActive(false);

				if (has_owner)
				{
					instance.RectTransform.SetParent(owner.DisabledContainer, false);
					instance.Owner = null;
				}

				Cache.Add(instance);
			}

			/// <summary>
			/// Request instances.
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="ownerID">Owner ID.</param>
			public virtual void RequestInstances(ListViewBase owner, InstanceID ownerID)
			{
				var instances = Instances.Of(ownerID);
				foreach (var instance in instances)
				{
					instance.MovedToCache();
					instance.Index = -1;
				}

				Requested.AddRange(instances);
				Instances.Remove(ownerID, false);

				if (Requested.Count == RequiredInstances)
				{
					return;
				}

				for (var i = Requested.Count; i < RequiredInstances; i++)
				{
					Requested.Add(Create(owner, ownerID));
				}

				// try to disable components except dragged one
				var index = Requested.Count - 1;
				while ((Requested.Count > RequiredInstances) && (index >= 0))
				{
					var component = Requested[index];
					#pragma warning disable 0618
					var disable_recycling = component.DisableRecycling || component.IsDragged;
					#pragma warning restore 0618
					if (!disable_recycling)
					{
						Disable(component);
						Requested.RemoveAt(index);
					}

					index--;
				}

				// if too much dragged components then disable any components
				index = Requested.Count - 1;
				while ((Requested.Count > RequiredInstances) && (index >= 0))
				{
					var component = Requested[index];
					Disable(component);
					Requested.RemoveAt(index);

					index--;
				}

				Requested.Sort(ComponentsComparison);
			}

			[DomainReloadExclude]
			static readonly Comparison<TItemView> ComponentsComparison = (x, y) =>
			{
				if (x.Index == y.Index)
				{
					return 0;
				}

				if (x.Index < 0)
				{
					return 1;
				}

				if (y.Index < 0)
				{
					return -1;
				}

				return x.Index.CompareTo(y.Index);
			};

			/// <summary>
			/// Create instance.
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="ownerID">Owner ID.</param>
			/// <returns>Instance.</returns>
			protected virtual TItemView Create(ListViewBase owner, InstanceID ownerID)
			{
				TItemView instance;

				if (Cache.Count > 0)
				{
					instance = Cache[Cache.Count - 1];
					instance.RectTransform.SetParent(owner.Container, false);
					instance.Owner = owner;

					Cache.RemoveAt(Cache.Count - 1);
				}
				else
				{
					instance = Compatibility.Instantiate(Template, owner.Container);
					Utilities.FixInstantiated(Template.RectTransform, instance.RectTransform);
					instance.Owner = owner;

					instance.Index = -2;
					instance.transform.SetAsLastSibling();
					instance.Init();

					foreach (var c in Callbacks)
					{
						c.Value.ComponentCreated(instance);
					}
				}

				instance.gameObject.SetActive(true);
				Callbacks[ownerID].ComponentActivated(instance);

				return instance;
			}

			/// <summary>
			/// Move component with the specified index to the Requested.
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="ownerID">Owner ID.</param>
			/// <param name="index">Index.</param>
			/// <param name="instanceCompare">Instance comparer.</param>
			/// <returns>true if component was moved; otherwise false.</returns>
			public virtual bool Require(ListViewBase owner, InstanceID ownerID, int index, Func<int, TItemView, bool> instanceCompare)
			{
				RequiredInstances += 1;

				var instances = Instances.Of(ownerID);
				for (var i = 0; i < instances.Count; i++)
				{
					var instance = instances[i];
					if (instanceCompare(index, instance))
					{
						Requested.Add(instance);
						Instances.RemoveAt(ownerID, i);

						return true;
					}
				}

				return false;
			}

			/// <summary>
			/// Request instance.
			/// </summary>
			/// <param name="ownerID">Owner ID.</param>
			/// <returns>Instance.</returns>
			public virtual TItemView RequestInstance(InstanceID ownerID)
			{
				var n = Requested.Count - 1;
				var instance = Requested[n];
				Requested.RemoveAt(n);
				Instances.Add(ownerID, instance);

				return instance;
			}

			/// <summary>
			/// Request instance with the specified index.
			/// </summary>
			/// <param name="ownerID">Owner ID.</param>
			/// <param name="index">Index.</param>
			/// <param name="instanceCompare">Instance comparer.</param>
			/// <param name="isNew">true if instance with the specified index was found; otherwise false.</param>
			/// <returns>Instance.</returns>
			public virtual TItemView RequestInstance(InstanceID ownerID, int index, Func<int, TItemView, bool> instanceCompare, out bool isNew)
			{
				for (var i = 0; i < Requested.Count; i++)
				{
					var instance = Requested[i];

					if (instanceCompare(index, instance))
					{
						Requested.RemoveAt(i);
						Instances.Add(ownerID, instance);

						isNew = instance.Index != index;
						return instance;
					}
				}

				var n = Requested.Count - 1;
				var result = Requested[n];
				Requested.RemoveAt(n);
				Instances.Add(ownerID, result);

				isNew = true;
				return result;
			}

			/// <summary>
			/// Apply function for each active component.
			/// </summary>
			/// <param name="ownerID">Owner ID.</param>
			/// <param name="action">Action.</param>
			public void ForEach(InstanceID ownerID, Action<TItemView> action)
			{
				var instances = Instances.Of(ownerID);
				foreach (var instance in instances)
				{
					action(instance);
				}
			}

			/// <summary>
			/// Apply function for each active and cached components.
			/// </summary>
			/// <param name="ownerID">Owner ID.</param>
			/// <param name="action">Action.</param>
			public void ForEachAll(InstanceID ownerID, Action<TItemView> action)
			{
				ForEach(ownerID, action);
				ForEachCache(action);
			}

			/// <summary>
			/// Apply function for each cached component.
			/// </summary>
			/// <param name="action">Action.</param>
			public void ForEachCache(Action<TItemView> action)
			{
				action(Template);

				foreach (var c in Cache)
				{
					action(c);
				}
			}

			/// <summary>
			/// Apply function for each cached component.
			/// </summary>
			/// <param name="action">Action.</param>
			public void ForEachCache(Action<ListViewItem> action)
			{
				action(Template);

				foreach (var instance in Cache)
				{
					action(instance);
				}
			}

			/// <summary>
			/// Set size of the components.
			/// </summary>
			/// <param name="ownerID">Owner ID.</param>
			/// <param name="size">Size.</param>
			public void SetSize(InstanceID ownerID, Vector2 size)
			{
				defaultSize = size;

				SetSize(Template, size);

				var instances = Instances.Of(ownerID);
				foreach (var instance in instances)
				{
					SetSize(instance, size);
				}

				foreach (var instance in Cache)
				{
					SetSize(instance, size);
				}
			}

			/// <summary>
			/// Set size.
			/// </summary>
			/// <param name="component">Component.</param>
			/// <param name="size">Size.</param>
			protected void SetSize(TItemView component, Vector2 size)
			{
				var item_rt = component.RectTransform;
				item_rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				item_rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
			}

			/// <summary>
			/// Set the style.
			/// </summary>
			/// <param name="ownerID">Owner ID.</param>
			/// <param name="styleBackground">Style for the background.</param>
			/// <param name="styleText">Style for the text.</param>
			/// <param name="style">Full style data.</param>
			public void SetStyle(InstanceID ownerID, StyleImage styleBackground, StyleText styleText, Style style)
			{
				Template.SetStyle(styleBackground, styleText, style);

				var instances = Instances.Of(ownerID);
				foreach (var instance in instances)
				{
					instance.SetStyle(styleBackground, styleText, style);
				}

				foreach (var instance in Cache)
				{
					instance.SetStyle(styleBackground, styleText, style);
				}
			}

			/// <summary>
			/// Destroy all instances.
			/// </summary>
			/// <param name="ownerID">Owner ID.</param>
			public virtual void Destroy(InstanceID ownerID)
			{
				var instances = Instances.Of(ownerID);
				foreach (var i in instances)
				{
					Disable(i);
				}

				instances.Clear();

				foreach (var r in Requested)
				{
					Disable(r);
				}

				Requested.Clear();

				Instances.Remove(ownerID);
				Callbacks.Remove(ownerID);

				// clear cache only if last
				if (Callbacks.Count == 0)
				{
					ClearCache();
				}
			}

			/// <summary>
			/// Clear cache.
			/// </summary>
			public void ClearCache()
			{
				foreach (var c in Cache)
				{
					Destroy(c);
				}

				Cache.Clear();
			}

			/// <summary>
			/// Enable Template.
			/// </summary>
			public virtual void EnableTemplate()
			{
				if (!Template.gameObject.activeSelf)
				{
					Template.gameObject.SetActive(true);
				}
			}

			/// <summary>
			/// Disable Template.
			/// </summary>
			public virtual void DisableTemplate()
			{
				if (!Utilities.IsNull(template))
				{
					template.gameObject.SetActive(false);
				}
			}

			/// <summary>
			/// Copy data.
			/// </summary>
			/// <param name="template">Template.</param>
			/// <param name="includeCache">Also copy cache.</param>
			public virtual void CopyFrom(ListViewItemTemplate<TItemView> template, bool includeCache = true)
			{
				if (includeCache)
				{
					Cache.AddRange(template.Cache);
					template.Cache.Clear();
				}

				Instances.CopyFrom(template.Instances);
				template.Instances.Clear();

				foreach (var callback in template.Callbacks)
				{
					Callbacks[callback.Key] = callback.Value;
				}

				template.Callbacks.Clear();

				RemoveDuplicates();
			}

			/// <summary>
			/// Set owner.
			/// </summary>
			/// <param name="owner">Owner.</param>
			public void SetOwner(ListViewBase owner)
			{
				foreach (var instance in instancesList)
				{
					instance.Owner = owner;

					if (instance.RectTransform.parent.GetInstanceID() != owner.Container.GetInstanceID())
					{
						instance.RectTransform.SetParent(owner.Container, false);
					}
				}

				Instances.OwnerChanged();
			}

			/// <summary>
			/// Destroy the instance.
			/// </summary>
			/// <param name="instance">Instance.</param>
			protected void Destroy(TItemView instance)
			{
				foreach (var c in Callbacks)
				{
					c.Value.ComponentDestroyed(instance);
				}

				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(instance.gameObject);
				}
#if UNITY_EDITOR
				else
				{
					UnityEngine.Object.DestroyImmediate(instance.gameObject);
				}
#endif
			}
		}
	}
}