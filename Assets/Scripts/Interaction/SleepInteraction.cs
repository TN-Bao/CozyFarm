using CozyFarm.Agent;
using CozyFarm.Tools;
using CozyFarm.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CozyFarm.Interaction
{
    public class SleepInteraction : MonoBehaviour, IInteractable
    {
        public List<ToolTypes> UsableTools { get; set; } = new() { ToolTypes.Hand };

        public UnityEvent OnAfterFinishedSleeping, OnMoveToNextDay;

        [SerializeField] private SceneTransitionEffect _transitionEffect;

        public bool CanInteract(IAgent agent)
           => UsableTools.Contains(agent.ToolsBag.CurrentTool.ToolType);

        private void Awake() {
            _transitionEffect = FindObjectOfType<SceneTransitionEffect>(true);
        }

        public void Interact(IAgent agent)
        {
            Debug.Log("Going to sleep");
            StartCoroutine(SleepTransition(agent));
        }

        private IEnumerator SleepTransition(IAgent agent)
        {
            if (_transitionEffect != null)
            {
                _transitionEffect.PlayTransition(false);
            }
            
            agent.Blocked = true;
            yield return new WaitForSecondsRealtime(1);

            OnMoveToNextDay?.Invoke();
            if (_transitionEffect != null)
            {
                _transitionEffect.PlayTransition(true);
            }
            yield return new WaitForSecondsRealtime(1);

            agent.Blocked = false;
            OnAfterFinishedSleeping?.Invoke();
            Debug.Log("Finish sleeping");
        }
    }
}
