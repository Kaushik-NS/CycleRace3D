using UnityEngine;
using UnityEngine.UI;

public class CycleAssignmentManager : MonoBehaviour
{
    public Button[] playerButtons;
    public Button[] cycleButtons;

    public Button startButton;

    int selectedPlayer = -1;
    int[] playerToCycle;   // stores mapping

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