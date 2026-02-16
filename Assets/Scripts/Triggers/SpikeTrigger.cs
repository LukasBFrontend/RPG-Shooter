using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{
    [SerializeField] Animator[] animators;
    [Range(1, 10)]
    [SerializeField] int damage;
    List<Character> _charactersInCollider = new();
    bool _sequenceIsActive = false;

    void Update()
    {
        if (_sequenceIsActive || _charactersInCollider.Count == 0)
        {
            return;
        }

        StartCoroutine(ActivationSequence());
        _sequenceIsActive = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<Character>(out var character))
        {
            return;
        }

        _charactersInCollider.Add(character);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<Character>(out var character))
        {
            return;
        }

        _charactersInCollider.Remove(character);
    }

    IEnumerator ActivationSequence()
    {
        const float DURATION = 1.5f;
        const float DAMAGE_FRAME = .85f;
        const float ACTIVATION_FRAME = .5f;
        float _elapsed = 0;
        bool hasActivated = false;
        bool hasDoneDamage = false;

        while (_elapsed <= DURATION)
        {
            if (!hasActivated && _elapsed >= ACTIVATION_FRAME)
            {
                foreach (Animator animator in animators)
                {
                    animator.SetBool("IsTriggered", true);
                }

                hasActivated = true;
            }
            else if (!hasDoneDamage && _elapsed >= DAMAGE_FRAME)
            {
                foreach (Character character in _charactersInCollider)
                {
                    character.TakeDamage(damage);
                }

                hasDoneDamage = true;
            }

            yield return new WaitForEndOfFrame();
            _elapsed += Time.deltaTime;
            foreach (Animator animator in animators)
            {
                animator.SetBool("IsTriggered", false);
            }
        }

        _sequenceIsActive = false;
    }
}
