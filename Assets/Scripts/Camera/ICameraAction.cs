using UnityEngine;

namespace GridSystem
{
    public interface ICameraAction
    {
        void PerformOnCamera(Camera camera);
    }
}