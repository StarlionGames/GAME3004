using System;
using UnityEngine;

public enum TileTypes {
    Grass,
    Stone,
    Coal,
    Iron,
    Gold,
    Diamond
}

public class TileProperties : MonoBehaviour
{
    public Vector3 _position { get; private set; }
    public bool _isVisible = true;
    public TileTypes tileType = TileTypes.Stone;

    BoxCollider _collider => GetComponent<BoxCollider>();
    MeshRenderer _meshRenderer => GetComponent<MeshRenderer>();

    public void Initialize(Vector3 pos, TileTypes type)
    {
        _position = pos;
        tileType = type;
        DetermineTypeVisuals();
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

    public void DetermineTypeVisuals()
    {
        switch (tileType)
        {
            case TileTypes.Grass: _meshRenderer.material.color = Color.green; break;
            case TileTypes.Stone: _meshRenderer.material.color = Color.grey; break;
            case TileTypes.Coal: _meshRenderer.material.color = Color.black; break;
            case TileTypes.Iron: _meshRenderer.material.color = Color.darkGray; break;
            case TileTypes.Gold: _meshRenderer.material.color = Color.gold; break;
            case TileTypes.Diamond: _meshRenderer.material.color = Color.lightBlue; break;
        }
    }
}
