using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPC : Character
{
    public Collider2D[] RaycastIgnore;
    [SerializeField] float detectionRange = 10f;
    [SerializeField] Attack[] attacks;
    Vector2 _previousFaceDir = new();
    List<Character> _charactersInRange = new();
    public float DetectionRange { get { return detectionRange; } }


    protected void TryAttack()
    {
        if (_charactersInRange.Count == 0)
        {
            return;
        }
        attacks.First().Attempt(this, _charactersInRange.ToArray());
    }

    /// <summary>
    /// Sets the NPC transform rotation equivalent to it's face direction, ignoring single frame changes
    /// </summary>
    protected void TurnNPCSmooth()
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

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponentInChildren<Player>();

        if (!player)
        {
            return;
        }
        _charactersInRange.Add(player);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (!player)
        {
            return;
        }
        _charactersInRange.Remove(player);
    }
}
