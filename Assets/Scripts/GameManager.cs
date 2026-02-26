using KikiNgao.SimpleBikeControl;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject HomeScreenPanel;
    //public Animator cameraAnimator;
    public Transform cameraTransform;
    public Transform gameplayCameraTarget;
    public GameObject CycleSelectionScreen;
    public GameObject SettingsScreen;

    public Toggle nameEntryToggle;

    public TMP_InputField[] playerInputs;
    public Button[] playerAssignmentButtons;



    public InputManager inputManager;
    public InputManager GetInputManager => inputManager;

    public static GameManager Instance;

    public float delay = 2f;
    public float returnSpeed = 3f;

    public static bool GameStarted = false;
    public GameObject errorText;
    public string Player1Name;
    public string Player2Name;      
    public string Player3Name;
    public string Player4Name;

    public TMP_Text[] playerHUDNames;

    public UnityEngine.UI.Slider meterSlider;
    public TrackGenerator trackGenerator;

    public AudioSource BGM;
    public AudioSource RaceSound;
    public AudioSource WinSound;

    public AudioSource SelectSound;


    void Awake()
    {
        Instance = this;

        // FORCE find input manager in scene
        inputManager = FindObjectOfType<InputManager>(true);

        if (inputManager == null)
        {
            Debug.LogError("NO InputManager found in scene!");
        }
        else
        {
            Debug.Log("InputManager found: " + inputManager.name);
        }
    }

    public void StartGame()
    {
        BGM.Stop();
        RaceSound.Play();
        GameStarted = true;
        HomeScreenPanel.SetActive(false);
        CycleSelectionScreen.SetActive(false);

        //nameEntryToggle.onValueChanged.AddListener(OnToggleChanged(true));
        //OnToggleChanged(nameEntryToggle.isOn);

        //Invoke(nameof(PlayCameraAnim), delay);


    }

    public void OnToggleChanged()
    {
        bool allowCustomNames = nameEntryToggle.isOn;

        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (playerInputs[i] == null) continue;

            // Enable/disable typing
            playerInputs[i].interactable = allowCustomNames;

            // If toggle OFF → assign default names
            if (!allowCustomNames)
            {
                playerInputs[i].text = "Player " + (i + 1);
            }
            else
            {
                playerInputs[i].text = " ";
            }
        }
        Player1Name = playerInputs[0].text;
        Player2Name = playerInputs[1].text;
        Player3Name = playerInputs[2].text;
        Player4Name = playerInputs[3].text;

        playerAssignmentButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = playerInputs[0].text;
        playerAssignmentButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = playerInputs[1].text;
        playerAssignmentButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = playerInputs[2].text;
        playerAssignmentButtons[3].GetComponentInChildren<TextMeshProUGUI>().text = playerInputs[3].text;


        UpdateHUDNames();
    }

    //void PlayCameraAnim()
    //{
    //    // Enable animator and play intro animation
    //    cameraAnimator.enabled = true;
    //    cameraAnimator.Play("RaceTrans", 0, 0f);   // force start from frame 0

    //    StartCoroutine(WaitThenReturn());
    //}

    //IEnumerator WaitThenReturn()
    //{
    //    // Wait one frame so Animator updates state
    //    yield return null;

    //    // Get current animation length correctly
    //    AnimatorStateInfo info = cameraAnimator.GetCurrentAnimatorStateInfo(0);
    //    float len = info.length;

    //    // Wait for animation to finish
    //    yield return new WaitForSeconds(len);

    //    //  VERY IMPORTANT: completely release Animator control
    //    cameraAnimator.Rebind();
    //    cameraAnimator.Update(0f);
    //    cameraAnimator.enabled = false;

    //    // Smoothly move camera back to gameplay target
    //    while (Vector3.Distance(cameraTransform.position, gameplayCameraTarget.position) > 0.01f)
    //    {
    //        cameraTransform.position = Vector3.Lerp(
    //            cameraTransform.position,
    //            gameplayCameraTarget.position,
    //            Time.deltaTime * returnSpeed);

    //        cameraTransform.rotation = Quaternion.Lerp(
    //            cameraTransform.rotation,
    //            gameplayCameraTarget.rotation,
    //            Time.deltaTime * returnSpeed);

    //        yield return null;
    //    }

    //    // Snap final exact transform
    //    cameraTransform.position = gameplayCameraTarget.position;
    //    cameraTransform.rotation = gameplayCameraTarget.rotation;
    //}

    public void GoToCycleSelectionScreen()
    {
        bool allValid = true;

        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (playerInputs[i] == null) continue;

            // Trim removes whitespace
            string name = playerInputs[i].text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                allValid = false;
                break;
            }
        }

        if (!allValid)
        {
            if (errorText != null)
            {
                errorText.SetActive(true);
                StopAllCoroutines();                     // prevent multiple timers
                StartCoroutine(HideErrorAfterDelay(2f)); // hide after 2 sec
            }
            return;
        }

        // Valid → hide error + continue
        if (errorText != null)
            errorText.SetActive(false);

        Player1Name = playerInputs[0].text;
        Player2Name = playerInputs[1].text;
        Player3Name = playerInputs[2].text;
        Player4Name = playerInputs[3].text;

        playerAssignmentButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = playerInputs[0].text;
        playerAssignmentButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = playerInputs[1].text;
        playerAssignmentButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = playerInputs[2].text;
        playerAssignmentButtons[3].GetComponentInChildren<TextMeshProUGUI>().text = playerInputs[3].text;

        UpdateHUDNames();

        CycleSelectionScreen.SetActive(true);
        SettingsScreen.SetActive(false);
        HomeScreenPanel.SetActive(false);
    }

    IEnumerator HideErrorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (errorText != null)
            errorText.SetActive(false);
    }

    public void GoToSettingsScreen()
    {
        HomeScreenPanel.SetActive(false);
        CycleSelectionScreen.SetActive(false);
        SettingsScreen.SetActive(true);
    }

    public void CloseSettingsScreen()
    {
        HomeScreenPanel.SetActive(true);
        CycleSelectionScreen.SetActive(false);
        SettingsScreen.SetActive(false);

        // BUILD TRACK USING SLIDER VALUE
        if (trackGenerator != null && meterSlider != null)
        {
            float selectedDistance = meterSlider.value;
            trackGenerator.BuildTrack(selectedDistance);
        }

    }

    public void SetTrackLength()
    {

    }

    void UpdateHUDNames()
    {
        if (playerHUDNames == null || playerHUDNames.Length < 4) return;

        playerHUDNames[0].text = Player1Name;
        playerHUDNames[1].text = Player2Name;
        playerHUDNames[2].text = Player3Name;
        playerHUDNames[3].text = Player4Name;
    }
}