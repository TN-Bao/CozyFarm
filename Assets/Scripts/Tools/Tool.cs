using CozyFarm.Agent;
using UnityEngine;

namespace CozyFarm.Tools
{
    public abstract class Tool
    {
        public ToolTypes ToolType { get; }
        public RuntimeAnimatorController ToolAnimator { get; set; }
        public Vector2Int ToolRange { get; set; } = Vector2Int.one;
        protected Tool(ToolTypes toolType)
        {
            this.ToolType = toolType;
        }

        public virtual void PutAway(IAgent agent){}
        public virtual void Equip(IAgent agent){}

        public abstract void UseTool(IAgent agent);
    }
}
