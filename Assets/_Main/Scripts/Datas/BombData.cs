using UnityEngine;

namespace _Main.Scripts.Datas
{
    [CreateAssetMenu(fileName = "Bomb", menuName = "Bomb", order = 0)]
    public class BombData : ScriptableObject
    {
        public int explosionX;
        public int explosionY;
        public float explosionWaitDuration;
    }
}