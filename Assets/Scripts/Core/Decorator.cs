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
        private Decoable _decoTarget;

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

        public override void Interactive(TacticsObject interactTarget)
        {
            _decoTarget.Interactive(interactTarget);

            DecoInteractive(interactTarget);
        }

        protected abstract void DecoInteractive(TacticsObject interactTarget);
    }
}
