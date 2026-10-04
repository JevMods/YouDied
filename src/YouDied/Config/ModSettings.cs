using BepInEx.Configuration;

namespace YouDied.Config
{
    public sealed class ModSettings : IModSettings
    {
        private readonly ConfigEntry<float> _volume;

        public ModSettings(ConfigFile file)
        {
            _volume = file.Bind(
                "Sound",
                "Volume",
                0.7f,
                new ConfigDescription(
                    "Volume of the death sound, from 0 (silent) to 1 (full).",
                    new AcceptableValueRange<float>(0f, 1f)));
        }

        public float Volume => _volume.Value;
    }
}
