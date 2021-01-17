using UnityEngine;

namespace GridSystem
{
    public abstract class CameraAction : MonoBehaviour
    {
        public abstract void Perform(Camera camera);
    }
}