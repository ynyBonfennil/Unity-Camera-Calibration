using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class CameraSetup : MonoBehaviour
{
    public JsonLoader jsonLoader;
    private Transforms data;

    private void Awake()
    {
        
    }

    [ContextMenu("Run")]
    void Run()
    {
        if (jsonLoader)
        {
            for (int i=0; i<jsonLoader.Data.frames.Length; i++)
            {
                string name = Path.GetFileName(jsonLoader.Data.frames[i].file_path);
	            GameObject cameraObject = new GameObject(name);
                cameraObject.AddComponent<Camera>();
                CameraParameter param = cameraObject.AddComponent<CameraParameter>();

                param.f_x = jsonLoader.Data.fl_x;
                param.f_y = jsonLoader.Data.fl_y;
                param.c_x = jsonLoader.Data.cx;
                param.c_y = jsonLoader.Data.cy;
                param.pixelWidth = jsonLoader.Data.w;
                param.pixelHeight = jsonLoader.Data.h;
                param.Extrinsic = jsonLoader.Data.frames[i].transform;

                param.ApplyChange();
	        }
	    }

    }
}
