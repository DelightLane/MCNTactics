using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : FZ.Singletone<GameManager>
{
    private SelectHandler _selectHandler;

    private GameManager()
    {
        _selectHandler = new SelectHandler();
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
                selected.EnqueueActor(actType);
            }
        }

        public void CancelAction()
        {
            var selected = _selectedObj as ActionObject;

            if (selected != null)
            {
                if (selected.HasActor())
                {
                    selected.DequeueActor();
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
    private class TurnHandler
    {
        private HashSet<eCombatTeam> _existTeams;

        private eCombatTeam _currentTeam;
        private int _remainActPoint;

        // TODO : 매직 넘버가 아니라 다른 방식으로 정의할 것
        private const int MaxRemainActPoint = 50;

        public void EndTurn()
        {
            _remainActPoint = MaxRemainActPoint;

            SetNextTurnTeam();
        }

        public bool IsTurnOver()
        {
            return _remainActPoint <= 0;
        }

        public bool UseActPoint(int actPoint)
        {
            if(_remainActPoint >= actPoint)
            {
                _remainActPoint -= actPoint;
                return true;
            }
            return false;
        }

        public eCombatTeam GetCurrentTeam()
        {
            return _currentTeam;
        }

        private void SetNextTurnTeam()
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
        #endregion
    }
}
