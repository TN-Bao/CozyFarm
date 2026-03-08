using CozyFarm.Agent;
using CozyFarm.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CozyFarm.Interaction
{
    public class WateringInteraction : MonoBehaviour, IInteractable
    {
        [field: SerializeField]
        public List<ToolTypes> UsableTools { get; set; } = new() { ToolTypes.WateringCan };

        public bool CanInteract(IAgent agent)
            => UsableTools.Contains(agent.ToolsBag.CurrentTool.ToolType);

        public void Interact(IAgent agent)
        {
            //agent.FieldController.WaterCropAt(transform.position);
        }
    }
}
