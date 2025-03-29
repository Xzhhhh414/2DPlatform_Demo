namespace UIWidgets
{
	using UIWidgets.Attributes;
	using UnityEngine;

	/// <summary>
	/// Scripts templates.
	/// </summary>
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/third-party-support/data-bind.html")]
	public class DataBindTemplates : ScriptableObject
	{
#if UNITY_EDITOR
		static DataBindTemplates instance;

		/// <summary>
		/// Instance.
		/// </summary>
		public static DataBindTemplates Instance
		{
			get
			{
				if (instance == null)
				{
					instance = UtilitiesEditor.LoadAssetWithGUID<DataBindTemplates>(ReferenceGUID.DataBindTemplates);
				}

				return instance;
			}

			set
			{
				instance = value;
			}
		}

		#if UNITY_2019_3_OR_NEWER
		/// <summary>
		/// Reload support.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		[DomainReload(nameof(instance))]
		static void StaticInit()
		{
			instance = null;
		}
		#endif
#endif

		/// <summary>
		/// Setter.
		/// </summary>
		[SerializeField]
		public TextAsset Setter;

		/// <summary>
		/// Provider.
		/// </summary>
		[SerializeField]
		public TextAsset Provider;

		/// <summary>
		/// Observer.
		/// </summary>
		[SerializeField]
		public TextAsset Observer;

		/// <summary>
		/// Synchronizer.
		/// </summary>
		[SerializeField]
		public TextAsset Synchronizer;
	}
}