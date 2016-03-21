using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCN
{
    /**
    *@brief 상태 추상 클래스
    *@details State 패턴을 구현하기 위한 추상 클래스.
    *StateMachine 객체를 통하여 조작된다.
    *@author Delight
    */
    public abstract class State : IDisposable
    {
        private TacticsObject _target;
        protected TacticsObject Target {
            get
            {
                return _target;
            }
        }

        public State(TacticsObject target)
        {
            _target = target;

            Initialize();
        }

        public virtual void Initialize() { }
        public virtual void Destroy() { }

        public abstract void Run();
        public virtual void Finish() { }

        public void Dispose()
        {
            _target = null;
        }
    }

    /**
    *@brief 상태 머신 클래스
    *@details State 패턴을 조작하기 위한 has a 클래스.
    *State 패턴이 필요한 클래스는 해당 클래스의 인스턴스를 지역 변수로 가진다.
    *@author Delight
    */
    public class StateMachine<T> where T : State, IDisposable
    {
        private Dictionary<string, T> _states = new Dictionary<string, T>();
        private T _currentState;

        public void StorageState(string stateName, T state)
        {
            _states.Add(stateName, state);
        }

        public State GetCurrentState()
        {
            return _currentState;
        }

        public void ChangeState(string stateName)
        {
            if(_currentState != null)
            {
                _currentState.Finish();
            }

            _currentState = _states[stateName];

            if(_currentState != null)
            {
                _currentState.Run();
            }
        }

        public void Dispose()
        {
            foreach(var state in _states)
            {
                if(state.Value != null)
                {
                    state.Value.Dispose();
                }
            }

            _states.Clear();
        }
    }
}
