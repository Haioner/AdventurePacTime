using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentPointMultiplier : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float timeToDestroy = 1;

    void Start()
    {
        text.text = FindObjectOfType<Manager>().currentPointMultiplier.ToString();
        Invoke(nameof(DestroyGameObject), timeToDestroy);
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }

}
