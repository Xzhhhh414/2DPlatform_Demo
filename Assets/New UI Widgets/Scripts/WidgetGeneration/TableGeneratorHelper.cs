namespace UIWidgets.WidgetGeneration
{
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Table generator helper.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/generator.html")]
	public class TableGeneratorHelper : ListViewGeneratorHelper
	{
		/// <summary>
		/// TableHeader.
		/// </summary>
		[FormerlySerializedAs("ResizableHeader")]
		public TableHeader TableHeader;
	}
}