using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform m_TrackingTarget;
    public float     m_RotationSpeed = 2.0f;
    public float     m_PitchRot = 30;
    public float     m_YOffset = 5;
    public float     m_ZOffset = 15;

    private Quaternion m_OffsetRot;

    public void OnSetup()
    {
        m_OffsetRot = Quaternion.Euler(m_PitchRot, 0, 0);
    }

    public void OnUpdate()
    {
        Vector3 CurrentDir  = Vector3.Normalize(Flatten(transform.position));
        Vector3 TrackingDir = Vector3.Normalize(Flatten(m_TrackingTarget.position));

        Quaternion CurrentRot = Quaternion.LookRotation(-CurrentDir);
        Quaternion TargetRot  = Quaternion.LookRotation(-TrackingDir);
        Quaternion NewRot     = Quaternion.Lerp(CurrentRot, TargetRot, Time.deltaTime * m_RotationSpeed);

        Vector3 NewPos = (Vector3.Normalize(NewRot * Vector3.back) * m_ZOffset) + new Vector3(0, m_YOffset, 0);

        transform.position = NewPos;
        transform.rotation = NewRot * m_OffsetRot;
    }

    static Vector3 Flatten(Vector3 Value)
    {
        return new Vector3(Value.x, 0, Value.z);
    }
}
