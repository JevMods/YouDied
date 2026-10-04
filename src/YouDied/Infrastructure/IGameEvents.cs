using System;

namespace YouDied.Infrastructure
{
    public interface IGameEvents
    {
        event Action LocalPlayerDied;
        event RespawnRequestedHandler RespawnRequested;
        event BlackScreenUpdatingHandler BlackScreenUpdating;
        event MessageShowingHandler MessageShowing;
    }
}
