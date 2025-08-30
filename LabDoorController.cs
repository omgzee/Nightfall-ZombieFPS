using UnityEngine;
using UnityEngine.AI;

public class LabDoorController : MonoBehaviour
{
    public ObjectiveManager objectiveManager;
    public Animator labDoorAnimator;

    public GameObject keyRemote;
    public GameObject keyPromptUI;

    public GameObject doorHintUI;         // ❌ Shown when door is locked and no key
    public GameObject openDoorPromptUI;   // ✅ New UI: “Press O to open door”

    public GameObject doorObject;         // 🔧 Assign the Door GameObject here (the one with NavMeshObstacle)

    private bool hasKeyRemote = false;
    private bool isNearKeyRemote = false;
    private bool isNearLabDoor = false;

    private NavMeshObstacle navObstacle;

    void Start()
    {
        keyPromptUI.SetActive(false);
        doorHintUI.SetActive(false);
        openDoorPromptUI.SetActive(false);

        if (doorObject != null)
        {
            navObstacle = doorObject.GetComponent<NavMeshObstacle>();
            if (navObstacle != null)
                navObstacle.enabled = true; // Block the path at start
        }
    }

    void Update()
    {
        if (isNearKeyRemote && Input.GetKeyDown(KeyCode.E))
        {
            hasKeyRemote = true;
            keyRemote.SetActive(false);
            keyPromptUI.SetActive(false);
            Debug.Log("Key Remote picked up!");

            objectiveManager.PickedUpKey(); // ✅ FSM progress
        }

        if (isNearLabDoor)
        {
            if (hasKeyRemote)
            {
                openDoorPromptUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.O))
                {
                    labDoorAnimator.SetBool("isOpen", true);
                    openDoorPromptUI.SetActive(false);

                    if (navObstacle != null)
                        navObstacle.enabled = false; // Allow enemies through

                    objectiveManager.OpenedLabDoor(); // ✅ FSM progress
                }
            }
            else
            {
                doorHintUI.SetActive(true);

                // ✅ Only call this if it's the correct objective
                if (objectiveManager.currentState == ObjectiveManager.GameObjectiveState.ReachLabDoor)
                {
                    objectiveManager.ReachedLabDoorWithoutKey();
                }
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KeyRemoteZone"))
        {
            isNearKeyRemote = true;
            keyPromptUI.SetActive(true);
        }

        if (other.CompareTag("LabDoorZone"))
        {
            isNearLabDoor = true;

            if (!hasKeyRemote)
                doorHintUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("KeyRemoteZone"))
        {
            isNearKeyRemote = false;
            keyPromptUI.SetActive(false);
        }

        if (other.CompareTag("LabDoorZone"))
        {
            isNearLabDoor = false;
            doorHintUI.SetActive(false);
            openDoorPromptUI.SetActive(false);
        }
    }
}
