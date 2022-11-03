using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BulletSettings
{
    [Header("Game Object")]
    public GameObject prefab;
    
    [Header("Draw Mesh Instanced")]
    public Mesh mesh;
    public Material mat;
    
    [Header("General")]
    public Vector2 bulletSize = new Vector2(0.03f, 0.06f);
    
    [Header("Trail")]
    public float relativeTrailWidth = 0.3f;
    public Vector2 bulletTrailLength = new Vector2(0.1f, 0.2f);
}
