using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.NetworkInformation;


public class SpellCooldown : MonoBehaviour
{
    PlayerController playerController;


    [SerializeField]
    private Image imageCooldown;
    [SerializeField]
    private TMP_Text textCoolDown;
    [SerializeField]
    private Image imageEdge;

    //variables for cooldownTimer
    private bool isCooldown = false;
    private float cooldownTime = 5.0f;
    private float cooldownTimer = 0.0f;


    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("No Player found in the scene");
        }
        playerController = player.GetComponent<PlayerController>();

    }
    // Start is called before the first frame update
    void Start()
    {
        textCoolDown.gameObject.SetActive(false);
        imageEdge.gameObject.SetActive(false);
        imageCooldown.fillAmount = 0.0f;
    }

    private void OnEnable()
    {
        playerController.SpellSkill01.AddListener(UseSpell);
    }

  


    // Update is called once per frame
    void Update()
    {
        /* 
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSpell();
        }
        */
        if (isCooldown)
        {
            ApplyCooldown();
        }
    }

    void ApplyCooldown()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer < 0.0f)
        {
            isCooldown = false;
            textCoolDown.gameObject.SetActive(false);

            imageEdge.gameObject.SetActive(false);
            imageCooldown.fillAmount = 0.0f;
        }
        else
        {
            textCoolDown.text = Mathf.RoundToInt(cooldownTimer).ToString();
            imageCooldown.fillAmount = cooldownTimer / cooldownTime;

            imageEdge.transform.localEulerAngles = new Vector3(0,0,360.0f*(cooldownTimer / cooldownTime));
        }
    }

    public void UseSpell()
    {
        if (isCooldown)
        {
            //µã»÷¼¼ÄÜ¼ü
        }
        else
        {
            isCooldown = true;
            textCoolDown.gameObject.SetActive(true);

            imageEdge.gameObject.SetActive(true);
            cooldownTimer = cooldownTime;
            
        }

    }
}
