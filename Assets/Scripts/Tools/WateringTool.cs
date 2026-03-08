using CozyFarm.Agent;
using CozyFarm.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CozyFarm.Tools
{
    public class WateringTool : Tool
    {
        private int _maxUses = 4;
        public int NumberOfUses { get; set; }
        
        public WateringTool(int itemID, string data) : base(itemID, data)
        {
            this.ToolType = ToolTypes.WateringCan;
            //NumberOfUses = _maxUses;
        }

        public override bool IsToolStillValid()
            => true;

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
            if (agent.FieldDetectorObject != null && agent.FieldDetectorObject.ValidSelectionPositions.Count > 0)
            {
                TryWateringCrop(agent);
            }
            else
            {
                TryInteractionWithSomething(agent);
            }
        }

        private void TryInteractionWithSomething(IAgent agent)
        {
            foreach (var interactable in agent.InteractionDetector.PerformDetection())
            {
                if (interactable.CanInteract(agent))
                {
                    agent.Blocked = true;
                    agent.AgentAnim.PlayAnimation(AnimationType.Watering);
                    if (ToolAnimator != null)
                    {
                        agent.AgentAnim.ToolAnim.SetAnimatorController(ToolAnimator);
                        agent.AgentAnim.ToolAnim.PlayAnimation();
                    }
                    agent.AgentAnim.OnAnimationOnce.AddListener(() =>
                    {
                        interactable.Interact(agent);
                    });
                    agent.AgentAnim.OnAnimationEnd.AddListener(() =>
                    {
                        agent.Blocked = false;
                        OnFinishedAction?.Invoke(agent);
                    }
                    );
                    return;
                }
            }
        }

        private void TryWateringCrop(IAgent agent)
        {
            List<Vector2> cropFields = agent.FieldDetectorObject.ValidSelectionPositions
                .Where(pos => agent.FieldController.IsThereCropAt(pos)).ToList();

            if (cropFields.Count <= 0)
            {
                Debug.Log("No crops to water here");
                return;
            }
            if (NumberOfUses <= 0)
            {
                Debug.Log("Watering can has NO water");
                return;
            }

            agent.Blocked = true;
            agent.AgentAnim.PlayAnimation(AnimationType.Watering);

            if (ToolAnimator != null)
            {
                agent.AgentAnim.OnAnimationOnce.AddListener(() =>
                {
                    foreach (var pos in cropFields)
                    {
                        agent.FieldController.WaterCropAt(pos);
                    }
                    NumberOfUses--;
                });

                agent.AgentAnim.OnAnimationEnd.AddListener(() =>
                {
                    agent.Blocked = false;
                    OnFinishedAction?.Invoke(agent);
                    agent.FieldController.PrintCropsStatus();
                });

                agent.AgentAnim.ToolAnim.SetAnimatorController(ToolAnimator);
                agent.AgentAnim.ToolAnim.PlayAnimation();
            }
        }

        public override string GetDataToSave()
        {
            return NumberOfUses.ToString();
        }

        public override void RestoreSaveData(string data)
        {
            NumberOfUses = string.IsNullOrEmpty(data) ? 0 : int.Parse(data);
        }

        public void Refill()
        {
            NumberOfUses = _maxUses;
        }
    }
}
