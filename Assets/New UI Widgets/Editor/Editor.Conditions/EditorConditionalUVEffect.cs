#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;

	/// <summary>
	/// Conditional editor for the classes derived from UVEffect.
	/// </summary>
	[CustomEditor(typeof(UVEffect), true)]
	public class EditorConditionalUVEffect : EditorConditional
	{
	}
}
#endif