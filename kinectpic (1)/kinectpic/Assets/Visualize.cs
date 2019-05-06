using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualize : MonoBehaviour
{
    public GameObject canvas;
    public GameObject obj;

    GameObject[] spheres;

    string fileData;
    string[] lines;
    string[] lineData;
    float x;
    float[,] data;

    float max = 0;
    float col;

    void Start()
    {
        loadCSV();

        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int r = 0; r < data.GetLength(1); r++)
            {
                if (data[i, r] > max) max = data[i, r];
            }
        }

        spheres = new GameObject[data.GetLength(0) * data.GetLength(1)];
        //CombineInstance[] combine = new CombineInstance[data.GetLength(0) * data.GetLength(1)];

        //Debug.Log(data.GetLength(0) * data.GetLength(1));

        int index = 0;

        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int r = 0; r < data.GetLength(1); r++)
            {
                index = data.GetLength(0) * i + r;
                //Debug.Log(index);
                spheres[index] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                spheres[index].transform.position = new Vector3(-3.0f + (6.0f * ((float)i / (float)data.GetLength(0))), -3.0f + (6.0f * ((float)r / (float)data.GetLength(1))), 0.0f);
                Destroy(spheres[index].GetComponent<Collider>());
                spheres[index].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                Renderer rend = spheres[index].GetComponent<Renderer>();
                Shader shader1 = Shader.Find("Diffuse");

                rend.material.shader = shader1;
                col = 1 - data[i, r] / max;
                rend.material.color = new Color(col, col, col, 1);
                spheres[index].transform.SetParent(obj.transform);
            }
        }

        /*
        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        
        obj.AddComponent<MeshFilter>();
        obj.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        */

        obj.transform.Rotate(new Vector3(0, 0, -90));
        obj.transform.position = new Vector3(-6.0f, -1.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void loadCSV()
    {
        fileData = System.IO.File.ReadAllText("Assets/depth.csv");
        lines = fileData.Split('\n');
        lineData = (lines[0].Trim()).Split(',');
        data = new float[lines.Length, lineData.Length];
        //data = new float[12, lineData.Length];
        for (int i = 0; i < lines.Length; i++)
        //for (int i = 0; i < 12; i++)
        {
            lineData = (lines[i].Trim()).Split(',');
            for (int r = 0; r < lineData.Length; r++)
            {
                float.TryParse(lineData[r], out x);
                data[i, r] = x;
            }
        }
    }

    public void hider() {
        canvas.SetActive(false);
        obj.SetActive(true);
    }
}
