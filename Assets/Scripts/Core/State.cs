using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCN
{
    public abstract class State
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
    }

    public class StateMachine<T> where T : State
    {
        private Dictionary<string, T> _states = new Dictionary<string, T>();
        private T _currentState;

        public void StorageState(string stateName, T state)
        {
            _states.Add(stateName, state);
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
    }
}
