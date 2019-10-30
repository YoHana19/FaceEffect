using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaceTrackingVFX;
using UnityEngine.Events;
using UnityEngine.Experimental.VFX;

internal class UnityEventArgInt : UnityEvent<int> {}

public class VertCountGetter : MonoBehaviour
{
    private ARFaceMeshBaker faceMeshBaker;
    private VisualEffect vfx;
    
    private void Awake()
    {
        vfx = GetComponent<VisualEffect>();
        faceMeshBaker = GetComponent<ARFaceMeshBaker>();
        faceMeshBaker.GetVertexCount.AddListener(SetVfxSpawnCount);
    }

    private void SetVfxSpawnCount(int count)
    {
        vfx.SetInt("PointCount", count);
    }
}
