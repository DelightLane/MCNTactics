using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager
{
    private static GameManager _instance;
    private Dictionary<Type, Handler> _handlers;

    private GameManager()
    {
        _handlers = new Dictionary<Type, Handler>();

        _handlers.Add(typeof(Select), new Select());
        _handlers.Add(typeof(Turn), new Turn());
    }

    public static T Get<T>() where T : Handler
    {
        if (_instance == null)
        {
            _instance = new GameManager();
        }

        return (T)_instance._handlers[typeof(T)];
    }

    #region Handler
    public abstract class Handler { }

    public class Select : Handler
    {
        public  TacticsObject Target { get; set; }

        public void Action(System.Type actType)
        {
            var selected = Target as ActionObject;

            if (selected != null)
            {
                selected.ReserveActor(actType);
            }
        }

        public void CancelAction()
        {
            var selected = Target as ActionObject;

            if (selected != null)
            {
                if (selected.HasActor())
                {
                    selected.CancelActor();
                }
                else
                {
                    selected.Deselect();
                }
            }
        }
    }

    // 유닛 오브젝트만 턴의 제약을 가진다.
    public class Turn : Handler
    {
        private HashSet<eCombatTeam> _existTeams = new HashSet<eCombatTeam>();

        private eCombatTeam _currentTeam;
        private int _remainActPoint = MaxRemainActPoint;

        // TODO : 매직 넘버가 아니라 다른 방식으로 정의할 것
        private const int MaxRemainActPoint = 50;

        public int ActPoint { get { return _remainActPoint; } }
        public eCombatTeam Team { get { return _currentTeam; } }

        public void RegisterTeam(eCombatTeam team)
        {
            _existTeams.Add(team);

            // TODO : 가장 처음 시작하는 팀을 선택하는 방식을 정할 것
            // 현재는 가장 마지막에 추가되는 유닛의 팀이 시작 팀이 되는 로직
            _currentTeam = team;
        }

        public void ResetRegisterTeams()
        {
            _existTeams.Clear();
        }

        public void EndTurn()
        {
            if (IsTurnOver())
            {
                PureEndTurn();
            }
        }

        public void ForceEndTurn()
        {
            // TODO : 누군가 액션을 하고 있다면 그걸 모두 취소시켜주어야 한다.
           PureEndTurn();
        }

        private void PureEndTurn()
        {
            _remainActPoint = MaxRemainActPoint;

            GoToNextTurnTeam();
        }

        public bool DoTurn(IUnitActor act)
        {
            int actPoint = act.ActPoint;

            if (_remainActPoint >= actPoint)
            {
                _remainActPoint -= actPoint;
                return true;
            }
            return false;
        }
        
        public void UndoTurn(IUnitActor act)
        {
            int actPoint = act.ActPoint;

            _remainActPoint += actPoint;
        }

        public bool IsTurnOver()
        {
            return _remainActPoint <= 0;
        }

        public eCombatTeam GetCurrentTeam()
        {
            return _currentTeam;
        }

        public bool IsCurrentTeam(eCombatTeam team)
        {
            return _currentTeam == team;
        }

        private void GoToNextTurnTeam()
        {
            int teamCount = Enum.GetValues(typeof(eCombatTeam)).Length - 1;

            if ((int)_currentTeam < teamCount)
            {
                ++_currentTeam;
            }
            else
            {
                _currentTeam = (eCombatTeam)0;
            }

            RepairTurnTeam();
        }

        private void RepairTurnTeam()
        {
            if (!_existTeams.Contains(_currentTeam))
            {
                PureEndTurn();
            }
        }
    }
    #endregion
}
