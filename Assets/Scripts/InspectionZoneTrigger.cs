using UnityEngine;

public class InspectionZoneTrigger : MonoBehaviour
{
    [SerializeField] private Color passColor = Color.green;
    [SerializeField] private string targetTag = "TestObject";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag))
        {
            return;
        }

        Debug.Log(other.gameObject.name + " entered inspection zone");

        Renderer targetRenderer = other.GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            targetRenderer.material.color = passColor;
        }
    }
}