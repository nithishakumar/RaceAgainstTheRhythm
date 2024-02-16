using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    Rigidbody rb;
    public float moveSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 currInput = GetInput();
        rb.velocity = currInput * moveSpeed;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("space");
            Debug.Log(transform.forward);
            rb.velocity = transform.forward * moveSpeed;
        }
    }

    Vector2 GetInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        return new Vector2(horizontalInput, verticalInput);
    }
}
