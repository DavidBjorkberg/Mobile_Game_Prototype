using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    float newPosX;
    float newPosZ;
    float smoothTime = 0.3f;
    float xVelocity;
    float zVelocity;
    GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void Update()
    {
        newPosX = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref xVelocity, smoothTime);
        newPosZ = Mathf.SmoothDamp(transform.position.z, player.transform.position.z, ref zVelocity, smoothTime);
        transform.position = new Vector3(newPosX, transform.position.y, newPosZ);
    }
}
