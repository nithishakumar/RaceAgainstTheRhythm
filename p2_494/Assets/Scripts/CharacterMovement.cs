using System;
using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float movementSpeed;
    public Transform movePoint;
    // Layer with all colliders/boundaries that player should not run into
    public LayerMask stopMovement;
    public float tileSize = 1f;

    private bool isMoving = false;

    void Update()
    {
        if (!isMoving)
        {
            HandleInput();
        }
        else
        {
            MoveToPoint();
        }
    }

    void HandleInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(horizontalInput) == 1f)
        {
            if (CanMoveInDirection(Vector3.right * horizontalInput))
            {
                MoveToAdjacentTile(Vector3.right * horizontalInput);
            }
        }
        else if (Mathf.Abs(verticalInput) == 1f)
        {
            if (CanMoveInDirection(Vector3.up * verticalInput))
            {
                MoveToAdjacentTile(Vector3.up * verticalInput);
            }
        }
    }

    bool CanMoveInDirection(Vector3 direction)
    {
        Collider[] colliders = Physics.OverlapSphere(movePoint.position + direction, 0.2f, stopMovement);
        return colliders.Length == 0;
    }

    void MoveToAdjacentTile(Vector3 direction)
    {
        movePoint.position += direction * tileSize;
        isMoving = true;
    }

    void MoveToPoint()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, movementSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            isMoving = false;
        }
    }
}