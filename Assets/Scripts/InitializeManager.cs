using UnityEngine;
using System.Collections;

public class InitializeManager : MonoBehaviour {
    [SerializeField]
    private Vector2 _mapSize;

	void Awake()
    {
        MapCreator.Instance.CreateTilemap(_mapSize);
    }
}
