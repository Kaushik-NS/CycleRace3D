using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    public GameObject trackBlockPrefab;
    public GameObject finishLinePrefab;

    public float blockLength = 30f;
    public Transform startPoint;

    GameObject currentFinish;

    public void BuildTrack(float targetDistance)
    {
        if (startPoint == null || trackBlockPrefab == null)
        {
            Debug.LogError("TrackGenerator missing references!");
            return;
        }

        // Clear old track
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        if (currentFinish != null)
            Destroy(currentFinish);

        int blocksNeeded = Mathf.CeilToInt(targetDistance / blockLength);

        GameObject lastBlock = null;

        // Spawn blocks along -X
        for (int i = 0; i < blocksNeeded; i++)
        {
            Vector3 pos = startPoint.position + new Vector3(-i * blockLength, 0, 0);
            lastBlock = Instantiate(trackBlockPrefab, pos, startPoint.rotation, transform);
        }

        //  SNAP FINISH TO END OF LAST BLOCK
        if (lastBlock != null && finishLinePrefab != null)
        {
            Renderer r = lastBlock.GetComponentInChildren<Renderer>();

            Vector3 finishPos;

            if (r != null)
            {
                // Bounds.min.x is the far end in -X direction
                finishPos = new Vector3(
                    r.bounds.min.x,
                    startPoint.position.y,
                    startPoint.position.z);
            }
            else
            {
                // fallback if no renderer found
                finishPos = lastBlock.transform.position + new Vector3(-blockLength, 0, 0);
            }

            currentFinish = Instantiate(
                finishLinePrefab,
                finishPos,
                startPoint.rotation,
                transform);
        }
    }
}