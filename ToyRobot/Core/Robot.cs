using System.Drawing;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using ToyRobot.Interface.Movement;

namespace ToyRobot.Core
{
    public class Robot : ILocationObject, IMovableObject
    {
        private readonly ILogger _logger;
        private readonly IMovementProcessor _movementProcessor;
        private bool _placed;

        public Robot(ILogger<Robot> logger, IMovementProcessor movementProcessor)
        {
            _logger = logger;
            _movementProcessor = movementProcessor;
        }

        public string Name
        {
            get { return "Robot"; }
        }

        public bool IsPlaced
        {
            get { return _placed; }
        }

        public Point Position { get; set; }

        public DirectionEnum Direction { get; set; }

        public void MoveForward()
        {
            if (RobotInitialized())
            {
                _movementProcessor.MoveForward(this);
            }
        }

        public void TurnRight()
        {
            if (RobotInitialized())
            {
                _movementProcessor.TurnRight(this);
            }
        }

        public void TurnLeft()
        {
            if (RobotInitialized())
            {
                _movementProcessor.TurnLeft(this);
            }
        }

        public void Place(Point position, DirectionEnum direction)
        {
            string extraLogging = $" with parameters: position={position} direction={direction}";
            LogCommand(extraLogging);
            if (!_movementProcessor.Place(this, position, direction))
            {
                _logger.LogWarning("Ignoring Place command. Position is invalid");
            }
            else
            {
                _placed = true;
            }
        }

        public string ReportPosition()
        {
            if (RobotInitialized())
            {
                string report = $"{Name} Location is {Position}, facing {Direction}";
                _logger.LogInformation($"{report}");
                return report;
            }
            return string.Empty;
        }

        private void LogCommand([CallerMemberName] string commandName = null!, string extendedInformation = null!)
        {
            _logger.LogInformation($"Received command: {commandName}{extendedInformation}");
        }

        private bool RobotInitialized([CallerMemberName] string commandName = null!)
        {
            string ignoreCommand = string.Empty;
            try
            {
                if (!_placed)
                {
                    ignoreCommand = ", but the command will be ignored. Robot has not been placed on Surface yet.";
                    return false;
                }
                return true;
            }
            finally
            {
                LogCommand(commandName, ignoreCommand);
            }
        }
    }
}
