using System;
using System.Numerics;

namespace Demonixis.InMoov.Navigation
{
    public class NavigationService : RobotService
    {
        public event Action MovementStarted;
        public event Action MovementCompleted;
    
        public virtual void Move(Vector3 location)
        {
        }
    }
}