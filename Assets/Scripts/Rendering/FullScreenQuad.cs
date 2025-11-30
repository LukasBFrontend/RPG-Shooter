using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FullscreenQuad : MonoBehaviour
{
    void Start()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        transform.localScale = new Vector3(width, height, 1f);

        // Put quad just in front of the camera
        transform.position = cam.transform.position + cam.transform.forward * 1f;
        transform.rotation = cam.transform.rotation;
    }
}
