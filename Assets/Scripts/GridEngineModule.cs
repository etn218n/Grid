using UnityEngine;

public abstract class GridEngineModule : MonoBehaviour
{
    public virtual void OnStart(GridEngine engine) { }
    public virtual void OnUpdate(GridEngine engine) { }
    public virtual void OnEnd(GridEngine engine) { }
}