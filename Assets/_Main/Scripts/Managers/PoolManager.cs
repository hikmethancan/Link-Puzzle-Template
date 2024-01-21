using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.Enums;
using _Main.Scripts.GamePlay.InGame;
using _Main.Scripts.Pool;
using _Main.Scripts.Utilities.Singletons;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class PoolManager : Singleton<PoolManager>
    {
        [SerializeField] private List<BasketBallPool> basketPools = new List<BasketBallPool>();

        public BasketBall GetBasketBall(int index)
        {
            var ball = basketPools[index].Get();
            ball.gameObject.SetActive(true);
            return ball;
        }
        public BasketBall GetBasketBall(BasketBallType type)
        {
            var ball = basketPools.FirstOrDefault(x => x.GetPrefab().ballType == type)?.Get();
            if (ball != null)
            {
                ball.gameObject.SetActive(true);
                return ball;
            }
            return null;
        }
    }
}