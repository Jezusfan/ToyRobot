using System.Drawing;

namespace ToyRobot.Interface.Movement
{
    public interface IRoutePlanner
    {
        bool PositionIsReachable(Point position);
    }
}
