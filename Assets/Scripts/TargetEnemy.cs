using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
[ExecuteInEditMode]
public class TargetEnemy : MonoBehaviour
{
    public GameObject owner;
    internal GameObject previous;
    public float distanceToPrevious;
    public void TargetOwner()
    {
        Selection.activeGameObject = owner;
    }
    private void Update()
    {
        if(previous != null)
        {
            distanceToPrevious = (transform.position - previous.transform.position).magnitude;
        }
    }
}
#endif
