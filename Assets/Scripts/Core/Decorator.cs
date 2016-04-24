using System;

namespace MCN
{
    // 해당 T 인터페이스를 구현하는 target을 가지며
    // 이 클래스를 상속받는 클래스는 target을 꾸며주어야 한다.
    public class Decorator<T> 
    {
        protected T _decoTarget;

        public Decorator(T target) { _decoTarget = target; }
    }
}
