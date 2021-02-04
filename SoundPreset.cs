using System;
using System.Collections.Generic;
using Assets.Plugins.AudioUtils;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Plugins.ClassExtensions.UnityExtensions;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Plugins.AudioUtils {
	[CreateAssetMenu(
		menuName = "Sound Preset", order = Constants.CreateAssetMenuOrder)]
	public class SoundPreset : ScriptableObject {
		[SerializeField]
		private AudioClip clip;

		[SerializeField]
		private AudioMixerGroup audioMixerGroup;

		[MinMaxSlider(0, 2)]
		[SerializeField]
		private Vector2 volume;

		[MinMaxSlider(0, 3)]
		[SerializeField]
		private Vector2 pitch;

		[SerializeField]
		private int playTimes = 1;

		[SerializeField]
		private Vector3Reference position;

		[FormerlySerializedAs("delayTime")]
		[SerializeField]
		private FloatReference delay;

		[SerializeField]
		private FloatReference delayBetweenPlays;

		[SerializeField]
		private AudioRolloffMode rolloffMode;

		[SerializeField]
		private float maxDistance = -1;

		[SerializeField]
		private bool loop;

		private static readonly Dictionary<SoundPreset, float> ClipLastTimePlayed = new Dictionary<SoundPreset, float>();

		private static int _counter;

		public async void Play() {
			if (!CanPlay()) {
				return;
			}

			for (int i = 0; i < playTimes; i++) {
				_counter++;

				GameObject audioSourceGameObject = new GameObject($"Sound{_counter}");

				AudioSource audioSource = audioSourceGameObject.AddComponent<AudioSource>();
				audioSource.clip = clip;

				SetAudioParameters(audioSource);

				if (!loop) {
					UnityEngine.GameObject.Destroy(audioSource.gameObject, audioSource.clip.length);
				}

				if (delay.Value > 0) {
					audioSource.PlayDelayed(delay.Value);
				}
				else {
					audioSource.Play();
				}

				await UniTask.Delay(TimeSpan.FromSeconds(clip.length));
			}
		}

		private bool CanPlay() {
			if (!clip) {
				return false;
			}

			if (ClipLastTimePlayed.TryGetValue(this, out float lastTimePlayed)) {
				if (!CheckHasReachedDelay(lastTimePlayed, delayBetweenPlays.Value)) {
					return false;
				}

				UpdateLastTimePlayed();
			}
			else if (delayBetweenPlays > 0) {
				UpdateLastTimePlayed();
			}

			return true;
		}

		private void UpdateLastTimePlayed() {
			ClipLastTimePlayed[this] = Time.time;
		}

		private static bool CheckHasReachedDelay(float lastTimePlayed, float delayBetweenPlays) {
			return lastTimePlayed + delayBetweenPlays < Time.time;
		}

		private void SetAudioParameters(AudioSource audioSource) {
			if (audioMixerGroup != null) {
				audioSource.outputAudioMixerGroup = audioMixerGroup;
			}

			if (maxDistance > 0) {
				audioSource.maxDistance = maxDistance;
			}

			audioSource.pitch = pitch.Random();

			if (position != null) {
				audioSource.gameObject.transform.position = position.Value;
			}

			audioSource.rolloffMode = rolloffMode;

			audioSource.volume = volume.Random();

			audioSource.loop = loop;
		}
	}
}