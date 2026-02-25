using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CycleAssignmentManager : MonoBehaviour
{
    public Button[] playerButtons;
    public Button[] cycleButtons;

    public Button startButton;

    public GameObject[] playerObjects;     // player characters in scene
    public Transform[] cycleSeatMounts;    // drag SeatMount of each cycle here

    int selectedPlayer = -1;
    int[] playerToCycle;   // stores mapping



    public GameManager GM;

    void Start()
    {
        startButton.enabled = false;
        playerToCycle = new int[playerButtons.Length];

        for (int i = 0; i < playerToCycle.Length; i++)
            playerToCycle[i] = -1;

        // Assign button listeners
        for (int i = 0; i < playerButtons.Length; i++)
        {
            int index = i;
            playerButtons[i].onClick.AddListener(() => SelectPlayer(index));
        }

        for (int i = 0; i < cycleButtons.Length; i++)
        {
            int index = i;
            cycleButtons[i].onClick.AddListener(() => SelectCycle(index));
        }


    }


    void SelectPlayer(int playerIndex)
    {
        // Ignore if already assigned
        if (playerToCycle[playerIndex] != -1) return;

        selectedPlayer = playerIndex;
        Debug.Log("Selected Player: " + (playerIndex + 1));
    }

    void SelectCycle(int cycleIndex)
    {
        if (selectedPlayer == -1) return;

        // Assign mapping
        playerToCycle[selectedPlayer] = cycleIndex;

        Debug.Log("Player " + (selectedPlayer + 1) +
                  " got Cycle " + (cycleIndex + 1));

        // Disable chosen player button
        playerButtons[selectedPlayer].interactable = false;

        // Disable chosen cycle button
        cycleButtons[cycleIndex].interactable = false;

        // ⭐ MOUNT PLAYER TO BIKE
        GameObject playerObj = playerObjects[selectedPlayer];
        Transform seatMount = cycleSeatMounts[cycleIndex];

        if (playerObj != null && seatMount != null)
        {
            // Find SeatingPosition inside SeatMount
            Transform seatingPos = seatMount.Find("SeatingPosition");

            // Parent to SeatMount
            playerObj.transform.SetParent(seatMount);

            if (seatingPos != null)
            {
                playerObj.transform.position = seatingPos.position;
                playerObj.transform.rotation = seatingPos.rotation;
            }
            else
            {
                // fallback
                playerObj.transform.localPosition = Vector3.zero;
                playerObj.transform.localRotation = Quaternion.identity;
            }
        }

        // Reset selection
        selectedPlayer = -1;

        CheckAllAssigned();
    }

    void CheckAllAssigned()
    {
        for (int i = 0; i < playerToCycle.Length; i++)
        {
            if (playerToCycle[i] == -1)
            {
                if (startButton != null)
                    startButton.enabled = false;
                return;
            }
        }

        // If reached here  all assigned
        if (startButton != null)
            startButton.enabled = true;
    }
}