using CozyFarm.Farming;
using CozyFarm.Input;
using CozyFarm.Interaction;
using CozyFarm.Tools;

namespace CozyFarm.Agent
{
    public interface IAgent
    {
        bool Blocked { get; set; }
        FieldDetector FieldDetectorObject { get; }
        AgentMover AgentMover { get; }
        PlayerInputFarm PlayerInput { get; }
        AgentAnimation AgentAnim { get; }
        InteractionDetector InteractionDetector { get; }
        ToolsBag ToolsBag { get; }
        FieldController FieldController { get; }
    }
}
