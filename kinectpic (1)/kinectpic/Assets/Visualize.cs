using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualize : MonoBehaviour
{
    public GameObject canvas;
    public GameObject obj;
    public GameObject head;
    public GameObject camera;

    GameObject[] spheres;

    string fileData;
    string[] lines;
    string[] lineData;
    float x;
    float[,] data;

    float max = 0, min = 10000;
    float col;

    float step = 0;

    bool spin = false;

    void Start()
    {
        loadCSV();

        for (int i = 1; i < data.GetLength(0); i++)
        {
            for (int r = 0; r < data.GetLength(1); r++)
            {
                if (data[i, r] > max) max = data[i, r];
               if (data[i, r] < min && data[i, r] != 0) min = data[i, r];
            }
        }

        //GameObject spheres;
        spheres = new GameObject[data.GetLength(0) * data.GetLength(1)];
        //CombineInstance[] combine = new CombineInstance[data.GetLength(0) * data.GetLength(1)];

        Debug.Log(data.GetLength(0) * data.GetLength(1));
        Debug.Log(data.GetLength(1));
        Debug.Log(data.GetLength(0));

        int index = 0;

        Renderer rend;
        Shader shader1 = Shader.Find("Diffuse");

        //Material[] mats = new Material[data.GetLength(0) * data.GetLength(1)];

        for (int i = 1; i < data.GetLength(0); i++)
        //for (int i = 0; i < 10; i++)
        {
            for (int r = 0; r < data.GetLength(1); r++)
            {
                index = data.GetLength(0) * i + r;
                if (data[i, r] != 0)
                {
                    spheres[index] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    spheres[index].transform.position = new Vector3(-3.5f + (6.0f * ((float)i / (float)data.GetLength(0))), -1.0f + (6.0f * ((float)r / (float)data.GetLength(1))), 2 - 2 * data[i, r] / (max - min));
                    Destroy(spheres[index].GetComponent<Collider>());
                    spheres[index].transform.localScale = new Vector3(0.009f, 0.009f, 0.009f);
                    rend = spheres[index].GetComponent<Renderer>();

                    rend.material.shader = shader1;
                    col = 1 - data[i, r] / max;
                    rend.material.color = new Color(col, col, col, 1);
                    spheres[index].transform.SetParent(obj.transform);
                    //mats[index] = rend.material;
                }
            }
        }

        /*
        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < combine.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        //obj.AddComponent<Renderer>();
        //shader1 = Shader.Find("Diffuse");
        //rend = obj.GetComponent<Renderer>();
        //rend.material.shader = shader1;

        obj.AddComponent<MeshFilter>();
        obj.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, false, true);

        obj.GetComponent<MeshRenderer>().materials = mats;
        */

        obj.transform.Rotate(new Vector3(0, 0, -90));
        obj.transform.position = new Vector3(-6.0f, -1.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {        
        if (spin)
        {
            float rot = 0.1f;

            if (step > 900 && step < 2700)
            {
                
                rot = -0.1f;
            }
       
            head.transform.Rotate(new Vector3(0, -rot, 0));
            obj.transform.Rotate(new Vector3(rot, 0, 0));

            Debug.Log(step);

            ScreenCapture.CaptureScreenshot(step.ToString() + ".png", 2);

            step++;

            if (step >= 3600)
            {
                spin = false;
                Debug.Log("Done.");
            }

        }
        

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

    public void hider()
    {
        canvas.SetActive(false);
        obj.SetActive(true);

        head.transform.position = new Vector3(0, 0, 0);
        head.transform.rotation = Quaternion.identity;
        head.transform.localScale = new Vector3(1, 1, 1);

        LineRenderer[] lr = head.GetComponentsInChildren<LineRenderer>();
        for(int i = 0; i < lr.Length; i++)
        {
            lr[i].useWorldSpace = false;
        }

        camera.transform.Translate(0, 0.5f, 0);

        head.GetComponent<MeshRenderer>().enabled = false;

        spin = true;
    }
}











//make container object, so no transforms are applied to hair
