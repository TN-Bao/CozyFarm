using System.Collections.Generic;
using CozyFarm.Agent;
using UnityEngine;

namespace CozyFarm.Tools
{
    public class HoeTool : Tool
    {
        public HoeTool(ToolTypes toolType) : base(toolType)
        {
        }

        public override void Equip(IAgent agent)
        {
            agent.FieldDetectorObject.StartChecking(ToolRange);
        }

        public override void PutAway(IAgent agent)
        {
            agent.FieldDetectorObject.StopChecking();
        }

        public override void UseTool(IAgent agent)
        {
            if (agent.FieldDetectorObject.IsNearField == false) return;
            
            List<Vector2> detectedPos = agent.FieldDetectorObject.ValidSelectionPositions;
            if (detectedPos.Count <= 0) return;

            agent.Blocked = true;
            agent.AgentAnim.OnAnimationEnd.AddListener(
                () =>
                {
                    foreach (Vector2 worldPos in detectedPos)
                    {
                        agent.FieldController.PrepareFieldAt(worldPos);
                    }
                    agent.Blocked = false;
                }
            );

            if (ToolAnimator != null)
            {
                agent.AgentAnim.ToolAnim.SetAnimatorController(ToolAnimator);
                agent.AgentAnim.ToolAnim.PlayAnimation();
            }

            agent.AgentAnim.PlayAnimation(AnimationType.Swing);
            return;
        }
    }
}
