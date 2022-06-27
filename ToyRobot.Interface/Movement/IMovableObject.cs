using System.Drawing;

namespace ToyRobot.Interface.Movement
{
    public interface IMovableObject
    {
        void MoveForward();

        void TurnRight();

        void TurnLeft();

        void Place(Point position, DirectionEnum direction);

        string ReportPosition();
    }
}
