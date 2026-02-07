using CozyFarm.DataStorage;
using UnityEngine;

namespace CozyFarm.Tools
{
    public class ToolFactory : MonoBehaviour
    {
        public static Tool CreateTool(ItemDescription description)
        {
            Tool tool = description.ToolType switch
            {
                ToolTypes.Hand => new HandTool(description.ToolType),
                ToolTypes.Hoe => new HoeTool(description.ToolType),
                _ => throw new System.NotImplementedException(
                    $"ToolType is not defined in the ToolFactory {description.ToolType}")              
            };

            tool.ToolAnimator = description.ToolAnimator;
            return tool;
        }
    }
}
