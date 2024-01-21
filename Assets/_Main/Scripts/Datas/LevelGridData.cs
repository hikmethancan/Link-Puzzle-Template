using System.Collections.Generic;
using _Main.Scripts.Enums;
using _Main.Scripts.GamePlay.InGame;
using NaughtyAttributes;
using UnityEngine;

namespace _Main.Scripts.Datas
{
    [System.Serializable]
    public class LevelGridData
    {
        [InfoBox("Is All Color Include")] public bool isAllColorInclude;
        [InfoBox("All Color Count")] public int allColorCount;
        
        [InfoBox("Goals")] 
        public List<Goal> goals = new List<Goal>();

        [Space] [Space] [Space] [InfoBox("Hamle sayısı.", EInfoBoxType.Normal)]
        public int moveCount = 10;

        [Space(15)]
        [InfoBox("Listeye ekleme çıkarma yapma. Spawn olmasını istemediğin rengin yüzdesini 0'a çek.",
            EInfoBoxType.Normal)]
        public List<ColorSpawnData> colorSpawnDatas;

        [Space(15)]
        [InfoBox("Listeye ekleme çıkarma yapma. Hedef kamyonu olmasını istemediğin rengin değerini 0 yap.",
            EInfoBoxType.Normal)]
        public TargetData[] targetDatas;

        [Space(50)] [InfoBox("Genel grid ayarları. Bu değerleri değiştirmene gerek yok.", EInfoBoxType.Normal)]
        public int gridXCount = 5;

        public int gridZCount = 6;
        [Range(2, 5)] public int minDrawableCount = 2;
    }

    [System.Serializable]
    public class TargetData
    {
        public BasketBallType basketballType;
        public int targetCount;
    }

    [System.Serializable]
    public class ColorSpawnData
    {
        public BasketBallType basketballType;
        [Range(0, 100)] public int chancePercent = 100;
    }
}