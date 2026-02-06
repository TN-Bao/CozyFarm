using System.Collections.Generic;
using CozyFarm.Agent;
using CozyFarm.Tools;

namespace CozyFarm.Interaction
{
    public interface IInteractable
    {
        List<ToolTypes> UsableTools { get; set; }

        bool CanInteract(IAgent agent);
        void Interact(IAgent agent);
    }
}
