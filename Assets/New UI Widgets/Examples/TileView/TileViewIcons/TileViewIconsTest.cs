namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test TileViewIcons.
	/// </summary>
	public class TileViewIconsTest : MonoBehaviour
	{
		/// <summary>
		/// Change layout settings.
		/// </summary>
		public void ChangeLayoutSettings()
		{
			TryGetComponent<TileViewIcons>(out var tiles);

			tiles.Layout.Spacing = new Vector2(5, 50);
			tiles.Layout.UpdateLayout();

			tiles.ScrollRect.TryGetComponent<ResizeListener>(out var rl);
			rl.OnResize.Invoke();
			rl.OnResizeNextFrame.Invoke();
		}
	}
}