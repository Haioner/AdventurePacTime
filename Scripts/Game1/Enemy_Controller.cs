using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy_Controller : MonoBehaviour
{
    public int points;
    Manager manager;
    Vector2 initialPos;

    [Header("Home")]
    public Transform homeDoor;
    public float homeCooldown;
    float initialHomeCooldown;
    public bool isHome;
    bool initialIsHome;

    [Header("AI")]
    public Transform[] nodes;
    public AIDestinationSetter destinationSetter;
    public AIPath aiPath;
    public Transform player;
    public float distance = 5;
    public float cooldownToSetter = 0;
    public Transform _target;
    int rand;
    int lastNumber;
    bool doOnce = true;
    Transform initialTarget;

    [Header("Freak")]
    public SpriteRenderer sr;
    public Sprite[] sprite;
    public float animationTime;
    int spriteIndex;
    bool freakOnce = true;

    private void Awake()
    {
        manager = FindObjectOfType<Manager>();
        initialPos = transform.position;
        initialHomeCooldown = homeCooldown;
        initialIsHome = isHome;
        initialTarget = _target;
    }

    private void Update()
    {
        if (!manager.isPlaying)
            return;

        GunterHome();
        TargetDestination();
        EnemySpeed();
        FreakMode();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Jake"))
        {
            if (!manager.isPlayerAttacking)
            {
                manager.JakeDeath();
            }
            else
            {
                manager.SetCoins(points);
                manager.GunterDeath(points);
                gameObject.SetActive(false);
                Invoke(nameof(ResetState), 5);
            }
        }
    }

    public void ResetState()
    {
        gameObject.SetActive(true);
        transform.position = initialPos;
        homeCooldown = initialHomeCooldown;
        isHome = initialIsHome;
        aiPath.canMove = false;
        cooldownToSetter = 0;

        CancelInvoke(nameof(RepeatSprite));
        freakOnce = true;
        sr.sprite = sprite[0];
    }

    public void GunterHome()
    {
        if (isHome)
        {
            homeCooldown -= 1 * Time.deltaTime;

            if(homeCooldown <= 0)
            {
                transform.position = Vector2.Lerp(transform.position, homeDoor.position, Time.deltaTime * 5);
            }
        }
        float dist = Vector2.Distance(transform.position, homeDoor.position);
        if(dist <= .04f)
        {
            isHome = false;
            homeCooldown = initialHomeCooldown;
        }
    }

    public void EnemySpeed()
    {
        if (isHome)
            aiPath.canMove = false;
        else
            aiPath.canMove = true;

        if (manager.isPlayerAttacking)
        {
            aiPath.maxSpeed = 2f;
            cooldownToSetter = 0;
        }
        else
        {
            aiPath.maxSpeed = 3f;
        }
    }

    public void TargetDestination()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (!isHome)
        {
            //If distance is less than 3
            if (dist <= distance)
            {
                if (!manager.isPlayerAttacking)
                    cooldownToSetter = 9;
            }
            //If distance is greather than 3 and cooldown is greather than 0
            else if(cooldownToSetter > 0)
            {
                cooldownToSetter -= 1 * Time.deltaTime;
            }

            //If cooldown is greather than 0
            if (cooldownToSetter > 0)
            {
                destinationSetter.target = player;
            }
            //If cooldown is less than 0
            else
            {
                RandomNode();
                destinationSetter.target = _target;
            }
        }
    }


    public void RandomNode()
    {
        float dist = Vector2.Distance(transform.position, _target.position);
        if (dist <= 0.5f && doOnce)
        {
            doOnce = false;
            if (rand == lastNumber)
            {
                rand = Random.Range(0, nodes.Length);
            }
            else
            {
                lastNumber = rand;
                _target = nodes[rand];
            }
        }
        else
            doOnce = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distance);
    }

    public void FreakMode()
    {
        if (manager.isPlayerAttacking)
        {
            if (freakOnce)
            {
                freakOnce = false;
                InvokeRepeating(nameof(RepeatSprite), animationTime, animationTime);
            }
        }
        else
        {
            freakOnce = true;
            CancelInvoke(nameof(RepeatSprite));
            sr.sprite = sprite[0];
        }
    }

    void RepeatSprite()
    {
        spriteIndex++;

        if (spriteIndex >= sprite.Length)
            spriteIndex = 0;

        sr.sprite = sprite[spriteIndex];
    }
}
