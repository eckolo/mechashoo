using UnityEngine;
using System.Collections;

public class AllLange : Gun
{
    /// <summary>
    /// リアクター
    /// </summary>
    [SerializeField]
    private Reactor reactor = null;
    /// <summary>
    /// 照準移動速度
    /// </summary>
    [SerializeField]
    private float siteSpeed = 1;

    Ship myShip = null;
    Transform fixedParent = null;
    Vector2 fixedPosition = Vector2.zero;
    /// <summary>
    /// 外部操縦モード
    /// </summary>
    public bool remote { get; set; } = false;

    bool _isFixed = true;
    bool isFixed
    {
        get {
            return _isFixed;
        }
        set {
            nowParent = value ? fixedParent : sysPanel.transform;
            if(value) myShip.InvertWidth(true);
            myShip.nomalUpdate = !value;
            _isFixed = value;
        }
    }

    public override void Start()
    {
        base.Start();
        myShip = AttachShip();
        fixedParent = nowParent;
        fixedPosition = position;
    }

    public override void Update()
    {
        base.Update();
        if(isFixed)
        {
            var direction = fixedPosition - position;
            if(direction.Scaling(baseMas).magnitude > myShip.maximumSpeed)
            {
                myShip.Thrust(direction, targetSpeed: myShip.maximumSpeed);
                nowAngle = nowAngle.ToVector().Correct(defAngle.ToVector(), Mathf.Max(1 - siteSpeed, 0)).ToAngle();
            }
            else
            {
                position = fixedPosition;
                myShip.SetVerosity(myShip.nowSpeed, 0);
                nowAngle = defAngle;
            }
        }
        else
        {
            var nearTarget = myShip.nowNearSiteTarget;
            if(!remote && nearTarget != null) myShip.Aiming(nearTarget.position);
        }
    }

    protected Ship AttachShip()
    {
        foreach(var component in GetComponents<Ship>()) Destroy(component);
        var ship = gameObject.AddComponent<Ship>();

        ship.heightPositive = heightPositive;
        ship.ableEnter = false;
        ship.isSolid = false;
        ship.accessoryStates.Add(new Ship.AccessoryState
        {
            rootPosition = handlePosition,
            positionZ = 1,
            entity = reactor,
            baseAngle = 0
        });
        ship.palamates = new Ship.Palamates { baseSiteSpeed = siteSpeed };
        ship.defaultAlignment = Vector2.right;
        ship.nomalUpdate = false;
        ship.Start();

        return ship;
    }

    protected override IEnumerator BeginMotion(int actionNum)
    {
        if(nowAction == ActionType.SINK) isFixed = !isFixed;
        if(!remote && !isFixed)
        {
            var nearTarget = myShip.nowNearSiteTarget;
            if(nearTarget == null) yield break;

            var targetPosition = user.position + Random.Range(0f, 359f).ToVector(viewSize.x / 6);
            yield return myShip.HeadingDestination(targetPosition, myShip.maximumSpeed);
            yield return myShip.StoppingAction();
        }
        yield break;
    }

    protected override IEnumerator Motion(int actionNum)
    {
        if(!isFixed && myShip.nowNearSiteTarget == null) yield break;
        if(isFixed && position != fixedPosition) yield break;
        yield return base.Motion(actionNum);
        yield break;
    }
}
