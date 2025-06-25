using System.Globalization;
using Content.Shared._SCP.Scps.PlagueDoctor.Components;
using Content.Shared._SCP.Scps.PlaugeDoctor.Components;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Timing;
using ZombieRevivableComponent = Content.Shared._SCP.Scps.PlagueDoctor.Components.ZombieRevivableComponent;


namespace Content.Shared._SCP.SkinContact;


public sealed class SharedPlagueDoctorSystem : EntitySystem
{
    //Move this to a component
    private static readonly TimeSpan EXPIRE_TIME = TimeSpan.FromMinutes(3);

    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<ZombieRevivableComponent, ExaminedEvent>(ExamineDead);
    }

    private void ExamineDead(EntityUid user, ZombieRevivableComponent comp, ExaminedEvent args)
    {
        if(!TryComp<PlagueDoctorComponent>(args.Examiner,out var doctorComponent)) return;

        using (args.PushGroup("plaugedoctor-revivable",1000))
        {
            var elapsed = _timing.CurTime - comp.DeathTime;
            var diff = MathHelper.Max((doctorComponent.ReviveTime - elapsed), TimeSpan.Zero);

            var timingText = "";

            if (diff == TimeSpan.Zero)
            {
                timingText = Loc.GetString("scp-plaugedoctor-revive-failure");
            }
            else
            {
                timingText = Loc.GetString("scp-plaugedoctor-revive-timer",
                    ("time", diff.ToString(@"mm\:ss", CultureInfo.CurrentCulture)));
            }


            args.PushMarkup($"{Loc.GetString("scp-plaugedoctor-revive-statement")}");
            args.PushMarkup(timingText);

        }
    }
}

