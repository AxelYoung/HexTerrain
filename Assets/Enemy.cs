using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageable {
    [SerializeField] protected int enemyHealth;

    public int health { get { return enemyHealth; } set { enemyHealth = value; } }

    public void Damage(int amount) {
        health -= amount;
        if (health <= 0) {
            Destroy(gameObject);
        }
    }
}
