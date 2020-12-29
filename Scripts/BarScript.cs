using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BarScript : MonoBehaviour
{
    public Camera mainCam;

    [SerializeField] private Image healthBar;

    [SerializeField] private float maxHealth;

    // [NonSerialized] public EventHandler<float> onTakeDamage;

    private float health;
    // Start is called before the first frame update

    private void Awake() {
        mainCam = MainDependencies.Instance.mainCamera;
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCam) {
            transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.back, mainCam.transform.rotation * Vector3.up);
        }
    }

    public void TakeDamage (float amount) {
        health -= amount;
        Debug.Log(health);

        healthBar.fillAmount = health / maxHealth;

        if (health <= 0) {
            Die();
        }
    }

    void Die() {
        Destroy(gameObject);
    }
}
