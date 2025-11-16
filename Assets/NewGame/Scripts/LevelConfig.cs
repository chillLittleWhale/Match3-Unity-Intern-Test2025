using UnityEngine;
using System;
using System.Collections.Generic;

namespace NewGame
{
    [CreateAssetMenu(fileName = "LevelConfig_new", menuName = "SO/LevelConfig", order = 0)]
    [Serializable]
    public class LevelConfig : ScriptableObject
    {
        public List<BoardConfig> BoardConfigs;
    }

    [Serializable]
    public struct BoardConfig
    {
        public Vector2Int Size; // kích thước board
        public Vector2 Origin; // gốc tọa độ
    }
}
