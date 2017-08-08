using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public partial class Sejiziuequje : Boss
{
    /// <summary>
    /// 真形態に到達するフラグ
    /// </summary>
    [SerializeField]
    private bool reachTrueFigure = false;
    /// <summary>
    /// 真形態時専用BGM
    /// </summary>
    [SerializeField]
    public AudioClip trueFigureBgm = null;
    /// <summary>
    /// 真形態
    /// </summary>
    bool trueFigure => reachTrueFigure && allowedTrueFiruge && onAttack && palamates.nowArmor < figureBorderArmor;
    /// <summary>
    /// 形態変化のボーダーライン装甲値
    /// </summary>
    float figureBorderArmor => maxArmor * 2 / 3;
    /// <summary>
    /// 真形態移行許可
    /// </summary>
    bool allowedTrueFiruge = false;
    /// <summary>
    /// チャージ状態
    /// </summary>
    bool onCharging = false;
    /// <summary>
    /// 付属パーツの目標地点
    /// </summary>
    Vector2 armorPositionTarget
    {
        get {
            if(!trueFigure) return Vector2.zero;
            return _armorPositionTarget;
        }
        set {
            _armorPositionTarget = value;
        }
    }
    Vector2 _armorPositionTarget = Vector2.zero;
    /// <summary>
    /// 付属パーツの目標角度
    /// </summary>
    float armorAngleTarget
    {
        get {
            if(!trueFigure) return 90;
            return _armorAngleTarget;
        }
        set {
            _armorAngleTarget = value;
        }
    }
    float _armorAngleTarget = 90;

    /// <summary>
    /// 最大装甲値
    /// </summary>
    public override float maxArmor => base.maxArmor * (reachTrueFigure ? 1 : 0.5f);

    bool endMotion => isDestroied || nowNearTarget == null;

    uint attackCount = 0;

    public override void Start()
    {
        base.Start();
        hands = allLanges.Select(weapon => new HandControler(weapon)).ToList();
        foreach(var hand in hands) hand.BeginMotion(this);
    }
    public override void Update()
    {
        base.Update();
        if(!trueFigure) foreach(var weaponBase in weaponBases) weaponBase.nowColor = nowColor;
        else foreach(var weaponBase in weaponBases) weaponBase.nowColor = new Color(1, 1, 1);
        if(alreadyOnceReaction && !isReaction) foreach(var hand in hands) hand.EndMotion(this);
    }
    protected override void UpdateMotion()
    {
        if(!endMotion && onCharging && !laser.finishCharge && laser.canAction) laser.Action(Weapon.ActionType.NOMAL);
        UpdateArmorState(armorPositionTarget, armorAngleTarget);
        base.UpdateMotion();
    }

    /// <summary>
    /// 本気モードフラグ
    /// </summary>
    protected override bool seriousMode => reachTrueFigure ?
        trueFigure ?
        palamates.nowArmor < figureBorderArmor / 2 :
        palamates.nowArmor < (maxArmor + figureBorderArmor) / 2 :
        base.seriousMode;

    List<HandControler> hands = new List<HandControler>();

    List<AllLange> allLanges => allWeapons.Take(2).Select(weapon => weapon.GetComponent<AllLange>()).ToList();
    Weapon grenade => allWeapons[2];
    ChargingWeapon laser => allWeapons[3].GetComponent<ChargingWeapon>();
    List<Weapon> guss => allWeapons.Skip(4).ToList();

    Vector2 armorPosition
    {
        get {
            if(!weaponBases.Any()) return Vector2.zero;
            return weaponBases.First().parentConnection.Rescaling(spriteSize);
        }
        set {
            if(!weaponBases.Any()) return;
            weaponBases.First().parentConnection = value.Scaling(spriteSize);
            weaponBases.Last().parentConnection = new Vector2(value.x, -value.y).Scaling(spriteSize);
            foreach(var weaponBase in weaponBases) weaponBase.AdjustPosition();
        }
    }
    float armorAngle
    {
        get {
            if(!weaponBases.Any()) return 0;
            return weaponBases.First().nowAngle;
        }
        set {
            if(!weaponBases.Any()) return;
            weaponBases.First().nowAngle = value.Compile();
            weaponBases.Last().nowAngle = -value.Compile();
        }
    }
    void SetArmorStateTarget(Vector2? setPosition = null, float? setAngle = null)
    {
        armorPositionTarget = setPosition ?? armorPositionTarget;
        armorAngleTarget = setAngle ?? armorAngleTarget;
        return;
    }
    KeyValuePair<Vector2, float> UpdateArmorState(Vector2 setPosition, float setAngle)
    {
        if(isDestroied) return new KeyValuePair<Vector2, float>(armorPosition, armorAngle);
        if(setPosition != armorPosition)
        {
            var direction = setPosition - armorPosition;
            var speed = direction.ToVector(siteSpeed).Rescaling(baseMas);
            armorPosition = direction.magnitude > speed.magnitude ? armorPosition + speed : setPosition;
        }
        if(setAngle != armorAngle)
        {
            var direction = setAngle.ToVector() - armorAngle.ToVector();
            var speed = direction.ToVector(siteSpeed).Rescaling(baseMas);
            armorAngle = direction.magnitude > speed.magnitude ? (armorAngle.ToVector() + speed).ToAngle() : setAngle;
        }
        return new KeyValuePair<Vector2, float>(armorPosition, armorAngle);
    }

    enum BodyMotionType
    {
        GRENADE,
        MISSILE,
        MISSILE_CHARGE,
        CRUISE,
        GRENADE_VOLLEY,
        GRENADE_BURST,
        LASER_CHARGE,
        LASER
    }
    enum HandMotionType
    {
        GRENADE,
        GRENADE_FIXED,
        LASER,
        LASER_SPIN,
        LASER_CLOSEUP
    }

    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns>コルーチン</returns>
    protected override IEnumerator MotionMove(int actionNum)
    {
        if(!allowedTrueFiruge)
        {
            allowedTrueFiruge = true;
            allowedTrueFiruge = trueFigure;
            if(trueFigure) yield return ProduceTransformation();
        }

        nextActionState = ActionPattern.AIMING;
        var reasonableSpeed = (maximumSpeed + lowerSpeed) / 2;
        AlwaysAttack();

        SetArmorStateTarget(new Vector2(0.2f, 0.5f), 60);
        for(int time = 0; time < interval * 2; time++)
        {
            if(endMotion) yield break;
            if(trueFigure) AlwaysAttack(interval: 24);
            var direction = nearTarget.position - position;
            if(direction.magnitude > grappleDistance) Thrust(direction, targetSpeed: reasonableSpeed);
            Aiming(nearTarget.position);
            yield return Wait(1);
        }
        AlwaysAttack();
        yield return StoppingAction();
        AlwaysAttack();

        nextActionIndex = trueFigure ?
            (int)new[] {
                BodyMotionType.GRENADE,
                BodyMotionType.MISSILE,
                BodyMotionType.MISSILE_CHARGE,
                BodyMotionType.CRUISE,
                BodyMotionType.GRENADE_VOLLEY,
                BodyMotionType.GRENADE_BURST,
                BodyMotionType.LASER_CHARGE,
                BodyMotionType.LASER
            }.SelectRandom(seriousMode ?
            new[] { 3, 1, 6, 1, 3, 3, onCharging ? 0 : 12, laser.finishCharge ? 72 : 0 } :
            new[] { 6, 1, 3, 1, 1, 1, 0, 0 }) :
            onTheWay ?
            (int)new[] {
                BodyMotionType.MISSILE,
                BodyMotionType.GRENADE,
                BodyMotionType.MISSILE_CHARGE,
                BodyMotionType.CRUISE
            }[attackCount % 4] :
            (int)new[] {
                BodyMotionType.GRENADE,
                BodyMotionType.MISSILE,
                BodyMotionType.MISSILE_CHARGE,
                BodyMotionType.CRUISE
            }.SelectRandom(seriousMode ? new[] { 6, 1, 3, 1 } : new[] { 1, 3, 1, 1 });
        yield break;
    }
    /// <summary>
    /// 照準操作行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns>コルーチン</returns>
    protected override IEnumerator MotionAiming(int actionNum)
    {
        nextActionState = ActionPattern.ATTACK;
        var motion = actionNum.Normalize<BodyMotionType>();
        AlwaysAttack();

        switch(motion)
        {
            case BodyMotionType.GRENADE:
                {
                    SetArmorStateTarget(new Vector2(0.3f, 0.2f), 90);
                    var targetPosition = nearTarget.position;
                    for(int time = 0; time < interval * 2; time++)
                    {
                        if(endMotion) yield break;
                        if(trueFigure) AlwaysAttack(interval: 6);
                        var direction = nearTarget.position - position;
                        if(direction.magnitude > gunDistance) Thrust(direction, targetSpeed: maximumSpeed);
                        else ThrustStop();
                        Aiming(targetPosition);
                        yield return Wait(1);
                    }
                    AlwaysAttack();
                    yield return StoppingAction();
                }
                break;
            case BodyMotionType.MISSILE:
                {
                    SetArmorStateTarget(new Vector2(0.3f, 0.5f), 45);
                    for(int time = 0; time < interval; time++)
                    {
                        if(endMotion) yield break;
                        if(trueFigure) AlwaysAttack(interval: 6);
                        var direction = nearTarget.position - position;
                        if(direction.magnitude > gunDistance) Thrust(direction, targetSpeed: maximumSpeed);
                        else ThrustStop();
                        Aiming(nearTarget.position + nearTarget.nowSpeed);
                        yield return Wait(1);
                    }
                    AlwaysAttack();
                    yield return StoppingAction();
                }
                break;
            case BodyMotionType.MISSILE_CHARGE:
                {
                    SetArmorStateTarget(new Vector2(0.3f, 0.5f), 45);
                    for(int time = 0; time < interval; time++)
                    {
                        if(endMotion) yield break;
                        if(trueFigure) AlwaysAttack(interval: 3);
                        var direction = nearTarget.position - position;
                        if(direction.magnitude > grappleDistance) Thrust(direction, targetSpeed: lowerSpeed);
                        else ThrustStop();
                        Aiming(nearTarget.position);
                        yield return Wait(1);
                    }
                    AlwaysAttack();
                    yield return StoppingAction();
                }
                break;
            case BodyMotionType.CRUISE:
                {
                    SetArmorStateTarget(new Vector2(0.1f, 0.5f), 75);
                    var destination = new Vector2(nearTarget.position.x, viewPosition.y * 2 - nearTarget.position.y);
                    yield return HeadingDestination(destination, maximumSpeed, grappleDistance, () => {
                        if(trueFigure) AlwaysAttack(interval: 12);
                    });
                    AlwaysAttack();
                    yield return StoppingAction();
                }
                break;
            case BodyMotionType.GRENADE_VOLLEY:
                {
                    SetArmorStateTarget(new Vector2(0.4f, 0.4f), 30);
                    for(int time = 0; time < interval; time++)
                    {
                        if(endMotion) yield break;
                        if(trueFigure) AlwaysAttack(interval: 24);
                        var direction = nearTarget.position - position;
                        var distance = (grappleDistance + gunDistance) / 2;
                        if(direction.magnitude > distance) Thrust(direction, targetSpeed: maximumSpeed);
                        else ThrustStop();
                        Aiming(nearTarget.position);
                        yield return Wait(1);
                    }
                    AlwaysAttack();
                    yield return StoppingAction();
                }
                break;
            case BodyMotionType.GRENADE_BURST:
                {
                    SetArmorStateTarget(new Vector2(0f, 1f), 0);
                    for(int time = 0; time < interval; time++)
                    {
                        if(endMotion) yield break;
                        if(trueFigure) AlwaysAttack(interval: 24);
                        var direction = nearTarget.position - position;
                        var distance = (grappleDistance + gunDistance) / 2;
                        if(direction.magnitude > distance) Thrust(direction, targetSpeed: maximumSpeed);
                        else ThrustStop();
                        Aiming(nearTarget.position);
                        yield return Wait(1);
                    }
                    AlwaysAttack();
                    yield return StoppingAction();
                }
                break;
            case BodyMotionType.LASER_CHARGE:
                AlwaysAttack();
                break;
            case BodyMotionType.LASER:
                {
                    SetArmorStateTarget(new Vector2(0f, 1f), 90);
                    for(int time = 0; time < interval; time++)
                    {
                        if(endMotion) yield break;
                        if(trueFigure) AlwaysAttack(interval: 48);
                        var direction = nearTarget.position - position;
                        if(direction.magnitude > gunDistance) Thrust(direction, targetSpeed: lowerSpeed);
                        else ThrustStop();
                        Aiming(nearTarget.position);
                        yield return Wait(1);
                    }
                    AlwaysAttack();
                    yield return StoppingAction();
                }
                break;
            default:
                break;
        }

        AlwaysAttack();
        yield break;
    }
    /// <summary>
    /// 攻撃行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns>コルーチン</returns>
    protected override IEnumerator MotionAttack(int actionNum)
    {
        nextActionState = ActionPattern.MOVE;
        var motion = actionNum.Normalize<BodyMotionType>();
        var finishMotion = true;
        AlwaysAttack();

        switch(motion)
        {
            case BodyMotionType.GRENADE:
                {
                    const int fireNum = 2;
                    for(int fire = 0; fire < fireNum; fire++)
                    {
                        yield return Wait(() => grenade.canAction);
                        var longRange = (position - nearTarget.position).magnitude > gunDistance;
                        SetFixedAlignment(position + siteAlignment);
                        grenade.Action(longRange ? Weapon.ActionType.NOMAL : Weapon.ActionType.NPC);

                        if(fire < fireNum)
                        {
                            AlwaysAttack();
                            yield return Wait(() => grenade.onAttack);
                            AlwaysAttack();
                            var targetPosition = nearTarget.position;
                            for(int time = 0; !grenade.canAction; time++)
                            {
                                if(endMotion) yield break;
                                if(trueFigure) AlwaysAttack(interval: 12);
                                Aiming(targetPosition);
                                yield return Wait(1);
                            }
                        }

                        AlwaysAttack();
                    }
                    yield return Wait(() => grenade.canAction);
                }
                break;
            case BodyMotionType.MISSILE:
                {
                    yield return Wait(() => grenade.canAction);
                    AlwaysAttack();
                    grenade.Action(Weapon.ActionType.SINK);
                    for(int time = 0; !grenade.onAttack; time++)
                    {
                        if(endMotion) yield break;
                        if(trueFigure) AlwaysAttack(interval: 12);
                        Aiming(nearTarget.position + nearTarget.nowSpeed);
                        yield return Wait(1);
                    }
                    AlwaysAttack();
                    yield return Wait(() => grenade.canAction);
                }
                break;
            case BodyMotionType.MISSILE_CHARGE:
                {
                    for(int time = 0; time < (trueFigure ? interval * 3 : interval); time++)
                    {
                        if(endMotion) yield break;
                        Thrust(position - nearTarget.position, targetSpeed: lowerSpeed);
                        Aiming(nearTarget.position);
                        AlwaysAttack(Weapon.ActionType.SINK);
                        yield return Wait(1);
                    }
                    yield return StoppingAction();
                    AlwaysAttack(Weapon.ActionType.SINK);
                    yield return Wait(() => grenade.canAction);
                    AlwaysAttack(Weapon.ActionType.SINK);
                    grenade.Action(Weapon.ActionType.SINK);
                    SetArmorStateTarget(new Vector2(0.3f, 0.5f), 45);
                    for(int time = 0; !grenade.onAttack; time++)
                    {
                        if(endMotion) yield break;
                        if(trueFigure) AlwaysAttack(interval: 12);
                        Aiming(nearTarget.position);
                        yield return Wait(1);
                    }
                    AlwaysAttack();
                    yield return Wait(() => grenade.canAction);
                }
                break;
            case BodyMotionType.CRUISE:
                {
                    SetArmorStateTarget(new Vector2(0f, 0.5f), 90);
                    for(int time = 0; time < interval / 2; time++)
                    {
                        yield return Wait(1);
                        AlwaysAttack();
                    }
                    var direction = new[] { 90f, -90f }.SelectRandom();
                    var timelimit = Random.Range(3, shipLevel) * 5 * interval * (trueFigure.ToInt() + 1);
                    for(int time = 0; time < timelimit; time++)
                    {
                        if(endMotion) yield break;
                        var directionTweak = (Vector2)(direction.ToRotation() * (position - nearTarget.position));
                        var destination = (position + directionTweak).Within(fieldLowerLeft, fieldUpperRight);
                        Thrust(destination - position, reactPower, maximumSpeed);
                        Aiming(destination);
                        AlwaysAttack(Weapon.ActionType.SINK);
                        yield return Wait(1);
                    }
                    yield return StoppingAction();
                }
                break;
            case BodyMotionType.GRENADE_VOLLEY:
                {
                    yield return Wait(() => guss.All(weapon => weapon.canAction));
                    var timelimit = Random.Range(2, shipLevel) * 4 * interval * (trueFigure.ToInt() + 1);
                    for(int time = 0; time < timelimit; time++)
                    {
                        if(endMotion) yield break;
                        Thrust(nearTarget.position - position, targetSpeed: lowerSpeed);
                        Aiming(nearTarget.position, siteSpeedTweak: 0.3f);
                        AlwaysAttack(Weapon.ActionType.SINK);
                        yield return Wait(1);
                    }
                }
                break;
            case BodyMotionType.GRENADE_BURST:
                {
                    for(int time = 0; time < interval / 2; time++)
                    {
                        yield return Wait(1);
                        AlwaysAttack();
                    }
                    var timelimit = Random.Range(2, shipLevel) * 4 * interval * (trueFigure.ToInt() + 1);
                    for(int time = 0; time < timelimit; time++)
                    {
                        if(endMotion) yield break;
                        var direction = nearTarget.position - position;
                        if(direction.magnitude > gunDistance) Thrust(direction, targetSpeed: lowerSpeed);
                        else ThrustStop();
                        Aiming(nearTarget.position, siteSpeedTweak: 0.3f);
                        AlwaysAttack(Weapon.ActionType.SINK);
                        yield return Wait(1);
                    }
                }
                break;
            case BodyMotionType.LASER_CHARGE:
                onCharging = true;
                break;
            case BodyMotionType.LASER:
                {
                    SetArmorStateTarget(new Vector2(0f, 1f), 90);
                    for(int time = 0; time < interval; time++)
                    {
                        if(endMotion) yield break;
                        Thrust(-siteAlignment, targetSpeed: lowerSpeed);
                        yield return Wait(1);
                    }
                    yield return StoppingAction();
                    yield return Wait(() => laser.canAction);
                    laser.Action(Weapon.ActionType.SINK);
                    yield return Wait(() => laser.onAttack);
                    while(laser.onAttack)
                    {
                        ThrustStop();
                        Aiming(nearTarget.position, siteSpeedTweak: 0.3f);
                        yield return Wait(1);
                    }
                    yield return Wait(interval);
                }
                break;
            default:
                break;
        }

        AlwaysAttack();
        SetArmorStateTarget(new Vector2(0.2f, 0.5f), 60);
        for(int time = 0; finishMotion && time < interval; time++)
        {
            if(trueFigure) AlwaysAttack(interval: 24);
            Aiming(nearTarget.position);
            ThrustStop();
            yield return Wait(1);
        }
        if(onTheWay && ++attackCount >= 4)
        {
            foreach(var hand in hands) hand.EndMotion(this);
            if(allLanges.All(allLange => allLange.isFixed)) nextActionState = ActionPattern.ESCAPE;
        }
        yield break;
    }

    /// <summary>
    /// 常に行われる攻撃行動
    /// </summary>
    private void AlwaysAttack(Weapon.ActionType? setAction = null, int interval = 1)
    {
        foreach(var weapon in guss)
        {
            if(weapon == null) continue;
            if(!alwaysAttackCount.ContainsKey(weapon)) alwaysAttackCount.Add(weapon, 0);
            if(++alwaysAttackCount[weapon] < interval) continue;
            alwaysAttackCount[weapon] = 0;

            weapon.Action(setAction ?? new[] {
                Weapon.ActionType.NOMOTION,
                Weapon.ActionType.NOMAL,
                Weapon.ActionType.SINK
            }.SelectRandom(seriousMode ? new[] { 12, 6, 1 } : new[] { 72, 12, 1 }));
            if(!gusAlignment.ContainsKey(weapon)) gusAlignment.Add(weapon, null);
            if(gusAlignment[weapon] == null && weapon.nextAction == Weapon.ActionType.SINK)
            {
                var parent = weapon.nowConnectParent;
                var targetPosition = parent.position
                    + (Vector2)(parent.nowAngle.ToRotation() * weapon.position)
                    + (parent.nowAngle + weapon.nowAngle).ToVector(gunDistance);
                gusAlignment[weapon] = SetFixedAlignment(targetPosition, true);
            }
        }
    }
    Dictionary<Weapon, int> alwaysAttackCount = new Dictionary<Weapon, int>();
    Dictionary<Weapon, Effect> gusAlignment = new Dictionary<Weapon, Effect>();
    protected override IEnumerator SinkingMotion()
    {
        foreach(var hand in hands) hand.EndMotion(this);
        if(reachTrueFigure)
        {
            foreach(var allrange in allLanges) allrange.DestroyMyself();
            var phaselimit = 27;
            var baseInterval = 36;
            for(var phase = 0; phase < phaselimit; phase++)
            {
                var setPosition = new Vector2(Random.Range(-spriteSize.x / 2, spriteSize.x / 2), Random.Range(-spriteSize.y / 2, spriteSize.y / 2));
                var sizeTweak = Easing.quadratic.Out(phase, phaselimit) * 2;
                OutbreakExplosion(sizeTweak, setPosition, new[] { 1, 2 }.SelectRandom());
                var explodInterval = Mathf.FloorToInt(Easing.quadratic.SubIn(baseInterval, phase, phaselimit));
                for(int time = 0; time < explodInterval; time++)
                {
                    ExertPower(nowSpeed, reactPower + nowSpeed.magnitude, 0);
                    yield return Wait(1);
                }
            }
            OutbreakExplosion(4, index: 3);
            OutbreakExplosion(4, index: 4);
            yield return Wait(180);
            yield break;
        }
        else
        {
            Escape();
            ableEnter = false;
            for(var time = 0; Mathf.Abs(position.x) < fieldUpperRight.x * 2 && Mathf.Abs(position.y) < fieldUpperRight.y * 2; time++)
            {
                if(time % interval == 0)
                {
                    var setPosition = new Vector2(Random.Range(-spriteSize.x / 2, spriteSize.x / 2), Random.Range(-spriteSize.y / 2, spriteSize.y / 2));
                    OutbreakExplosion(1, setPosition, 1);
                }
                Thrust(normalCourse = normalCourse + position - nearTarget.position, targetSpeed: maximumSpeed);
                Aiming(position + normalCourse);
                yield return Wait(1);
            }
        }
        foreach(var hand in hands) hand.DestroyMyself();
        yield break;
    }

    /// <summary>
    /// 真形態への移行演出
    /// </summary>
    /// <returns>コルーチン</returns>
    public IEnumerator ProduceTransformation()
    {
        MainSystems.SetBGM();
        SetArmorStateTarget(new Vector2(0.2f, 0.5f), 60);
        foreach(var hand in hands) hand.PauseMotion(this);
        ableEnter = false;
        StartCoroutine(HeadingDestination(Vector2.zero, maximumSpeed * 2));
        StartCoroutine(AimingAction(position + Vector2.right * grappleDistance * nearTarget.position.x.ToSign()));
        yield return sys.nowStage.ProduceCaution(480);
        ableEnter = true;
        foreach(var hand in hands) hand.BeginMotion(this);
        MainSystems.SetBGM(trueFigureBgm);
        yield break;
    }

    /// <summary>
    /// 射撃適正距離
    /// </summary>
    protected override float gunDistance => viewSize.x / 2;

    public override void DestroyMyself(bool system)
    {
        foreach(var hand in hands) hand.DestroyMyself();
        base.DestroyMyself(system);
    }
}
