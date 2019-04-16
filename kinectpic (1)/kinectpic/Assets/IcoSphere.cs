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
    public Texture2D orig;
    Texture2D tex;
    GameObject[] strands;
    GameObject[] origStrands;
    int offset, interp;

    BezierSpline[] spline;
    Vector3[,] origspline;


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
        tex = Instantiate(orig);
        tex.hideFlags = HideFlags.HideAndDontSave;
        Debug.Log(tex.height);
        //loadTex();
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
        fileData = System.IO.File.ReadAllText("Assets/image1azz.csv");
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
        strands = new GameObject[data.GetLength(0) / 3];
    }






    void loadTex()
    {
        tex = new Texture2D(2, 2);
        tex = Resources.Load<Texture2D>("Assets/image1a.jpg");
        tex.hideFlags = HideFlags.HideAndDontSave;
        Debug.Log(tex.height);
    }





    void drawVertexNormals()
    {
        //float normalLength = 0.1f;

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
        spline = new BezierSpline[data.GetLength(0) / 3];


        bool found;
        int rand = 0;
        offset = 1;
        interp = 20;

        origspline = new Vector3[spline.Length, data.GetLength(1) + offset + interp];

        Vector3 cur = new Vector3(0, 0, 0);

        for (int i = 0; i < data.GetLength(0) - 3; i += 3)
        {
            GameObject pine = new GameObject();
            strands[i / 3] = pine;
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
                if (strandUsed[rand] == false && (Mathf.Abs(cur.x - data[i, 0]) < 0.3) && (cur.y > data[i + 1, 0]))
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
                        spline[i / 3][1 + r].position = Vector3.Lerp(spline[i / 3][u].position, new Vector3(data[i, 0], data[i + 1, 0], data[i + 2, 0] - 0.25f), ((float)(r + 1) - (float)u) / ((float)interp - (float)u));
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

            for (int v = 0; v < origspline.GetLength(1); v++)
            {
                origspline[i / 3, v] = new Vector3(spline[i / 3][v].position.x, spline[i / 3][v].position.y, spline[i / 3][v].position.z);
            }

            LineRenderer lr = pine.AddComponent<LineRenderer>();

            lr.positionCount = data.GetLength(1) + offset + interp;

            List<Vector3> temp = new List<Vector3>();

            for (int r = 0; r < data.GetLength(1) + offset + interp; r++)
            {
                temp.Add(spline[i / 3][r].position);
            }

            lr.SetPositions(temp.ToArray());
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;

            Material lineMat = new Material(Shader.Find("Particles/Standard Unlit"));

            lr.material = lineMat;


            Texture2D stran = new Texture2D(data.GetLength(1) + offset + interp, 1);

            for (int t = 0; t < data.GetLength(1) + offset + interp; t++)
            {
                stran.SetPixel(t, 0, getFromImage(spline[i / 3][t].position));
            }
            stran.Apply();

            lr.material.SetTexture("_MainTex", stran);

            /*
            Gradient gradient;
            GradientColorKey[] colorKey;
            GradientAlphaKey[] alphaKey;

            gradient = new Gradient();

            colorKey = new GradientColorKey[8];
            alphaKey = new GradientAlphaKey[8];
            Color pix = new Color();

            for (int t = 1; t < 8; t++)
            {
                pix = getFromImage(spline[i / 3][offset + interp + 1 * t].position);
                colorKey[t].color = pix;
                colorKey[t].time = t * (1.0f / 7.0f);
                alphaKey[t].alpha = 1.0f;
                alphaKey[t].time = t * (1.0f / 7.0f);
            }

            colorKey[0].color = colorKey[1].color;
            colorKey[0].time = 0.0f;
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;

            gradient.SetKeys(colorKey, alphaKey);

            lr.colorGradient = gradient;
            */


        }




    }

    public void reColour(Texture2D tex)
    {
        this.tex = tex;
        for (int i = 0; i < strands.Length; i++)
        {
            LineRenderer lr = strands[i].GetComponent<LineRenderer>();

            List<Vector3> temp = new List<Vector3>();

            for (int r = 0; r >= strands[i].GetComponent<BezierSpline>().Count; r++)
            {
                temp.Add(spline[i][r].position);
            }

            lr.SetPositions(temp.ToArray());
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;

            Texture2D stran = new Texture2D(data.GetLength(1) + offset + interp, 1);

            for (int t = 0; t < data.GetLength(1) + offset + interp; t++)
            {
                stran.SetPixel(t, 0, getFromImage(spline[i / 3][t].position));
            }
            stran.Apply();

            lr.material.SetTexture("_MainTex", stran);
        }
    }

    /*

    public void reColour(Texture2D tex)
    {
        this.tex = tex;
        for (int i = 0; i < strands.Length; i++)
        {
            LineRenderer lr = strands[i].GetComponent<LineRenderer>();

            List<Vector3> temp = new List<Vector3>();

            for (int r = 0; r < strands[i].GetComponent<BezierSpline>().Count; r++)
            {
                temp.Add(spline[i][r].position);
            }

            lr.SetPositions(temp.ToArray());
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;

            Gradient gradient;
            GradientColorKey[] colorKey;
            GradientAlphaKey[] alphaKey;

            gradient = new Gradient();

            colorKey = new GradientColorKey[8];
            alphaKey = new GradientAlphaKey[8];

            Color pix = new Color();

            for (int t = 1; t < 8; t++)
            {
                pix = getFromImage(spline[i][(int)(strands[i].GetComponent<BezierSpline>().Count / 2) + 1 * t].position);
                colorKey[t].color = pix;
                colorKey[t].time = t * (1.0f / 7.0f);
                alphaKey[t].alpha = 1.0f;
                alphaKey[t].time = t * (1.0f / 7.0f);
            }

            colorKey[0].color = colorKey[1].color;
            colorKey[0].time = 0.0f;
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;

            gradient.SetKeys(colorKey, alphaKey);

            lr.colorGradient = gradient;
        }
    }
    */


    Color getFromImage(Vector3 point)
    {
        float imagex = map(point.x, 1.0f, -1.0f, 1434.9249f * 1.2f, 2776.696f * 0.8f);
        float imagey = map(point.y, 1.1f, -3.0f , 103.52703f * 1.2f, 2788.4207f * 0.8f);

        int importx = (int)map(imagex, 1434.9249f * 1.2f, 2776.696f * 0.8f, (1434.9249f * 1.2f / 4288.0f * 2048.0f), (2776.696f * 0.8f / 4288.0f * 2048.0f));
        int importy = (int)map(imagey, 103.52703f * 1.2f, 2788.4207f * 0.8f, (103.52703f * 1.2f / 2848.0f * 1024.0f), (2788.4207f * 0.8f / 2848.0f * 1024.0f));
        return tex.GetPixel(importx, importy);
    }








    public void cut(Vector3 plane)
    {
        //Debug.Log(Vector3.Dot(plane, Vector3.up * 10.0f) < 0);
        for (int i = 0; i < strands.Length; i++)
        {
            BezierSpline cur = strands[i].GetComponent<BezierSpline>();
            for (int r = 1; r < cur.Count; r++)
            {
                if (Vector3.Dot(plane, cur[r].position) < 0)
                {
                    for (int l = r; l < cur.Count; l++)
                    {
                        //Debug.Log(i + " " + r + " " + l);
                        cur[l].position = cur[r - 1].position;
                    }
                    break;
                }
            }
        }
        reColour(tex);
    }

    public void cut(Plane plane)
    {
        //Debug.Log(Vector3.Dot(plane, Vector3.up * 10.0f) < 0);
        for (int i = 0; i < strands.Length; i++)
        {
            BezierSpline cur = strands[i].GetComponent<BezierSpline>();
            for (int r = 1; r < cur.Count; r++)
            {
                if (!plane.GetSide(10.0f * Vector3.up))
                {
                    if (plane.GetSide(cur[r].position))
                    {
                        for (int l = r; l < cur.Count; l++)
                        {
                            //Debug.Log(i + " " + r + " " + l);
                            cur[l].position = cur[r - 1].position;
                        }
                        break;
                    }
                }
                else
                {
                    if (!plane.GetSide(cur[r].position))
                    {
                        for (int l = r; l < cur.Count; l++)
                        {
                            //Debug.Log(i + " " + r + " " + l);
                            cur[l].position = cur[r - 1].position;
                        }
                        break;
                    }
                }
            }
        }
        reColour(tex);
    }

    public void cutTEST(Vector3 plane)
    {
        int yay = 0;
        int nay = 0;
        for (int i = 0; i < strands.Length; i++)
        {
            BezierSpline cur = strands[i].GetComponent<BezierSpline>();
            Vector3 lowest = cur[0].position;

            for (int r = 0; r < cur.Length; r++)
            {
                if (Vector3.Dot(plane, cur[r].position) > 0) yay++;
                else nay++;


            }
        }
        Debug.Log(yay);
        Debug.Log(nay);
    }







    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }












    public void hairstyle()
    {
        int count = 50;

        Vector3[] old = new Vector3[count];
        BezierSpline news;
        Debug.Log(spline.Length);

        for (int i = 0; i < spline.Length; i++)
        {
            news = spline[i].GetComponent<BezierSpline>();
            for (int z = 0; z < count; z++)
            {
                old[z] = news.GetPoint((float)z / (float)count);
            }

            news.Initialize(count);

            news[0].position = new Vector3(old[0].x, old[0].y, old[0].z);
            //news[0].precedingControlPointLocalPosition = new Vector3(news[0].precedingControlPointLocalPosition.x / 10, news[0].precedingControlPointLocalPosition.y, news[0].precedingControlPointLocalPosition.z);
            for (int r = 1; r < news.Count; r++)
            {
                news[r].position = new Vector3(0.1f * Mathf.Sin(r) + old[r].x, 0.1f * Mathf.Cos(r) + old[r].y, old[r].z);
                //news[r].precedingControlPointLocalPosition = new Vector3(news[r].precedingControlPointLocalPosition.x / 10, news[r].precedingControlPointLocalPosition.y, news[r].precedingControlPointLocalPosition.z);
            }
            news.AutoConstructSpline2();
        }
        reColour(tex);
    }

    public void hairstyle2()
    {
        int count = 50;

        Vector3[] old = new Vector3[count];
        BezierSpline news;
        Debug.Log(spline.Length);

        for (int i = 0; i < spline.Length; i++)
        {
            news = spline[i].GetComponent<BezierSpline>();
            for (int z = 0; z < count; z++)
            {
                old[z] = news.GetPoint((float)z / (float)count);
            }

            news.Initialize(count);

            news[0].position = new Vector3(old[0].x, old[0].y, old[0].z);
            //news[0].precedingControlPointLocalPosition = new Vector3(news[0].precedingControlPointLocalPosition.x / 10, news[0].precedingControlPointLocalPosition.y, news[0].precedingControlPointLocalPosition.z);
            for (int r = 1; r < news.Count; r++)
            {
                news[r].position = new Vector3(0.1f * Mathf.Sin(r) + old[r].x, old[r].y, old[r].z);
                //news[r].precedingControlPointLocalPosition = new Vector3(news[r].precedingControlPointLocalPosition.x / 10, news[r].precedingControlPointLocalPosition.y, news[r].precedingControlPointLocalPosition.z);
            }
            news.AutoConstructSpline2();
        }
        reColour(tex);
    }








}