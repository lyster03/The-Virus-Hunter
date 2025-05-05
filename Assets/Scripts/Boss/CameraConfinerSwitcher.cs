using UnityEngine;
using Cinemachine;

public class CameraConfinerSwitcher : MonoBehaviour
{
    public CinemachineConfiner confiner;
    public PolygonCollider2D normalBounds;
    public PolygonCollider2D bossBounds;

    public void SwitchToBossBounds()
    {
        confiner.m_BoundingShape2D = bossBounds;
        
    }

    public void SwitchToNormalBounds()
    {
        confiner.m_BoundingShape2D = normalBounds;
        
    }
}
