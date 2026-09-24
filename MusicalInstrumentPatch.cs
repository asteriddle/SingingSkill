using System;
using System.Collections.Generic;
using System.Text;
using MonoPatcherLib;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;

namespace SDM.SingingSkill
{
    [TypePatch(typeof(MusicalInstrument))]
    public class MusicalInstrumentPatch
    {


        public abstract class PlayInstrument<TTarget> : Interaction<Sim, TTarget>, ISkillCallbackUser, MusicalInstrument.IPlayInstrumentInteraction, IPersistPostLoad where TTarget : MusicalInstrument
        {
            public event MusicalInstrument.PlayInstrument<TTarget>.PerformanceFinished PerformanceFinishedEvent;


            // Token: 0x0600442B RID: 17451
            public abstract bool FindPlaceToPlay();

            // Token: 0x0600442C RID: 17452
            public abstract void EnterInstrumentStateMachineAndPlay();

            // Token: 0x0600442D RID: 17453
            public abstract bool DoPlayLoop();

            // Token: 0x0600442E RID: 17454
            public abstract void FinishPlayingAndExitStateMachine();

            // Token: 0x0600442F RID: 17455
            public abstract void TriggerAudio(bool turnOn);


            // Token: 0x06004432 RID: 17458 RVA: 0x00105CE8 File Offset: 0x00104CE8
            public virtual bool DoPlayInstrument()
            {

                StyledNotification.Show(new Sims3.UI.StyledNotification.Format("doplayinstrument fired", StyledNotification.NotificationStyle.kGameMessageNegative));


                bool flag = this.Target is ICarryable;
                this.Target.IsInBeingPlayedInteraction = true;
                if (!this.FindPlaceToPlay())
                {
                    if (!flag)
                    {
                        this.Target.ClearUseList();
                    }
                    return false;
                }
                this.mSkill = (this.Actor.SkillManager.AddElement(this.Target.SkillName) as MusicSkill);
                StyledNotification.Show(new Sims3.UI.StyledNotification.Format("musicskill found and assigned", StyledNotification.NotificationStyle.kGameMessageNegative));
                if (!flag && !this.Target.IsActorUsingMe(this.Actor))
                {
                    this.Target.AddToUseList(this.Actor);
                }
                this.EnterInstrumentStateMachineAndPlay();
                StyledNotification.Show(new Sims3.UI.StyledNotification.Format("state machine entered", StyledNotification.NotificationStyle.kGameMessageNegative));

                base.BeginCommodityUpdates();
                this.mPlaying = true;
                this.StartPlaying();
                StyledNotification.Show(new Sims3.UI.StyledNotification.Format("started playing", StyledNotification.NotificationStyle.kGameMessageNegative));

                bool flag2 = this.DoPlayLoop();
                StyledNotification.Show(new Sims3.UI.StyledNotification.Format("play loop started", StyledNotification.NotificationStyle.kGameMessageNegative));

                this.mPlaying = false;
                this.StopPlaying();
                if (flag2 && this.Actor.HasTrait(TraitNames.Virtuoso))
                {
                    TraitTipsManager.ShowTraitTip(13271263770231522944UL, this.Actor, TraitTipsManager.TraitTipCounterIndex.Virtuoso, TraitTipsManager.kVirtuosoCountOfGuitarPlayed);
                }
                if (this.PerformanceFinishedEvent != null)
                {
                    if (this.Actor.HasExitReason(this.Target.Tuning.ReasonsNotToAllowTips | ExitReason.BuffFailureState))
                    {
                        this.PerformanceFinishedEvent(false);
                    }
                    else
                    {
                        this.PerformanceFinishedEvent(true);
                    }
                }
                base.EndCommodityUpdates(flag2);
                if (!this.Actor.HasExitReason(ExitReason.BuffFailureState))
                {
                    float num = SimClock.ElapsedTime(TimeUnit.Minutes) + this.Target.Tuning.LongestWatcherWaitTimeMinutes;
                    while (this.PerformanceFinishedEvent != null && SimClock.ElapsedTime(TimeUnit.Minutes) < num)
                    {
                        Simulator.Sleep(30U);
                    }
                }
                this.FinishPlayingAndExitStateMachine();
                if (!flag && this.Target.IsActorUsingMe(this.Actor))
                {
                    this.Target.RemoveFromUseList(this.Actor);
                }
                return flag2;
            }


            // Token: 0x06004434 RID: 17460 RVA: 0x00105F1D File Offset: 0x00104F1D
            public void FirePerformanceCompleteEvent(bool allowTip)
            {
                if (this.PerformanceFinishedEvent != null)
                {
                    this.PerformanceFinishedEvent(allowTip);
                }
            }

            
            public virtual void StartPlaying()
            {
                this.mStopShouldBeCalled = true;
                this.AddWhilePlayingInteractions();
                if (this.mSkill != null)
                {
                    this.BroadcastPlaying(this.mSkill.SkillLevel);
                    this.mSkill.RegisterForSkillLevelUpEvent(new Skill.SkillLevelUpCallback(this.SkillLevelUpCallback), this);
                }
                else
                {
                    StyledNotification.Show(new StyledNotification.Format("kill yourself", StyledNotification.NotificationStyle.kGameMessagePositive));

                }
                this.Target.mWatchBroadcaster = new ReactionBroadcaster(this.Target, this.Target.WatchBroadcasterParams, this.Target.GetWatchInteraction());
                this.Target.mIsBeingPlayed = true;
                this.Target.ResetWatcherCount();
            }

            // Token: 0x06004436 RID: 17462 RVA: 0x00105FE4 File Offset: 0x00104FE4
            public virtual void SkillLevelUpCallback(int skillLevel)
            {
                if (this.mChangeAudioOnLevelUp)
                {
                    this.TriggerAudio(false);
                    this.TriggerAudio(true);
                }
                this.BroadcastPlaying(skillLevel);
            }

            // Token: 0x06004437 RID: 17463 RVA: 0x00106004 File Offset: 0x00105004
            public void BroadcastPlaying(int skillLevel)
            {
                base.SetParameter(this.Target.SkillParameterName, this.mSkill.GetSkillLevelParameterForJazzGraph());
                if (skillLevel >= this.Target.Tuning.LevelDanceBecomesAvailable)
                {
                    this.Target.AddInteraction(this.Target.GetDanceInteraction(), true);
                }
                this.Target.StartBroadcast(this.Actor, skillLevel);
            }

            // Token: 0x06004438 RID: 17464 RVA: 0x0010608C File Offset: 0x0010508C
            public virtual void StopPlaying()
            {
                if (this.mSkill != null)
                {
                    this.mSkill.UnRegisterForSkillLevelUpEvent(new Skill.SkillLevelUpCallback(this.SkillLevelUpCallback), this);
                    if (this.mSkill.SkillLevel >= this.Target.Tuning.LevelDanceBecomesAvailable)
                    {
                        this.Target.RemoveInteractionByType(this.Target.GetDanceInteraction());
                    }
                }
                else
                {
                    StyledNotification.Show(new StyledNotification.Format("kill urself 2", StyledNotification.NotificationStyle.kGameMessagePositive));
                }
                this.CleanupBroadcasts();
                this.mStopShouldBeCalled = false;
                this.Target.mIsBeingPlayed = false;
                this.Target.ResetWatcherCount();
            }

            // Token: 0x06004439 RID: 17465 RVA: 0x00106130 File Offset: 0x00105130
            public void CleanupBroadcasts()
            {
                if (this.Target.mWatchBroadcaster != null)
                {
                    this.Target.mWatchBroadcaster.Dispose();
                    this.Target.mWatchBroadcaster = null;
                }
                this.Target.RemoveInteractionByType(Sim.GiveTipToPlayer.Singleton);
                this.Target.RemoveInteractionByType(this.Target.GetWatchInteraction());
                this.Actor.RemoveInteractionByType(this.Target.GetProxyInteraction());
                this.Target.StopBroadcast(this.Actor);
            }

            // Token: 0x0600443A RID: 17466 RVA: 0x001061E0 File Offset: 0x001051E0
            public Skill.SkillLevelUpCallback GetSkillCallback()
            {
                return new Skill.SkillLevelUpCallback(this.SkillLevelUpCallback);
            }

            // Token: 0x0600443B RID: 17467 RVA: 0x001061F0 File Offset: 0x001051F0
            public void AddWhilePlayingInteractions()
            {
                this.Target.AddInteraction(this.Target.GetWatchInteraction());
                this.Target.AddInteraction(Sim.GiveTipToPlayer.Singleton);
                this.Actor.AddInteraction(this.Target.GetProxyInteraction());
            }

            // Token: 0x0600443C RID: 17468 RVA: 0x00106254 File Offset: 0x00105254
            public void PersistPostLoad()
            {
                if (this.mPlaying)
                {
                    this.AddWhilePlayingInteractions();
                    if (this.mSkill != null && this.mSkill.SkillLevel >= this.Target.Tuning.LevelDanceBecomesAvailable)
                    {
                        this.Target.AddInteraction(this.Target.GetDanceInteraction(), true);
                    }
                }
            }

            // Token: 0x04001DC5 RID: 7621
            public MusicSkill mSkill;

            // Token: 0x04001DC7 RID: 7623
            public bool mChangeAudioOnLevelUp = true;

            // Token: 0x04001DC8 RID: 7624
            public bool mPlaying;

            // Token: 0x04001DC9 RID: 7625
            public bool mStopShouldBeCalled;

            // Token: 0x02000806 RID: 2054
            // (Invoke) Token: 0x0600443E RID: 17470
            public delegate void PerformanceFinished(bool allowTip);

            // Token: 0x02000807 RID: 2055
            public class PlayDefinition<T> : InteractionDefinition<Sim, MusicalInstrument, T>, IWakesLightSleeper where T : MusicalInstrument.PlayInstrument<TTarget>, new()
            {
                // Token: 0x06004441 RID: 17473 RVA: 0x001062C0 File Offset: 0x001052C0
                public override bool Test(Sim a, MusicalInstrument target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous && !MusicalInstrument.PlayInstrument<TTarget>.AutonomousCheck(a))
                    {
                        return false;
                    }
                    bool flag = false;
                    if (target.InUse)
                    {
                        SittableComponent sittable = target.Sittable;
                        flag = (sittable == null || sittable.GetPartSimIsIn(a) == null);
                    }
                    return !flag && target.CanUseSharedTest(a, isAutonomous);
                }

                // Token: 0x06004442 RID: 17474 RVA: 0x0010630C File Offset: 0x0010530C
                public override string GetInteractionName(Sim a, MusicalInstrument target, InteractionObjectPair interaction)
                {
                    string entryKey;
                    if (GameUtils.IsInstalled(ProductVersion.EP3))
                    {
                        entryKey = "Gameplay/Objects/HobbiesSkills/" + target.GetPlayName() + "/PlayAlone:InteractionName";
                    }
                    else
                    {
                        entryKey = "Gameplay/Objects/HobbiesSkills/" + target.GetPlayName() + "/Play:InteractionName";
                    }
                    object[] parameters = new object[0];
                    return Localization.LocalizeString(entryKey, parameters);
                }
            }
        }


    }
}
