using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "RuleTile_", menuName = "ScriptableObject/RuleTile")]
public class RuleTileSO : ScriptableObject
{
    public List<RuleTileValueClass> ruleTiles;
}

[System.Serializable]
public class RuleTileValueClass
{
    public NeighborDirection neighbor;
    public Tile tile;
}
