using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vertexnormals : MonoBehaviour
{
    private Mesh mesh;
    public float normalLength = 0.1f;
    // Use this for initialization
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 norm = transform.TransformDirection(mesh.normals[i]);
            Vector3 vert = transform.TransformPoint(mesh.vertices[i]);
            if (((vert.z > 0.5 && vert.y > -0.5) || vert.y > 0.5) && (vert.x < 0.5 || vert.x > -0.5))
            {
                Debug.DrawRay(vert, norm * normalLength, Color.red);
            }
        }
    }
}
