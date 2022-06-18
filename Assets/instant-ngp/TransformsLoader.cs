using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TransformsLoader : MonoBehaviour
{
    public string FilePath;
    public Transforms Data = new Transforms();
    
    private string jsonText;

    private void Awake()
    {
        FilePath = Application.dataPath;
    }

    [ContextMenu("LoadJson")]
    void LoadJson()
    {
        if (Path.GetExtension(FilePath) == ".json")
        {
            if (File.Exists(FilePath))
            {
                jsonText = File.ReadAllText(FilePath);

                // JsonUtility cannot be used for json with 2d array.
                //Data = JsonUtility.FromJson<Transforms>(jsonText);

                // Instead we use Newtonsoft Json
                Data = JsonConvert.DeserializeObject<Transforms>(jsonText);

                // Format transform_matrix into transform
                // so the unity can display in the editor.
                for (int i=0; i<Data.frames.Length; i++)
                {
                    List<float[]> mat = Data.frames[i].transform_matrix;
                    Data.frames[i].transform = new Matrix4x4(
                        new Vector4(mat[0][0], mat[1][0], mat[2][0], mat[3][0]),
                        new Vector4(mat[0][1], mat[1][1], mat[2][1], mat[3][1]),
                        new Vector4(mat[0][2], mat[1][2], mat[2][2], mat[3][2]),
                        new Vector4(mat[0][3], mat[1][3], mat[2][3], mat[3][3]));
		        }

	        }
            else
            {
                Debug.LogError("File not found: " + FilePath);
	        }
	    }
        else
        { 
            Debug.LogError(FilePath + " is not a JSON file.");
	    }
    }
}
