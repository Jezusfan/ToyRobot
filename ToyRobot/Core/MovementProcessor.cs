using System.Drawing;
using Microsoft.Extensions.Logging;
using ToyRobot.Interface.Movement;

namespace ToyRobot.Core
{
    public class MovementProcessor : IMovementProcessor
    {
        private readonly IRoutePlanner _routePlanner;
        private readonly ILogger<MovementProcessor> _logger;

        public MovementProcessor(IRoutePlanner routePlanner, ILogger<MovementProcessor> logger)
        {
            _routePlanner = routePlanner;
            _logger = logger;
        }

        public bool MoveForward(ILocationObject movingObject)
        {
            try
            {
                var newPosition = GetMovePosition(movingObject.Position, movingObject.Direction);
                if (_routePlanner.PositionIsReachable(newPosition))
                {
                    _logger.LogInformation($"Moving {movingObject.Name} to new position: {newPosition}");
                    movingObject.Position = newPosition;
                    return true;
                }
                _logger.LogWarning($"Boundary reached! Cannot move {movingObject.Name} forward.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Executing command {nameof(MoveForward)} for {movingObject.Name} failed! Reason: {e.Message}");
            }
            return false;
        }

        public bool TurnRight(ILocationObject movingObject)
        {
            try
            {
                int max = Enum.GetNames(typeof(DirectionEnum)).Count();
                int current = (int)movingObject.Direction; //zero-based enum
                current += 1;
                if (current == max)
                    current = 0;
                movingObject.Direction = (DirectionEnum)current;
                _logger.LogInformation($"{movingObject.Name} now faces: {movingObject.Direction}");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Executing command {nameof(TurnRight)} for {movingObject.Name} failed! Reason: {e.Message}");
                return false;
            }
        }

        public bool TurnLeft(ILocationObject movingObject)
        {
            try
            {
                int current = (int)movingObject.Direction; //zero-based enum
                current -= 1;
                if (current < 0)
                    current = Enum.GetNames(typeof(DirectionEnum)).Count() - 1;
                movingObject.Direction = (DirectionEnum)current;
                _logger.LogInformation($"{movingObject.Name} now faces: {movingObject.Direction}");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Executing command {nameof(TurnLeft)} for {movingObject.Name} failed! Reason: {e.Message}");
                return false;
            }
        }

        public bool Place(ILocationObject movingObject, Point position, DirectionEnum direction)
        {
            if (!_routePlanner.PositionIsReachable(position))
            {
                return false;
            }
            else
            {
               movingObject.Position = position;
               movingObject.Direction = direction;
               return true;
            }
        }

        internal Point GetMovePosition(Point position, DirectionEnum direction)
        {
            switch (direction)
            {
                case DirectionEnum.North:
                    return new Point(position.X, position.Y + 1);
                case DirectionEnum.South:
                    return new Point(position.X, position.Y - 1);
                case DirectionEnum.East:
                    return new Point(position.X + 1, position.Y);
                case DirectionEnum.West:
                    return new Point(position.X - 1, position.Y);
                default:
                    throw new NotImplementedException($"Moving {position} has not been implemented!");
            }
        }

    }
}
