using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorwayTrigger : DirectionalTrigger
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<Player>(out var player))
        {
            return;
        }
        Vector3 _intersect = Collider.bounds.ClosestPoint(other.transform.position);

        Vector2 _relative = _intersect - Collider.bounds.center;
        Vector2 _offsetA = -_relative / 4f;
        Vector2 _offsetB = -_relative * 2.1f;

        Vector2 _pointA;
        Vector2 _pointB;

        if (Axis == Axis.Horizontal)
        {
            _pointA = new(_intersect.x + _offsetA.x, Collider.bounds.center.y);
            _pointB = _pointA + new Vector2(_offsetB.x, 0);
        }
        else
        {

            _pointA = new(Collider.bounds.center.x, _intersect.y + _offsetA.y);
            _pointB = _pointA + new Vector2(0, _offsetB.y);
        }

        StartCoroutine(MoveSequence(player, new() { _pointA, _pointB }, Axis == Axis.Horizontal));
    }

    IEnumerator MoveSequence(Character character, List<Vector2> path, bool usingSpriteCenter = false)
    {
        GameState.InputDisabled = true;
        float _totalDistance = Vector2.Distance(path[0], path[^1]);

        while (path.Count > 0)
        {
            Vector2 _characterPos = usingSpriteCenter
                ? character.SpriteCenter()
                : character.ColliderCenter();
            ;
            Vector2 _target = path[0];

            float _t = Vector2.Distance(_characterPos, path[^1]) / _totalDistance;
            character.Controller.MovespeedMultiplier = Mathf.Lerp(.75f, .6f, _t * _t);
            character.Controller.Move(_target - _characterPos);

            if (Vector2.Distance(_characterPos, _target) < .075f)
            {
                path.RemoveAt(0);
            }

            yield return new WaitForEndOfFrame();
        }

        GameState.InputDisabled = false;
        character.Controller.MovespeedMultiplier = 1f;
    }
}
