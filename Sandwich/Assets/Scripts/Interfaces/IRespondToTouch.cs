using UnityEngine;

/// <summary>
/// Can be implemented by anything that needs to have their own touch response
/// </summary>
public interface IRespondToTouch
{
    void AttemptFlip(Vector3 dir);
}