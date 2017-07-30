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
    /// 真形態
    /// </summary>
    bool trueFigure => reachTrueFigure && palamates.nowArmor < maxArmor / 2;

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
        if(isReaction) foreach(var hand in hands) hand.BeginMotion(this);
        else foreach(var hand in hands) hand.PauseMotion(this);
    }

    /// <summary>
    /// 本気モードフラグ
    /// </summary>
    protected override bool seriousMode => reachTrueFigure ?
        trueFigure ? palamates.nowArmor < maxArmor / 4 : palamates.nowArmor < maxArmor * 3 / 4 :
        base.seriousMode;

    List<HandControler> hands = new List<HandControler>();

    List<AllLange> allLanges => allWeapons.Take(2).Select(weapon => weapon.GetComponent<AllLange>()).ToList();
    Weapon grenade => allWeapons[2];
    Weapon laser => allWeapons[3];
    List<Weapon> guss => allWeapons.Skip(4).ToList();

    enum BodyMotionType
    {
        HAND_GRENADE_BURST,
        HAND_ASER_BURST,
        HUGE_GRENAFE,
        CRUISE,
        GRENADE_VOLLEY,
        GRENADE_BURST,
        HUGE_LASER_CHARGE,
        HUGE_LASER
    }
    enum HandMotionType
    {
        GRENADE,
        GRENADE_FIXED,
        GRENADE_BURST,
        LASER,
        LASER_FIXED,
        LASER_BURST,
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
        nextActionState = ActionPattern.MOVE;
        for(int time = 0; time < interval * 2; time++)
        {
            var direction = nearTarget.position - position;
            if(direction.magnitude > grappleDistance) Thrust(direction, targetSpeed: lowerSpeed);
            Aiming(nearTarget.position);
            SetBaseAimingAll();
            yield return Wait(1);
        }
        yield return StoppingAction();
        nextActionIndex = seriousMode ?
            (int)new[] {
                BodyMotionType.HAND_GRENADE_BURST,
                BodyMotionType.HAND_ASER_BURST,
                BodyMotionType.HUGE_GRENAFE,
                BodyMotionType.CRUISE,
                BodyMotionType.GRENADE_VOLLEY,
                BodyMotionType.GRENADE_BURST,
                BodyMotionType.HUGE_LASER_CHARGE,
                BodyMotionType.HUGE_LASER
            }.SelectRandom() :
            (int)new[] {
                BodyMotionType.HAND_GRENADE_BURST,
                BodyMotionType.HAND_ASER_BURST,
                BodyMotionType.HUGE_GRENAFE,
                BodyMotionType.CRUISE,
                BodyMotionType.GRENADE_VOLLEY,
                BodyMotionType.GRENADE_BURST,
                BodyMotionType.HUGE_LASER_CHARGE,
                BodyMotionType.HUGE_LASER
            }.SelectRandom();
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
        yield return AimingAction(nearTarget.position, interval, aimingProcess: () => Thrust(nowForward, reactPower, lowerSpeed));
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
        if(!inField) yield break;
        SetFixedAlignment(position + siteAlignment);
        foreach(var weapon in bodyWeapons)
        {
            if(weapon == null) continue;
            weapon.Action();
        }
        for(var time = 0; time < interval; time++)
        {
            ThrustStop();
            yield return Wait(1);
        }
        yield break;
    }
    /// <summary>
    /// 常に行われる攻撃行動
    /// </summary>
    private void AlwaysAttack()
    {
        foreach(var weapon in bodyWeapons)
        {
            if(weapon == null) continue;
            weapon.Action();
        }
    }
    protected override IEnumerator SinkingMotion()
    {
        foreach(var hand in hands) hand.EndMotion(this);
        foreach(var allLange in allLanges) allLange.myShip.DestroyMyself();
        yield return base.SinkingMotion();
        yield break;
    }
}
