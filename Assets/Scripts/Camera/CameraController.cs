using GridSystem;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] 
    private Camera mainCamera;

    [SerializeField]
    private List<CameraAction> cameraActions = new List<CameraAction>();
    
    private void Update()
    {
        foreach (var action in cameraActions)
            action.Perform(mainCamera);
    }
}