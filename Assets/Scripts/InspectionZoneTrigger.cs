using UnityEngine;

public class InspectionZoneTrigger : MonoBehaviour
{
    [SerializeField] private Color passColor = Color.green;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name + " entered inspection zone");

        Renderer targetRenderer = other.GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            targetRenderer.material.color = passColor;
        }
    }
}