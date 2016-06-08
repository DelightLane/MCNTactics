using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// 유닛이 사용하는 모든 액터는 이 인터페이스를 상속받아야 한다.
public interface IUnitActor
{
    int ActPoint { get; }
}

