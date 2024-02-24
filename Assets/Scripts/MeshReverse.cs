using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScriptTest : MonoBehaviour
{
    void Start() {
        MeshFilter MeshFilter = GetComponent<MeshFilter>();
        Mesh Mesh = Instantiate(MeshFilter.sharedMesh);
        Mesh.triangles = Mesh.triangles.Reverse().ToArray();
        MeshFilter.sharedMesh = Mesh;
    }
}
