using UnityEngine;
using System.Collections.Generic;

public class MultiCycleCamera : MonoBehaviour
{
    public List<Transform> targets = new List<Transform>();

    public float followDistance = 12f;
    public float height = 6f;
    public float smoothSpeed = 5f;
    public float lookSmooth = 5f;

    void LateUpdate()
    {
        if (targets.Count == 0) return;

        Vector3 center = GetCenterPoint();
        Vector3 forward = GetAverageForward();

        // Position camera behind racers
        Vector3 desiredPos = center
                           - forward * followDistance
                           + Vector3.up * height;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            Time.deltaTime * smoothSpeed);

        // Look slightly ahead of racers
        Vector3 lookPoint = center + forward * 5f;

        Quaternion targetRot = Quaternion.LookRotation(lookPoint - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * lookSmooth);
    }

    Vector3 GetCenterPoint()
    {
        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);

        foreach (var t in targets)
            bounds.Encapsulate(t.position);

        return bounds.center;
    }

    Vector3 GetAverageForward()
    {
        Vector3 sum = Vector3.zero;

        foreach (var t in targets)
            sum += t.forward;

        return (sum / targets.Count).normalized;
    }
}