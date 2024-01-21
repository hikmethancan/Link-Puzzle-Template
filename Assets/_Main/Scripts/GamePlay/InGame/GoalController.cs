using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.Enums;
using _Main.Scripts.Managers;
using _Main.Scripts.UI;
using UnityEngine;

namespace _Main.Scripts.GamePlay.InGame
{
    public class GoalController : MonoBehaviour
    {
        [SerializeField] private List<GoalUI> goalUIs = new List<GoalUI>();
        [SerializeField] private GoalUI allIncludeUI;
        public bool IsAllColorInclude { get; set; }
        private List<GoalUI> _currentGoals = new List<GoalUI>();
        
        public void SetGoals(List<Goal> goals)
        {
            if (GridManager.Instance.ActiveLevelData.isAllColorInclude)
            {
                var initGoal = allIncludeUI;
                var goal = Instantiate(initGoal, transform);
              
                goal.gameObject.SetActive(false);
                goal.gameObject.SetActive(true);
                goal.SetCount(GridManager.Instance.ActiveLevelData.allColorCount,false);
                _currentGoals.Add(goal);
            }
            for (int i = 0; i < goals.Count; i++)
            {
                var initGoal = goalUIs.FirstOrDefault(x => x.GetBasketType() == goals[i].ballType);
                if (initGoal is not null)
                {
                    var goal = Instantiate(initGoal, transform);
                    goal.SetCount(goals[i].count,true);
                    _currentGoals.Add(goal);    
                }
            }
        }

        public void SetCount(BallType ballType,int count)
        {
            // if (GridManager.Instance.ActiveLevelGridData.isAllColorInclude)
            // {
            //     var getAllGoal = _currentGoals.FirstOrDefault(x => x.GetBasketType() == BasketBallType.All);
            //     if (getAllGoal != null) getAllGoal.SetCount(count);  
            //     return;
            // }
            var getGoal = _currentGoals.FirstOrDefault(x => x.GetBasketType() == ballType);
            if (getGoal != null) getGoal.SetCount(count,true);
        }
    }
}