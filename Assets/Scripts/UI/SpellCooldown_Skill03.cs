using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.NetworkInformation;
using System;

public class SpellCooldown_Skill03 : MonoBehaviour
{
    PlayerController playerController;

    [SerializeField]
    private Image imageCooldown;
    [SerializeField]
    private TMP_Text textCoolDown;
    [SerializeField]
    private Image imageEdge;
    [SerializeField]
    private Image skill03Icon;
    [SerializeField]
    private Sprite spriteOriginal;
    [SerializeField]
    private Sprite spriteClearCD;

    //variables for cooldownTimer
    private bool isCooldown = false;
    private float cooldownTime = 5.0f;
    private float cooldownTimer = 0.0f;
    private int buttonState = 1;
    
    
    private void Awake()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("No Player found in the scene");
        }
        playerController = player.GetComponent<PlayerController>();

        textCoolDown.gameObject.SetActive(false);
        imageEdge.gameObject.SetActive(false);
        imageCooldown.fillAmount = 0.0f;

        //CallOnEnableMethods();
        EventManager.Instance.AddListener(CustomEventType.SpellSkill03, UseSpell);
        EventManager.Instance.AddListener(CustomEventType.Skill03ClearCDSuccess, ChangeBtnState);

        SetSkill03ButtonImage();
        
    }

    //private void CallOnEnableMethods()
    //{
    //    playerController.SpellSkill03.AddListener(UseSpell);
    //    playerController.skill03ClearCDSuccess.AddListener(ChangeBtnState);
    //}

    void OnDestroy()
    {
        EventManager.Instance.RemoveListener(CustomEventType.SpellSkill03, UseSpell);
        EventManager.Instance.RemoveListener(CustomEventType.Skill03ClearCDSuccess, ChangeBtnState);
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
        buttonState = 1;
        SetSkill03ButtonImage();

        if (isCooldown)
        {
            //������ܼ�
        }
        else
        {
            isCooldown = true;
            textCoolDown.gameObject.SetActive(true);

            imageEdge.gameObject.SetActive(true);
            cooldownTimer = cooldownTime;
            
        }

    }

    private void ChangeBtnState()
    {
        buttonState = 2;
        SetSkill03ButtonImage();
    }



    private void SetSkill03ButtonImage()
    {
        Image IconImage = skill03Icon.GetComponent<Image>();

        if (buttonState == 1)
        {
            IconImage.sprite = spriteOriginal;

        }else if (buttonState == 2) 
        {
            IconImage.sprite = spriteClearCD;
        }

        if (IconImage = null)
        {
            IconImage.sprite = spriteOriginal;
        }
        
        

    }



}
