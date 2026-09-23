using System;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
[assembly: Tunable] // Do not forget this line!!

//Template Created by Battery

namespace SDM.SingingSkill
{
    public class ExampleBootstrapperClass : GameObject
    {
        static bool HasBeenLoaded = false;

        [Tunable]
        protected static bool init = false;

        static ExampleBootstrapperClass()
        {
            LoadSaveManager.ObjectGroupsPreLoad += ExampleBootstrapperClass.OnPreload;
        }

        static void OnPreload()
        {
            try
            {
                if (HasBeenLoaded) return; // you only want to run it once per gameplay session
                HasBeenLoaded = true;

                // fill this in with the resourcekey of your SKIL xml
                XmlDbData data = XmlDbData.ReadData(new ResourceKey(0xA750848F8D7661C6, 0xA8D58BE5, 0x00000000), false);

                if (data == null)
                {
                    return;
                }
                SkillManager.ParseSkillData(data, true);

            }
            catch (Exception ex)
            {
                return;
            }
        }
    }

}
