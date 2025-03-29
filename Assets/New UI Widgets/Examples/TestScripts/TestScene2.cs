namespace UIWidgets.Examples
{
	using EasyLayoutNS;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Test scene.
	/// </summary>
	public class TestScene2 : MonoBehaviour
	{
		/// <summary>
		/// ListViewIcons.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("listViewIcons")]
		protected ListViewIcons ListViewIcons;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
#pragma warning disable 0618
			ListViewIcons.Sort = false;
#pragma warning restore 0618
		}

		/// <summary>
		/// SteamSpyView.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("steamSpyView")]
		protected SteamSpyView SteamSpyView;

		/// <summary>
		/// DefaultItem for Table.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("tableDefaultItem")]
		protected SteamSpyComponent TableDefaultItem;

		/// <summary>
		/// Table header.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("tableHeader")]
		protected GameObject TableHeader;

		/// <summary>
		/// Table BackgroundColor.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("tableBackground")]
		protected Color TableBackgroundColor;

		/// <summary>
		/// DefaultItem for TileView.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("tileDefaultItem1")]
		protected SteamSpyComponent TileDefaultItem1;

		/// <summary>
		/// Another DefaultItem for TileView.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("tileDefaultItem2")]
		protected SteamSpyComponent TileDefaultItem2;

		/// <summary>
		/// TileView BackgroundColor.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("tileBackground")]
		protected Color TileViewBackgroundColor;

		/// <summary>
		/// Set DefaultItem for table.
		/// </summary>
		public void SetTableDefaultItem()
		{
			TableHeader.SetActive(true);

			var scrollRectTransform = SteamSpyView.GetScrollRect().transform as RectTransform;
			scrollRectTransform.sizeDelta = new Vector2(-20, -28);
			scrollRectTransform.anchoredPosition = new Vector2(-10, -14);

			SteamSpyView.Container.TryGetComponent<EasyLayout>(out var layout);
			layout.ChildrenWidth = ChildrenSize.DoNothing;
			layout.Spacing = new Vector2(1, 1);

			if (SteamSpyView.Container.TryGetComponent<Image>(out var bg))
			{
				bg.color = TableBackgroundColor;
			}

			SteamSpyView.DefaultItem = TableDefaultItem;
		}

		/// <summary>
		/// Set DefaultItem for TileView.
		/// </summary>
		public void SetTileDefaultItem1()
		{
			TableHeader.SetActive(false);

			var scrollRectTransform = SteamSpyView.GetScrollRect().transform as RectTransform;
			scrollRectTransform.sizeDelta = new Vector2(-20, 0);
			scrollRectTransform.anchoredPosition = new Vector2(-10, 0);

			SteamSpyView.Container.TryGetComponent<EasyLayout>(out var layout);
			layout.ChildrenWidth = ChildrenSize.SetPreferred;
			layout.Spacing = new Vector2(5, 5);

			if (SteamSpyView.Container.TryGetComponent<Image>(out var bg))
			{
				bg.color = TileViewBackgroundColor;
			}

			SteamSpyView.DefaultItem = TileDefaultItem1;
		}

		/// <summary>
		/// Set another DefaultItem for TileView.
		/// </summary>
		public void SetTileDefaultItem2()
		{
			TableHeader.SetActive(false);

			var scrollRectTransform = SteamSpyView.GetScrollRect().transform as RectTransform;
			scrollRectTransform.sizeDelta = new Vector2(-20, 0);
			scrollRectTransform.anchoredPosition = new Vector2(-10, 0);

			SteamSpyView.Container.TryGetComponent<EasyLayout>(out var layout);
			layout.ChildrenWidth = ChildrenSize.SetPreferred;
			layout.Spacing = new Vector2(5, 5);

			if (SteamSpyView.Container.TryGetComponent<Image>(out var bg))
			{
				bg.color = TileViewBackgroundColor;
			}

			SteamSpyView.DefaultItem = TileDefaultItem2;
		}
	}
}