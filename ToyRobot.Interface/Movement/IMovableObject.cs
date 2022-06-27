using System.Drawing;

namespace ToyRobot.Interface.Movement
{
    /// <summary>
    /// Interface describing commands that can be given to a movable object 
    /// </summary>
    public interface IMovableObject
    {
        /// <summary>
        /// Instruct the object to move one position forward
        /// </summary>
        void MoveForward();

        /// <summary>
        /// Instruct the object to turn 90 degrees to the right
        /// </summary>
        void TurnRight();

        /// <summary>
        /// Instruct the object to turn 90 degrees to the left
        /// </summary>
        void TurnLeft();

        /// <summary>
        /// Instruct the object that is will be placed on a Surface
        /// </summary>
        /// <param name="position">Location on the Surface</param>
        /// <param name="direction">Direction the movable object will be facing</param>
        void Place(Point position, DirectionEnum direction);

        /// <summary>
        /// Returns the position and direction as human-readable text
        /// </summary>
        string ReportPosition();
    }
}
