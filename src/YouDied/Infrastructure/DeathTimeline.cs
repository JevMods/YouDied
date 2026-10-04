using UnityEngine;

namespace YouDied.Infrastructure
{
    internal static class DeathTimeline
    {
        public const float RespawnDelaySeconds = 5.2f;
        public const float TitleStartSeconds = 0.5f;
        public const float BlackFadeStartSeconds = 3.95f;
        public const float BlackFadeSeconds = 1f;

        private const float FadeInSeconds = 0.7f;
        private const float HoldEndSeconds = 2.85f;
        private const float FadeOutSeconds = 0.7f;
        private const float StartScale = 0.94f;
        private const float EndScale = 1.06f;

        public static float Visibility(float time)
        {
            var fadeIn = Mathf.SmoothStep(0f, 1f, (time - TitleStartSeconds) / FadeInSeconds);
            var fadeOut = Mathf.SmoothStep(0f, 1f, (time - HoldEndSeconds) / FadeOutSeconds);
            return fadeIn * (1f - fadeOut);
        }

        public static float TitleScale(float time)
        {
            var shownSeconds = HoldEndSeconds + FadeOutSeconds - TitleStartSeconds;
            var shown = (time - TitleStartSeconds) / shownSeconds;
            return Mathf.Lerp(StartScale, EndScale, shown);
        }
    }
}
