using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class Character : MonoBehaviour
{
    public string charName;
    public ClassController.classes classState;
    protected Class charClass;
    protected List<Status> statuses = new List<Status>();
    protected List<Status> removeStatuses = new List<Status>();
    protected List<ItemController.items> items = new List<ItemController.items>();
    private Mover mover;
    protected List<Weapon> weapons = new List<Weapon>();
    public enum orientations { north, south, east, west };
    public orientations orientation = orientations.north;
    protected int movesLeft;
    protected int abilitiesLeft;
    protected int itemsLeft;
    protected int actionsLeft;
    protected bool turnOver = true;
    public enum states { move, ability, none };
    public states state = states.none;
    public AIController.AIBehaviours AIBehaviour;
    protected AI ai = null;
    protected Attributes attributes = new Attributes();
    protected LevelSystem level = new LevelSystem();
    protected Animator animator;
    protected SpriteRenderer rend;

    protected GridController gridController;
    protected LevelController levelController;
    protected Cursor cursor;

    private bool doingTurn = false, damageShake = false;
    private List<Character> abilityTargets = new List<Character>();
    private List<Character> abilityHitTargets = new List<Character>();
    private GameObject damageNum;
    private Vector3 damageDirection;
    private int damageShakeCounter = 0, hitCount = 0;
    private AI originalAI;

    public GameObject shadow;
    public Vector3 shadowOffset;

    private bool protectBool = false;
    private int protectCounter = 0;
    private Vector3 protectPrevPos;

    private ActiveAbility usingAbility;
    private string moveAnimation;
    private float prevSpeed;

    public void doAbilityMove(string _moveAnimation, List<Tile> moveTargets, float speed)
    {
        moveAnimation = _moveAnimation;
        if(moveAnimation != "")
        {
            animator.SetBool(moveAnimation, true);
        }
        prevSpeed = mover.getSpeed();
        mover.setSpeed(speed);
        mover.abilityMoveToTile(moveTargets[0]);
    }

    public void endAbilityMove()
    {
        if(moveAnimation != "")
        {
            animator.SetBool(moveAnimation, false);
        }
        mover.setSpeed(prevSpeed);
        usingAbility.endMoveAbility();
    }

    public void setUsingAbility(ActiveAbility _usingAbility)
    {
        usingAbility = _usingAbility;
    }

    public void endUsingAbility()
    {
        usingAbility.endBeginAbility();
    }

    public void setOriginalAI()
    {
        ai = originalAI;
    }

    public List<ItemController.items> getItems()
    {
        return items;
    }

    public void addItem(ItemController.items item)
    {
        items.Add(item);
    }

    public void removeItem(ItemController.items item)
    {
        items.Remove(item);
    }

    public void useItem(ItemController.items item)
    {
        items.Remove(item);
        ItemController.Instance.createItem(item).GetComponent<Item>().use();
    }

    public string getName()
    {
        return charName;
    }

    public Tile getTileInFront()
    {
        Vector3 temp = transform.position;
        switch (orientation)
        {
            case orientations.north:
                temp.y += gridController.getTileWorldWidth();
                break;
            case orientations.south:
                temp.y -= gridController.getTileWorldWidth();
                break;
            case orientations.east:
                temp.x += gridController.getTileWorldWidth();
                break;
            case orientations.west:
                temp.x -= gridController.getTileWorldWidth();
                break;
        }
        return gridController.getGrid().findCenterTile(temp);
    }

    public LevelSystem getLevelSystem()
    {
        return level;
    }

    public void setDoingTurn(bool _doingTurn)
    {
        doingTurn = _doingTurn;
    }

    public bool getDoingTurn()
    {
        return doingTurn;
    }


    public Mover getMover()
    {
        return mover;
    }

    public states getState()
    {
        return state;
    }

    public void animationFrameChange()
    {
        foreach (Weapon weapon in weapons)
        {
            weapon.setOffsets();
        }
    }

    public void setAI(AI _ai)
    {
        ai = _ai;
        ai.setCharacter(this);
    }

    public AI getAI()
    {
        return ai;
    }

    public void removeAI()
    {
        ai = null;
    }

    public Status findStatus(StatusController.statuses status)
    {
        Status temp = StatusController.Instance.getStatus(status);
        foreach (var stat in statuses)
        {
            if (stat.getName() == temp.getName())
            {
                return stat;
            }
        }
        return null;
    }

    public List<Status> getStatuses()
    {
        return statuses;
    }

    public void addWeapon(Weapon weapon)
    {
        weapons.Add(weapon);
    }

    public void removeWeapon(Weapon weapon)
    {
        weapons.Remove(weapon);
    }

    public void useAbility(Ability ability)
    {

    }

    public void useAbility(Ability ability, List<Character> targets)
    {
        abilityHitTargets.Clear();
        abilityTargets.Clear();
        if (targets.Count > 0)
        {
            foreach (var target in targets)
            {
                abilityTargets.Add(target);
            }
        }
        else
        {
            gainXP(targets);
        }
    }

    public void gainXP(List<Character> targets)
    {
        float targetLevel = 0f;
        foreach (Character target in targets)
        {
            if (target.getLevelSystem().getLevel() > targetLevel)
            {
                targetLevel = target.getLevelSystem().getLevel();
            }
        }
        if (targetLevel == 0)
        {
            targetLevel = getLevelSystem().getLevel();
        }
        level.gainXP(targetLevel);
    }

    public int getAbilitiesLeft()
    {
        return abilitiesLeft;
    }

    public void setAbilitiesLeft(int _abilitiesLeft)
    {
        abilitiesLeft = _abilitiesLeft;
    }

    public int getActionsLeft()
    {
        return actionsLeft;
    }

    public void setActionsLeft(int _actionsLeft)
    {
        actionsLeft = _actionsLeft;
    }

    public int getItemsLeft()
    {
        return itemsLeft;
    }

    public void setItemsLeft(int _itemsLeft)
    {
        itemsLeft = _itemsLeft;
    }

    public void incrementAbilitiesLeft()
    {
        abilitiesLeft++;
        incrementActionsLeft();
    }

    public void decrementAbilitiesLeft()
    {
        abilitiesLeft--;
        decrementActionsLeft();
    }

    public void incrementItemsLeft()
    {
        itemsLeft++;
        incrementActionsLeft();
    }

    public void decrementItemsLeft()
    {
        itemsLeft--;
        decrementActionsLeft();
    }

    public void incrementActionsLeft()
    {
        actionsLeft++;
    }

    public void decrementActionsLeft()
    {
        actionsLeft--;
    }

    public void moveUpLevel()
    {
        mover.moveUpLevel();
    }

    public void moveDownLevel()
    {
        mover.moveDownLevel();
    }

    public void pushToTile(Tile tile)
    {
        mover.setToTile(tile);
    }

    public void addStatus(Status status)
    {
        if (!attributes.getResistanceChance() || status.getType() == Status.types.buff)
        {
            statuses.Add(status);
            status.setCharacter(this);
            status.doStart();
        }
    }

    public void addStatus(Status status, Character attacker)
    {
        if (!attributes.getResistanceChance() || status.getType() == Status.types.buff)
        {
            status.setCharacter(this);
            statuses.Add(status);
            status.doStart();
            if (attacker != null)
            {
                attacker.addAbilityHitTargets(this);
                if (status.GetType() == typeof(StatusTaunt))
                {
                    ((StatusTaunt)status).setTauntTarget(attacker);
                }
                else if (status.GetType() == typeof(StatusProtected))
                {
                    ((StatusProtected)status).setProtector(attacker);
                }
            }
        }
        else
        {
            if (attacker != null)
            {
                attacker.removeAbilityTargets(this);
            }
        }
    }

    public void removeStatus(StatusController.statuses status)
    {
        Status temp = StatusController.Instance.getStatus(status);
        Status res = statuses.SingleOrDefault(s => s.name == temp.name);
        if (res != null)
        {
            statuses.Remove(res);
        }
    }

    public void removeStatus(Status status)
    {
        statuses.Remove(status);
    }

    public void addRemoveStatus(Status status)
    {
        removeStatuses.Add(status);
    }

    public List<Character> getAbilityTargets()
    {
        return abilityTargets;
    }

    public List<Character> getAbilityHitTargets()
    {
        return abilityHitTargets;
    }

    public void removeAbilityTargets(Character target)
    {
        if (target != null)
            abilityTargets.Remove(target);
        if (abilityTargets.Count == 0)
        {
            if (abilityHitTargets.Count > 0)
            {
                gainXP(abilityHitTargets);
                abilityHitTargets.Clear();
            }
        }
    }

    public void addAbilityHitTargets(Character target)
    {
        if (abilityTargets.Count > 0)
        {
            if (target != null)
            {
                abilityHitTargets.Add(target);
            }
            removeAbilityTargets(target);
        }
    }

    public void setDamageShake(bool _damageShake)
    {
        damageShake = _damageShake;
    }

    public void doDamageShake(Vector3 _damageDirection)
    {
        setDamageShake(true);
        damageDirection = _damageDirection;
        damageShakeCounter = 0;
    }

    public Attributes getSpecialAttributes(ActiveDamageMoveTarget.DamageBehaviours damageBehaviour)
    {
        Attributes result = attributes.DeepCopy();
        foreach (StatusSpecialAtt status in statuses.OfType<StatusSpecialAtt>())
        {
            if (status.getDamageBehaviour() == damageBehaviour)
            {
                result.addAttributes(status.getAttributes());
            }
        }
        return result;
    }

    public Attributes getSpecialAttributes(ActiveDamageMoveTarget.DamageBehaviours damageBehaviour, Attributes _attributes)
    {
        Attributes result = _attributes.DeepCopy();
        foreach (StatusSpecialAtt status in statuses.OfType<StatusSpecialAtt>())
        {
            if (status.getDamageBehaviour() == damageBehaviour)
            {
                result.addAttributes(status.getAttributes());
            }
        }
        return result;
    }

    public void doProtect(Vector3 pos)
    {
        if (!protectBool)
        {
            protectBool = true;
            protectCounter = 0;
            protectPrevPos = transform.position;
        }
        else
        {
            protectCounter = 0;
        }
        transform.position = pos;
    }

    public void takeDamage(int damage, ActiveDamageTarget.DamageBehaviours damageBehaviour, Character attacker)
    {
        if (!charClass.getBlock(damageBehaviour))
        {
            if(attacker != null)
            {
                if (attacker.getAbilityHitTargets().SingleOrDefault(x => x.GetHashCode() == this.GetHashCode()) == null)
                {
                    attacker.addAbilityHitTargets(this);
                }
                Vector3 heading = attacker.transform.position - transform.position;
                float distance = heading.magnitude;
                doDamageShake(heading / distance);
            }
            else
            {
                doDamageShake(new Vector3(0,1,0));
            }
            attributes.getHP().decrease(damage);
            if (damageNum == null)
            {
                damageNum = UIController.Instance.createNeonWord(transform.position, damage.ToString(), Word.types.flashing, Word.colors.red);
            }
            else
            {
                damageNum.GetComponent<Word>().redrawText((damage + Int32.Parse(damageNum.GetComponent<Word>().getText())).ToString(), Word.types.flashing, Word.colors.red);
            }
            if (damage > 0)
                SurfaceController.Instance.createSurface(SurfaceController.surfaces.blood, gridController.getGrid().findVectorTile(transform.position).getWorldObjectFloor());
            if (attributes.getHP().getCurValue() == 0)
            {
                die();
            }
        }
        else
        {
            if (attacker != null)
            {
                if (attacker.getAbilityTargets().Count > 0)
                {
                    attacker.removeAbilityTargets(this);
                }
            }
        }
    }

    public void takeDamage(int damage, ActiveDamageTarget.DamageBehaviours damageBehaviour, Attributes _attributes, Character attacker)
    {
        if (_attributes != null)
        {
            Attributes _realAtt = attacker.getSpecialAttributes(damageBehaviour, _attributes);
            Attributes realAtt = getSpecialAttributes(damageBehaviour);
            StatusProtected protect = findStatus(StatusController.statuses.protect) as StatusProtected;
            if(protect != null)
            {
                realAtt = protect.getProtector().getSpecialAttributes(damageBehaviour);
            }
            if (_realAtt.getAccuracyChance(realAtt))
            {
                if (!charClass.getBlock(damageBehaviour))
                {
                    float crit = 1f;
                    if(_realAtt.getCriticalChance())
                    {
                        crit = 1.5f;
                    }
                    int finalDamage = (int)((float)damage * _realAtt.getAttackMultiplier(realAtt) * _realAtt.getLuckMultiplier() * crit);
                    if(protect == null)
                        takeDamage(finalDamage, damageBehaviour, attacker);
                    else
                    {
                        protect.getProtector().doProtect(transform.position + (Converter.getPixelWorldSizeVector(gridController.getGrid().getDirectiontoTile
                            (gridController.getGrid().findVectorTile(transform.position), gridController.getGrid().findCenterTile(attacker.transform.position)))*15));
                        protect.getProtector().takeDamage(finalDamage, damageBehaviour, attacker);
                    }
                }
            }
        }
        else
        {
            takeDamage(damage, damageBehaviour, attacker);
        }
    }

    public void takeHealing(int healing, Character healer)
    {
        attributes.getHP().increase(healing);
        if(healer != null)
        {
            if(healer.getAbilityTargets().Count > 0)
            {
                healer.addAbilityHitTargets(this);
            }
        }
    }

    public void reduceAP(int AP)
    {
        attributes.getAP().decrease(AP);
    }

    public void regenAP()
    {
        attributes.getAP().increase(1);
    }

    public virtual void die()
    {
        LevelController.Instance.removeCharacter(this);
        Destroy(gameObject);
    }

    void Awake()
    {
        doAwake();
    }

    public Animator getAnimator()
    {
        return animator;
    }

    protected virtual void doAwake()
    {
        gridController = GridController.Instance;
        levelController = LevelController.Instance;
        cursor = Cursor.Instance;
        mover = gameObject.AddComponent<Mover>();
        mover.setCharacter(this);
        level.setCharacter(this);
        levelController.addCharacter(this);
        animator = gameObject.GetComponent<Animator>();
        rend = gameObject.GetComponent<SpriteRenderer>();
        if(AIBehaviour != AIController.AIBehaviours.none)
            ai = AIController.Instance.getAI(AIBehaviour, this);
        setOrientation(orientation);
        originalAI = ai;
        shadow = Instantiate(shadow, transform.position, transform.rotation);
        shadow.transform.parent = transform;
        shadow.transform.position += Converter.getPixelWorldSizeVector(shadowOffset);
    }

    public SpriteRenderer getSpriteRenderer()
    {
        return rend;
    }

	// Use this for initialization
	void Start () {
        charClass = ClassController.Instance.getClass(classState);
        charClass.setCharacter(this);
        attributes = charClass.getAttributes().DeepCopy();
        attributes.addAttributes(level.getCurAtt());
        doStart();
	}

    protected virtual void doStart()
    {
        //startTurn();
        gridController.getGrid().findCenterTile(transform.position).setCharacter(this);
        //addStatus(StatusController.Instance.getStatus(StatusController.statuses.burned));

    }
	
	// Update is called once per frame
	void Update () {
        doUpdate();
        mover.doUpdate();
	}

    protected virtual void doUpdate()
    {
        if(protectBool)
        {
            protectCounter++;
            if(protectCounter == 30)
            {
                protectBool = false;
                protectCounter = 0;
                transform.position = protectPrevPos;
            }
        }
        if(damageShake)
        {
            if(damageShakeCounter == 0)
            {
                transform.position -= damageDirection * Converter.getPixelWorldSize(3);
                hitCount++;
            }
            if(damageShakeCounter == 10)
            {
                transform.position += hitCount * damageDirection * Converter.getPixelWorldSize(3);
                damageShake = false;
                hitCount = 0;
            }
            damageShakeCounter++;
        }
        if(!getTurnOver() && actionsLeft <= 0 && movesLeft <= 0 && state == states.none)
        {
            endTurn();
        }
        if (ai == null)
        {
            if (state == states.move)
            {
                checkCursorTile();
            }
            else if (state == states.ability)
            {
                checkCursorTile();
            }
        }
        else
        {
            ai.doUpdate();
        }
    }

    protected void checkCursorTile()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Vector3 mousePos = cursor.transform.position;
            mousePos.z = transform.position.z;
            Tile mouseTile = gridController.findClosestTile(mousePos);
            if (mouseTile != null)
            {
                if (state == states.move)
                {
                    gridController.setMoveTiles(this);
                    if (gridController.checkMoveTiles(mouseTile))
                    {
                        setDoingTurn(true);
                        mover.moveToTile(mouseTile);
                        gridController.setStateNone();
                        setStateNone();
                    }
                }
                else if(state == states.ability)
                {
                    setDoingTurn(true);
                    gridController.setAbilityTile(mouseTile);
                }
            }

        }
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            setDoingTurn(false);
            if(state == states.move)
            {
                gridController.setStateNone();
                setStateNone();
                UIController.Instance.createActionMenu(this);
            }
            else if(state == states.ability)
            {
                gridController.setStateNone();
                setStateNone();
                gridController.getAbility().toggleGrid();
                incrementAbilitiesLeft();
                UIController.Instance.createAbilityMenu(this);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            setDoingTurn(false);
            if (state == states.move)
            {
                gridController.setStateNone();
                setStateNone();
            }
            else if (state == states.ability)
            {
                gridController.setStateNone();
                setStateNone();
                gridController.getAbility().toggleGrid();
                incrementAbilitiesLeft();
            }
        }
    }

    public void doLevelUp()
    {
        attributes.addAttributes(level.getLevelUpAtt());
    }

    public void setState(states _state)
    {
        switch(_state)
        {
            case states.none:
                setStateNone();
                break;
            case states.move:
                setStateMove();
                break;
            case states.ability:
                setStateAbility();
                break;
        }
    }

    public void setStateNone()
    {
        state = states.none;
    }

    public void setStateMove()
    {
        state = states.move;
    }

    public void setStateAbility()
    {
        state = states.ability;
    }

    public void rotateLeft()
    {
        switch(orientation)
        {
            case orientations.north:
                setOrientation(orientations.west);
                break;
            case orientations.south:
                setOrientation(orientations.east);
                break;
            case orientations.east:
                setOrientation(orientations.north);
                break;
            case orientations.west:
                setOrientation(orientations.south);
                break;
        }
    }

    public void rotateRight()
    {
        switch (orientation)
        {
            case orientations.north:
                setOrientation(orientations.east);
                break;
            case orientations.south:
                setOrientation(orientations.west);
                break;
            case orientations.east:
                setOrientation(orientations.south);
                break;
            case orientations.west:
                setOrientation(orientations.north);
                break;
        }
    }

    public void setOrientation(orientations _orientation)
    {
        orientation = _orientation;
        animator.SetInteger("orientation", (int)orientation);
    }

    public orientations getOrientation()
    {
        return orientation;
    }

    public Attributes getAttributes()
    {
        return attributes;
    }

    public void resetActionsLeft()
    {
        actionsLeft = 1;
        itemsLeft = 1;
        abilitiesLeft = 1;
    }

    public virtual void startTurn()
    {
        turnOver = false;
        resetMovesLeft();
        resetActionsLeft();
        regenAP();
        if(ai != null)
        {
            ai.startTurn();
        }
        foreach(Status status in statuses)
        {
            status.doUpdate();
        }
        foreach(Status status in removeStatuses)
        {
            removeStatus(status);
        }
        removeStatuses.Clear();
    }

    public void endTurn()
    {
        if (weapons.Count == 0)
        {
            turnOver = true;
            setDoingTurn(false);
            if (levelController.getIsPlayerTurn())
                levelController.checkPlayerTurnOver();
            else
                levelController.checkEnemyTurnOver();
        }
    }

    public bool getTurnOver()
    {
        return turnOver;
    }

    public void setTurnOver(bool _turnOver)
    {
        turnOver = _turnOver;
    }

    public void resetMovesLeft()
    {
        movesLeft = getAttributes().getSpeed().getCurMaxValue();
    }

    public void setMovesLeft(int i)
    {
        movesLeft = i;
    }

    public int getMovesLeft()
    {
        return movesLeft;
    }

    public Class getClass()
    {
        return charClass;
    }

    public void decrementMovesLeft()
    {
        movesLeft--;
    }

    public virtual void selected()
    {
        if (!getTurnOver())
        {
            UIController.Instance.createActionMenu(this);
        }
    }
}
