namespace DefaultRotations.Ranged;

[BetaRotation]
[SourceCode(Path = "main/DefaultRotations/Ranged/MCH_Mille.cs")]
[LinkDescription("https://media.discordapp.net/attachments/964474941716193290/1129104164039966730/mch_rotation.png")]
[RotationDesc(ActionID.ChainSaw, ActionID.Wildfire)]
public sealed class MCH_Default : MCH_Base
{
    public override string GameVersion => "6.45";

    public override string RotationName => "Mille Delayed Tools";

    private readonly String[] TinctureList = new string[3] { "Grade 6 Tincture of Dexterity", "Grade 7 Tincture of Dexterity", "Grade 8 Tincture of Dexterity" };

    private bool IsOpener = false;
    private int OpenerCount = 0;

    protected override IAction CountDownAction(float remainTime)
    {
        //if (remainTime < CountDownAhead)
        //{
        //    if (AirAnchor.CanUse(out var act1)) return act1;
        //    else if (!AirAnchor.EnoughLevel && HotShot.CanUse(out act1)) return act1;
        //}
        //if (remainTime < 2 && UseBurstMedicine(out var act)) return act;
        //if (remainTime < 5 && Reassemble.CanUse(out act, CanUseOption.EmptyOrSkipCombo | CanUseOption.IgnoreClippingCheck)) return act;
        if (remainTime <= TinctureOfDexterity8.AnimationLockTime && Configs.GetBool("MCH_Pot") == true)
        {
            switch(Configs.GetCombo("MCH_Pot"))
            {
                case 0: return TinctureOfDexterity6;
                case 1: return TinctureOfDexterity7;
                case 2: return TinctureOfDexterity8;
            }
        }
        if (remainTime > 0 && remainTime < 30) 
        {
            IsOpener = true;
        }
        return base.CountDownAction(remainTime);
    }
    
    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("MCH_Tincture", true, "Use Tincture")
            .SetCombo("MCH_Pot", 2, "Tincture Grade", TinctureList);
    }
    
    private bool Opener (out IAction act)
    {
        if (TimeSinceLastAction.TotalSeconds > 3)
        {
            IsOpener = false;
        }
        {
            switch (OpenerCount)
            {
                case 0: return OpenerStep(IsLastGCD(false, SplitShot), SplitShot.CanUse(out act, CanUseOption.MustUse));
                case 1: return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                case 2: return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));
                case 3: return OpenerStep(IsLastGCD(true, Drill), Drill.CanUse(out act, CanUseOption.MustUse));
                case 4: return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUse));
                case 5: return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));
                case 6: return OpenerStep(IsLastGCD(false, SlugShot), SlugShot.CanUse(out act, CanUseOption.MustUse));
                case 7: return OpenerStep(IsLastAbility(false, BarrelStabilizer), BarrelStabilizer.CanUse(out act, CanUseOption.MustUseEmpty));
                case 8: return OpenerStep(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));
                case 9: return OpenerStep(IsLastGCD(true, AirAnchor), AirAnchor.CanUse(out act, CanUseOption.MustUse));
                case 10: return OpenerStep(IsLastAbility(false, Ricochet), Ricochet.CanUse(out act, CanUseOption.MustUseEmpty));
                case 11: return OpenerStep(IsLastAbility(false, GaussRound), GaussRound.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility));
                case 12: return OpenerStep(IsLastGCD(true, CleanShot), CleanShot.CanUse(out act, CanUseOption.MustUse));
                case 13: return OpenerStep(IsLastAbility(false, Reassemble), Reassemble.CanUse(out act, CanUseOption.MustUseEmpty | CanUseOption.OnLastAbility)); 
                case 14: return OpenerStep(IsLastAbility(false, Wildfire), Wildfire.CanUse(out act, CanUseOption.MustUse | CanUseOption.OnLastAbility));
                case 15: return OpenerStep(IsLastGCD(true, ChainSaw), ChainSaw.CanUse(out act, CanUseOption.MustUse));
                case 16: return OpenerStep(IsLastAbility(true, RookAutoturret), RookAutoturret.CanUse(out act, CanUseOption.MustUse));
                case 17: return OpenerStep(IsLastAbility(false, Hypercharge), Hypercharge.CanUse(out act, CanUseOption.IgnoreClippingCheck | CanUseOption.OnLastAbility | CanUseOption.MustUseEmpty));
                case 18:
                    IsOpener = false;
                    break;
            }
        }

        act = null;
        return false;
    }

    private bool OpenerStep(bool condition, bool result)
    {
        if (condition)
        {
            OpenerCount++;
        }
        else
        {
            return result;
        }
        return false;
    }

    protected override bool GeneralGCD(out IAction act)
    {
        while (IsOpener) { return Opener(out act); }



        //Overheated
        if (AutoCrossbow.CanUse(out act)) return true;
        if (HeatBlast.CanUse(out act)) return true;

        //Long Cds
        if (BioBlaster.CanUse(out act)) return true;
        if (!SpreadShot.CanUse(out _))
        {
            if (AirAnchor.CanUse(out act)) return true;
            else if (!AirAnchor.EnoughLevel && HotShot.CanUse(out act)) return true;

            if (Drill.CanUse(out act)) return true;
        }

        if (!CombatElapsedLessGCD(4) && ChainSaw.CanUse(out act, CanUseOption.MustUse)) return true;

        //Aoe
        if (ChainSaw.CanUse(out act)) return true;
        if (SpreadShot.CanUse(out act)) return true;

        //Single
        if (CleanShot.CanUse(out act)) return true;
        if (SlugShot.CanUse(out act)) return true;
        if (SplitShot.CanUse(out act)) return true;

        return false;
    }

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (Configs.GetBool("MCH_Reassemble") && ChainSaw.EnoughLevel && nextGCD.IsTheSameTo(true, ChainSaw))
        {
            if (Reassemble.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        }
        if (Ricochet.CanUse(out act, CanUseOption.MustUse)) return true;
        if (GaussRound.CanUse(out act, CanUseOption.MustUse)) return true;

        if (!Drill.EnoughLevel && nextGCD.IsTheSameTo(true, CleanShot)
            || nextGCD.IsTheSameTo(false, AirAnchor, ChainSaw, Drill))
        {
            if (Reassemble.CanUse(out act, CanUseOption.EmptyOrSkipCombo)) return true;
        }
        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool AttackAbility(out IAction act)
    {
        if (InBurst)
        {
            if (UseBurstMedicine(out act)) return true;
            if ((IsLastAbility(false, Hypercharge) || Heat >= 50) && !CombatElapsedLess(10)
                && Wildfire.CanUse(out act, CanUseOption.OnLastAbility)) return true;
        }

        if (!CombatElapsedLess(12) && CanUseHypercharge(out act)) return true;
        if (CanUseRookAutoturret(out act)) return true;

        if (BarrelStabilizer.CanUse(out act)) return true;

        if (CombatElapsedLess(8)) return false;

        var option = CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo;
        if (GaussRound.CurrentCharges <= Ricochet.CurrentCharges)
        {
            if (Ricochet.CanUse(out act, option)) return true;
        }
        if (GaussRound.CanUse(out act, option)) return true;

        return base.AttackAbility(out act);
    }

    //private static bool AirAnchorBlockTime(float time)
    //{
    //    if (AirAnchor.EnoughLevel)
    //    {
    //        return AirAnchor.IsCoolingDown && AirAnchor.WillHaveOneCharge(time);
    //    }
    //    else
    //    {
    //        return HotShot.IsCoolingDown && HotShot.WillHaveOneCharge(time);
    //    }
    //}

    private static bool CanUseRookAutoturret(out IAction act)
    {
        act = null;
        if (AirAnchor.EnoughLevel)
        {
            if (!AirAnchor.IsCoolingDown || AirAnchor.ElapsedAfter(18)) return false;
        }
        else
        {
            if (!HotShot.IsCoolingDown || HotShot.ElapsedAfter(18)) return false;
        }

        return RookAutoturret.CanUse(out act);
    }

    const float REST_TIME = 6f;
    private static bool CanUseHypercharge(out IAction act)
    {
        act = null;

        //if (BarrelStabilizer.IsCoolingDown && BarrelStabilizer.WillHaveOneChargeGCD(8))
        //{
        //    if (AirAnchorBlockTime(8)) return false;
        //}
        //else
        //{
        //    if (AirAnchorBlockTime(12)) return false;
        //}

        //Check recast.
        if (!SpreadShot.CanUse(out _))
        {
            if (AirAnchor.EnoughLevel)
            {
                if (AirAnchor.WillHaveOneCharge(REST_TIME)) return false;
            }
            else
            {
                if (HotShot.EnoughLevel && HotShot.WillHaveOneCharge(REST_TIME)) return false;
            }
        }
        if (Drill.EnoughLevel && Drill.WillHaveOneCharge(REST_TIME)) return false;
        if (ChainSaw.EnoughLevel && ChainSaw.WillHaveOneCharge(REST_TIME)) return false;

        return Hypercharge.CanUse(out act);
    }
}
