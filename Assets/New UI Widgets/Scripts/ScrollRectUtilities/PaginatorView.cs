namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for the paginator.
	/// </summary>
	public abstract partial class PaginatorBase : UIBehaviourInteractable, IStylable
	{
		/// <summary>
		/// ScrollRect Paginator.
		/// </summary>
		[Serializable]
		public class PaginatorView
		{
			/// <summary>
			/// Default page.
			/// </summary>
			[SerializeField]
			protected ScrollRectPage defaultPage;

			/// <summary>
			/// Default page.
			/// </summary>
			public ScrollRectPage DefaultPage => defaultPage;

			/// <summary>
			/// Active page.
			/// </summary>
			[SerializeField]
			protected ScrollRectPage activePage;

			/// <summary>
			/// Active page.
			/// </summary>
			public ScrollRectPage ActivePage => activePage;

			/// <summary>
			/// The previous page.
			/// </summary>
			[SerializeField]
			protected ScrollRectPage prevPage;

			/// <summary>
			/// The previous page.
			/// </summary>
			public ScrollRectPage PrevPage => prevPage;

			/// <summary>
			/// The next page.
			/// </summary>
			[SerializeField]
			protected ScrollRectPage nextPage;

			/// <summary>
			/// The next page.
			/// </summary>
			public ScrollRectPage NextPage => nextPage;

			/// <summary>
			/// The default pages.
			/// </summary>
			[SerializeField]
			[HideInInspector]
			protected List<ScrollRectPage> DefaultPages = new List<ScrollRectPage>();

			/// <summary>
			/// The default pages cache.
			/// </summary>
			[SerializeField]
			[HideInInspector]
			protected BasePool<ScrollRectPage> defaultPagesCache;

			/// <summary>
			/// The default pages cache.
			/// </summary>
			public BasePool<ScrollRectPage> DefaultPagesCache => defaultPagesCache;

			/// <summary>
			/// Skip page template.
			/// </summary>
			[SerializeField]
			protected RectTransform SkipPage;

			/// <summary>
			/// SkipPage instances.
			/// </summary>
			[SerializeField]
			[HideInInspector]
			protected List<RectTransform> SkipPages = new List<RectTransform>();

			/// <summary>
			/// Cache of SkipPage instances.
			/// </summary>
			[SerializeField]
			[HideInInspector]
			protected BasePool<RectTransform> skipPagesCache;

			/// <summary>
			/// Pages container.
			/// </summary>
			[SerializeField]
			protected RectTransform pagesContainer;

			/// <summary>
			/// Pages container.
			/// </summary>
			public RectTransform PagesContainer
			{
				get
				{
					if (pagesContainer != null)
					{
						return pagesContainer;
					}

					if (defaultPage != null)
					{
						return defaultPage.transform.parent as RectTransform;
					}

					return null;
				}
			}

			/// <summary>
			/// Cache of SkipPage instances.
			/// </summary>
			public BasePool<RectTransform> SkipPagesCache => skipPagesCache;

			/// <summary>
			/// Hide active page if has only one page.
			/// </summary>
			[SerializeField]
			[Tooltip("Hide active page if has only one page.")]
			protected bool HideIfOnePage = true;

			[SerializeField]
			[Tooltip("Number of visible page buttons.\nThe first and last page buttons are always shown.\nSet to 0 to display buttons for all pages.")]
			int visiblePagesCount = 0;

			/// <summary>
			/// Count of the visible pages.
			/// </summary>
			public int VisiblePagesCount
			{
				get => visiblePagesCount;

				set
				{
					if (value < 0)
					{
						value = 0;
					}

					if (visiblePagesCount != value)
					{
						visiblePagesCount = value;
						UpdateObjects(Owner.CurrentPage);
					}
				}
			}

			/// <summary>
			/// Determines whether the specified pageComponent is null.
			/// </summary>
			[DomainReloadExclude]
			protected static readonly Predicate<object> IsNullComponent = pageComponent => pageComponent == null;

			/// <summary>
			/// Owner.
			/// </summary>
			protected PaginatorBase Owner;

			/// <summary>
			/// Go to page specified.
			/// </summary>
			protected UnityAction<int> GoToPage;

			/// <summary>
			/// Go to previous page.
			/// </summary>
			protected UnityAction<int> Prev;

			/// <summary>
			/// Go to next page.
			/// </summary>
			protected UnityAction<int> Next;

			bool isInited;

#if UNITY_EDITOR

			/// <summary>
			/// Set templates.
			/// </summary>
			/// <param name="defaultPage">Default page.</param>
			/// <param name="activePage">Active page.</param>
			/// <param name="nextPage">Next page.</param>
			/// <param name="prevPage">Previous page.</param>
			/// <param name="skipPage">Skip page.</param>
			/// <param name="pagesContainer">Pages container.</param>
			/// <param name="visiblePagesCount">Amount of visible pages.</param>
			public void SetTemplates(RectTransform defaultPage, RectTransform activePage, RectTransform nextPage, RectTransform prevPage, RectTransform skipPage, RectTransform pagesContainer, int visiblePagesCount)
			{
				if ((defaultPage != null) && !defaultPage.TryGetComponent(out this.defaultPage))
				{
					this.defaultPage = UnityEditor.Undo.AddComponent<ScrollRectPage>(defaultPage.gameObject);
				}

				if ((activePage != null) && !activePage.TryGetComponent(out this.activePage))
				{
					this.activePage = UnityEditor.Undo.AddComponent<ScrollRectPage>(activePage.gameObject);
				}

				if ((nextPage != null) && !nextPage.TryGetComponent(out this.nextPage))
				{
					this.nextPage = UnityEditor.Undo.AddComponent<ScrollRectPage>(nextPage.gameObject);
				}

				if ((prevPage != null) && !prevPage.TryGetComponent(out this.prevPage))
				{
					this.prevPage = UnityEditor.Undo.AddComponent<ScrollRectPage>(prevPage.gameObject);
				}

				SkipPage = skipPage;

				if ((pagesContainer == null) && (defaultPage != null))
				{
					pagesContainer = defaultPage.transform.parent as RectTransform;
				}

				this.pagesContainer = pagesContainer;
				this.visiblePagesCount = visiblePagesCount;
			}
#endif

			/// <summary>
			/// Init this instance.
			/// </summary>
			/// <param name="owner">Owner.</param>
			public void Init(PaginatorBase owner)
			{
				if (isInited)
				{
					return;
				}

				isInited = true;

				InitOnce(owner);
			}

			/// <summary>
			/// Init this instance only once.
			/// </summary>
			/// <param name="owner">Owner.</param>
			protected virtual void InitOnce(PaginatorBase owner)
			{
				Owner = owner;
				GoToPage = page =>
				{
					if (Owner.IsInteractable())
					{
						Owner.GoToPage(page);
					}
				};

				Prev = _ =>
				{
					if (Owner.IsInteractable())
					{
						Owner.Prev();
					}
				};

				Next = _ =>
				{
					if (Owner.IsInteractable())
					{
						Owner.Next();
					}
				};

				if (SkipPage != null)
				{
					SkipPage.gameObject.SetActive(false);

					if ((skipPagesCache == null) || (defaultPagesCache.Template == null))
					{
						skipPagesCache = new BasePool<RectTransform>(SkipPage);
					}
				}

				if (defaultPage != null)
				{
					defaultPage.gameObject.SetActive(false);

					if ((defaultPagesCache == null) || (defaultPagesCache.Template == null))
					{
						defaultPagesCache = new BasePool<ScrollRectPage>(defaultPage);
					}
				}

				if (prevPage != null)
				{
					prevPage.SetPage(0);
					prevPage.OnPageSelect.AddListener(Prev);
				}

				if (nextPage != null)
				{
					nextPage.OnPageSelect.AddListener(Next);
				}
			}

			/// <summary>
			/// Set interactable view.
			/// </summary>
			/// <param name="state">State.</param>
			public virtual void SetInteractableView(bool state)
			{
				foreach (var page in DefaultPages)
				{
					SetInteractableView(page, state);
				}

				SetInteractableView(defaultPage, state);
				SetInteractableView(activePage, state);
				SetInteractableView(prevPage, state);
				SetInteractableView(nextPage, state);
			}

			/// <summary>
			/// Set interactable view.
			/// </summary>
			/// <param name="page">Page.</param>
			/// <param name="state">View.</param>
			protected virtual void SetInteractableView(ScrollRectPage page, bool state)
			{
				if (page == null)
				{
					return;
				}

				if (page.TryGetComponent<Button>(out var btn))
				{
					btn.interactable = state;
				}
			}

			/// <summary>
			/// Set shared default pages.
			/// </summary>
			/// <param name="shared">Pool.</param>
			/// <returns>Previously used pages.</returns>
			public BasePool<ScrollRectPage> SetSharedDefaultPages(BasePool<ScrollRectPage> shared)
			{
				if (ReferenceEquals(shared, defaultPagesCache))
				{
					return null;
				}

				var previous = defaultPagesCache;
				if (previous != null)
				{
					if (shared.Merge(previous))
					{
						defaultPagesCache = shared;
					}
					else if (isInited)
					{
						RequireDefaultPages(0);
						defaultPagesCache = shared;
						UpdateObjects(Owner.CurrentPage);
					}
					else
					{
						defaultPagesCache = shared;
					}
				}
				else
				{
					defaultPagesCache = shared;
				}

				return previous;
			}

			/// <summary>
			/// Set shared skip pages.
			/// </summary>
			/// <param name="shared">Pool.</param>
			/// <returns>Previously used pages.</returns>
			public BasePool<RectTransform> SetSharedSkipPages(BasePool<RectTransform> shared)
			{
				if (ReferenceEquals(shared, skipPagesCache))
				{
					return null;
				}

				var previous = skipPagesCache;
				if (previous != null)
				{
					if (shared.Merge(previous))
					{
						skipPagesCache = shared;
					}
					else if (isInited)
					{
						RequireDefaultPages(0);
						skipPagesCache = shared;
						UpdateObjects(Owner.CurrentPage);
					}
					else
					{
						skipPagesCache = shared;
					}
				}
				else
				{
					skipPagesCache = shared;
				}

				return previous;
			}

			/// <summary>
			/// Create required instances of the DefaultPage.
			/// </summary>
			/// <param name="required">Required instances.</param>
			protected virtual void RequireDefaultPages(int required)
			{
				DefaultPages.RemoveAll(IsNullComponent);

				if (DefaultPages.Count < required)
				{
					for (int i = DefaultPages.Count; i < required; i++)
					{
						AddComponent(i);
					}
				}
				else
				{
					for (int i = required; i < DefaultPages.Count; i++)
					{
						DefaultPages[i].OnPageSelect.RemoveListener(GoToPage);
						defaultPagesCache.Release(DefaultPages[i]);
					}

					DefaultPages.RemoveRange(required, DefaultPages.Count - required);
				}
			}

			/// <summary>
			/// Reset skip pages.
			/// </summary>
			protected virtual void ResetSkipPages()
			{
				foreach (var skip in SkipPages)
				{
					skip.SetAsLastSibling();
					skipPagesCache.Release(skip);
				}

				SkipPages.Clear();
			}

			/// <summary>
			/// Set SkipPage.
			/// </summary>
			/// <param name="nextTransform">Next transform.</param>
			/// <returns>SkipPage instance.</returns>
			protected virtual RectTransform SetSkipPage(RectTransform nextTransform)
			{
				var skip = skipPagesCache.Get(PagesContainer);
				skip.SetSiblingIndex(nextTransform.GetSiblingIndex());

				var index = nextTransform.GetSiblingIndex();
				if (skip.GetSiblingIndex() > index)
				{
					skip.SetSiblingIndex(index);
				}

				SkipPages.Add(skip);

				return skip;
			}

			/// <summary>
			/// Adds page the component.
			/// </summary>
			/// <param name="page">Page.</param>
			protected virtual void AddComponent(int page)
			{
				var component = defaultPagesCache.Get(PagesContainer);
				component.OnPageSelect.AddListener(GoToPage);

				component.transform.SetAsLastSibling();
				component.gameObject.SetActive(true);
				component.SetPage(page);

				DefaultPages.Add(component);
			}

			/// <summary>
			/// Update objects.
			/// </summary>
			/// <param name="page">Current page.</param>
			public virtual void UpdateObjects(int page)
			{
				if (defaultPagesCache.Template != null)
				{
					RequireDefaultPages(Owner.VisiblePages.Count);

					ResetSkipPages();

					var current_index = 0;
					for (int i = 0; i < DefaultPages.Count; i++)
					{
						var dp = DefaultPages[i];
						var visible_page = Owner.VisiblePages[i];
						var current = visible_page == page;

						dp.SetPage(visible_page);
						dp.gameObject.SetActive(!current);

						if (current)
						{
							current_index = dp.RectTransform.GetSiblingIndex();
						}
					}

					activePage.gameObject.SetActive(Owner.IsValid && (Owner.Pages > 1 || !HideIfOnePage));
					activePage.SetPage(page);
					activePage.transform.SetSiblingIndex(current_index);

					if (skipPagesCache.Template != null)
					{
						var prev = Owner.VisiblePages.Count > 0 ? Owner.VisiblePages[0] - 1 : 0;
						for (int i = 0; i < DefaultPages.Count; i++)
						{
							var visible_page = Owner.VisiblePages[i];
							if (visible_page - prev != 1)
							{
								SetSkipPage(DefaultPages[i].RectTransform);
							}

							prev = visible_page;
						}
					}

					if (PagesContainer.TryGetComponent<LayoutGroup>(out var layout))
					{
						LayoutUtilities.UpdateLayout(layout);
					}
				}

				if (prevPage != null)
				{
					prevPage.SetState(Owner.IsPrevAvailable(page));
					prevPage.SetPage(0);
					prevPage.transform.SetAsFirstSibling();
				}

				if (nextPage != null)
				{
					nextPage.SetState(Owner.IsNextAvailable(page));
					nextPage.SetPage(Owner.Pages - 1);
					nextPage.transform.SetAsLastSibling();
				}
			}

			/// <summary>
			/// Removes the callback.
			/// </summary>
			/// <param name="page">Page.</param>
			protected virtual void RemoveCallback(ScrollRectPage page) => page.OnPageSelect.RemoveListener(GoToPage);

			/// <summary>
			/// Destroy this instance.
			/// </summary>
			public void Destroy()
			{
				DefaultPages.RemoveAll(IsNullComponent);
				foreach (var p in DefaultPages)
				{
					RemoveCallback(p);
				}

				if (prevPage != null)
				{
					prevPage.OnPageSelect.RemoveListener(Prev);
				}

				if (nextPage != null)
				{
					nextPage.OnPageSelect.RemoveListener(Next);
				}
			}

			#region IStylable implementation

			/// <summary>
			/// Set widget properties from specified style.
			/// </summary>
			/// <param name="style">Style data.</param>
			/// <returns><c>true</c>, if children game objects was processed, <c>false</c> otherwise.</returns>
			public virtual bool SetStyle(Style style)
			{
				if (defaultPage != null)
				{
					if (defaultPage.TryGetComponent<Image>(out var bg))
					{
						style.Paginator.DefaultBackground.ApplyTo(bg);
					}

					Utilities.RequireComponent<ScrollRectPage>(defaultPage).SetStyle(style.Paginator.DefaultText, style);
				}

				if (activePage != null)
				{
					if (activePage.TryGetComponent<Image>(out var bg))
					{
						style.Paginator.ActiveBackground.ApplyTo(bg);
					}

					Utilities.RequireComponent<ScrollRectPage>(activePage).SetStyle(style.Paginator.ActiveText, style);
				}

				foreach (var page in DefaultPages)
				{
					if (page.TryGetComponent<Image>(out var bg))
					{
						style.Paginator.DefaultBackground.ApplyTo(bg);
					}

					Utilities.RequireComponent<ScrollRectPage>(page).SetStyle(style.Paginator.DefaultText, style);
				}

				foreach (var page in defaultPagesCache)
				{
					if (page.TryGetComponent<Image>(out var bg))
					{
						style.Paginator.DefaultBackground.ApplyTo(bg);
					}

					Utilities.RequireComponent<ScrollRectPage>(page).SetStyle(style.Paginator.DefaultText, style);
				}

				if (prevPage != null)
				{
					if (prevPage.TryGetComponent<Image>(out var bg))
					{
						style.Paginator.DefaultBackground.ApplyTo(bg);
					}

					style.Paginator.DefaultText.ApplyTo(prevPage.transform.Find("Text"));
				}

				if (nextPage != null)
				{
					if (nextPage.TryGetComponent<Image>(out var bg))
					{
						style.Paginator.DefaultBackground.ApplyTo(bg);
					}

					style.Paginator.DefaultText.ApplyTo(nextPage.transform.Find("Text"));
				}

				return true;
			}

			/// <summary>
			/// Set style options from widget properties.
			/// </summary>
			/// <param name="style">Style data.</param>
			/// <returns><c>true</c>, if children game objects was processed, <c>false</c> otherwise.</returns>
			public virtual bool GetStyle(Style style)
			{
				if (defaultPage != null)
				{
					if (defaultPage.TryGetComponent<Image>(out var bg))
					{
						style.Paginator.DefaultBackground.GetFrom(bg);
					}

					Utilities.RequireComponent<ScrollRectPage>(defaultPage).GetStyle(style.Paginator.DefaultText, style);
				}

				if (activePage != null)
				{
					if (activePage.TryGetComponent<Image>(out var bg))
					{
						style.Paginator.ActiveBackground.GetFrom(bg);
					}

					Utilities.RequireComponent<ScrollRectPage>(activePage).GetStyle(style.Paginator.ActiveText, style);
				}

				if (prevPage != null)
				{
					if (prevPage.TryGetComponent<Image>(out var bg))
					{
						style.Paginator.DefaultBackground.GetFrom(bg);
					}

					style.Paginator.DefaultText.GetFrom(prevPage.transform.Find("Text"));
				}

				if (nextPage != null)
				{
					if (nextPage.TryGetComponent<Image>(out var bg))
					{
						style.Paginator.DefaultBackground.GetFrom(bg);
					}

					style.Paginator.DefaultText.GetFrom(nextPage.transform.Find("Text"));
				}

				return true;
			}
			#endregion
		}
	}
}