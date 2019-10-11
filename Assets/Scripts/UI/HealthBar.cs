using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    float maxHealth;
    float currentPercentage;
    Image bar;

    void Start()
    {
        bar = GetComponent<Image>();
    }

    void Update()
    {
        bar.fillAmount = Mathf.MoveTowards(bar.fillAmount, currentPercentage, 0.005f);
    }

    public void SetHealth (Damageable status) {
        maxHealth = status.maxHitPoints;
        currentPercentage = status.currentHealth / maxHealth;
    }
    public void UpdateHealthBar(Damageable status) {
        currentPercentage = status.currentHealth / maxHealth;
    }
}
