using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    protected override void doAwake()
    {
        base.doAwake();
    }

    public override void startTurn()
    {
        base.startTurn();
        //endTurn();
    }

    protected override void doStart()
    {
        gridController.getGrid().findCenterTile(transform.position).setCharacter(this);
    }

    public override void selected()
    {
        
    }

}
