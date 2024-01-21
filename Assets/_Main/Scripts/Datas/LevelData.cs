using System.Collections.Generic;
using _Main.Scripts.Enums;
using _Main.Scripts.GamePlay.InGame;
using NaughtyAttributes;
using UnityEngine;

namespace _Main.Scripts.Datas
{
    [System.Serializable]
    public class LevelData
    {
        [Space(50)] [InfoBox("Grid's Row Column Value", EInfoBoxType.Normal)]
        public int column = 5;

        public int row = 6;

        [Space(50)] [InfoBox("Minimum selected ball count for match", EInfoBoxType.Normal)] [Range(2, 5)]
        public int minDrawableCount = 2;

        [InfoBox("Is All Color Include")] public bool isAllColorInclude;
        [InfoBox("All Color Count")] public int allColorCount;
        [Space(30)] [InfoBox("Goals")] public List<Goal> goals = new List<Goal>();

        [Space(30)] [InfoBox("Move Count.", EInfoBoxType.Normal)]
        public int moveCount = 10;

        [Space(15)]
        [InfoBox("Balls Spawn Percentages",
            EInfoBoxType.Normal)]
        public List<BallSpawnData> ballSpawnDatas;
    }


    [System.Serializable]
    public class BallSpawnData
    {
        public BallType ballType;
        [Range(0, 100)] public int chancePercentage = 100;
    }
}