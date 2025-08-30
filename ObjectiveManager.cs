using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectiveManager : MonoBehaviour
{
    public GameObject keyObject;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI killCounterText;
    public AudioClip objectiveCompleteSound;
    private AudioSource audioSource;

    // Add references for your popup UI
    public GameObject objectiveCompletePanel;
    public CanvasGroup objectiveCanvasGroup;
    public float popupDuration = 2f;

    public enum GameObjectiveState
    {
        None,
        GetWeapon,
        ReachLabDoor,
        FindKey,
        OpenLabDoor,
        FindTheCureSample,
        ClearOutAreaAndSurvive
    }

    public GameObjectiveState currentState = GameObjectiveState.None;

    private bool weaponPicked = false;
    private int zombieKills = 0;
    private bool cureFound = false;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        currentState = GameObjectiveState.GetWeapon;
        SetObjective("Pick up your weapon");
        UpdateKillUI();
        if (keyObject != null)
            keyObject.SetActive(false); 
    }

    private void SetObjective(string newObjective)
    {
        objectiveText.text = "Objective: " + newObjective;
    }

    // General method to change state and show popup
    private void ChangeObjectiveState(GameObjectiveState newState, string objectiveDescription)
    {
        currentState = newState;
        SetObjective(objectiveDescription);
        StartCoroutine(ShowObjectiveCompletePopup());

    }

    public void MarkWeaponPicked()
    {
        if (currentState == GameObjectiveState.GetWeapon)
        {
            weaponPicked = true;
            ChangeObjectiveState(GameObjectiveState.ReachLabDoor, "Find the main lab gate");
        }
    }

    public void ReachedLabDoorWithoutKey()
    {
        if (currentState == GameObjectiveState.ReachLabDoor)
        {
            ChangeObjectiveState(GameObjectiveState.FindKey, "Find the key");

            if (keyObject != null)
                keyObject.SetActive(true); // Show the key now
        }
    }


    public void PickedUpKey()
    {
        if (currentState == GameObjectiveState.FindKey)
        {
            ChangeObjectiveState(GameObjectiveState.OpenLabDoor, "Open the lab door");
        }
    }

    public void OpenedLabDoor()
    {
        if (currentState == GameObjectiveState.OpenLabDoor)
        {
            ChangeObjectiveState(GameObjectiveState.FindTheCureSample, "Find the cure sample");
        }
    }

    public void FoundCureSample()
    {
        if (currentState == GameObjectiveState.FindTheCureSample)
        {
            cureFound = true;
            ChangeObjectiveState(GameObjectiveState.ClearOutAreaAndSurvive, "Clear out until the military arrives!");
        }
    }

    public bool HasCureSample()
    {
        return cureFound;
    }

    public void AddKill()
    {
        zombieKills++;
        UpdateKillUI();
    }

    private void UpdateKillUI()
    {
        if (killCounterText != null)
        {
            killCounterText.text = "KILLED: " + zombieKills;
        }
    }

    // Popup coroutine
    private IEnumerator ShowObjectiveCompletePopup()
    {
        if (objectiveCompletePanel == null || objectiveCanvasGroup == null)
            yield break;

        objectiveCompletePanel.SetActive(true);
        objectiveCanvasGroup.alpha = 0f;
        if (objectiveCompleteSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(objectiveCompleteSound);
        }

        // Fade in
        float fadeInDuration = 0.4f;
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            objectiveCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null;
        }

        // Wait while visible
        yield return new WaitForSeconds(popupDuration);

        // Fade out
        float fadeOutDuration = 0.4f;
        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            objectiveCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            yield return null;
        }

        objectiveCompletePanel.SetActive(false);
    }
}
