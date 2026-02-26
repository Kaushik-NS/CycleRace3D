using UnityEngine;
using System.Collections.Generic;

public class FollowCamera : MonoBehaviour
{
    public List<Transform> racers = new List<Transform>();

    public Vector3 offset = new Vector3(0, 6, -10);
    public float smoothSpeed = 5f;

    Transform currentLeader;

    void LateUpdate()
    {
        if (racers == null || racers.Count == 0) return;

        currentLeader = GetLeader();
        if (currentLeader == null) return;

        // Position behind leader (relative to its facing direction)
        Vector3 desiredPos = currentLeader.position +
                             currentLeader.rotation * offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            Time.deltaTime * smoothSpeed);

        // Look forward but keep camera upright
        Vector3 lookDir = currentLeader.forward;
        lookDir.y = 0f;
        lookDir.Normalize();
        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rot,
                Time.deltaTime * smoothSpeed);
        }
    }

    Transform GetLeader()
    {
        Transform leader = racers[0];
        float bestX = leader.position.x;

        for (int i = 1; i < racers.Count; i++)
        {
            // moving toward -X → smaller X means ahead
            if (racers[i].position.x < bestX)
            {
                bestX = racers[i].position.x;
                leader = racers[i];
            }
        }

        return leader;
    }
}