using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using FZ;

public class GameManager : FZ.GeneralSingletone<GameManager, GameManager.Handler>
{
    private GameManager()
    {
        FZ.GeneralSingletone<GameManager, GameManager.Handler>.RegisterHandler(new Select());
        FZ.GeneralSingletone<GameManager, GameManager.Handler>.RegisterHandler(new Turn());
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
    public class Turn : Handler, FZ.IObservable<eCombatTeam>
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
            // 현재는 가장 enum의 처음에 있으며 등록된 팀이 시작팀
            _currentTeam = eCombatTeam.UNSELECT;
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

            SelectCurrectTurnTeam();
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

        public void SelectCurrectTurnTeam()
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

            bool repair = RepairTurnTeam();

            if (!repair)
            {
                NotifyTeamsTurn();
            }
        }

        private bool RepairTurnTeam()
        {
            if (!_existTeams.Contains(_currentTeam))
            {
                PureEndTurn();

                return true;
            }

            return false;
        }

        private List<IObserver<eCombatTeam>> _observers = new List<IObserver<eCombatTeam>>();

        public void Subscribe(IObserver<eCombatTeam> observer)
        {
            if (_observers != null)
            {
                _observers.Add(observer);
            }
        }

        public void Unsubscribe(IObserver<eCombatTeam> observer)
        {
            if (_observers != null)
            {
                _observers.Remove(observer);
            }
        }

        private void NotifyTeamsTurn()
        {
            for(int i = 0; i < _observers.Count; ++i)
            {
                _observers[i].OnNext(this.Team);
            }
        }
    }
    #endregion
}
