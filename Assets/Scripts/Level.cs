using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "New Level")]
public class Level : ScriptableObject
{
    public int levelNumber;
    public int moves;
    public int[] initialNumbers;
    public int[] correctOrder;
}
