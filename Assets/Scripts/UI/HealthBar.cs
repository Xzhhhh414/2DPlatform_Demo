using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Net.NetworkInformation;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text healthBarText;


    Damageable playerDamageable;


    private void Awake()
    {
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        //if (player == null)
        //{
        //    Debug.Log("No Player found in the scene");
        //}
        //playerDamageable = player.GetComponent<Damageable>();
    }


    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("No Player found in the scene");
        }
        playerDamageable = player.GetComponent<Damageable>();


        healthSlider.value = CalculateSliderPercentage(playerDamageable.Health , playerDamageable .MaxHealth);
        healthBarText.text = playerDamageable.Health + " / " + playerDamageable.MaxHealth;

        CallOnEnableMethods();
    }

    private void CallOnEnableMethods()
    {
        playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    //private void OnEnable()
    //{
    //    
    //}

    private void OnDisable()
    {
        playerDamageable.healthChanged.RemoveListener(OnPlayerHealthChanged);
    }

    private float CalculateSliderPercentage(float currentHealth, float maxHealth)
    {
        return currentHealth / maxHealth;
    }

    private void OnPlayerHealthChanged(int newHealth, int maxHealth)
    {
        healthSlider.value = CalculateSliderPercentage(newHealth, maxHealth);
        healthBarText.text = "HP" + newHealth + " / " + maxHealth;
    }


}
