using System.Drawing;
using ToyRobot.Configuration;
using ToyRobot.Interface.Movement;

namespace ToyRobot.Core
{
    public class RoutePlanner: IRoutePlanner
    {
        private readonly SurfaceConfig _surfaceConfig;

        public RoutePlanner(SurfaceConfig surfaceConfig)
        {
            _surfaceConfig = surfaceConfig;
        }

        public bool PositionIsReachable(Point position)
        {
            if (position.X > _surfaceConfig.MaxX || position.Y > _surfaceConfig.MaxY)
                return false;
            if (position.X < _surfaceConfig.MinX || position.Y < _surfaceConfig.MinY)
                return false;
            return true;
        }
    }
}
