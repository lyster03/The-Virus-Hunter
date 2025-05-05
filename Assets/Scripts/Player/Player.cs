using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Rigidbody2D rb;

    [Header("References")]
    public Camera cam;

    private Vector2 movement;
    public Vector2 mousePos;


    void Update()
    {
        
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;


        if (cam != null)
        {
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    void FixedUpdate()
    {

        Vector2 targetPos = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
        
        rb.MovePosition(targetPos);
       




    }
}
