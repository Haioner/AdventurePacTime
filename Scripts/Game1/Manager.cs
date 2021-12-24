using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [Header("Game State")]
    public bool isPlaying = false;

    [Header("Start text")]
    public TextMeshProUGUI touchText;
    public Image swordImage;
    public float frequency;
    public float magnitude;

    [Header("Score")]
    public int score;
    public TextMeshProUGUI scoreText;
    public TakeCoins[] coins;
    public int scoreMultiplier = 1;
    public float multiplierTimer = 8;
    [HideInInspector]public int currentPointMultiplier;
    public GameObject gunterPointText;

    [Header("Fight mode")]
    public bool isPlayerAttacking = false;
    public float fightTimer = 8;
    public Image chocolateFill;

    [Header("Player")]
    public PlayerController player;
    public bool playerCanMove = true;

    [Header("Enemys")]
    public int guntersToSpawn;
    public Enemy_Controller[] enemys;

    [Header("Health")]
    public int health;
    public int numberOfHearts;
    public Image[] hearts;
    bool doOnce = true;

    [Header("Audio")]
    public AudioSource music;
    bool isStoppingMusic = false;
    float initialVolume;

    public AudioSource playerDeathSource;
    public AudioSource gunterDeathSource;
    public AudioSource ScareghostSource;

    private void Awake()
    {
        initialVolume = music.volume;
    }

    private void Update()
    {
        //Start game if is not playing
        if (Input.GetMouseButtonDown(0) && !isPlaying && numberOfHearts > 0)
        {
            isPlaying = true;
            doOnce = true;
            NewGame();
        }

        if(numberOfHearts <= 0 && doOnce)
        {
            doOnce = false;
            isPlaying = false;
            Invoke(nameof(NewGame), 3f);
        }

        if(chocolateFill.fillAmount > 0)
        {
            chocolateFill.fillAmount -= Time.deltaTime / fightTimer;
        }

        TouchTextManager();
        StopMusic();
    }

    void StartMusic()
    {
        music.volume = initialVolume;
        music.Play();
    }

    void StopMusic()
    {
        if (isStoppingMusic && music.volume > 0)
        {
            music.volume -= Time.deltaTime;
        }

        if (music.volume <= 0)
        {
            isStoppingMusic = false;
            music.Stop();
        }
    }

    void TouchTextManager()
    {
        if (isPlaying)
        {
            touchText.gameObject.SetActive(false);        
            swordImage.gameObject.SetActive(false);        
        }
        else
        {
            touchText.alpha = magnitude + Mathf.Sin(Time.timeSinceLevelLoad * frequency) * magnitude;
            var tempColor = swordImage.color;
            tempColor.a = magnitude + Mathf.Sin(Time.timeSinceLevelLoad * frequency) * magnitude;
            swordImage.color = tempColor;
        }
    }

    public void CoinChecker()
    {
        if (!HasReaminingCoins())
        {
            player.gameObject.SetActive(false);

            for (int i = 0; i < enemys.Length; i++)
            {
                enemys[i].gameObject.SetActive(false);
            }

            isPlaying = false;
            Invoke(nameof(NewGame), 3f);
        }
    }

    private bool HasReaminingCoins()
    {
        for (int i = 0; i < coins.Length; i++)
            if (coins[i].gameObject.activeSelf)
                return true;
        return false;
    }


    public void SetCoins(int value)
    {
        //Set score, score text
        score += value;
        scoreText.text = "Score: "+ score.ToString();
    }

    public void SetFightingMode()
    {
        //TODO : Can kill gunter
        isPlayerAttacking = true;
        CancelInvoke(nameof(DisableFighting));
        Invoke(nameof(DisableFighting), fightTimer);

        chocolateFill.fillAmount = fightTimer;

        isStoppingMusic = true;
        ScareghostSource.Play();
    }

    void DisableFighting()
    {
        if (!music.isPlaying)
            StartMusic();

        ScareghostSource.Stop();

        isPlayerAttacking = false;
        ResetMultiplier();
    }

    public void NewGame()
    {
        //Reset game, lives, score, coins
        ResetGame();
        health = 3;
        numberOfHearts = 3;
        score = 0;
        scoreText.text ="Score: " + score.ToString();
        SetHeart();
        chocolateFill.fillAmount = 0;
        touchText.gameObject.SetActive(true);
        swordImage.gameObject.SetActive(true);

        for (int i = 0; i < coins.Length; i++)
            coins[i].ResetPosition();
    }

    public void ResetGame()
    {
        //Reset multiplier, player pos, player moves, enemy pos
        DisableFighting();
        ResetMultiplier();
        player.ResetState();
        playerCanMove = true;

        for (int i = 0; i < enemys.Length; i++)
        {
            enemys[i].ResetState();
        }
    }

    public void JakeDeath()
    {
        if (numberOfHearts > 1)
        {
            playerCanMove = false;
            player.gameObject.SetActive(false);
            numberOfHearts--;
            ResetGame();
        }
        else
        {
            playerCanMove = false;
            player.gameObject.SetActive(false);
            numberOfHearts--;
        }
        DisableFighting();
        chocolateFill.fillAmount = 0;
        SetHeart();
        playerDeathSource.Play();
    }

    void SetHeart()
    {
        //Health
        if (health > numberOfHearts)
            health = numberOfHearts;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < numberOfHearts)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;
        }
    }

    public void GunterDeath(int value)
    {
        //Add score * multiplier when player kill gunter
        //Reset multiplier after timer
        score += value * scoreMultiplier;
        currentPointMultiplier = value * scoreMultiplier;
        Instantiate(gunterPointText, player.gameObject.transform.position, Quaternion.identity);
        scoreMultiplier++;
        CancelInvoke(nameof(ResetMultiplier));
        Invoke(nameof(ResetMultiplier), multiplierTimer);
        gunterDeathSource.Play();
    }

    public void ResetMultiplier()
    {
        scoreMultiplier = 1;
    }
}
