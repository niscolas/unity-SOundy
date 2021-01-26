using System;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Audio;

namespace Plugins.AudioUtils {
	[Serializable]
	public class SoundPreset {
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
		private Vector3Reference position;

		[SerializeField]
		private FloatReference delayTime;

		[SerializeField]
		private AudioRolloffMode rolloffMode;

		[SerializeField]
		private float maxDistance = -1;

		[SerializeField]
		private bool loop;

		public AudioClip Clip => clip;

		public AudioMixerGroup AudioMixerGroup => audioMixerGroup;

		public Vector2 Volume => volume;

		public Vector2 Pitch => pitch;

		public Vector3Reference Position => position;

		public FloatReference DelayTime => delayTime;

		public AudioRolloffMode RolloffMode => rolloffMode;

		public float MaxDistance => maxDistance;

		public bool Loop => loop;
	}
}