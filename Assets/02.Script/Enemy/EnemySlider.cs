using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemySlider : MonoBehaviour
{
    public Slider healthSlider;
    public bool isBoss;
    public TextMeshProUGUI bossNameText;

    private void OnEnable()
    {
        healthSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        if (!isBoss)
        {
            this.transform.LookAt(2 * transform.position - Camera.main.transform.position);
        }
    }

    public void SetSliderValue(Enemy enemy)
    {
        healthSlider.maxValue = enemy.baseMaxHealth;
        healthSlider.value = enemy.health;
    }
}
