﻿using UnityEngine;
using System;
using System.Collections.Generic;

namespace MCN
{
    public interface IActorQueue
    {
        void AddActor(MCN.Actor actor);
        void EnqueueActor(System.Type actorType);
        void DequeueActor();
    }

    // 실행시 큐로 쌓이는 행동 추상 클래스
    // TacticsObject의 내부에 큐로 쌓이면서 실행된다.
    public abstract class Actor
    {
        // 각 액터 별로 필요한 가중치를 설정한다.
        // ex) MoveActor에서 weight는 이동 범위이다.
        [SerializeField]
        private Dictionary<string, int> _weight;
        
        protected IActorQueue ActTarget { get; private set; }

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

        public void Initialize(IActorQueue actTarget, List<StringIntPair> weights)
        {
            if (actTarget == null)
            {
                throw new UnityException("Act Target is null.");
            }

            this.ActTarget = actTarget;

            foreach (var actorWeight in weights)
            {
                SetWeight(actorWeight);
            }

            Initialize();
        }

        protected virtual void Initialize() { }

        protected void FinishActor()
        {
            if (ActTarget != null)
            {
                ActTarget.DequeueActor();
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

    public class ActorMachine : IActorQueue
    {
        // Actor의 큐. 만약 큐에 Actor가 있다면 그 Actor는 사용될 준비가 된 것이다.
        private LinkedList<MCN.Actor> _actorQueue = new LinkedList<MCN.Actor>();

        // 해당 TacticsObject가 행동을 취할 수 있는 Actor들
        private Dictionary<string, MCN.Actor> _actors = new Dictionary<string, MCN.Actor>();

        public void AddActor(MCN.Actor actor)
        {
            if (actor.CheckAbsoluteWeightKey())
            {
                _actors.Add(actor.GetType().ToString(), actor);
            }
        }

        public void EnqueueActor(System.Type actorType)
        {
            if (!actorType.IsSubclassOf(typeof(MCN.Actor)) ||
               !_actors.ContainsKey(actorType.ToString()))
            {
                throw new UnityException("Actor's type is not correct.");
            }

            _actorQueue.AddLast(_actors[actorType.ToString()]);
        }

        public MCN.Actor GetActiveActor()
        {
            if (_actorQueue.Count > 0)
            {
                var actor = _actorQueue.First.Value;

                return actor;
            }

            return null;
        }

        public void DequeueActor()
        {
            if (_actorQueue.Count > 0)
            {
                _actorQueue.RemoveFirst();
            }
        }
    }
}