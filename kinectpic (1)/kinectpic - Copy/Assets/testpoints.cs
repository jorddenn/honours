using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

public class testpoints : MonoBehaviour
{
    string fileData;
    string[] lines;
    string[] lineData;
    float x;
    float[,] data;

    // Start is called before the first frame update
    void Start()
    {

        fileData = System.IO.File.ReadAllText("Assets/image1az.csv");
        lines = fileData.Split('\n');
        lineData = (lines[0].Trim()).Split(',');
        data = new float[lines.Length, lineData.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            lineData = (lines[i].Trim()).Split(',');
            for (int r = 0; r < lineData.Length; r++)
            {
                float.TryParse(lineData[r], out x);
                data[i, r] = x;
                //Debug.Log(x);
            }
        }

        //drawPoints();
        drawCurve();


    }

    // Update is called once per frame
    void Update()
    {

    }

    void drawPoints()
    {
        for (int a = 0; a < data.GetLength(0) - 3; a += 3)
        {
            for (int b = 0; b < data.GetLength(1); b++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = new Vector3(data[a, b], data[a + 1, b], 0);
                sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }
    }

    void drawCurve()
    {
        BezierSpline[] spline = new BezierSpline[data.GetLength(0)];

        for (int i = 0; i < data.GetLength(0) - 3; i += 3)
        {
            spline[i / 3] = new GameObject().AddComponent<BezierSpline>();
            spline[i / 3].Initialize(data.GetLength(1));
            for (int r = 0; r < data.GetLength(1); r++)
            {
                spline[i / 3][r].position = new Vector3(data[i, r], data[i + 1, r], data[i + 2, r]);
                if (r == data.GetLength(1) - 1) {
                    spline[i / 3][r].handleMode = BezierPoint.HandleMode.Free;
                    spline[i / 3].AutoConstructSpline2();
                    spline[i/3].DrawGizmos(new Color(1,0,1,1), 4);
                }
            }
            
        }

    }
}
