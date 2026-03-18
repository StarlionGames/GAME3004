using System;
using UnityEngine;

public class TileProperties : MonoBehaviour
{
    public Vector3 _position { get; private set; }
    public bool _isVisible = true;

    BoxCollider _collider => GetComponent<BoxCollider>();
    MeshRenderer _meshRenderer => GetComponent<MeshRenderer>();

    public void Initialize(Vector3 pos)
    {
        _position = pos;
    }

    public void OnDestroy()
    {
        MapGenerator.OnTileDestroyed?.Invoke(gameObject);
    }

    public void ModifyVisuals(bool isEnabled)
    {
        _collider.enabled = isEnabled;
        _meshRenderer.enabled = isEnabled;

        _isVisible = isEnabled;
    }
}
