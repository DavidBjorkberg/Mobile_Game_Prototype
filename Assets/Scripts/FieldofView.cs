using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldofView : MonoBehaviour
{

    //Lägg till layermask för obstacles
    Mesh mesh;
    internal GameObject enemy;
    Vector3 origin;
    float startAngle;
    [Range(0.0f, 360.0f)]
    public float fov = 90f;
    [Range(0.0f, 200)]
    public int rayCount = 50;
    [Range(0.0f, 50.0f)]
    public float viewDistance = 10f;
    void Start()
    {
        transform.SetParent(GameObject.Find("FoV's").transform);
        transform.position = Vector3.zero;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }
    private void LateUpdate()
    {
        SetAimDirection(enemy.transform.forward);
        SetOrigin(enemy.transform.position);
        float angle = startAngle;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            float angleRad = angle * (Mathf.PI / 180f);
            Vector3 angleVector = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
            Vector3 vertex;
            if (!Physics.Raycast(origin, angleVector, out RaycastHit hit, viewDistance) || hit.transform.gameObject.GetInstanceID() == gameObject.GetInstanceID())
            {
                vertex = origin + angleVector * viewDistance;
            }
            else
            {
                vertex = hit.point;
            }

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }
            vertexIndex++;
            angle -= angleIncrease;
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }
    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }
    public void SetAimDirection(Vector3 aimDirection)
    {
        aimDirection.Normalize();
        startAngle = Mathf.Atan2(aimDirection.z, aimDirection.x) * Mathf.Rad2Deg;
        if (startAngle < 0)
        {
            startAngle += 360;
        }
        startAngle += fov / 2;
    }
}