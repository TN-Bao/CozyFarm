using UnityEngine;

namespace CozyFarm.Feedback
{
    public class PlaySoundSimple : MonoBehaviour
    {
        [SerializeField] private AudioClip _soundToPlay;
        [SerializeField] private AudioSource _audioSource;

        public void StartPlaying()
        {
            _audioSource.PlayOneShot(_soundToPlay);
        }
    }
}
