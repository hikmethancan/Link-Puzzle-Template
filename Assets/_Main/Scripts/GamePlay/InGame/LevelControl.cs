using _Main.Scripts.Datas;
using _Main.Scripts.Managers;
using UnityEngine;

namespace _Main.Scripts.GamePlay.InGame
{
    public class LevelControl : MonoBehaviour
    {
        [SerializeField] private LevelData levelData;


        private void Start()
        {
            GridManager.Instance.SetLevelData(levelData);
            GridManager.Instance.Setup();
        }
    }
}
