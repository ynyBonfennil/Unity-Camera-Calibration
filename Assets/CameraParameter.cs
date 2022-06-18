using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraParameter : MonoBehaviour
{
    private Camera cam;
    public Matrix4x4 Extrinsic;
    public float f_x = 0;
    public float f_y = 0;
    public float c_x = 0;
    public float c_y = 0;
    public float pixelWidth = 0;
    public float pixelHeight = 0;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam) {
            Debug.Log("Camera Parameter: Successfully got camera component.");
	    }
        if (!cam) {
            Debug.LogError("CameraParameter: Failed to get camera component.");
	    }
    }

    [ContextMenu("Get Change")]
    private void GetChange()
    {
        float mmToPixelX = cam.pixelWidth / cam.sensorSize.x;
        float mmToPixelY = cam.pixelHeight / cam.sensorSize.y;
        f_x = cam.focalLength * mmToPixelX;
        f_y = cam.focalLength * mmToPixelY;
        c_x = cam.pixelWidth / 2f + cam.lensShift.x * mmToPixelX;
        c_y = cam.pixelHeight / 2f + cam.lensShift.y * mmToPixelY;
        pixelWidth = cam.pixelWidth;
        pixelHeight = cam.pixelHeight;

        // RT in unity's coordinate system
        Matrix4x4 R_unity = new Matrix4x4();
        R_unity.SetColumn(0, transform.rotation * new Vector4(1, 0, 0, 0));
        R_unity.SetColumn(1, transform.rotation * new Vector4(0, 1, 0, 0));
        R_unity.SetColumn(2, transform.rotation * new Vector4(0, 0, 1, 0));

        Matrix4x4 coordTransform = new Matrix4x4(
                                        new Vector4(1, 0, 0, 0),
                                        new Vector4(0, 1, 0, 0),
                                        new Vector4(0, 0, -1, 0),
                                        new Vector4(0, 0, 0, 0));

        Matrix4x4 defaultCameraR_inv = new Matrix4x4(
                                        new Vector4(1, 0, 0, 0),
                                        new Vector4(0, 0, -1, 0),
                                        new Vector4(0, 1, 0, 0),
                                        new Vector4(0, 0, 0, 0));

        Matrix4x4 R_0 = coordTransform * defaultCameraR_inv * R_unity * coordTransform;
        Vector4 T_0 = coordTransform * defaultCameraR_inv * transform.position;

        for (int i=0; i<3; i++)
        { 
            for (int j=0; j<3; j++)
            {
                R_0[i, j] = Mathf.Round(R_0[i, j] * 1e6f) * 1e-6f;
	        }	
	    }

        Extrinsic = new Matrix4x4(
                    new Vector4(R_0[0, 0], R_0[1, 0], R_0[2, 0], 0),
                    new Vector4(R_0[0, 1], R_0[1, 1], R_0[2, 1], 0),
                    new Vector4(R_0[0, 2], R_0[1, 2], R_0[2, 2], 0),
                    new Vector4(T_0[0], T_0[1], T_0[2], 1));
    }

    [ContextMenu("Apply Change")]
    private void ApplyChange()
    {
        cam.usePhysicalProperties = true;

        // Camera Intrinsics
        cam.pixelRect = new Rect(new Vector2(0, 0), new Vector2(pixelWidth, pixelHeight));
        float mmToPixelX = pixelWidth / cam.sensorSize.x;
        float mmToPixelY = pixelHeight / cam.sensorSize.y;
        cam.focalLength = f_x / mmToPixelX;
        cam.focalLength = f_y / mmToPixelY;
        cam.lensShift = new Vector2(
	                        (c_x - pixelWidth / 2f) / mmToPixelX,
			                (c_y - pixelHeight / 2f) / mmToPixelY);

        // Camera Extrinsics
        Vector4 T_0 = new Vector4(Extrinsic[0, 3], Extrinsic[1, 3], Extrinsic[2, 3], 0);
        Matrix4x4 R_0 = new Matrix4x4();
        R_0.SetColumn(0, Extrinsic.GetColumn(0));
        R_0.SetColumn(1, Extrinsic.GetColumn(1));
        R_0.SetColumn(2, Extrinsic.GetColumn(2));
        R_0.SetColumn(3, new Vector4(0, 0, 0, 0));

        Matrix4x4 coordTransform = new Matrix4x4(
                                        new Vector4(1, 0, 0, 0),
                                        new Vector4(0, 1, 0, 0),
                                        new Vector4(0, 0, -1, 0),
                                        new Vector4(0, 0, 0, 0));

        Matrix4x4 defaultCameraR = new Matrix4x4(
                                        new Vector4(1, 0, 0, 0),
                                        new Vector4(0, 0, 1, 0),
                                        new Vector4(0, -1, 0, 0),
                                        new Vector4(0, 0, 0, 0));

        // R_0 in the original coordinate system can be described in unity coordinate as
        // coordTransform^-1 * R_0 * coordTransform
        // where coordTransform is the rotation from the original to unity coordinate.
        Matrix4x4 R_unity = defaultCameraR * coordTransform * R_0 * coordTransform;

        Quaternion q = Quaternion.LookRotation(R_unity.GetColumn(2), R_unity.GetColumn(1));
        transform.rotation = q;
        transform.position = defaultCameraR * coordTransform * T_0;
    }
}
