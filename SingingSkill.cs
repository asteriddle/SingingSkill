using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;

namespace SDM.SingingSkill
{
    public class SingingSkill : BandSkill
    {
        public new static string LocalizeString(string name, params object[] parameters)
        {
            return Localization.LocalizeString("Gameplay/Skills/Singing:" + name, parameters);
        }

        public new static string LocalizeString(bool isFemale, string name, params object[] parameters)
        {
            return Localization.LocalizeString(isFemale, "Gameplay/Skills/Singing:" + name, parameters);
        }

        public SingingSkill(SkillNames guid) : base(guid)
        {
        }

        public SingingSkill()
        {
        }

        public override List<Guitar.Composition> MasterTracks
        {
            get
            {
                return SingingSkill.SingingMasterCompositions;
            }
        }

        public override List<Guitar.Composition> Tracks
        {
            get
            {
                return SingingSkill.SingingCompositions;
            }
        }

        public override bool IsOpportunityTrack(Guitar.Composition comp)
        {
            return comp.AudioClip == "drums_masterd";
        }

        public override string CompositionPrefix
        {
            get
            {
                return "singing";
            }
        }

        public override string MedatorInstanceNameForNpcObject
        {
            get
            {
                return "microphoneBasic";
            }
        }

        public override ProductVersion MedatorInstanceProductVersionForNpcObject
        {
            get
            {
                return ProductVersion.EP3;
            }
        }

        public override string LocalizeSkillString(string name, params object[] parameters)
        {
            return SingingSkill.LocalizeString(base.SkillOwner.IsFemale, name, parameters);
        }

        public override Type TypeOfInstrument
        {
            get
            {
                return typeof(IMicrophone);
            }
        }

        public override void SetupAmbientRabbitholeSkillClassSounds(ObjectGuid targetGuid, ref ObjectSound classSound1, ref ObjectSound classSound2)
        {
            classSound1 = new ObjectSound(targetGuid, "rhole_school_artclass_loop");
            classSound1.StartLoop();
            classSound2 = new ObjectSound(targetGuid, "rhole_school_drum_class_oneshot");
            classSound2.StartLoop();
        }

        public new const string sLocalizationKey = "Gameplay/Skills/Singing";

        public const string kOpportunityAudioClip = "drums_masterd";

        public static List<Guitar.Composition> SingingCompositions = new List<Guitar.Composition>();

        // Token: 0x040079E2 RID: 31202
        public static List<Guitar.Composition> SingingMasterCompositions = new List<Guitar.Composition>();
    }

}
