using UnityEngine;

public class SplitScreenSetup : MonoBehaviour
{
    public Camera[] playerCameras;   // assign cameras here

    void Start()
    {
        int count = playerCameras.Length;

        if (count == 1)
        {
            playerCameras[0].rect = new Rect(0, 0, 1, 1);
        }
        else if (count == 2)
        {
            playerCameras[0].rect = new Rect(0, 0, 0.5f, 1);
            playerCameras[1].rect = new Rect(0.5f, 0, 0.5f, 1);
        }
        else if (count == 3)
        {
            playerCameras[0].rect = new Rect(0, 0.5f, 1, 0.5f);
            playerCameras[1].rect = new Rect(0, 0, 0.5f, 0.5f);
            playerCameras[2].rect = new Rect(0.5f, 0, 0.5f, 0.5f);
        }
        else if (count >= 4)
        {
            playerCameras[0].rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            playerCameras[1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            playerCameras[2].rect = new Rect(0, 0, 0.5f, 0.5f);
            playerCameras[3].rect = new Rect(0.5f, 0, 0.5f, 0.5f);
        }
    }
}