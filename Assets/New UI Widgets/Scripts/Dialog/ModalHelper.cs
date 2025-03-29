namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Modal helper for UI widgets.
	/// <example>modalID = ModalHelper.Open(this, modalSprite, modalColor);
	/// //...
	/// ModalHelper.Close(modalID);</example>
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[HelpURL("https://ilih.name/unity-assets/UIWidgets/docs/components/modal-helper.html")]
	public class ModalHelper : MonoBehaviour, ITemplatable, IPointerClickHandler
	{
		static readonly Dictionary<InstanceID, ModalHelper> Used = new Dictionary<InstanceID, ModalHelper>();

		static Templates<ModalHelper> Templates = new Templates<ModalHelper>();

		static string key = "ModalTemplate";

		#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		[DomainReload(nameof(Templates), nameof(Used), nameof(key))]
		static void StaticInit()
		{
			Templates = new Templates<ModalHelper>();
			Used.Clear();
			key = "ModalTemplate";
		}
		#endif

		bool isTemplate = true;

		/// <summary>
		/// Gets a value indicating whether this instance is template.
		/// </summary>
		public bool IsTemplate
		{
			get => isTemplate;

			set => isTemplate = value;
		}

		/// <summary>
		/// Gets the name of the template.
		/// </summary>
		public string TemplateName
		{
			get;
			set;
		}

		Image image;

		/// <summary>
		/// Background image.
		/// </summary>
		public Image Image
		{
			get
			{
				if (image == null)
				{
					TryGetComponent(out image);
				}

				return image;
			}
		}

		readonly UnityEvent OnClick = new UnityEvent();

		/// <summary>
		/// Create modal helper with the specified parent, sprite and color.
		/// </summary>
		/// <param name="parentGameObject">Parent game object.</param>
		/// <param name="sprite">Sprite.</param>
		/// <param name="color">Color.</param>
		/// <param name="onClick">onClick callback</param>
		/// <param name="parent">Parent.</param>
		/// <returns>Modal helper ID.</returns>
		public static InstanceID Open(Component parentGameObject, Sprite sprite = null, Color? color = null, UnityAction onClick = null, RectTransform parent = null)
		{
			// check if in cache
			if (!Templates.Exists(key))
			{
				Templates.FindTemplates();
				CreateTemplate();
			}

			var modal = Templates.Instance(key);

			if (parent == null)
			{
				parent = UtilitiesUI.FindTopmostCanvas(parentGameObject.transform);
			}

			modal.gameObject.SetActive(true);

			var rt = modal.transform as RectTransform;

			rt.SetParent(parent, false);
			rt.SetAsLastSibling();

			rt.localPosition = Vector3.zero;
			rt.sizeDelta = new Vector2(0, 0);
			rt.anchorMin = new Vector2(0, 0);
			rt.anchorMax = new Vector2(1, 1);
			rt.anchoredPosition = new Vector2(0, 0);

			if (sprite != null)
			{
				modal.Image.sprite = sprite;
			}

			if (color != null)
			{
				modal.Image.color = (Color)color;
			}

			modal.OnClick.RemoveAllListeners();
			if (onClick != null)
			{
				modal.OnClick.AddListener(onClick);
			}

			var id = new InstanceID(modal);
			Used.Add(id, modal);
			return id;
		}

		/// <summary>
		/// Creates the template.
		/// </summary>
		static void CreateTemplate()
		{
			var template = new GameObject(key);

			var modal = template.AddComponent<ModalHelper>();
			template.AddComponent<Image>();
			var le = template.AddComponent<LayoutElement>();
			le.ignoreLayout = true;

			Templates.Add(key, modal);

			template.SetActive(false);
		}

		/// <summary>
		/// Gets the modal instance by id.
		/// </summary>
		/// <returns>The instance.</returns>
		/// <param name="id">ID.</param>
		public static ModalHelper GetInstance(InstanceID id)
		{
			return ((Used != null) && Used.ContainsKey(id))
				? Used[id]
				: null;
		}

		/// <summary>
		/// Set instance as last sibling.
		/// </summary>
		/// <param name="id">Instance ID.</param>
		/// <returns>true if instance exists; otherwise false.</returns>
		public static bool SetAsLastSibling(InstanceID? id)
		{
			if (id.HasValue)
			{
				return SetAsLastSibling(id.Value);
			}

			return false;
		}

		/// <summary>
		/// Set instance as last sibling.
		/// </summary>
		/// <param name="id">Instance ID.</param>
		/// <returns>true if instance exists; otherwise false.</returns>
		public static bool SetAsLastSibling(InstanceID id)
		{
			var instance = GetInstance(id);

			if (instance != null)
			{
				instance.transform.SetAsLastSibling();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Close modal helper with the specified ID.
		/// </summary>
		/// <param name="id">ID.</param>
		/// <returns>true if id is not null and modal was closed; otherwise false.</returns>
		public static bool Close(ref InstanceID? id)
		{
			if (!id.HasValue)
			{
				return false;
			}

			Close(id.Value);
			id = null;

			return true;
		}

		/// <summary>
		/// Close modal helper with the specified ID.
		/// </summary>
		/// <param name="id">ID.</param>
		public static void Close(InstanceID id)
		{
			if ((Used != null) && Used.ContainsKey(id))
			{
				Used[id].OnClick.RemoveAllListeners();
				Templates.ToCache(Used[id]);
				Used.Remove(id);
			}
		}

		/// <summary>
		/// Remove template.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Used.Remove(new InstanceID(this));
			Templates.Delete(key);
		}

		/// <summary>
		/// Process the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			OnClick.Invoke();
		}
	}
}