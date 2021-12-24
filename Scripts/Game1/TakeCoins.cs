using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeCoins : MonoBehaviour
{
    [Header("UI")]
    public Transform targetPos;
    public float speed = 5;
    bool canMove;
    public GameObject particle;
    Vector2 initialPosition;
    Vector2 initialScale;

    [Header("Type")]
    public bool isHotChocolate = false;
    public int coinValue;
    public AudioSource _audio;

    Manager manager;

    private void Awake()
    {
        manager = FindObjectOfType<Manager>();
        initialPosition = transform.position;
        initialScale = transform.localScale;
    }

    private void Update()
    {
        MoveToUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Jake"))
        {
            manager.CoinChecker();
            float randPitch = Random.Range(.9f, 1.1f);
            _audio.pitch = randPitch;
            _audio.Play();
            canMove = true;
        }
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
        transform.localScale = initialScale;
        gameObject.SetActive(true);
    }

    void MoveToUI()
    {
        if (canMove)
        {
            transform.position = Vector2.Lerp(transform.position, targetPos.position, Time.deltaTime * speed);
            Vector2 tempScale = new Vector2(0.61f, 0.61f);
            transform.localScale = Vector2.Lerp(transform.localScale, tempScale, Time.deltaTime * speed);
        }

        float dist = Vector2.Distance(transform.position, targetPos.position);
        if(dist < .5f)
        {
            if (isHotChocolate)
            {
                manager.SetFightingMode();
            }

            Instantiate(particle, targetPos);
            manager.SetCoins(coinValue);
            gameObject.SetActive(false);
            manager.CoinChecker();
            canMove = false;
        }
    }

}
