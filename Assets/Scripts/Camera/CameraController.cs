using System.Collections.Generic;
using GridSystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class CameraController : SerializedMonoBehaviour
{
    [SerializeField] 
    private Camera mainCamera;

    [OdinSerialize] 
    private List<ICameraAction> cameraActionsOnUpdate = new List<ICameraAction>();
    
    private void Update()
    {
        cameraActionsOnUpdate.ForEach(a => a.PerformOnCamera(mainCamera));
    }
}