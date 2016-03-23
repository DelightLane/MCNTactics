using UnityEngine;
using System.Collections;

// 본 게임 내부의 모든 오브젝트 컨트롤 클래스들의 최상위 부모 클래스
public abstract class TacticsObject : MonoBehaviour
{
    #region operator overloading
    // Decoable == 연산자 오버로딩 대응을 위한 연산자 오버로딩
    public static bool operator ==(TacticsObject lt, TacticsObject rt)
    {
        if (System.Object.ReferenceEquals(lt, rt))
        {
            return true;
        }

        if (((object)lt == null) || ((object)rt == null))
        {
            return false;
        }

        if (lt is MCN.Decoable && rt is MCN.Decoable)
        {
            return (lt as MCN.Decoable) == (rt as MCN.Decoable);
        }
        
        return (object)lt == (object)rt;
    }

    public static bool operator !=(TacticsObject lt, TacticsObject rt)
    {
        return !(rt == lt);
    }

    public override bool Equals(object o)
    {
        if (this is MCN.Decoable)
        {
            (this as MCN.Decoable).Equals(o);
        }

        return base.Equals(o);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion

    public virtual void Interactive(TacticsObject interactTarget) { }

    public virtual void OnTouchEvent(eTouchEvent touch) { }
}
