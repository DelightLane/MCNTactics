using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCN
{
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
