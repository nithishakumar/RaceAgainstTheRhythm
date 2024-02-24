using System;
using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    bool isMoving = false;
    Vector3 origPos, targetPos;
    public float timeToMove = 0.2f;
    public LayerMask stopMovement;

    private void Update()
    {
        if (!isMoving)
        {
            
            if (Input.GetAxisRaw("Vertical") == 1 && CanMoveInDirection(Vector3.up))
            {
                StartCoroutine(MovePlayer(Vector3.up));
            }
            else if (Input.GetAxisRaw("Horizontal") == -1 && CanMoveInDirection(Vector3.left))
            {
                StartCoroutine(MovePlayer(Vector3.left));
            }
            else if (Input.GetAxisRaw("Vertical") == -1 && CanMoveInDirection(Vector3.down))
            {
                StartCoroutine(MovePlayer(Vector3.down));
            }
            else if (Input.GetAxisRaw("Horizontal") == 1 && CanMoveInDirection(Vector3.right))
            {
                StartCoroutine(MovePlayer(Vector3.right));
            }
            
        }
    }

    IEnumerator MovePlayer(Vector3 direction)
    {
        isMoving = true;

        float elapsedTime = 0;
        origPos = transform.position;
        targetPos = transform.position + direction;

        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        isMoving = false;

    }

    bool CanMoveInDirection(Vector3 direction)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + direction, 0.2f, stopMovement);
        return colliders.Length == 0;
    }
}