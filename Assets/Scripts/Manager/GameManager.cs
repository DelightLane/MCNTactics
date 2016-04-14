using UnityEngine;
using System.Collections;

public class GameManager : MCN.Singletone<GameManager>
{
    private TacticsObject _selectedObj;

    public TacticsObject SelectedObj
    {
        get
        {
            return _selectedObj;
        }

        set
        {
            _selectedObj = value;
        }
    }

    private GameManager()
    {
        // TODO : 나은 위치에서 로드하게 수정
        // 리플랙션을 사용해서 데이터들을 로드하게 하는 건 어떨지?
        DataManager.Instance.LoadData(new UnitDataFactory());
    }
}
