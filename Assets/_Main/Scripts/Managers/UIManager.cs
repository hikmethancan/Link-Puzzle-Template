using System;
using _Main.Scripts.Utilities.Singletons;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Managers
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private TMP_Text moveCountText;
        [SerializeField] private TMP_Text comboText;

        public TMP_Text ComboText => comboText;


        private void OnEnable()
        {
            comboText.gameObject.SetActive(false);
        }

        public void SetMoveCountText(int moveCount)
        {
            moveCountText.SetText($"Move Count : {moveCount}");
        }

        public void SetComboText(string text)
        {
            comboText.SetText(text);
        }
    }
}