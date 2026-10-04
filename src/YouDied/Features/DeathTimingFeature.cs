using UnityEngine;
using YouDied.Infrastructure;

namespace YouDied.Features
{
    public sealed class DeathTimingFeature : IFeature
    {
        private readonly IGameEvents _events;
        private float _diedAt = float.NegativeInfinity;

        public DeathTimingFeature(IGameEvents events)
        {
            _events = events;
        }

        public void Enable()
        {
            _events.RespawnRequested += OnRespawnRequested;
            _events.LocalPlayerDied += OnLocalPlayerDied;
            _events.BlackScreenUpdating += OnBlackScreenUpdating;
        }

        public void Disable()
        {
            _events.RespawnRequested -= OnRespawnRequested;
            _events.LocalPlayerDied -= OnLocalPlayerDied;
            _events.BlackScreenUpdating -= OnBlackScreenUpdating;
        }

        private void OnRespawnRequested(ref float delay, bool afterDeath)
        {
            if (!afterDeath)
            {
                return;
            }

            delay = DeathTimeline.RespawnDelaySeconds;
            Game.instance.m_fadeTimeDeath = DeathTimeline.BlackFadeSeconds;
        }

        private void OnLocalPlayerDied()
        {
            _diedAt = Time.unscaledTime;
        }

        private void OnBlackScreenUpdating(Player player, ref bool skip)
        {
            skip = IsBeforeBlackFade(player);
        }

        private bool IsBeforeBlackFade(Player player)
        {
            return player != null
                && player.IsDead()
                && Time.unscaledTime - _diedAt < DeathTimeline.BlackFadeStartSeconds;
        }
    }
}
