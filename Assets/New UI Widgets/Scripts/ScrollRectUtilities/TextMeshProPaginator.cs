namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Paginator for the TextMeshPro text.
	/// </summary>
	public class TextMeshProPaginator : PaginatorBase, IUpdatable
	{
#if UIWIDGETS_TMPRO_SUPPORT
		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		protected TMPro.TMP_Text Text;

		/// <inheritdoc/>
		public override int CurrentPage
		{
			get => Text.pageToDisplay - 1;

			set
			{
				Text.pageToDisplay = value + 1;
				Init();
				UpdateObjects(CurrentPage);
			}
		}

		/// <inheritdoc/>
		protected override bool IsValid => Text != null;

		/// <inheritdoc/>
		protected override void OnEnable()
		{
			base.OnEnable();

			Updater.Add(this);
		}

		/// <inheritdoc/>
		protected override void OnDisable()
		{
			base.OnDisable();

			Updater.Remove(this);
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		public virtual void RunUpdate()
		{
			if (Pages == Text.textInfo.pageCount)
			{
				return;
			}

			Pages = Text.textInfo.pageCount;
		}
#else

		/// <inheritdoc/>
		public override int CurrentPage
		{
			get => throw new System.NotImplementedException("TextMeshPro support is not enabled");
			set => throw new System.NotImplementedException("TextMeshPro support is not enabled");
		}

		/// <inheritdoc/>
		protected override bool IsValid => throw new System.NotImplementedException("TextMeshPro support is not enabled");

		/// <summary>
		/// Update this instance.
		/// </summary>
		public virtual void RunUpdate()
		{
		}
#endif
	}
}