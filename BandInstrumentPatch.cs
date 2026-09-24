using System;
using System.Collections.Generic;
using System.Text;
using MonoPatcherLib;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Toys;
using Sims3.Gameplay.Skills;
using Sims3.SimIFace;
using Sims3.UI;

namespace SDM.SingingSkill
{
    [TypePatch(typeof(BandInstrument))]
    public class BandInstrumentPatch
    {

        public abstract class PlayBandInstrument<TTarget> : MusicalInstrument.PlayInstrument<TTarget>, IGigInteraction where TTarget : BandInstrument
        {
            
            
            public virtual bool ShouldDisableStereos
            {
                get
                {
                    return this.Actor.IsSelectable;
                }
            }

            
            public override void EnterInstrumentStateMachineAndPlay()
            {
                try
                {
                    // bro we are singing
                    if (this.Target.SkillParameterName == "SingingSkill")
                    {
                        StyledNotification.Show(new Sims3.UI.StyledNotification.Format("singing yay", StyledNotification.NotificationStyle.kGameMessageNegative));

                        try
                        {
                            base.EnterStateMachine("solo_generic", "Enter", "x");
                            base.SetParameter("AnimationName", "a2a_soc_neutral_singFriendly_friendly_neutral_x");
                            base.AddOneShotScriptEventHandler(100U, new SacsEventHandler(this.SoundControl));
                            base.AnimateSim("Play Animation");
                            StyledNotification.Show(new Sims3.UI.StyledNotification.Format("sing animation started ok?", StyledNotification.NotificationStyle.kGameMessageNegative));
                        }
                        catch (Exception ex)
                        {
                            StyledNotification.Show(new Sims3.UI.StyledNotification.Format("exception caught AFTER object was id'd as microphone while trying to parse state machine", StyledNotification.NotificationStyle.kGameMessageNegative));
                            return;
                        }

                        
                    }
                    // not singing
                    else
                    {
                        StyledNotification.Show(new Sims3.UI.StyledNotification.Format("not singing", StyledNotification.NotificationStyle.kGameMessageNegative));
                        this.Target.SetupStateMachine(this);
                        base.SetParameter(this.Target.SkillParameterName, this.mSkill.GetSkillLevelParameterForJazzGraph());
                        base.AddOneShotScriptEventHandler(100U, new SacsEventHandler(this.SoundControl));
                        base.AnimateSim("Play");
                    }
                }
                catch (Exception ex)
                {
                    StyledNotification.Show(new Sims3.UI.StyledNotification.Format("exception caught at EnterInstrumentStateMachineAndPlay stage", StyledNotification.NotificationStyle.kGameMessageNegative));
                    return;
                }
                
                    
            }


            public override void TriggerAudio(bool turnOn)
            {
                if (this.Target.SkillParameterName == "SingingSkill")
                {
                    StyledNotification.Show(new Sims3.UI.StyledNotification.Format("we're singing, trigger audio", StyledNotification.NotificationStyle.kGameMessageNegative));
                    string text;

                    text = "vo_sing_frnder_c_frndlyA";

                    this.mInstrumentSound = new ObjectSound(this.Target.ObjectId, text);
                }
                else if (turnOn)
                {
                 StyledNotification.Show(new Sims3.UI.StyledNotification.Format("not singing trigger audio 1", StyledNotification.NotificationStyle.kGameMessageNegative));
                    int index = 0;
                    if (this.mSkill != null)
                    {
                        index = this.mSkill.SkillLevel;
                    }
                    this.mInstrumentSound = new ObjectSound(this.Target.ObjectId, this.Target.GetLoopForSkillLevel(index));
                    this.mInstrumentSound.Start(3U);
                    if (this.mJig != null && this.mJig.Amp != null)
                    {
                        this.mJig.Amp.StartVFX();
                        return;
                    }
                }
                else if (this.mInstrumentSound != null)
                {
                 StyledNotification.Show(new Sims3.UI.StyledNotification.Format("turn off music", StyledNotification.NotificationStyle.kGameMessageNegative));
                    this.mInstrumentSound.Dispose();
                    this.mInstrumentSound = null;
                }
            }

            public void SoundControl(StateMachineClient smc, IEvent evt)
            {
                switch (evt.EventId)
                {
                    case 100U:
                        this.TriggerAudio(true);
                        return;
                    case 101U:
                        this.TriggerAudio(false);
                        return;
                    default:
                        return;
                }
            }

        

            public ObjectSound mInstrumentSound;

            public BandInstrument.Jig mJig;

            public void DestroyJig()
            {
                if (this.Target.mTipJar != null)
                {
                    this.Target.mTipJar.Destroy();
                    this.Target.mTipJar = null;
                }
                if (this.mJig != null)
                {
                    if (this.Target.Parent == this.mJig)
                    {
                        this.Target.UnParent();
                    }
                    base.FirePerformanceCompleteEvent(false);
                    if (this.mJig.Amp != null)
                    {
                        this.mJig.Amp.Destroy();
                        this.mJig.Amp = null;
                    }
                    this.mJig.Destroy();
                    this.mJig = null;
                }
            }

            public override bool Run()
            {
                this.Target.IsInBeingPlayedInteraction = true;
                if (!this.Target.ReadyToPlay(this.Actor))
                {
                    return false;
                }
                StyledNotification.Show(new Sims3.UI.StyledNotification.Format("about to fire doplayinstrument from bandinstrument", StyledNotification.NotificationStyle.kGameMessageNegative));
                bool result = this.DoPlayInstrument();
                this.DestroyJig();
                this.Target.DonePlaying(this.Actor);
                return result;
            }


        }








    }
}
