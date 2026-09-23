using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.SimIFace.CustomContent;

namespace SDM.SingingSkill
{
    [Persistable]
    public interface IMicrophone : IBandInstrument, IMusicalInstrument, IGameObject, IScriptObject, IScriptLogic, IHasScriptProxy, IObjectUI, IExportableContent
    {
    }
}
