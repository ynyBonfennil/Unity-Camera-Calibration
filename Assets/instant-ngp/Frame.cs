using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Frame
{
    public string file_path;
    public float sharpness;
    public List<float[]> transform_matrix;

    // This parameter is for display in the editor use only.
    // Its values are based on transform_matrix;
    public Matrix4x4 transform;
}
