using UnityEngine;

public class FollowCycle : MonoBehaviour
{
    public Transform target;     // assign CameraMount
    public float smooth = 8f;

    void LateUpdate()
    {
        if (!target) return;

        transform.position = Vector3.Lerp(
            transform.position,
            target.position,
            Time.deltaTime * smooth);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            target.rotation,
            Time.deltaTime * smooth);
    }
}