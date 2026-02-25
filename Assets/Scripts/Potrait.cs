using UnityEngine;

public class Potrait : MonoBehaviour
{
    public Camera[] cams;

    [Range(0.05f, 0.4f)]
    public float height = 0.12f;   //  THIN STRIP (try 0.12–0.22)

    public float topMargin = 0.01f;

    void Start()
    {
        int count = cams.Length;
        if (count == 0) return;

        float width = 1f / count;
        float y = 1f - height - topMargin;

        for (int i = 0; i < count; i++)
        {
            if (!cams[i]) continue;
            cams[i].rect = new Rect(i * width, y, width, height);
        }
    }
}