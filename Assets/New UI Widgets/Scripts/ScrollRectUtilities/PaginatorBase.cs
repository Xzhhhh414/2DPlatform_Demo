namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Base class for the paginator.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/widgets/controls/paginator.html")]
	public abstract partial class PaginatorBase : UIBehaviourInteractable, IStylable
	{
		/// <summary>
		/// View.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("View")]
		protected PaginatorView view;

		/// <summary>
		/// View.
		/// </summary>
		public PaginatorView View => view;

		/// <summary>
		/// Visible pages count.
		/// </summary>
		public int VisiblePagesCount
		{
			get => view.VisiblePagesCount;

			set => view.VisiblePagesCount = value;
		}

		/// <summary>
		/// Required fields are specified.
		/// </summary>
		protected abstract bool IsValid
		{
			get;
		}

		/// <summary>
		/// Current page.
		/// </summary>
		public abstract int CurrentPage
		{
			get;

			set;
		}

		int pages;

		/// <summary>
		/// Pages count.
		/// </summary>
		public virtual int Pages
		{
			get => pages;

			protected set
			{
				pages = Mathf.Max(1, value);
				UpdateObjects(CurrentPage);
			}
		}

		/// <summary>
		/// Visible pages.
		/// </summary>
		protected List<int> VisiblePages = new List<int>();

		/// <summary>
		/// OnPageSelect event.
		/// </summary>
		[SerializeField]
		public ScrollRectPageSelect OnPageSelect = new ScrollRectPageSelect();

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			view ??= new PaginatorView();

			view.Init(this);
		}

		/// <inheritdoc/>
		protected override void OnInteractableChange(bool interactableState)
		{
			view?.SetInteractableView(interactableState);
		}

		/// <summary>
		/// Refresh this instance.
		/// </summary>
		public virtual void Refresh()
		{
			Init();
			GoToPage(CurrentPage, true);
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();
			view.Destroy();
		}

		/// <summary>
		/// Is previous page available?
		/// </summary>
		/// <param name="page">Current page.</param>
		/// <returns>true if previous page available; otherwise false.</returns>
		protected virtual bool IsPrevAvailable(int page) => (page != 0) && (Pages > 0);

		/// <summary>
		/// Is next page available?
		/// </summary>
		/// <param name="page">Current page.</param>
		/// <returns>true if next page available; otherwise false.</returns>
		protected virtual bool IsNextAvailable(int page) => (page != (Pages - 1)) && (Pages > 0);

		/// <summary>
		/// Go to page.
		/// </summary>
		/// <param name="page">Page.</param>
		protected virtual void GoToPage(int page)
		{
			GoToPage(page, false);
		}

		/// <summary>
		/// Go to page.
		/// </summary>
		/// <param name="page">Page.</param>
		/// <param name="forceUpdate">If set to <c>true</c> force update.</param>
		protected virtual void GoToPage(int page, bool forceUpdate)
		{
			if (!IsValid)
			{
				UpdateObjects(0);
				return;
			}

			page = Mathf.Clamp(page, 0, Pages - 1);
			if ((CurrentPage == page) && (!forceUpdate))
			{
				UpdateObjects(page);
				return;
			}

			UpdateObjects(page);

			CurrentPage = page;

			OnPageSelect.Invoke(CurrentPage);
		}

		/// <summary>
		/// Update objects.
		/// </summary>
		/// <param name="page">Page.</param>
		protected virtual void UpdateObjects(int page)
		{
			if (page >= Pages)
			{
				page = Pages - 1;
			}

			CalculateVisiblePages(page);

			view.UpdateObjects(page);
			OnInteractableChange(IsInteractable());
		}

		/// <summary>
		/// Calculate visible pages.
		/// </summary>
		/// <param name="page">Current page.</param>
		protected virtual void CalculateVisiblePages(int page)
		{
			VisiblePages.Clear();

			if (VisiblePagesCount == 0)
			{
				for (int i = 0; i < Pages; i++)
				{
					VisiblePages.Add(i);
				}

				return;
			}

			if (Pages > 0)
			{
				VisiblePages.Add(0);
			}

			if (Pages > 1)
			{
				var min = Mathf.Max(1, page - (VisiblePagesCount / 2));
				var max = Mathf.Min(Pages, min + VisiblePagesCount);
				if ((min == 1) && ((max - min) == VisiblePagesCount))
				{
					max -= 1;
				}
				else if ((max > VisiblePagesCount) && ((max - min) != VisiblePagesCount))
				{
					min = max - VisiblePagesCount;
				}

				for (int i = min; i < max; i++)
				{
					VisiblePages.Add(i);
				}

				if (max < Pages)
				{
					VisiblePages.Add(Pages - 1);
				}
			}
		}

		/// <summary>
		/// Go to the next page.
		/// </summary>
		public virtual void Next()
		{
			if (CurrentPage == (Pages - 1))
			{
				return;
			}

			CurrentPage += 1;
		}

		/// <summary>
		/// Go to the previous page.
		/// </summary>
		public virtual void Prev()
		{
			if (CurrentPage == 0)
			{
				return;
			}

			CurrentPage -= 1;
		}

		/// <summary>
		/// Go to the first page.
		/// </summary>
		public virtual void FirstPage()
		{
			CurrentPage = 0;
		}

		/// <summary>
		/// Go to the last page.
		/// </summary>
		public virtual void LastPage()
		{
			if (Pages > 0)
			{
				return;
			}

			CurrentPage = Pages - 1;
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style) => view.SetStyle(style);

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style) => view.GetStyle(style);
		#endregion
	}
}