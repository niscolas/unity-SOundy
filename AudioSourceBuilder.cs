using Plugins.ClassExtensions.UnityExtensions;
using UnityEngine;
using UnityEngine.Audio;

namespace Plugins.AudioUtils {
	public class AudioSourceBuilder {
		private static int _counter;

		private AudioClip clip;
		private AudioMixerGroup mixerGroup;
		private AudioRolloffMode? rolloffMode;
		private GameObject gameObject;
		private SoundPreset soundPreset;
		private Vector3? position;
		private bool withAutoPlay;
		private bool withAutoDestroy;
		private bool withLoop;
		private float delay = -1;
		private float maxDistance = -1;
		private float pitch = -1;
		private float volume = -1;

		public AudioSourceBuilder FromPreset(SoundPreset pSoundPreset) {
			soundPreset = pSoundPreset;
			return this;
		}

		public AudioSourceBuilder OnGameObject(GameObject pGameObject) {
			gameObject = pGameObject;
			return this;
		}

		public AudioSourceBuilder WithClip(AudioClip pClip) {
			clip = pClip;
			return this;
		}

		public AudioSourceBuilder WithAudioMixerGroup(AudioMixerGroup pMixerGroup) {
			mixerGroup = pMixerGroup;
			return this;
		}

		public AudioSourceBuilder WithVolume(float pVolume) {
			volume = pVolume;
			return this;
		}

		public AudioSourceBuilder WithPitch(float pPitch) {
			pitch = pPitch;
			return this;
		}

		public AudioSourceBuilder WithPosition(Vector3? pPosition) {
			position = pPosition;
			return this;
		}

		public AudioSourceBuilder WithMaxDistance(float pMaxDistance) {
			maxDistance = pMaxDistance;
			return this;
		}

		public AudioSourceBuilder WithAudioRollofMode(AudioRolloffMode pRolloffMode) {
			rolloffMode = pRolloffMode;
			return this;
		}

		public AudioSourceBuilder WithAutoPlay() {
			withAutoPlay = true;
			return this;
		}

		public AudioSourceBuilder WithAutoDestroy() {
			withAutoDestroy = true;
			return this;
		}

		public AudioSourceBuilder WithLoop() {
			withLoop = true;
			return this;
		}

		public AudioSourceBuilder WithDelay(float pDelay) {
			delay = pDelay;
			return this;
		}

		public AudioSource Build() {
			if (soundPreset != null) {
				clip = soundPreset.Clip;
				volume = soundPreset.Volume.Random();
				pitch = soundPreset.Pitch.Random();
				delay = soundPreset.DelayTime;
				mixerGroup = soundPreset.AudioMixerGroup;
				rolloffMode = soundPreset.RolloffMode;
				position = soundPreset.Position;
				maxDistance = soundPreset.MaxDistance;
				withLoop = soundPreset.Loop;
			}

			if (!clip) {
				return null;
			}

			if (!gameObject) {
				gameObject = new GameObject($"Sound{_counter}");
			}

			_counter++;

			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.clip = clip;

			SetAudioParameters(audioSource);

			if (withAutoDestroy && !withLoop) {
				Object.Destroy(audioSource.gameObject, audioSource.clip.length);
			}

			Play(audioSource);

			return audioSource;
		}

		private void SetAudioParameters(AudioSource audioSource) {
			if (mixerGroup != null) {
				audioSource.outputAudioMixerGroup = mixerGroup;
			}

			if (maxDistance > 0) {
				audioSource.maxDistance = maxDistance;
			}

			audioSource.pitch = pitch;

			if (position != null) {
				gameObject.transform.position = position.Value;
			}

			if (rolloffMode != null) {
				audioSource.rolloffMode = rolloffMode.Value;
			}

			audioSource.volume = volume;

			if (withLoop) {
				audioSource.loop = true;
			}
		}

		private void Play(AudioSource audioSource) {
			if (!withAutoPlay) {
				return;
			}

			if (delay > 0) {
				audioSource.PlayDelayed(delay);
			}
			else {
				audioSource.Play();
			}
		}
	}
}