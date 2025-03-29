namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// List view item component.
	/// </summary>
	public class ListViewStringComponent : ListViewItem, IViewData<string>
	{
		/// <summary>
		/// The Text component.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with TextAdapter.")]
		public Text Text;

		/// <summary>
		/// The text adapter.
		/// </summary>
		[SerializeField]
		public TextAdapter TextAdapter;

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				#pragma warning disable 0618
				Foreground = new Graphic[] { UtilitiesUI.GetGraphic(TextAdapter), };
				#pragma warning restore
				GraphicsForegroundVersion = 1;
			}

			base.GraphicsForegroundInit();
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="item">Text.</param>
		public virtual void SetData(string item)
		{
			TextAdapter.text = item.Replace("\\n", "\n");
		}

		/// <summary>
		/// Upgrade serialized data to the latest version.
		/// </summary>
		public override void Upgrade()
		{
			base.Upgrade();

#pragma warning disable 0618
			if (Text == null)
			{
				Text = GetComponentInChildren<Text>(true);
			}

			Utilities.RequireComponent(Text, ref TextAdapter);
#pragma warning restore 0618
		}
	}
}