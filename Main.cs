using System;
using MonoPatcherLib;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;

//Template Created by Battery

namespace SDM.SingingSkill
{
    [Plugin]
    public class Main
    {
        static bool HasBeenLoaded = false;

        [Tunable]
        public static bool kInstantiator = false;

        static Main()
        {
            MonoPatcher.PatchAll();
            LoadSaveManager.ObjectGroupsPreLoad += Main.OnPreload;
            World.sOnWorldLoadFinishedEventHandler = (EventHandler)Delegate.Combine(World.sOnWorldLoadFinishedEventHandler, new EventHandler(Main.OnWorldFinishedLoading));
        }

        private static void OnWorldFinishedLoading(object sender, EventArgs e)
        {
            StyledNotification.Show(new StyledNotification.Format("singing skill loaded!", StyledNotification.NotificationStyle.kGameMessagePositive));
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
