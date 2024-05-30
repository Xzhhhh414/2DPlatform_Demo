using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;
    public Canvas gameCanvas;
    public GameObject NPCHealthBarPrefab;

    public void Start()
    {
        gameCanvas = FindObjectOfType<Canvas>();


    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<GameObject>(CustomEventType.MonsterSpawned, SpawnNPCHealthBar);
        EventManager.Instance.AddListener<GameObject, int>(CustomEventType.CharacterDamaged, CharacterTookDamage);
        EventManager.Instance.AddListener<GameObject, int>(CustomEventType.CharacterHealed, CharacterHealed);

    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<GameObject>(CustomEventType.MonsterSpawned, SpawnNPCHealthBar);
        EventManager.Instance.RemoveListener<GameObject, int>(CustomEventType.CharacterDamaged, CharacterTookDamage);
        EventManager.Instance.RemoveListener<GameObject, int>(CustomEventType.CharacterHealed, CharacterHealed);

    }


    public void CharacterTookDamage(GameObject character, int damageReceived)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();
        tmpText.text = damageReceived.ToString();
        HealthText healthText = tmpText.GetComponent<HealthText>();
        if (healthText != null)
        {
            healthText.followingObject = character;
            if (character.CompareTag("Player"))
            {
                healthText.textColor = Color.red;
            }else
            {
                healthText.textColor = Color.white;
            }
        }

        if (character.CompareTag("Monster"))
        {
            Monster monster = character.GetComponent<Monster>();
            MonsterHealthBar monsterHealthBar = monster.healthBar.GetComponent<MonsterHealthBar>();
            if (monsterHealthBar != null)
            {
                monsterHealthBar.GetDamaged(damageReceived);
            }
        }
  
    }

    public void CharacterHealed(GameObject character, int healthRestored)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = healthRestored.ToString();
    }

    public void OnExitGame(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
                Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif

            #if (UNITY_EDITOR)
                UnityEditor.EditorApplication.isPlaying = false;
            #elif (UNITY_STANDALONE)
                Application.Quit();
            #elif(UNITY_WEBGL)
                SceneManager.LoadScene("QuitScene");
            #endif

        }
    }


    public void SpawnNPCHealthBar(GameObject character)
    {
        Vector3 spawnPositionHealthBar = Camera.main.WorldToScreenPoint(character.transform.position);
        GameObject temp_NPCHealthBar = Instantiate(NPCHealthBarPrefab, spawnPositionHealthBar, Quaternion.identity, gameCanvas.transform);
        temp_NPCHealthBar.SetActive(false);

        MonsterHealthBar NPCHealthBar = temp_NPCHealthBar.GetComponent<MonsterHealthBar>();
        NPCHealthBar.followingObject = character;

        Monster monster = character.GetComponent<Monster>();
        if (monster != null)
        {
            monster.healthBar = temp_NPCHealthBar;
        }

    }
}
