namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Text proxy interface.
	/// </summary>
	public interface ITextProxy
	{
		/// <summary>
		/// Gameobject.
		/// </summary>
		GameObject GameObject
		{
			get;
		}

		/// <summary>
		/// Graphic component.
		/// </summary>
		Graphic Graphic
		{
			get;
		}

		/// <summary>
		/// Text.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Compatibility with Unity.Text.")]
		string text
		{
			get;
			set;
		}

		/// <summary>
		/// Color.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Compatibility with Unity.Text.")]
		Color color
		{
			get;
			set;
		}

		/// <summary>
		/// Font size.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Compatibility with Unity.Text.")]
		float fontSize
		{
			get;
			set;
		}

		/// <summary>
		/// Font style.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Compatibility with Unity.Text.")]
		FontStyle fontStyle
		{
			get;
			set;
		}

		/// <summary>
		/// Text alignment.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Compatibility with Unity.Text.")]
		TextAnchor alignment
		{
			get;
			set;
		}

		/// <summary>
		/// Bold.
		/// </summary>
		bool Bold
		{
			get;
			set;
		}

		/// <summary>
		/// Italic.
		/// </summary>
		bool Italic
		{
			get;
			set;
		}
	}
}