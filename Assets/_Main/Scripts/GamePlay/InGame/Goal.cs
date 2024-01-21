using _Main.Scripts.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Main.Scripts.GamePlay.InGame
{
    [System.Serializable]
    public class Goal
    {
        public int count;
        public BallType ballType;
    }
}