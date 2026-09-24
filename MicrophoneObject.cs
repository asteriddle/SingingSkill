using System.Collections.Generic;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;


namespace Sims3.Gameplay.Objects.SDM.SingingSkill
{
    public class Microphone : BandInstrument
    {
        


        public override GameObject.EnvironmentMotive EnvironmentTuning
        {
            get
            {
                return Microphone.kEnvironmentTuning;
            }
        }

        
        public override LookAtTuning LookAtTuning
        {
            get
            {
                return Drums.kLookAtTuning;
            }
        }

        public new static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString("Gameplay/Objects/HobbiesSkills/Singing:" + name, parameters);
        }

        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(Microphone.Play.Singleton);
            base.AddInventoryInteraction(Microphone.Play.Singleton);
            base.AddInteraction(Microphone.PlayForTips.Singleton);
            base.AddInventoryInteraction(Microphone.PlayForTips.Singleton);
            base.AddInteraction(Microphone.Perform.Singleton);
            base.AddInventoryInteraction(Microphone.Perform.Singleton);
            base.AddInteraction(Microphone.Learn.Singleton);
            base.AddInventoryInteraction(Microphone.Learn.Singleton);
            base.AddInteraction(BandInstrument.InvokeSerenade.Singleton);
            base.AddInventoryInteraction(BandInstrument.InvokeSerenade.Singleton);
            if (GameUtils.IsInstalled(ProductVersion.EP3))
            {
                base.AddInteraction(Microphone.Jam.Singleton);
                base.AddInteraction(Microphone.PerformGig.Singleton);
                base.AddInventoryInteraction(Microphone.Jam.Singleton);
            }
        }

        public override bool HandToolAllowPlacementInSlot(IGameObject objectToPlaceInSlot, Slot slot, AdditionalSlotPlacementCheckResults checks)
        {
            return slot != Microphone.kTipJarSlot && base.HandToolAllowPlacementInSlot(objectToPlaceInSlot, slot, checks);
        }

        public InteractionInstance CreateLearnInstance(IActor a, InteractionPriority priority, SheetMusic music)
        {
            Microphone.Learn learn = Microphone.Learn.Singleton.CreateInstance(this, a, priority, false, true) as Microphone.Learn;
            learn.MusicToLearn = music;
            return learn;
        }

        
        public override string GetLoopForSkillLevelPrefix
        {
            get
            {
                // change this to microphone laters
                return "drums";
            }
        }

        public override Origin GetBuffOrigin()
        {
            return Origin.FromDrums;
        }


        public override SkillNames SkillName
        {
            get
            {
                return (SkillNames)0x688F2BC6;
            }
        }


        public override string SkillParameterName
        {
            get
            {
                return "SingingSkill";
            }
        }


        public override EventTypeId PlayedEventName
        {
            get
            {
                return EventTypeId.kPlayedDrums;
            }
        }


        public override bool UseCallOverAnimation
        {
            get
            {
                return true;
            }
        }

        public override string GetPlayName()
        {
            return "Sing";
        }


        public override string PlayForTipsSubwayList
        {
            get
            {
                // set this to microphone laters
                return "subway_playfortips_plist_drums";
            }
        }

        // Token: 0x060045A4 RID: 17828 RVA: 0x0010C2B0 File Offset: 0x0010B2B0
        public override InteractionDefinition JamWithBandInstrumentDefinition(RockBand.GenreTypes genre)
        {
            return new Microphone.Jam.Definition(genre, false);
        }

        // Token: 0x060045A5 RID: 17829 RVA: 0x0010C2B9 File Offset: 0x0010B2B9
        public override InteractionDefinition PerformGigDefintion(RockBand.GenreTypes genre)
        {
            return new Microphone.PerformGig.PushDefinition(genre);
        }

        // Token: 0x060045A6 RID: 17830 RVA: 0x0010C2C4 File Offset: 0x0010B2C4
        public override bool ReadyToPlay(Sim actor)
        {
            if (!actor.Inventory.Contains(this))
            {
                this.mInstrumentPos = this.Position;
                this.mInstrumentFwd = base.ForwardVector;
                this.StartedOnPlatform = World.IsOnPlatform(this.Position);
                if (!actor.RouteToObjectRadiusAndCheckInUse(this, 1.5f))
                {
                    return false;
                }
                this.AddToUseList(actor);
                if (!this.StartedOnPlatform && !this.InstrumentPrefersToPlayWherePlaced())
                {
                    actor.PlaySoloAnimation("a2o_object_genericSwipe_x", true);
                    base.FadeOut(true, false, 0.5f);
                    if (!actor.Inventory.TryToAdd(this))
                    {
                        this.RemoveFromUseList(actor);
                        return false;
                    }
                    return true;
                }
            }
            else
            {
                this.AddToUseList(actor);
                this.mInstrumentPos = actor.Position + actor.ForwardVector.Normalize() * 0.4f;
                this.mInstrumentFwd = actor.ForwardVector;
                this.StartedOnPlatform = false;
            }
            return true;
        }

        public override bool RouteBeforePlaying(Sim sim, out BandInstrument.Jig jig)
        {
            Vector3 mInstrumentPos = this.mInstrumentPos;
            Vector3 mInstrumentFwd = this.mInstrumentFwd;
            Vector3 zero = Vector3.Zero;
            jig = (GlobalFunctions.CreateObjectOutOfWorld(base.GetResourceKey(), "Sims3.Gameplay.Objects.HobbiesSkills.BandInstrument+Jig", null) as BandInstrument.Jig);
            jig.SetOpacity(0f, 0f);
            jig.SetPosition(mInstrumentPos);
            jig.SetForward(mInstrumentFwd);
            jig.AddToWorld();
            bool flag;
            bool flag2;
            if (!base.MoveJigToGoodSpot(sim, jig, ref mInstrumentPos, ref mInstrumentFwd, ref zero, out flag, out flag2))
            {
                return false;
            }
            jig.PlaceJig(mInstrumentPos, zero);
            sim.RemoveExitReason(ExitReason.ObjectStateChanged);
            bool flag3 = sim.RouteToObjectRadius(jig, 1.5f);
            if (flag3 && flag2)
            {
                if (sim.Inventory.Contains(this) && sim.Inventory.TryToRemove(this))
                {
                    this.AddToWorld();
                    this.SetOpacity(0f, 0f);
                    base.SetPosition(jig.Position);
                    base.SetForward(jig.ForwardVector);
                    this.SetOpacityTransition(0f, 1f, 1f);
                }
                else
                {
                    flag3 = false;
                }
            }
            jig.Destroy();
            Simulator.Sleep(0U);
            int num;
            if (flag3 && sim.RouteToSlotListAndCheckInUse(this, Microphone.kRoutingSlots, out num))
            {
                this.mIsMirrored = (num == 1);
                flag3 = true;
            }
            else
            {
                flag3 = false;
            }
            return flag3;
        }

        public override void DonePlaying(Sim actor)
        {
            if (!actor.Inventory.Contains(this) && (this.WasPlayedFromInventory || (!base.LotCurrent.IsResidentialLot && this.WhoLastPlacedMe == actor)))
            {
                ExitReason exitReason = actor.ExitReason;
                actor.ClearExitReasons();
                actor.RouteTurnToFace(this.Position);
                actor.AddExitReason(exitReason);
                actor.PlaySoloAnimation("a2o_object_genericSwipe_x", true);
                base.FadeOut(false, false, 0.5f);
                if (!actor.TryAddObjectToInventory(this))
                {
                    base.FadeIn();
                }
            }
            base.DonePlaying(actor);
        }

        public override bool InstrumentPrefersToPlayWherePlaced()
        {
            return true;
        }

        public override IGameObject PlaceTipsContainer(BandInstrument.Jig jig)
        {
            Vector3 positionOfSlot = base.GetPositionOfSlot(Microphone.kTipJarSlot);
            Vector3 forwardOfSlot = base.GetForwardOfSlot(Microphone.kTipJarSlot);
            return GlobalFunctions.CreateObject("TipJar", positionOfSlot, 1, forwardOfSlot);
        }

        public override void SetupStateMachine(InteractionInstance inst)
        {
            inst.AcquireStateMachine("Drums");
            inst.SetActor("x", inst.InstanceActor);
            inst.SetParameter("AnimationName", "a2a_soc_neutral_singFriendly_friendly_neutral_x");
            inst.SetActor("drumKit", this);
            inst.EnterState("x", "Enter");
            inst.SetParameter("isMirrored", this.mIsMirrored);
        }

        public override InteractionDefinition GetDanceInteraction()
        {
            return Microphone.Dance.Singleton;
        }

        public override InteractionDefinition GetProxyInteraction()
        {
            return Microphone.MicrophoneInteractionProxy.Singleton;
        }

        public override InteractionDefinition GetWatchInteraction()
        {
            return Microphone.Watch.Singleton;
        }

        public override void PushDanceSoloInteraction(Sim actor, IDanceable instrument, InteractionPriority priority)
        {
            Microphone.Dance dance = Microphone.Dance.Singleton.CreateInstance(instrument as Microphone, actor, priority, false, true) as Microphone.Dance;
            if (dance != null)
            {
                actor.InteractionQueue.Add(dance);
            }
        }

        public new const string sLocalizationKey = "Gameplay/Objects/HobbiesSkills/Microphone";

        public const string kPlayingForTipsSubwayList = "subway_playfortips_plist_drums";

        [Tunable]
        public new static GameObject.EnvironmentMotive kEnvironmentTuning = new GameObject.EnvironmentMotive();

        [TunableComment("Added to tune look-at threshold.  Also used to tune interestingness.")]
        [Tunable]
        public static LookAtTuning kLookAtTuning = LookAtTuning.DefaultHighThreshold();

        public SitData mSitPart = new SitData(PartArea.Middle, Slot.RoutingSlot_2, (Slot)2820733094U, "_02", SitStyle.Dining);

        public static Slot kTipJarSlot = (Slot)2820733095U;

        public static Slot[] kRoutingSlots = new Slot[]
        {
            Slot.RoutingSlot_0,
            Slot.RoutingSlot_1
        };

        public bool mIsMirrored;

        public class Play : BandInstrument.PlayBandInstrument<Microphone>
        {
            public override void StartPlaying()
            {
                base.StartPlaying();
                this.Actor.LookAtManager.DisableLookAts();
            }

            public override void StopPlaying()
            {
                this.Actor.LookAtManager.EnableLookAts();
                base.StopPlaying();
            }

            public static InteractionDefinition Singleton = new Microphone.Play.Definition();

            public class Definition : MusicalInstrument.PlayInstrument<Microphone>.PlayDefinition<Microphone.Play>
            {
            }
        }

        // Token: 0x02000836 RID: 2102
        public class PlayForTips : BandInstrument.PlayForTips<Microphone>
        {
            public override void StartPlaying()
            {
                base.StartPlaying();
                this.Actor.LookAtManager.DisableLookAts();
            }

            public override void StopPlaying()
            {
                this.Actor.LookAtManager.EnableLookAts();
                base.StopPlaying();
            }

            public static InteractionDefinition Singleton = new Microphone.PlayForTips.Definition();

            public class Definition : BandInstrument.PlayForTips<Microphone>.TipsDefinition<Microphone.PlayForTips>
            {
            }
        }

        public class Perform : BandInstrument.Perform<Microphone>
        {
            public override Sims3.Gameplay.Skills.Guitar.Composition GetComposition()
            {
                return (base.InteractionDefinition as Microphone.Perform.Definition).Composition;
            }

            public override void StartPlaying()
            {
                base.StartPlaying();
                this.Actor.LookAtManager.DisableLookAts();
            }

            public override void StopPlaying()
            {
                this.Actor.LookAtManager.EnableLookAts();
                base.StopPlaying();
            }

            public static InteractionDefinition Singleton = new Microphone.Perform.Definition();

            public class Definition : BandInstrument.Perform<Microphone>.PerformDefinition<Microphone.Perform>
            {
                public Definition()
                {
                }

                // Token: 0x060045C2 RID: 17858 RVA: 0x0010C7AE File Offset: 0x0010B7AE
                public Definition(Sims3.Gameplay.Skills.Guitar.Composition comp) : base(comp)
                {
                }

                // Token: 0x060045C3 RID: 17859 RVA: 0x0010C7B7 File Offset: 0x0010B7B7
                public override BandInstrument.Perform<Microphone>.PerformDefinition<Microphone.Perform> CreateNewDefinition(Sims3.Gameplay.Skills.Guitar.Composition comp)
                {
                    return new Microphone.Perform.Definition(comp);
                }
            }
        }

        public class Learn : BandInstrument.Learn<Microphone>
        {
            public override SheetMusic GetSheetMusic(InteractionDefinition def)
            {
                return (def as Microphone.Learn.Definition).SheetMusic;
            }

            public override void StartPlaying()
            {
                base.StartPlaying();
                this.Actor.LookAtManager.DisableLookAts();
            }

            public override void StopPlaying()
            {
                this.Actor.LookAtManager.EnableLookAts();
                base.StopPlaying();
            }

            public static InteractionDefinition Singleton = new Microphone.Learn.Definition();

            public class Definition : BandInstrument.Learn<Microphone>.LearnDefinition<Microphone.Learn>
            {
                public Definition()
                {
                }

                public Definition(SheetMusic sheetMusic) : base(sheetMusic)
                {
                }

                public override BandInstrument.Learn<Microphone>.LearnDefinition<Microphone.Learn> CreateNewDefinition(SheetMusic sheetMusic)
                {
                    return new Microphone.Learn.Definition(sheetMusic);
                }
            }
        }

        public class Watch : MusicalInstrument.WatchBase<Microphone>
        {

            public override MusicalInstrument.WatchBase<Microphone>.WatchTuning Tuning
            {
                get
                {
                    return Microphone.Watch.kWatchTuning;
                }
            }

            [Tunable]
            public static MusicalInstrument.WatchBase<Microphone>.WatchTuning kWatchTuning = new MusicalInstrument.WatchBase<Microphone>.WatchTuning();

            public static InteractionDefinition Singleton = new Microphone.Watch.Definition();

            public class Definition : MusicalInstrument.WatchBase<Microphone>.WatchDefinition<Microphone.Watch>
            {
            }
        }

        public class Dance : MusicalInstrument.DanceBase
        {
           
            public override MusicalInstrument.DanceBase.DanceTuning Tuning
            {
                get
                {
                    return Microphone.Dance.kDanceTuning;
                }
            }

            [Tunable]
            public static MusicalInstrument.DanceBase.DanceTuning kDanceTuning = new MusicalInstrument.DanceBase.DanceTuning();

            public new static InteractionDefinition Singleton = new Microphone.Dance.Definition();

            public new class Definition : MusicalInstrument.DanceBase.DanceDefinition<Microphone.Dance>
            {
            }
        }

        public class MicrophoneInteractionProxy : MusicalInstrument.MusicalInstrumentInteractionProxy
        {
            public static InteractionDefinition Singleton = new Microphone.MicrophoneInteractionProxy.Definition();

            [DoesntRequireTuning]
            public class Definition : MusicalInstrument.MusicalInstrumentInteractionProxy.ProxyDefinition<Microphone.MicrophoneInteractionProxy, Microphone>
            {
            }
        }

        public class Jam : BandInstrument.JamWithInstrument<Microphone>
        {
            public override void Init(ref InteractionInstanceParameters parameters)
            {
                base.Init(ref parameters);
                Microphone.Jam.Definition definition = parameters.InteractionDefinition as Microphone.Jam.Definition;
                if (definition != null)
                {
                    if (definition.MenuText == null)
                    {
                        base.DetermineGenre();
                        this.mInteractionName = RockBand.LocalizeString(this.mGenre.ToString(), new object[0]);
                        return;
                    }
                    this.mGenre = definition.Genre;
                    this.mInteractionName = definition.MenuText;
                }
            }

            public override void StartPlaying()
            {
                base.StartPlaying();
                this.Actor.LookAtManager.DisableLookAts();
            }

            public override void StopPlaying()
            {
                this.Actor.LookAtManager.EnableLookAts();
                base.StopPlaying();
            }

            public static InteractionDefinition Singleton = new Microphone.Jam.Definition();

            public class Definition : BandInstrument.JamWithInstrument<Microphone>.JamWithInstrumentDefinition<Microphone.Jam>
            {
                public Definition()
                {
                }

                public Definition(RockBand.GenreTypes genre, bool isSolo) : base(genre, isSolo)
                {
                }

                public override void AddInteractions(InteractionObjectPair iop, Sim actor, BandInstrument target, List<InteractionObjectPair> results)
                {
                    BandInstrument bandInstrument = BandInstrument.JamWithInstrument<Microphone>.InstrumentToJoin(actor, target);
                    if (bandInstrument == null)
                    {
                        foreach (RockBand.GenreTypes genre in RockBand.kJamGenreTypes)
                        {
                            results.Add(new InteractionObjectPair(new Microphone.Jam.Definition(genre, true), target));
                        }
                        return;
                    }
                    results.Add(new InteractionObjectPair(new Microphone.Jam.Definition(bandInstrument.JamController.Genre, false), target));
                }
            }
        }

        public class PerformGig : BandInstrument.PerfomrGigWithGenre<Microphone>
        {
            public override void Init(ref InteractionInstanceParameters parameters)
            {
                base.Init(ref parameters);
                Microphone.PerformGig.Definition definition = parameters.InteractionDefinition as Microphone.PerformGig.Definition;
                this.mGenre = definition.Genre;
            }

            // Token: 0x060045E0 RID: 17888 RVA: 0x0010C9F4 File Offset: 0x0010B9F4
            public override void StartPlaying()
            {
                base.StartPlaying();
                this.Actor.LookAtManager.DisableLookAts();
            }

            public override void StopPlaying()
            {
                this.Actor.LookAtManager.EnableLookAts();
                base.StopPlaying();
            }

            public static InteractionDefinition Singleton = new Microphone.PerformGig.Definition();

            public class Definition : BandInstrument.PerfomrGigWithGenre<Microphone>.PerfomrGigWithGenreDefinition<Microphone.PerformGig>
            {
                public Definition()
                {
                }

                public Definition(RockBand.GenreTypes genre) : base(genre)
                {
                }

                public override void AddInteractions(InteractionObjectPair iop, Sim actor, Microphone target, List<InteractionObjectPair> results)
                {
                    foreach (RockBand.GenreTypes genre in RockBand.kPerformanceGenreTypes)
                    {
                        results.Add(new InteractionObjectPair(new Microphone.PerformGig.Definition(genre), target));
                    }
                }
            }

            public class PushDefinition : Microphone.PerformGig.Definition
            {
                public PushDefinition()
                {
                }

                public PushDefinition(RockBand.GenreTypes genre) : base(genre)
                {
                }

                public override bool Test(Sim a, Microphone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return base.SharedTest(isAutonomous, a);
                }
            }
        }
    }
}
