using CozyFarm.Agent;
using CozyFarm.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CozyFarm.Interaction
{
    public class FillInWaterInteraction : MonoBehaviour, IInteractable
    {
        [field: SerializeField]
        public List<ToolTypes> UsableTools { get; set; } = new() { ToolTypes.WateringCan };
        public UnityEvent OnInteract;

        public bool CanInteract(IAgent agent)
            => UsableTools.Contains(agent.ToolsBag.CurrentTool.ToolType);

        public void Interact(IAgent agent)
        {
            agent.ToolsBag.RestoreCurrentTool(agent);
            OnInteract?.Invoke();
        }
    }
}
