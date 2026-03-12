using UnityEngine;

public class SpikeBallSlotTrigger : MonoBehaviour
{
    const float FUSE_THRESHOLD = .25f;
    [SerializeField] Animator animator;
    SpikeBall _spikeBall;
    bool _isLocked = false;

    void Update()
    {
        if (_isLocked || _spikeBall == null)
        {
            return;
        }
        float _dist = Vector2.Distance(transform.position, _spikeBall.transform.position);

        if (_dist < FUSE_THRESHOLD)
        {
            Destroy(_spikeBall.gameObject);
            animator.SetBool("IsFusing", true);
            _isLocked = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<SpikeBall>(out var spikeBall))
        {
            _spikeBall = spikeBall;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<SpikeBall>(out var spikeBall))
        {
            _spikeBall = null;
        }
    }
}
