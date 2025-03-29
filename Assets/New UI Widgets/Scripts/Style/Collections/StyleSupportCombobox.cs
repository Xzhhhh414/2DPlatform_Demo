namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the combobox.
	/// </summary>
	public class StyleSupportCombobox : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Combobox.
		/// </summary>
		[SerializeField]
		public GameObject Combobox;

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				style.Combobox.Background.ApplyTo(bg);
			}

			if ((Combobox != null) && (Combobox.GetInstanceID() != gameObject.GetInstanceID()) && Combobox.TryGetComponent<IStylable>(out var s))
			{
				s.SetStyle(style);
			}

			return true;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			if (TryGetComponent<Image>(out var bg))
			{
				style.Combobox.Background.GetFrom(bg);
			}

			if ((Combobox != null) && (Combobox.GetInstanceID() != gameObject.GetInstanceID()) && Combobox.TryGetComponent<IStylable>(out var s))
			{
				s.GetStyle(style);
			}

			return true;
		}
		#endregion
	}
}