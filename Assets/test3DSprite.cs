using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;

public class test3DSprite : MonoBehaviour
{
    public Sprite sprite;
    public GameObject point;
    // Start is called before the first frame update

    void Start()
    {
        Vector2[] dsf = {new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(2,2), new Vector2(0,2)};

        string output = "[";
        foreach(Vector2 v in sprite.vertices){
            Instantiate(point, transform.position + new Vector3(v.x, v.y, 0), transform.rotation);
            output += "(" + v.x + "," + v.y + "), ";
        }
        Debug.Log(output + "]");

        GetComponent<MeshFilter>().mesh = CreateMesh(dsf, .2f);
    }

    static Mesh CreateMesh(Vector2 [] poly, float thickness)
    {
        // convert polygon to triangles
        Triangulator triangulator = new Triangulator(poly);
        int[] tris = triangulator.Triangulate();
        Mesh m = new Mesh();
        Vector3[] vertices = new Vector3[poly.Length*2];
       
        for(int i=0;i<poly.Length;i++)
        {
            vertices[i].x = poly[i].x;
            vertices[i].y = poly[i].y;
            vertices[i].z = -thickness/2; // front vertex
            vertices[i+poly.Length].x = poly[i].x;
            vertices[i+poly.Length].y = poly[i].y;
            vertices[i+poly.Length].z = thickness/2;  // back vertex    
        }
        int[] triangles = new int[tris.Length*2+poly.Length*6];
        int count_tris = 0;
        for(int i=0;i<tris.Length;i+=3)
        {
            triangles[i] = tris[i+1];
            triangles[i+1] = tris[i+2];
            triangles[i+2] = tris[i];
        } // front vertices
        count_tris+=tris.Length;
        for(int i=0;i<tris.Length;i+=3)
        {
            triangles[count_tris+i] = tris[i+1]+poly.Length;
            triangles[count_tris+i+1] = tris[i]+poly.Length;
            triangles[count_tris+i+2] = tris[i+2]+poly.Length;
        } // back vertices
        count_tris+=tris.Length;
        for(int i=0;i<poly.Length;i++)
        {
          // triangles around the perimeter of the object
            int n = (i+1)%poly.Length;
            triangles[count_tris] = i;
            triangles[count_tris+1] = n;
            triangles[count_tris+2] = i + poly.Length;
            triangles[count_tris+3] = n;
            triangles[count_tris+4] = n + poly.Length;
            triangles[count_tris+5] = i + poly.Length;
            count_tris += 6;
        }
        m.vertices = vertices;
        m.triangles = triangles;
        m.RecalculateNormals();
        m.RecalculateBounds();
        m.Optimize();
        return m;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
