using UnityEngine;

public class NPC : Character
{
    Vector2 _previousFaceDir = new();
    public void RenderFaceDirection()
    {
        bool _isFaceDirSame = _previousFaceDir == FaceDir;
        _previousFaceDir = FaceDir;

        if (!_isFaceDirSame)
        {
            return;
        }

        float _angle = -Mathf.Round(Mathf.Atan2(FaceDir.y, FaceDir.x) * Mathf.Rad2Deg + 90);

        transform.rotation = Quaternion.Euler(new(0, 0, Mathf.Round(Mathf.Atan2(FaceDir.y, FaceDir.x) * Mathf.Rad2Deg + 90)));
        /* Pixelate pixelate = GetComponentInChildren<Pixelate>();
        if (pixelate)
        {
            pixelate.Rotation = Quaternion.Euler(new(0, 0, _angle));
        }
        else
        {
            Debug.LogError($"No pixelate script found on {gameObject.name}, could not render face direction.");
        } */

    }
}
