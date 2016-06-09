using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : FZ.Singletone<GameManager>
{
    private SelectHandler _selectHandler;
    private TurnHandler _turnHandler;

    private GameManager()
    {
        _selectHandler = new SelectHandler();
        _turnHandler = new TurnHandler();
    }

    #region SelectHandler
    private class SelectHandler
    {
        private TacticsObject _selectedObj;

        public void Set(TacticsObject obj)
        {
            _selectedObj = obj;
        }

        public TacticsObject Get()
        {
            return _selectedObj;
        }

        public void Action(System.Type actType)
        {
            var selected = _selectedObj as ActionObject;

            if (selected != null)
            {
                selected.ReserveActor(actType);
            }
        }

        public void CancelAction()
        {
            var selected = _selectedObj as ActionObject;

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

    public TacticsObject SelectedObj
    {
        get
        {
            return _selectHandler.Get();
        }

        set
        {
            _selectHandler.Set(value);
        }
    }

    public void ActionSelectObj(System.Type actType)
    {
        _selectHandler.Action(actType);
    }

    public void CancelActionSelectObj()
    {
        _selectHandler.CancelAction();
    }
    #endregion

    #region TurnHandler
    // 유닛 오브젝트만 턴의 제약을 가진다.
    private class TurnHandler
    {
        private HashSet<eCombatTeam> _existTeams = new HashSet<eCombatTeam>();

        private eCombatTeam _currentTeam;
        private int _remainActPoint = MaxRemainActPoint;

        // TODO : 매직 넘버가 아니라 다른 방식으로 정의할 것
        private const int MaxRemainActPoint = 50;

        public void RegisterTeam(eCombatTeam team)
        {
            _existTeams.Add(team);

            // TODO : 가장 처음 시작하는 팀을 선택하는 방식을 정할 것
            // 현재는 가장 마지막에 추가되는 유닛의 팀이 시작 팀이 되는 로직
            _currentTeam = team;
        }

        public int RemainActPoint { get { return _remainActPoint; } }
        public eCombatTeam CurrentTeam { get { return _currentTeam; } }

        public void ResetRegisterTeams()
        {
            _existTeams.Clear();
        }

        public void EndTurn()
        {
            _remainActPoint = MaxRemainActPoint;

            GoToNextTurnTeam();
        }

        public bool DoTurn(int actPoint)
        {
            if (_remainActPoint >= actPoint)
            {
                _remainActPoint -= actPoint;
                return true;
            }
            return false;
        }

        public void UndoTurn(int actPoint)
        {
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
                EndTurn();
            }
        }
    }

    public int TurnActPoint { get { return _turnHandler.RemainActPoint; } }
    public eCombatTeam TurnTeam { get { return _turnHandler.CurrentTeam; } }

    public void RegisterJoinTeam(eCombatTeam team)
    {
        _turnHandler.RegisterTeam(team);
    }

    public void ResetJoinTeams()
    {
        _turnHandler.ResetRegisterTeams();
    }

    public void ForceEndTurn()
    {
        _turnHandler.EndTurn();
    }

    public void EndTurn()
    {
        if(_turnHandler.IsTurnOver())
        {
            _turnHandler.EndTurn();
        }
    }

    public bool DoTurn(IUnitActor act)
    {
        int actPoint = act.ActPoint;

        return _turnHandler.DoTurn(actPoint);
    }

    public void UndoTurn(IUnitActor act)
    {
        int actPoint = act.ActPoint;

        _turnHandler.UndoTurn(actPoint);
    }

    public eCombatTeam GetCurrentTeam()
    {
        return _turnHandler.GetCurrentTeam();
    }

    public bool IsCurrentTeam(eCombatTeam team)
    {
        return _turnHandler.IsCurrentTeam(team);
    }
    #endregion
}
