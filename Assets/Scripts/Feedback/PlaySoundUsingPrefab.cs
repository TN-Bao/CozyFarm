using UnityEngine;

namespace CozyFarm.Feedback
{
    public class PlaySoundUsingPrefab : MonoBehaviour
    {
        [SerializeField] private PlayClipDestroy _audioPrefab;
        [SerializeField] private AudioClip[] _audioClips;

        void Awake()
        {
            Debug.Assert(_audioClips != null && _audioClips.Length > 0,
            "Audio Clips array must have some audio clips", gameObject);
        }

        public void CreateSoundCheck()
        {
            PlayClipDestroy audioObject =
                Instantiate(_audioPrefab, transform.position, Quaternion.identity);

                audioObject.PlayClip(_audioClips[Random.Range(0, _audioClips.Length)]);
        }
    }
}
