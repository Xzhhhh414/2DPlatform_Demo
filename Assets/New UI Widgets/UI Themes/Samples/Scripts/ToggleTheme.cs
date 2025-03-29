namespace UIThemes.Samples
{
	using UnityEngine;

	/// <summary>
	/// Toggle theme active variation.
	/// </summary>
	public class ToggleTheme : MonoBehaviour
	{
		/// <summary>
		/// Toggle.
		/// </summary>
		[SerializeField]
		public Theme Theme;

		/// <summary>
		/// Toggle variation.
		/// </summary>
		/// <param name="name">Variation name.</param>
		public void Toggle(string name)
		{
			Theme.SetActiveVariation(name);
		}
	}
}