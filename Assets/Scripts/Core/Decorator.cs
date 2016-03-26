using UnityEngine;
using System;
using System.Collections.Generic;

namespace MCN
{
    public abstract class Decoable : TacticsObject
    {
        #region operator overloading
        // 항상 DecoInstance끼리 비교하게 하기 위해 == 연산자 오버로딩
        public static bool operator ==(Decoable lt, Decoable rt)
        {
            if (System.Object.ReferenceEquals(lt, rt))
            {
                return true;
            }

            if (((object)lt == null) || ((object)rt == null))
            {
                return false;
            }
            
            if(lt is DecoInstance && rt is Decorator)
            {
                return (lt as DecoInstance) == (rt as Decorator);
            }

            if (rt is DecoInstance && lt is Decorator)
            {
                return (rt as DecoInstance) == (lt as Decorator);
            }

            return (object)lt == (object)rt;
        }

        public static bool operator !=(Decoable lt, Decoable rt)
        {
            return !(rt == lt);
        }

        public override bool Equals(object o)
        {
            if (this is DecoInstance)
            {
                (this as DecoInstance).Equals(o);
            }

            if(this is Decorator)
            {
                (this as Decorator).Equals(o);
            }

            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }

    public class DecoInstance : Decoable
    {
        private Decoable _decoTarget;

        public Decoable Get
        {
            get
            {
                if(_decoTarget == null)
                {
                    _decoTarget = this;
                }
                return _decoTarget;
            }
        }

        #region operator overloading
        public static bool operator ==(DecoInstance lt, Decorator rt)
        {
            if (System.Object.ReferenceEquals(lt, rt))
            {
                return true;
            }

            if (((object)lt == null) || ((object)rt == null))
            {
                return false;
            }

            return rt == lt;
        }

        public static bool operator !=(DecoInstance lt, Decorator rt)
        {
            return !(rt == lt);
        }

        public override bool Equals(object o)
        {
            if (o is Decorator)
            {
                this.Equals(o as Decorator);
            }

            return base.Equals(o);
        }

        public bool Equals(Decorator o)
        {
            if (o == null)
            {
                return false;
            }

            return o == this;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        public void Decorated(Decorator deco)
        {
            _decoTarget = deco;
        }
    }

    public abstract class Decorator : Decoable
    {
        // 각 데코레이터 별로 필요한 가중치를 설정한다.
        // ex) MoveDecorator에서 weight는 이동 범위이다.
        [SerializeField]
        private Dictionary<string, int> _weight;

        private Decoable _decoTarget;

        protected abstract string[] AbsoluteWeightKey();

        // 최상위를 리턴시킨다.
        protected MCN.Decoable DecoTarget
        {
            get
            {
                if(_decoTarget is Decorator)
                {
                    return (_decoTarget as Decorator).DecoTarget;
                }

                return _decoTarget;
            }
        }

        void Start()
        {
            foreach(var key in AbsoluteWeightKey())
            {
                if(_weight == null || !_weight.ContainsKey(key))
                {
                    throw new Exception(string.Format("{0} is not available. because weight '{1}' key is not initialized.", this.GetType().Name, key));
                }
            }
        }

        public int GetWeight(string key)
        {
            if(_weight.ContainsKey(key))
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
            if(info != null)
            {
                if(_weight == null)
                {
                    _weight = new Dictionary<string, int>();
                }

                _weight[info.key] = info.value;
            }
        }

        public void Decoration(Decoable target)
        {
            this._decoTarget = target;

            var root = DecoTarget as DecoInstance;

            if(root != null)
            {
                root.Decorated(this);
            }
        }

        #region operator overloading
        public static bool operator ==(Decorator lt, DecoInstance rt)
        {
            if (System.Object.ReferenceEquals(lt, rt))
            {
                return true;
            }

            if (((object)lt == null) || ((object)rt == null))
            {
                return false;
            }

            return lt.DecoTarget == rt;
        }

        public static bool operator !=(Decorator lt, DecoInstance rt)
        {
            return !(rt == lt);
        }

        public override bool Equals(object o)
        {
            if (o is DecoInstance)
            {
                this.Equals(o as DecoInstance);
            }

            return base.Equals(o);
        }

        public bool Equals(DecoInstance o)
        {
            if (o == null)
            {
                return false;
            }

            return this == o;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        sealed public override void OnTouchEvent(eTouchEvent touch)
        {
            base.OnTouchEvent(touch);

            _decoTarget.OnTouchEvent(touch);

            DecoOnTouchEvent(touch);
        }

        sealed public override void Interactive(TacticsObject interactTarget)
        {
            base.Interactive(interactTarget);

            _decoTarget.Interactive(interactTarget);

            DecoInteractive(interactTarget);
        }

        protected virtual void DecoInteractive(TacticsObject interactTarget) { }

        protected virtual void DecoOnTouchEvent(eTouchEvent touch) { }
    }
}
