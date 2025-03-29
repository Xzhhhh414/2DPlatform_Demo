namespace UIWidgets.Examples.VHAR
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Item.
	/// </summary>
	[Serializable]
	public class Item
	{
		/// <summary>
		/// Texture.
		/// </summary>
		[SerializeField]
		public Texture2D Texture;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		[Multiline]
		public string Name;

		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		[Multiline]
		public string Text;
	}
}