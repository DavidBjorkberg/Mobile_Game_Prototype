using UnityEngine;
using UnityEngine.AI;
public class DrawPath : MonoBehaviour
{
    Mesh pathMesh;
    internal NavMeshPath navPath;
    int currentCorner = 0;
    Vector3 currentEdge;
    float sizeOfQuad = 0.5f;
    float pathWidth = 1f;
    Vector3 direction;
    void Start()
    {
        transform.position = Vector3.zero;
        transform.SetParent(GameObject.Find("Paths").transform);
        transform.position += Vector3.up * 0.5f;
        navPath = new NavMeshPath();
        pathMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = pathMesh;
    }
    //lengthOfPath has to be divisible by .5f
    public void DrawCurrentPath(float movementSpeed)
    {
        if (navPath.corners.Length > 1)
        {
            currentCorner = 0;
            pathMesh.Clear();

            int nrOfQuads = (int)movementSpeed * 2;
            Vector3[] vertices = new Vector3[6 * nrOfQuads];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] indices = new int[nrOfQuads * 2 * 3];

            int vertexIndex = 0;
            int indicesIndex = 0;
            int uvIndex = 0;

            currentEdge = navPath.corners[0];
            Vector3 nextEdge;
            //Orientation assumes path goes from left to right
            Vector3 upperLeftVertex;
            Vector3 upperRightVertex;
            Vector3 lowerLeftVertex;
            Vector3 lowerRightVertex;
            for (int i = 0; i < nrOfQuads; i++)
            {

                direction = CalculateDirectionToNext();
                nextEdge = currentEdge + direction * sizeOfQuad;
                //If at the last quad and the edge isn't at the last corner, stretch the quad to match with the last corner
                if (i == nrOfQuads - 1 && nextEdge != navPath.corners[navPath.corners.Length - 1])
                {
                    float stretchDistance = Vector3.Distance(nextEdge, navPath.corners[navPath.corners.Length - 1]);
                    if (stretchDistance >= sizeOfQuad)
                    {
                        int quadsToAdd = Mathf.FloorToInt(stretchDistance / sizeOfQuad);
                        //nrOfQuads += quadsToAdd;
                        System.Array.Resize(ref vertices, nrOfQuads * 6);
                        System.Array.Resize(ref uv, vertices.Length);
                        System.Array.Resize(ref indices, nrOfQuads * 2 * 3);
                    }
                    else
                    {
                        //Vector3 stretchDirection = (navPath.corners[navPath.corners.Length - 1] - nextEdge).normalized;
                        //nextEdge = currentEdge + stretchDirection * (sizeOfQuad + stretchDistance);
                    }
                }


                Vector3 vectorToEdge = Vector3.Cross(direction, Vector3.up);
                //Calculate vertices of quad
                upperLeftVertex = currentEdge + vectorToEdge * (pathWidth / 2);
                upperRightVertex = nextEdge + vectorToEdge * (pathWidth / 2);
                lowerLeftVertex = currentEdge - vectorToEdge * (pathWidth / 2);
                lowerRightVertex = nextEdge - vectorToEdge * (pathWidth / 2);

                //Set vertices of quad
                vertices[vertexIndex] = lowerLeftVertex;
                vertices[vertexIndex + 1] = upperLeftVertex;
                vertices[vertexIndex + 2] = upperRightVertex;
                vertices[vertexIndex + 3] = lowerLeftVertex;
                vertices[vertexIndex + 4] = upperRightVertex;
                vertices[vertexIndex + 5] = lowerRightVertex;

                //Reverse triangles if they are backfaced
                Vector3 sideA = vertices[vertexIndex] - vertices[vertexIndex + 1];
                Vector3 sideB = vertices[vertexIndex] - vertices[vertexIndex + 2];
                if (Vector3.Cross(sideA, sideB).y < 0)
                {
                    vertices[vertexIndex + 1] = upperRightVertex;
                    vertices[vertexIndex + 2] = upperLeftVertex;
                }
                sideA = vertices[vertexIndex + 3] - vertices[vertexIndex + 4];
                sideB = vertices[vertexIndex + 3] - vertices[vertexIndex + 5];
                if (Vector3.Cross(sideA, sideB).y < 0)
                {
                    vertices[vertexIndex + 4] = lowerRightVertex;
                    vertices[vertexIndex + 5] = upperRightVertex;
                }

                //Set index
                for (int j = 0; j < 6; j++)
                {
                    indices[indicesIndex + j] = indicesIndex + j;
                }
                //Draw step
                if ((i + 1) % movementSpeed == 0)
                {
                    uv[uvIndex] = new Vector2(0.5f, 0);
                    uv[uvIndex +  1] = new Vector2(0.5f, 1);
                    uv[uvIndex + 2] = new Vector2(1, 1);
                    uv[uvIndex + 3] = new Vector2(0.5f, 0);
                    uv[uvIndex + 4] = new Vector2(1, 1);
                    uv[uvIndex + 5] = new Vector2(1, 0);
                }
                else
                {
                    uv[uvIndex] = new Vector2(0, 0);
                    uv[uvIndex + 1] = new Vector2(0, 1);
                    uv[uvIndex + 2] = new Vector2(0.5f, 1);
                    uv[uvIndex + 3] = new Vector2(0, 0);
                    uv[uvIndex + 4] = new Vector2(0.5f, 1);
                    uv[uvIndex + 5] = new Vector2(0.5f, 0);
                }
                currentEdge = nextEdge;
                uvIndex += 6;
                indicesIndex += 6;
                vertexIndex += 6;

            }
            pathMesh.vertices = vertices;
            pathMesh.uv = uv;
            pathMesh.triangles = indices;
            pathMesh.RecalculateBounds();

        }


    }
    Vector3 CalculateDirectionToNext()
    {
        if (currentCorner + 1 >= navPath.corners.Length)
        {
            direction = navPath.corners[currentCorner] - currentEdge;
        }
        else
        {

            Vector3 edgeToCorner = navPath.corners[currentCorner + 1] - currentEdge;
            edgeToCorner.Normalize();
            Vector3 cornerToCorner = navPath.corners[currentCorner + 1] - navPath.corners[currentCorner];
            cornerToCorner.Normalize();

            if (edgeToCorner != cornerToCorner)
            {
                currentCorner++;
            }
            //If not at the last corner,update cornerToCorner with new corners
            if (currentCorner + 1 < navPath.corners.Length)
            {
                //  edgeToCorner = navPath.corners[currentCorner + 1] - currentEdge;
                cornerToCorner = navPath.corners[currentCorner + 1] - navPath.corners[currentCorner];
                direction = cornerToCorner;
            }
            else
            {
                direction = edgeToCorner;
            }

        }
        direction.Normalize();
        return direction;
    }
}
