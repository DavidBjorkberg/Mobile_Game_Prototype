using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    private SphereCollider collider;
    private LineRenderer lineRenderer;
    void Start()
    {
        collider = GetComponent<SphereCollider>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Enemy enemy))
        {

        }
    }
}
