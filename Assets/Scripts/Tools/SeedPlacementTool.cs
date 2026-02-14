using System;
using CozyFarm.Agent;
using UnityEngine;

namespace CozyFarm.Tools
{
    public class SeedPlacementTool : Tool, IQuantity
    {
        public int CropID { get; set; } = 0;
        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; }
        }

        public SeedPlacementTool(int itemID, string data) : base(itemID, data)
        {
            this.ToolType = ToolTypes.SeedPlacer;
        }

        public override string GetDataToSave()
        {
            return JsonUtility.ToJson(new SeeToolData
            {
                cropID = CropID,
                quantity = _quantity
            });
        }

        public override void RestoreSaveData(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new System.Exception("Failed to create tool because data passed is null");

            SeeToolData savedData = JsonUtility.FromJson<SeeToolData>(data);
            CropID = savedData.cropID;
            _quantity = savedData.quantity;
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
            if (agent.FieldDetectorObject.ValidSelectionPositions.Count < 0) return;

            agent.Blocked = true;
            agent.AgentAnim.PlayAnimation(AnimationType.PickUp);
            OnPerformAction?.Invoke();

            agent.AgentAnim.OnAnimationEnd.AddListener(
                () =>
                {
                    foreach (var pos in agent.FieldDetectorObject.ValidSelectionPositions)
                    {
                        if (agent.FieldController.CanIPlaceCropsHere(pos))
                        {
                            agent.FieldController.PlaceCropAt(pos, CropID);
                        }
                        else
                        {
                            Debug.Log($"Cannot place crop at {pos}");
                        }
                    }
                    _quantity--;
                    OnFinishedAction?.Invoke(agent);
                    agent.Blocked = false;
                });

            agent.FieldController.PrintCropsStatus();
        }

        public override bool IsToolStillValid()
        {
            return _quantity > 0;
        }
    }

    [Serializable]
    public struct SeeToolData
    {
        public int cropID, quantity;
    }
}
