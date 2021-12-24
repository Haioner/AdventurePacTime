using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector2 initialPos;
    public Rigidbody2D rb { get; private set; }
    Player_hook hook;

    private void Start()
    {
        hook = GetComponent<Player_hook>();
        rb = GetComponent<Rigidbody2D>();
        initialPos = transform.position;
    }

    public void ResetState()
    {
        //Reset player position
        transform.position = initialPos;
        gameObject.SetActive(true);
        hook.Detatch();
        rb.freezeRotation = false;
    }
}
