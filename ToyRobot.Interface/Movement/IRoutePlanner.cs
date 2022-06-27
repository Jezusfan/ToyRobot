using System.Drawing;

namespace ToyRobot.Interface.Movement
{
    /// <summary>
    /// Interface to evaluate if a position is reachable
    /// </summary>
    public interface IRoutePlanner
    {
        /// <summary>
        /// Evaluate if a position is reachable
        /// </summary>
        /// <param name="position"></param>
        /// <returns>True if the position is reachable, False otherwise</returns>
        bool PositionIsReachable(Point position);
    }
}
