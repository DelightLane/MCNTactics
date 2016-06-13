using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FZ
{
    /**
    *@brief 상태 인터페이스
    *@details State 패턴을 구현하기 위한 인터페이스
    *State<T> 제네릭 클래스가 구현한다.
    *@author Delight
    */
    public interface IState
    {
        void Initialize();
        void Destroy();

        void Run();
        void Finish();
    }

    /**
    *@brief 상태 추상 클래스
    *@details State 패턴을 구현하기 위한 추상 클래스.
    *StateMachine 객체를 통하여 조작된다.
    *@author Delight
    */
    public abstract class State<T> : IState, IDisposable
    {
        private T _target;
        protected T Target {
            get
            {
                return _target;
            }
        }

        public State(T target)
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
            _target = default(T);
        }
    }

    /**
    *@brief 상태 머신 클래스
    *@details State 패턴을 조작하기 위한 has a 클래스.
    *State 패턴이 필요한 클래스는 해당 클래스의 인스턴스를 지역 변수로 가진다.
    *@author Delight
    */
    public class StateMachine<T> where T : IState, IDisposable
    {
        private Dictionary<Type, T> _states = new Dictionary<Type, T>();
        private T _currentState;

        public void StorageState(T state)
        {
            _states.Add(state.GetType(), state);
        }

        public T GetCurrentState()
        {
            return _currentState;
        }

        public void ChangeState<T2>() where T2 : T
        {
            if(_currentState != null)
            {
                _currentState.Finish();
            }

            _currentState = _states[typeof(T2)];

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
