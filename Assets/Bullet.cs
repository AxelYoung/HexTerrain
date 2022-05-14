using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [SerializeField] float speed;
    [SerializeField] float lifeLength = 1;
    [SerializeField] int power;
    float lifeTime = 0;

    void Update() {
        transform.position += transform.forward * speed * Time.deltaTime;

        lifeTime += Time.deltaTime;
        if (lifeTime >= lifeLength) {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider col) {
        IDamageable hit = col.GetComponent<IDamageable>();
        if (col != null) {
            hit.Damage(power);
            Destroy(gameObject);
        }
    }
}
