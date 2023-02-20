using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameData
{
    [Serializable]
    public class FloorData
    {
        public string floorName;
        public Sprite floorSprite;
        public bool isLeftFloor;
        public List<MovingObjectData> objects = new List<MovingObjectData>();
    }
}