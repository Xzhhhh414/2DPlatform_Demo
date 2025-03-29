namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// UI cursor settings.
	/// </summary>
	[System.Obsolete("Replaced with CursorsSelector.")]
	public class UICursorSettings : MonoBehaviourInitiable
	{
		/// <summary>
		/// Default cursor.
		/// </summary>
		[SerializeField]
		protected Texture2D DefaultCursor;

		/// <summary>
		/// Default cursor hot spot.
		/// </summary>
		[SerializeField]
		protected Vector2 DefaultCursorHotSpot;

		/// <inheritdoc/>
		protected override void InitOnce()
		{
			base.InitOnce();

			UICursor.Default = new Cursors.Cursor(DefaultCursor, DefaultCursorHotSpot);
		}
	}
}