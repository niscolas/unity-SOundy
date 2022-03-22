using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using niscolas.UnityUtils.Core.Extensions;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Audio;

namespace SOundy
{
    [CreateAssetMenu(
        fileName = "SOundy",
        menuName = Constants.CreateAssetMenuPrefix + "SOundy Asset",
        order = Constants.CreateAssetMenuOrder)]
    public partial class SOundyAsset : ScriptableObject
    {
#if ODIN_INSPECTOR
        [Title("Sound Preset")]
        [InlineEditor(InlineEditorModes.SmallPreview)]
        [PropertySpace(SpaceAfter = 15)]
#endif
        [SerializeField]
        private AudioClip[] _clips;

#if ODIN_INSPECTOR
        [TabGroup("Data", "Basic")]
#endif
        [SerializeField]
        private AudioMixerGroup _audioMixerGroup;

#if ODIN_INSPECTOR
        [TabGroup("Data", "Basic")]
        [MinMaxSlider(0.1f, 2, true)]
#endif
        [SerializeField]
        private Vector2Reference _volumeRange = new Vector2Reference(Vector2.one);

#if ODIN_INSPECTOR
        [TabGroup("Data", "Advanced")]
        [MinMaxSlider(0.1f, 3, true)]
#endif
        [SerializeField]
        private Vector2Reference _pitchRange = new Vector2Reference(Vector2.one);

#if ODIN_INSPECTOR
        [TabGroup("Data", "Advanced")]
        [BoxGroup("Data/Advanced/Time Settings")]
#endif
        [SerializeField]
        private bool _loop;

#if ODIN_INSPECTOR
        [TabGroup("Data", "Advanced")]
        [BoxGroup("Data/Advanced/Time Settings")]
#endif
        [SerializeField]
        private IntReference _playTimeCount = new IntReference(1);

#if ODIN_INSPECTOR
        [TabGroup("Data", "Advanced")]
        [BoxGroup("Data/Advanced/Time Settings")]
        [FormerlySerializedAs("delayTime")]
#endif
        [SerializeField]
        private FloatReference _delay = new FloatReference();

#if ODIN_INSPECTOR
        [TabGroup("Data", "Advanced")]
        [BoxGroup("Data/Advanced/Time Settings")]
#endif
        [SerializeField]
        private FloatReference _delayBetweenPlays = new FloatReference();

#if ODIN_INSPECTOR
        [TabGroup("Data", "Advanced")]
        [BoxGroup("Data/Advanced/3D")]
#endif
        [SerializeField]
        private Vector3Reference _position = new Vector3Reference();

#if ODIN_INSPECTOR
        [TabGroup("Data", "Advanced")]
        [BoxGroup("Data/Advanced/3D")]
#endif
        [SerializeField]
        private AudioRolloffMode _rolloffMode;

#if ODIN_INSPECTOR
        [TabGroup("Data", "Advanced")]
        [BoxGroup("Data/Advanced/3D")]
#endif
        [SerializeField]
        private FloatReference _maxDistance = new FloatReference(-1);

        private static readonly Dictionary<SOundyAsset, float> ClipLastTimePlayed =
            new Dictionary<SOundyAsset, float>();

        private static int _counter;

        public async void Play()
        {
            if (!CanPlay())
            {
                return;
            }

            for (int i = 0; i < _playTimeCount; i++)
            {
                _counter++;

                GameObject audioSourceGameObject = new GameObject($"Sound{_counter}");

                AudioSource audioSource = audioSourceGameObject.AddComponent<AudioSource>();
                AudioClip clip = _clips.RandomElement();
                audioSource.clip = clip;

                SetAudioParameters(audioSource);

                if (!_loop)
                {
                    Destroy(audioSource.gameObject, audioSource.clip.length);
                }

                if (_delay.Value > 0)
                {
                    audioSource.PlayDelayed(_delay.Value);
                }
                else
                {
                    audioSource.Play();
                }

                await UniTask.Delay(TimeSpan.FromSeconds(clip.length));
            }
        }

        private bool CanPlay()
        {
            if (_clips.IsNullOrEmpty())
            {
                return false;
            }

            if (ClipLastTimePlayed.TryGetValue(this, out float lastTimePlayed))
            {
                if (!CheckHasReachedDelay(lastTimePlayed, _delayBetweenPlays.Value))
                {
                    return false;
                }

                UpdateLastTimePlayed();
            }
            else if (_delayBetweenPlays > 0)
            {
                UpdateLastTimePlayed();
            }

            return true;
        }

        private void UpdateLastTimePlayed()
        {
            ClipLastTimePlayed[this] = Time.time;
        }

        private static bool CheckHasReachedDelay(float lastTimePlayed, float delayBetweenPlays)
        {
            return lastTimePlayed + delayBetweenPlays < Time.time;
        }

        private void SetAudioParameters(AudioSource audioSource)
        {
            if (_audioMixerGroup != null)
            {
                audioSource.outputAudioMixerGroup = _audioMixerGroup;
            }

            if (_maxDistance > 0)
            {
                audioSource.maxDistance = _maxDistance;
            }

            audioSource.pitch = _pitchRange.Value.Random();

            if (_position != null)
            {
                audioSource.gameObject.transform.position = _position.Value;
            }

            audioSource.rolloffMode = _rolloffMode;

            audioSource.volume = _volumeRange.Value.Random();

            audioSource.loop = _loop;
        }
    }
}