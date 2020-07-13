using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HFComponentRemover : MonoBehaviour
{
    public enum ComponentTypes 
    {
        Collider,
        NavMeshObstacle,
        TileHighLight,
        SpriteRenderer,
    }

    public ComponentTypes TypeToRemove;

    [ContextMenu("Remove Components in childrens")]
    public void RemoveComponentAction() 
    {
        switch (TypeToRemove) {
            case ComponentTypes.Collider:
                RemoveComponent<Collider>();
                break;
            case ComponentTypes.NavMeshObstacle:
                RemoveComponent<NavMeshObstacle>();
                break;
            case ComponentTypes.TileHighLight:
                RemoveComponent<TileHighlight>();
                break;
            case ComponentTypes.SpriteRenderer:
                RemoveComponent<SpriteRenderer>();
                break;
            default:
                break;
        }
    }

    public void RemoveComponent<T>() where T : Component
    {
        T[] obstacles;
        obstacles = GetComponentsInChildren<T>();

        foreach (var item in obstacles) 
        {
            DestroyImmediate(item);
        }
    }
}
