using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    Transform player;
    Vector3 offset;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - player.transform.position;
    }

    void Update() {
        transform.position = player.transform.position + offset;
    }
}
