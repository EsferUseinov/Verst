using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;

public class WristAnchor : MonoBehaviour
{
    [SerializeField, Interface(typeof(IHand))] private Object hand;
    [SerializeField] private GameObject content;
    [SerializeField] private Vector3 positionOffset = new Vector3(0f, 0.025f, -0.04f);
    [SerializeField] private Vector3 rotationOffset;

    private IHand trackedHand;

    private void Awake()
    {
        trackedHand = hand as IHand;
    }

    private void LateUpdate()
    {
        Pose wrist = default;
        bool tracked = trackedHand != null && trackedHand.IsTrackedDataValid
            && trackedHand.GetJointPose(HandJointId.HandWristRoot, out wrist);
        if (content.activeSelf != tracked)
        {
            content.SetActive(tracked);
        }

        if (tracked)
        {
            transform.SetPositionAndRotation(wrist.position + wrist.rotation * positionOffset,
                wrist.rotation * Quaternion.Euler(rotationOffset));
        }
    }
}
