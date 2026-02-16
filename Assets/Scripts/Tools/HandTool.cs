using System.Collections;
using System.Collections.Generic;
using CozyFarm.Agent;
using CozyFarm.Interaction;
using UnityEngine;

namespace CozyFarm.Tools
{
    public class HandTool : Tool
    {
        public HandTool(int itemID, string data) : base(itemID, data)
        {
            this.ToolType = ToolTypes.Hand;
        }

        public override void Equip(IAgent agent)
        {
            agent.FieldDetectorObject.StartChecking(ToolRange);
        }

        public override bool IsToolStillValid()
        {
            return true;
        }

        public override void PutAway(IAgent agent)
        {
            agent.FieldDetectorObject.StopChecking();
        }

        public override void UseTool(IAgent agent)
        {
            IEnumerable<IInteractable> interactables = null;
            if (agent.FieldDetectorObject.IsNearField)
            {
                if (agent.FieldDetectorObject.ValidSelectionPositions.Count > 0)
                {
                    interactables = agent.InteractionDetector.PerformDetection(
                        agent.FieldDetectorObject.ValidSelectionPositions[0]);
                }
            }
            if (interactables == null)
                interactables = agent.InteractionDetector.PerformDetection();

            foreach (IInteractable item in interactables)
            {
                if (item.CanInteract(agent))
                {
                    agent.Blocked = true;

                    agent.AgentAnim.OnAnimationEnd.AddListener(
                        () =>
                        {
                            agent.Blocked = false;
                            item.Interact(agent);
                        }
                    );

                    agent.AgentAnim.PlayAnimation(AnimationType.PickUp);
                    return;
                }
            }
        }
    }

    public enum ToolTypes
    {
        None,
        Hand,
        Hoe,
        SeedPlacer
    }
}
