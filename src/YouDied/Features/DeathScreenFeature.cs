using YouDied.Config;
using YouDied.Infrastructure;

namespace YouDied.Features
{
    public sealed class DeathScreenFeature : IFeature
    {
        private const string DeathMessage = "$msg_youdied";

        private static readonly char[] Exclamations = { '!', '！', '¡', ' ' };

        private readonly IGameEvents _events;
        private readonly IModSettings _settings;
        private readonly DeathScreen _screen;

        public DeathScreenFeature(IGameEvents events, IModSettings settings, DeathScreen screen)
        {
            _events = events;
            _settings = settings;
            _screen = screen;
        }

        public void Enable()
        {
            _events.LocalPlayerDied += OnLocalPlayerDied;
            _events.MessageShowing += OnMessageShowing;
        }

        public void Disable()
        {
            _events.LocalPlayerDied -= OnLocalPlayerDied;
            _events.MessageShowing -= OnMessageShowing;
        }

        private void OnLocalPlayerDied()
        {
            var title = Localization.instance.Localize(DeathMessage)
                .Trim(Exclamations)
                .ToUpperInvariant();
            _screen.Play(title, _settings.Volume);
        }

        private void OnMessageShowing(string message, ref bool suppress)
        {
            if (message == DeathMessage)
            {
                suppress = true;
            }
        }
    }
}
