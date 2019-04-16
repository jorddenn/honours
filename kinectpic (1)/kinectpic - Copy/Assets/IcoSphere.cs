using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;

//1/2 (4 + 5 2^(2 n)) vertices
//


public class IcoSphere : MonoBehaviour
{
    string fileData;
    string[] lines;
    string[] lineData;
    float x;
    float[,] data;
    Vector3[] hairRegion;
    bool[] strandUsed;
    Mesh mesh;

    struct TriangleIndices
    {
        public int v1;
        public int v2;
        public int v3;

        public TriangleIndices(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }



    void Awake()
    {
        loadCSV();
        generateSphere();
    }

    void Start()
    {
        drawVertexNormals();
        //drawPoints();
        drawCurve();
    }

    void Update()
    {
        //drawVertexNormals();
    }










    void generateSphere()
    {
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        mesh = filter.mesh;
        mesh.Clear();

        List<Vector3> vertList = new List<Vector3>();
        Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

        int recursionLevel = 5;
        float radius = 1f;

        // create 12 vertices of a icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        vertList.Add(new Vector3(-1f, t, 0f).normalized * radius);
        vertList.Add(new Vector3(1f, t, 0f).normalized * radius);
        vertList.Add(new Vector3(-1f, -t, 0f).normalized * radius);
        vertList.Add(new Vector3(1f, -t, 0f).normalized * radius);

        vertList.Add(new Vector3(0f, -1f, t).normalized * radius);
        vertList.Add(new Vector3(0f, 1f, t).normalized * radius);
        vertList.Add(new Vector3(0f, -1f, -t).normalized * radius);
        vertList.Add(new Vector3(0f, 1f, -t).normalized * radius);

        vertList.Add(new Vector3(t, 0f, -1f).normalized * radius);
        vertList.Add(new Vector3(t, 0f, 1f).normalized * radius);
        vertList.Add(new Vector3(-t, 0f, -1f).normalized * radius);
        vertList.Add(new Vector3(-t, 0f, 1f).normalized * radius);


        // create 20 triangles of the icosahedron
        List<TriangleIndices> faces = new List<TriangleIndices>();

        // 5 faces around point 0
        faces.Add(new TriangleIndices(0, 11, 5));
        faces.Add(new TriangleIndices(0, 5, 1));
        faces.Add(new TriangleIndices(0, 1, 7));
        faces.Add(new TriangleIndices(0, 7, 10));
        faces.Add(new TriangleIndices(0, 10, 11));

        // 5 adjacent faces 
        faces.Add(new TriangleIndices(1, 5, 9));
        faces.Add(new TriangleIndices(5, 11, 4));
        faces.Add(new TriangleIndices(11, 10, 2));
        faces.Add(new TriangleIndices(10, 7, 6));
        faces.Add(new TriangleIndices(7, 1, 8));

        // 5 faces around point 3
        faces.Add(new TriangleIndices(3, 9, 4));
        faces.Add(new TriangleIndices(3, 4, 2));
        faces.Add(new TriangleIndices(3, 2, 6));
        faces.Add(new TriangleIndices(3, 6, 8));
        faces.Add(new TriangleIndices(3, 8, 9));

        // 5 adjacent faces 
        faces.Add(new TriangleIndices(4, 9, 5));
        faces.Add(new TriangleIndices(2, 4, 11));
        faces.Add(new TriangleIndices(6, 2, 10));
        faces.Add(new TriangleIndices(8, 6, 7));
        faces.Add(new TriangleIndices(9, 8, 1));


        // refine triangles
        for (int i = 0; i < recursionLevel; i++)
        {
            List<TriangleIndices> faces2 = new List<TriangleIndices>();
            foreach (var tri in faces)
            {
                // replace triangle by 4 triangles
                int a = getMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
                int b = getMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
                int c = getMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);

                faces2.Add(new TriangleIndices(tri.v1, a, c));
                faces2.Add(new TriangleIndices(tri.v2, b, a));
                faces2.Add(new TriangleIndices(tri.v3, c, b));
                faces2.Add(new TriangleIndices(a, b, c));
            }
            faces = faces2;
        }

        mesh.vertices = vertList.ToArray();

        List<int> triList = new List<int>();
        for (int i = 0; i < faces.Count; i++)
        {
            triList.Add(faces[i].v1);
            triList.Add(faces[i].v2);
            triList.Add(faces[i].v3);
        }
        mesh.triangles = triList.ToArray();
        mesh.uv = new Vector2[mesh.vertices.Length];

        Vector3[] normales = new Vector3[vertList.Count];
        for (int i = 0; i < normales.Length; i++)
            normales[i] = vertList[i].normalized;


        mesh.normals = normales;

        mesh.RecalculateBounds();

        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Diffuse"));

    }

    int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
    {
        // first check if we have it already
        bool firstIsSmaller = p1 < p2;
        long smallerIndex = firstIsSmaller ? p1 : p2;
        long greaterIndex = firstIsSmaller ? p2 : p1;
        long key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (cache.TryGetValue(key, out ret))
        {
            return ret;
        }

        // not in cache, calculate it
        Vector3 point1 = vertices[p1];
        Vector3 point2 = vertices[p2];
        Vector3 middle = new Vector3
        (
            (point1.x + point2.x) / 2f,
            (point1.y + point2.y) / 2f,
            (point1.z + point2.z) / 2f
        );

        // add vertex makes sure point is on unit sphere
        int i = vertices.Count;
        vertices.Add(middle.normalized * radius);

        // store it, return index
        cache.Add(key, i);

        return i;
    }











    void loadCSV()
    {
        fileData = System.IO.File.ReadAllText("Assets/image1az.csv");
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







    void drawVertexNormals()
    {
        float normalLength = 0.1f;

        mesh = GetComponent<MeshFilter>().mesh;

        hairRegion = mesh.vertices;
        strandUsed = new bool[hairRegion.Length];

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 norm = transform.TransformDirection(mesh.normals[i]);
            Vector3 vert = transform.TransformPoint(mesh.vertices[i]);
            if (((vert.z > 0.3 && vert.y > 0.25) || vert.y > 0.75) && (vert.x < 0.5 || vert.x > -0.5))
            {
                strandUsed[i] = false;
                //Debug.DrawRay(vert, norm * normalLength, Color.red);
            }
            else
            {
                strandUsed[i] = true;
            }
        }
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

        bool found;
        int rand = 0;
        int offset = 1;
        int interp = 20;

        Vector3 cur = new Vector3(0, 0, 0);

        for (int i = 0; i < data.GetLength(0) - 3; i += 3)
        {
            GameObject pine = new GameObject();
            pine.name = "Curve " + i / 3;
            spline[i / 3] = pine.AddComponent<BezierSpline>();
            spline[i / 3].Initialize(data.GetLength(1) + offset + interp);
            //pine.SetActive(false);
            found = false;

            while (!found)
            {
                cur = new Vector3();
                rand = Random.Range(0, hairRegion.Length);
                cur = transform.TransformPoint(hairRegion[rand]);
                if (strandUsed[rand] == false && (Mathf.Abs(cur.x - data[i, 0]) < 0.2) && (cur.y > data[i + 1, 0]))
                {
                    strandUsed[rand] = true;
                    found = true;
                }
            }

            Vector3 norm = transform.TransformDirection(mesh.normals[rand]);

            spline[i / 3][0].position = new Vector3(cur.x, cur.y, cur.z);

            for (int u = 0; u < interp + 1; u++)
            {
                spline[i / 3][1 + u].position = Vector3.Lerp(spline[i / 3][0].position, new Vector3(data[i, 0], data[i + 1, 0], data[i + 2, 0]), (float)(u + 1) / (float)interp);

                if (spline[i / 3][1 + u].position.y < -0.25f)
                {
                    spline[i / 3][1 + u].position = new Vector3(spline[i / 3][1 + u].position.x, 1.0f, spline[i / 3][1 + u].position.z);
                }
                if ((Mathf.Pow(spline[i / 3][1 + u].position.x, 2.0f) + Mathf.Pow(spline[i / 3][1 + u].position.y - 0.5f, 2.0f) + Mathf.Pow(spline[i / 3][1 + u].position.z, 2.0f) < 1.0f) && spline[i / 3][0].position.y > 0.5f)
                {
                    float pos = Mathf.Pow(Mathf.Pow(1.0f, 2.0f) - Mathf.Pow(spline[i / 3][1 + u].position.x, 2.0f) - Mathf.Pow(spline[i / 3][1 + u].position.z, 2.0f), 0.5f) + 0.5f;
                    spline[i / 3][1 + u].position = new Vector3(spline[i / 3][1 + u].position.x, pos * Mathf.Cos(spline[i / 3][1 + u].position.x) + 0.1f, spline[i / 3][1 + u].position.z);
                }
                else
                {
                    for (int r = u; r < interp + 1; r++)
                    {
                        spline[i / 3][1 + r].position = Vector3.Lerp(spline[i / 3][u].position, new Vector3(data[i, 0], data[i + 1, 0], data[i + 2, 0]), ((float)(r + 1) - (float)u) / ((float)interp - (float)u));
                    }
                    break;
                }

            }


            for (int r = offset + interp; r < data.GetLength(1) + offset + interp; r++)
            {
                spline[i / 3][r].position = new Vector3(data[i, r - offset - interp], data[i + 1, r - offset - interp], data[i + 2, r - offset - interp]);
            }
            spline[i / 3].AutoConstructSpline2();
            spline[i / 3].HideGizmos();

            

            LineRenderer lr = pine.AddComponent<LineRenderer>();

            lr.positionCount = data.GetLength(1) + offset + interp;

            List<Vector3> temp = new List<Vector3>();

            for (int r = 0; r < data.GetLength(1) + offset + interp; r++) {
                temp.Add(spline[i / 3][r].position);
            }

            lr.SetPositions(temp.ToArray());
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;

            lr.material = new Material(Shader.Find("Particles / Standard Surface"));

            Gradient gradient;
            GradientColorKey[] colorKey;
            GradientAlphaKey[] alphaKey;

            gradient = new Gradient();

            colorKey = new GradientColorKey[2];
            colorKey[0].color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            colorKey[0].time = 0.0f;
            colorKey[1].color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            colorKey[1].time = 1.0f;

            alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 1.0f;
            alphaKey[1].time = 1.0f;

            gradient.SetKeys(colorKey, alphaKey);

            lr.colorGradient = gradient;

            




















            /*
            if (spline[i / 3][1].position.z < 0)
            {
                spline[i / 3][1].position = new Vector3(cur.x, cur.y + 0.1f, cur.z + 0.1f);
                spline[i / 3][2].position = new Vector3(spline[i / 3][1].position.x, 1.1f * Mathf.Cos(spline[i / 3][1].position.x), 0.0f);

                if (spline[i / 3][1].position.z < -0.5)
                {
                    spline[i / 3][3].position = new Vector3(spline[i / 3][1].position.x, spline[i / 3][1].position.y, -spline[i / 3][1].position.z);
                }
                else
                {
                    spline[i / 3][3].position = new Vector3(spline[i / 3][1].position.x, spline[i / 3][1].position.y, -spline[i / 3][1].position.z + 0.5f);
                }

                for (int  u = 1; u < interp + 1; u++)
                {
                    spline[i / 3][3 + u].position = Vector3.Lerp(spline[i / 3][3].position, new Vector3(data[i, 0], data[i + 1, 0], data[i + 2, 0]), (float)u / (float)interp);
                }

                for (int r = offset + interp; r < data.GetLength(1) + offset + interp; r++)
                {
                    spline[i / 3][r].position = new Vector3(data[i, r - offset - interp], data[i + 1, r - offset - interp], data[i + 2, r - offset - interp]);
                }
                spline[i / 3].AutoConstructSpline2();
                spline[i / 3].DrawGizmos(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 4);

            }

            else {
                spline[i / 3][1].position = new Vector3(cur.x, cur.y, cur.z + 0.1f);
                spline[i / 3][2].position = (spline[i / 3][1].position + new Vector3(data[i, 0], data[i + 1, 0], data[i + 2, 0])) / 2;

                for (int u = 1; u < interp + 1; u++)
                {
                    spline[i / 3][2 + u].position = Vector3.Lerp(spline[i / 3][2].position, new Vector3(data[i, 0], data[i + 1, 0], data[i + 2, 0]), (float)u / (float)interp);
                }

                for (int r = offset + interp; r < data.GetLength(1) + offset + interp; r++)
                {
                    spline[i / 3][r - 1].position = new Vector3(data[i, r - offset - interp], data[i + 1, r - offset - interp], data[i + 2, r - offset - interp]);
                }
                spline[i / 3][data.GetLength(1) + offset + interp - 1].position = new Vector3(data[i, data.GetLength(1) - 1], data[i + 1, data.GetLength(1) - 1], data[i + 2, data.GetLength(1) - 1]);
                spline[i / 3].AutoConstructSpline2();
                spline[i / 3].DrawGizmos(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 4);
            }
            */



        }

    }
}