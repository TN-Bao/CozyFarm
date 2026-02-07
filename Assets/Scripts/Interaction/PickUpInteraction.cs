using System.Collections.Generic;
using CozyFarm.Agent;
using CozyFarm.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace CozyFarm.Interaction
{
    public class PickUpInteraction : MonoBehaviour, IInteractable
    {
        [field: SerializeField]
        public List<ToolTypes> UsableTools { get; set; }
            = new List<ToolTypes>();

        public UnityEvent OnPickup;

        public bool CanInteract(IAgent agent)
            => UsableTools.Contains(agent.ToolsBag.CurrentTool.ToolType);

        public void Interact(IAgent agent)
        {
            OnPickup?.Invoke();
            Destroy(gameObject);
        }
    }
}
