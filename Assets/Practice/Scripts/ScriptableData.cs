using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ScriptableData", menuName = "wanaka-training/ScriptableData", order = 0)]
public class ScriptableData : ScriptableObject
{
    public List<Color> colors = new List<Color>();
    public List<int> selectedColorIndex = new List<int>();
}
