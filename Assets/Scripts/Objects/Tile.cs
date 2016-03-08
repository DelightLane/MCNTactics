using UnityEngine;
using System.Collections;

public class Tile : TacticsObject {
    private TacticsObject _attached;

    public TacticsObject GetAttachObject()
    {
        return _attached;
    }

    public void AttachObject(TacticsObject obj)
    {
        _attached = obj;
    }
}
