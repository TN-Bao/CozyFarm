using System;
using CozyFarm.Agent;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

namespace CozyFarm.Tools
{
    public abstract class Tool
    {
        public ToolTypes ToolType { get; protected set; }
        public Action OnPerformAction, OnStartedAction;
        public Action<IAgent> OnFinishedAction;
        public RuntimeAnimatorController ToolAnimator { get; set; }
        public Vector2Int ToolRange { get; set; } = Vector2Int.one;
        
        public int ItemIndex { get; set; }
        protected Tool(int itemID, string data)
        {
            this.ItemIndex = itemID;
            RestoreSaveData(data);
        }

        public virtual void RestoreSaveData(string data){}

        public virtual string GetDataToSave() => String.Empty;
        public abstract bool IsToolStillValid();

        public virtual void PutAway(IAgent agent){}
        public virtual void Equip(IAgent agent){}

        public abstract void UseTool(IAgent agent);
    }
}
