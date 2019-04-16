using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class cut : MonoBehaviour
{
    Button cut1, cut2, cut3;
    Ray ray1, ray2;
    Vector3 plane;
    Plane test;
    public IcoSphere cutHair;
    public GameObject quaf;

    void Start()
    {
        
    }

    public void Cut()
    {

    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray1.origin, ray1.direction, Color.magenta, 100);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray2.origin, ray2.direction, Color.magenta, 100);
        }
        if (ray1.direction != Vector3.zero && ray2.direction != Vector3.zero)
        {
            plane = Vector3.Cross(ray1.direction, ray2.direction);
            test = new Plane((ray1.origin + ray1.direction),  (ray1.origin + ray2.direction), ray1.origin);
            //test.Translate(new Vector3(0, quaf.transform.position.y, 0));

            Debug.Log("plane " + plane);
            Debug.Log("test " + test);
            Debug.Log("orig " + ray1.origin);
            Debug.Log("dirr " + ray1.direction);
            //Debug.DrawRay(Vector3.zero, plane.normal * 10f, Color.green, 100);
            quaf.transform.rotation = Quaternion.LookRotation(test.normal, Vector3.up);
            ray1.direction = Vector3.zero;
            ray2.direction = Vector3.zero;
            //cutHair.cut(plane);
            cutHair.cut(test);

        }
    }

}
