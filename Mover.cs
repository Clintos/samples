using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {

    public int pixels_per_move = 30;
    public float speed = 1.5f;
    private Vector3 target;
    private float step;
    private bool moving = false;
    private Character character;
    private Stack<Tile> moveStack = new Stack<Tile>();

    private GridController gridController;
    private bool abilityMove = false;

    public void setSpeed(float _speed)
    {
        speed = _speed;
    }

    public float getSpeed()
    {
        return speed;
    }

    public void setCharacter(Character _character)
    {
        character = _character;
    }

    public void moveUp()
    {
        character.setOrientation(Character.orientations.south);
        target = transform.position + new Vector3(0, -pixels_per_move * Converter.pixelsToWorld, 0);
        if(target != transform.position)
        {
            startMoving();
        }
    }

    public void moveDown()
    {
        character.setOrientation(Character.orientations.north);
        target = transform.position + new Vector3(0, pixels_per_move * Converter.pixelsToWorld, 0);
        if (target != transform.position)
        {
            startMoving();
        }
    }

    public void moveLeft()
    {
        character.setOrientation(Character.orientations.west);
        target = transform.position + new Vector3(-pixels_per_move * Converter.pixelsToWorld, 0, 0);
        if (target != transform.position)
        {
            startMoving();
        }
    }

    public void moveRight()
    {
        character.setOrientation(Character.orientations.east);
        target = transform.position + new Vector3(pixels_per_move * Converter.pixelsToWorld, 0, 0);
        if (target != transform.position)
        {
            startMoving();
        }
    }

    public void moveUpLevel()
    {
        Vector3 temp = character.transform.position;
        temp.z += gridController.getTileWorldWidth();
        character.transform.position = temp;
    }

    public void moveDownLevel()
    {
        Vector3 temp = character.transform.position;
        temp.z -= gridController.getTileWorldWidth();
        character.transform.position = temp;
    }

    void Awake()
    {
        gridController = GridController.Instance;
    }
    
	// Use this for initialization
	void Start () {
		
	}

    public void doStart()
    {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void doUpdate()
    {
        if (moving)
        {
            step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            if (target == transform.position)
            {
                stopMoving();
                if(moveStack.Count > 0)
                {
                    target = moveStack.Pop().getCenter();
                    doMove();
                }
                else
                {
                    if(abilityMove)
                    {
                        abilityMove = false;
                        character.endAbilityMove();
                    }
                }
            }
        }
    }

    public bool getMoving()
    {
        return moving;
    }

    public void setMoving(bool _moving)
    {
        moving = _moving;
        character.getAnimator().SetBool("moving", moving);
    }

    public void startMoving()
    {
        if (character.getMovesLeft() > 0 || abilityMove)
        {
            gridController.getGrid().findCenterTile(transform.position).setCharacter(null);
            setMoving(true);
        }
        else
        {
            target = transform.position;
        }
    }

    public void stopMoving()
    {
        setMoving(false);
        Tile temp = gridController.getGrid().findCenterTile(transform.position);
        temp.setCharacter(character);
        if (!abilityMove)
        {
            character.decrementMovesLeft();
            if (moveStack.Count <= 0)
            {
                character.setMovesLeft(0);
            }
        }
        tileEntered(temp);
    }

    public void tileEntered(Tile tile)
    {
        if(tile.getWorldObject() != null)
        {
            tile.getWorldObjectFloor().characterEntered();
        }
    }

    public void setTarget(Vector3 _target)
    {
        target = _target;
    }

    public void moveToTile(Tile tile)
    {
        moveStack = gridController.getMoveStack(character, tile);
        target = moveStack.Pop().getCenter();
        doMove();
    }

    public void abilityMoveToTile(Tile tile)
    {
        moveStack = gridController.getMoveStack(character, tile, 99);
        target = moveStack.Pop().getCenter();
        abilityMove = true;
        doMove();
    }

    public void setToTile(Tile tile)
    {
        if(tile.getMoveable())
        {
            character.transform.position = tile.getCenter();
            gridController.getGrid().findCenterTile(transform.position).setCharacter(character);
            tileEntered(tile);
        }
        else
        {
            if(tile.getCharacter() != null)
            {
                tile.getCharacter().pushToTile(gridController.getGrid().findCenterTile(tile.getCenter() + (tile.getCenter() - character.transform.position)));
                setToTile(tile);
            }
        }
    }

    public Character.orientations getOrientationAtTile(Tile tile)
    {
        moveStack = gridController.getMoveStack(character, tile);
        target = moveStack.Pop().getCenter();
        Character.orientations orient = character.getOrientation();
        doGhostMove(ref orient, character.transform.position);
        return orient;
    }

    private void doGhostMove(ref Character.orientations orientation, Vector3 position)
    {
        Vector3 direction = target - position;
        if (direction.x > 0)
        {
            orientation = Character.orientations.east;
        }
        else if (direction.x < 0)
        {
            orientation = Character.orientations.west;
        }
        else if (direction.y > 0)
        {
            orientation = Character.orientations.north;
        }
        else if (direction.y < 0)
        {
            orientation = Character.orientations.south;
        }
        position = target;
        if (moveStack.Count > 0)
        {
            target = moveStack.Pop().getCenter();
            doGhostMove(ref orientation, position);
        }
    }

    private void doMove()
    {
        Vector3 direction = target - transform.position;
        if(direction.x > 0)
        {
            moveRight();
        }
        else if(direction.x < 0)
        {
            moveLeft();
        }
        else if(direction.y > 0)
        {
            moveDown();
        }
        else if(direction.y < 0)
        {
            moveUp();
        }
    }
}
