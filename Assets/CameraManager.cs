using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{

    [SerializeField] Camera mainCamera;
    public static CameraManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive)
        {
            // 查找子场景中的相机
            foreach (GameObject rootObj in scene.GetRootGameObjects())
            {
                Camera cam = rootObj.GetComponentInChildren<Camera>();
                if (cam != null)
                {
                    var camData = cam.GetUniversalAdditionalCameraData();
                    if (camData.renderType == CameraRenderType.Overlay)
                    {
                        var baseData = mainCamera.GetUniversalAdditionalCameraData();
                        if (!baseData.cameraStack.Contains(cam))
                        {
                            baseData.cameraStack.Add(cam);
                            Debug.Log($"Overlay camera '{cam.name}' added to base camera.");
                        }
                    }
                }
            }
        }
    }

}
