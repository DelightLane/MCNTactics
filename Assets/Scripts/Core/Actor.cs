using UnityEngine;
using System;
using System.Collections.Generic;

namespace MCN
{
    // 실행시 큐로 쌓이는 행동 추상 클래스
    // TacticsObject의 내부에 큐로 쌓이면서 실행된다.
    public abstract class Actor
    {
        // 각 액터 별로 필요한 가중치를 설정한다.
        // ex) MoveActor에서 weight는 이동 범위이다.
        [SerializeField]
        private Dictionary<string, int> _weight;
        
        protected TacticsObject ActTarget { get; private set; }

        protected abstract string[] AbsoluteWeightKey();

        public Actor() { }

        public bool CheckAbsoluteWeightKey()
        {
            foreach (var key in AbsoluteWeightKey())
            {
                if (_weight == null || !_weight.ContainsKey(key))
                {
                    throw new UnityException(string.Format("{0} is not available. because weight '{1}' key is not initialized.", this.GetType().Name, key));
                }
            }

            return true;
        }

        public void Initialize(TacticsObject actTarget)
        {
            if (actTarget == null)
            {
                throw new UnityException("Act Target is null.");
            }

            this.ActTarget = actTarget;

            Initialize();
        }

        protected virtual void Initialize() { }

        protected void FinishActor()
        {
            if (ActTarget != null)
            {
                ActTarget.FinishActor();
            }
        }

        public int GetWeight(string key)
        {
            if (_weight.ContainsKey(key))
            {
                return _weight[key];
            }

            return 0;
        }

        public void SetWeight(string key, int weight)
        {
            _weight[key] = weight;
        }

        public void SetWeight(Pair<string, int> info)
        {
            if (info != null)
            {
                if (_weight == null)
                {
                    _weight = new Dictionary<string, int>();
                }

                _weight[info.key] = info.value;
            }
        }

        public virtual void Interactive(TacticsObject interactTarget) { }

        public virtual bool OnTouchEvent(eTouchEvent touch) { return true; }
    }
}
