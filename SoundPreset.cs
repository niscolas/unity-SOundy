using System;
using System.Collections.Generic;
using AudioUtils;
using Cysharp.Threading.Tasks;
using niscolas.UnityUtils.Core.Extensions;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace SoundPreset
{
	[CreateAssetMenu(
		menuName = "Sound Preset", 
		order = Constants.CreateAssetMenuOrder)]
	public class SoundPreset : ScriptableObject
	{
		[Title("Sound Preset")]
		[InlineEditor(InlineEditorModes.SmallPreview)]
		[PropertySpace(SpaceAfter = 15)]
		[SerializeField]
		private AudioClip[] clips;

		[TabGroup("Data", "Basic")]
		[SerializeField]
		private AudioMixerGroup audioMixerGroup;

		[TabGroup("Data", "Basic")]
		[MinMaxSlider(0.1f, 2, true)]
		[SerializeField]
		private Vector2 volume = Vector2.one;

		[TabGroup("Data", "Advanced")]
		[MinMaxSlider(0.1f, 3, true)]
		[SerializeField]
		private Vector2 pitch = Vector2.one;

		[TabGroup("Data", "Advanced")]
		[BoxGroup("Data/Advanced/Time Settings")]
		[SerializeField]
		private bool loop;

		[TabGroup("Data", "Advanced")]
		[BoxGroup("Data/Advanced/Time Settings")]
		[SerializeField]
		private int playTimes = 1;

		[TabGroup("Data", "Advanced")]
		[BoxGroup("Data/Advanced/Time Settings")]
		[FormerlySerializedAs("delayTime")]
		[SerializeField]
		private FloatReference delay;

		[TabGroup("Data", "Advanced")]
		[BoxGroup("Data/Advanced/Time Settings")]
		[SerializeField]
		private FloatReference delayBetweenPlays;

		[TabGroup("Data", "Advanced")]
		[BoxGroup("Data/Advanced/3D")]
		[SerializeField]
		private Vector3Reference position;

		[TabGroup("Data", "Advanced")]
		[BoxGroup("Data/Advanced/3D")]
		[SerializeField]
		private AudioRolloffMode rolloffMode;

		[TabGroup("Data", "Advanced")]
		[BoxGroup("Data/Advanced/3D")]
		[SerializeField]
		private float maxDistance = -1;

		private static readonly Dictionary<SoundPreset, float> ClipLastTimePlayed = new Dictionary<SoundPreset, float>();

		private static int _counter;

		public async void Play()
		{
			if (!CanPlay())
			{
				return;
			}

			for (int i = 0; i < playTimes; i++)
			{
				_counter++;

				GameObject audioSourceGameObject = new GameObject($"Sound{_counter}");

				AudioSource audioSource = audioSourceGameObject.AddComponent<AudioSource>();
				AudioClip clip = clips.RandomElement();
				audioSource.clip = clip;

				SetAudioParameters(audioSource);

				if (!loop)
				{
					Destroy(audioSource.gameObject, audioSource.clip.length);
				}

				if (delay.Value > 0)
				{
					audioSource.PlayDelayed(delay.Value);
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
			if (clips.IsNullOrEmpty())
			{
				return false;
			}

			if (ClipLastTimePlayed.TryGetValue(this, out float lastTimePlayed))
			{
				if (!CheckHasReachedDelay(lastTimePlayed, delayBetweenPlays.Value))
				{
					return false;
				}

				UpdateLastTimePlayed();
			}
			else if (delayBetweenPlays > 0)
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
			if (audioMixerGroup != null)
			{
				audioSource.outputAudioMixerGroup = audioMixerGroup;
			}

			if (maxDistance > 0)
			{
				audioSource.maxDistance = maxDistance;
			}

			audioSource.pitch = pitch.Random();

			if (position != null)
			{
				audioSource.gameObject.transform.position = position.Value;
			}

			audioSource.rolloffMode = rolloffMode;

			audioSource.volume = volume.Random();

			audioSource.loop = loop;
		}
	}
}