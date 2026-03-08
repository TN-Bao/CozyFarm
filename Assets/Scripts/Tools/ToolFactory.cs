using CozyFarm.DataStorage;
using UnityEngine;

namespace CozyFarm.Tools
{
    public class ToolFactory : MonoBehaviour
    {
        public static Tool CreateTool(ItemDescription description, string data = null)
        {
            Tool tool = description.ToolType switch
            {
                ToolTypes.Hand => new HandTool(description.ID, data),
                ToolTypes.Hoe => new HoeTool(description.ID, data),
                ToolTypes.SeedPlacer => new SeedPlacementTool(description.ID, data),
                ToolTypes.WateringCan => new WateringTool(description.ID, data),
                _ => throw new System.NotImplementedException(
                    $"ToolType is not defined in the ToolFactory {description.ToolType}")              
            };

            tool.ToolAnimator = description.ToolAnimator;
            tool.ToolRange = description.ToolRange;
            return tool;
        }
    }
}
