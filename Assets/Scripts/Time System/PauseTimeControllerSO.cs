using UnityEngine;

namespace CozyFarm.TimeSystem
{
    [CreateAssetMenu]
    public class PauseTimeControllerSO : ScriptableObject
    {
        public void SetTimePause(bool timeFreez)
        {
            if (timeFreez)
            {
                Debug.Log($"<b><size=15>Time</size></b> pause <color=red> {timeFreez} </color>");
            }
            else
            {
                Debug.Log($"<b><size=15>Time</size></b> pause <color=green> {timeFreez} </color>");
            }

            Time.timeScale = timeFreez ? 0 : 1;
        }
    }
}
