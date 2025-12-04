using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FullscreenQuad : MonoBehaviour
{
    void Start()
    {
        Camera _cam = Camera.main;
        float _height = 2f * _cam.orthographicSize;
        float _width = _height * _cam.aspect;

        transform.localScale = new Vector3(_width, _height, 1f);
        transform.SetPositionAndRotation(_cam.transform.position + _cam.transform.forward * 1f, _cam.transform.rotation);
    }
}
