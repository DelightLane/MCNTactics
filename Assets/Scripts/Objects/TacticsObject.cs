using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 본 게임 내부의 모든 오브젝트 컨트롤 클래스들의 최상위 부모 클래스
public abstract class TacticsObject : MonoBehaviour
{
    public virtual void Interactive(TacticsObject interactTarget) { }

    public virtual bool OnTouchEvent(eTouchEvent touch) { return true; }
}
