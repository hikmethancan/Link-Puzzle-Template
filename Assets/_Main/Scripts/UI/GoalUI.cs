using _Main.Scripts.Enums;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Main.Scripts.UI
{
    public class GoalUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text countText;
        [FormerlySerializedAs("basketType")] [SerializeField] private BallType type;
        [SerializeField] private Image icon;
        [SerializeField] private Image frame;
        public BallType GetBasketType()
        {
            return type;
        }
        public void SetCount(float count,bool isAnimate)
        {
            if (isAnimate)
            {
                transform.DOComplete(); 
                transform.DOPunchScale(Vector3.one * .3f, .2f, 2, 2f).SetEase(Ease.OutBack).OnComplete(()=>transform.DORewind());    
            }
            countText.SetText($"x{count}");
            if (count <= 0)
            {
                icon.color = Color.gray;
                frame.color = Color.gray;
            }
        }
    }
}