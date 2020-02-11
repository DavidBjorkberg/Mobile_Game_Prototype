using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[ExecuteInEditMode]
public class UpdateWaypointsInEdit : MonoBehaviour
{
    private void Start()
    {
        if(Application.isPlaying)
        {
            GetComponent<LineRenderer>().positionCount = 0;
        }
    }
    void Update()
    {
        if (!Application.isPlaying)
        {
            GetComponent<Enemy>().waypoints[0].transform.position = transform.position;
            GetComponent<LineRenderer>().positionCount = GetComponent<Enemy>().waypoints.Count;
            for (int i = 0; i < GetComponent<Enemy>().waypoints.Count; i++)
            {
                Vector3 pos = GetComponent<Enemy>().waypoints[i].transform.position;
                GetComponent<LineRenderer>().SetPosition(i, pos);
            }

        }
    }
}
