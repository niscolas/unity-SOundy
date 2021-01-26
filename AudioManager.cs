namespace Plugins.AudioUtils {
	public static class AudioManager {
		public static void Play(SoundPreset data) {
			new AudioSourceBuilder()
				.FromPreset(data)
				.WithAutoPlay()
				.WithAutoDestroy()
				.Build();
		}
	}
}