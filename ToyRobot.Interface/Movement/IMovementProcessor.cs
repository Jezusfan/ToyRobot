using System.Drawing;

namespace ToyRobot.Interface.Movement
{
    public interface IMovementProcessor
    {
        /// <summary>
        /// If there is space to do so, moves the object 1 position forward
        /// </summary>
        /// <param name="movingObject"></param>
        /// <returns>True if the movement was successful, False otherwise</returns>
        bool MoveForward(ILocationObject movingObject);

        /// <summary>
        /// Changes the direction of the object 90 degrees to the right
        /// </summary>
        /// <param name="movingObject"></param>
        /// <returns>True if the change was successful, False otherwise</returns>
        bool TurnRight(ILocationObject movingObject);

        /// <summary>
        /// Changes the direction of the object 90 degrees to the left
        /// </summary>
        /// <param name="movingObject"></param>
        /// <returns>True if the change was successful, False otherwise</returns>
        bool TurnLeft(ILocationObject movingObject);

        /// <summary>
        /// After positional validation, places movingObject on the provided position, facing the provided direction
        /// </summary>
        /// <param name="movingObject"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns>True if the position is valid, and False otherwise</returns>
        bool Place(ILocationObject movingObject, Point position, DirectionEnum direction);
    }
}
