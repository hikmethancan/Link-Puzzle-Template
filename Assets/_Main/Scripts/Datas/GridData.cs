using UnityEngine;

namespace _Main.Scripts.Datas
{
    [CreateAssetMenu(fileName = "Grid", menuName = "Grid", order = 0)]
    public class GridData : ScriptableObject
    {
        public LevelData levelData;
        public int row;
        public int column;
        
    }
}