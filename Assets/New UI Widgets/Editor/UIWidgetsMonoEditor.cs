#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;

	/// <summary>
	/// Base editor for the UIWidgets components.
	/// </summary>
	public class UIWidgetsMonoEditor : Editor
	{
		/// <summary>
		/// Validate targets.
		/// </summary>
		protected virtual void ValidateTargets()
		{
			foreach (var t in targets)
			{
				if (t is IValidateable v)
				{
					v.Validate();
				}
			}
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			ValidateTargets();

			base.OnInspectorGUI();

			ValidateTargets();
		}
	}
}
#endif