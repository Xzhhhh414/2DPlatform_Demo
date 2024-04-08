using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellCooldown : MonoBehaviour
{
    [SerializeField]
    private Image imageCooldown;
    [SerializeField]
    private TMP_Text textCoolDown;
    [SerializeField]
    private Image imageEdge;

    //variables for cooldownTimer
    private bool isCooldown = false;
    private float cooldownTime = 10.0f;
    private float cooldownTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        textCoolDown.gameObject.SetActive(false);
        imageEdge.gameObject.SetActive(false);
        imageCooldown.fillAmount = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSpell();
        }
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
