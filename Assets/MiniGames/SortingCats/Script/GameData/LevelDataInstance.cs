using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    [CreateAssetMenu(fileName = "LevelDataInstance", menuName = "LevelData/LevelDataInstance", order = 0)]
    public class LevelDataInstance : ScriptableObject
    {
        public List<FloorData> floors = new List<FloorData>();
    }
}