using System.Collections.Generic;
using _Main.Scripts.GamePlay.InGame;
using _Main.Scripts.Pool;
using _Main.Scripts.Utilities.Singletons;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class PoolManager : Singleton<PoolManager>
    {
        [SerializeField] private List<BallPool> ballPools = new List<BallPool>();

        public Ball GetBall(int index)
        {
            var ball = ballPools[index].Get();
            ball.gameObject.SetActive(true);
            return ball;
        }
    }
}