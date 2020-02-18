using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    private SphereCollider sphereCollider;
    private LineRenderer lineRenderer;
    public float expandSpeed;
    internal float range;
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        DrawCircle();

        sphereCollider.radius += expandSpeed * Time.deltaTime;
        if(sphereCollider.radius >= range)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Enemy enemy))
        {
            enemy.SetChaseState(transform.position);
        }
    }
    void DrawCircle()
    {
        int segments = 50;
        float x;
        float z;

        float angle = 20f;
        lineRenderer.positionCount = segments + 1;
        for (int i = 0; i < (segments + 1); i++)
        {
            x = transform.position.x + Mathf.Sin(Mathf.Deg2Rad * angle) * sphereCollider.radius;
            z = transform.position.z + Mathf.Cos(Mathf.Deg2Rad * angle) * sphereCollider.radius;
            lineRenderer.SetPosition(i, new Vector3(x, 1, z));
            angle += (360f / segments);
        }
    }
}
