using System.Drawing;
using System.Text.RegularExpressions;
using ExtendedConsole;
using Microsoft.Extensions.Logging;
using ToyRobot.Configuration;
using ToyRobot.Interface.Movement;

namespace ToyRobot.Console
{
    public class ConsoleInputProvider
    {
        private readonly ILogger<ConsoleInputProvider> _logger;
        private readonly SurfaceConfig _surfaceConfig;
        private IMovableObject _robot;
        private ExConsole _exConsole = new ExConsole(); 

        public ConsoleInputProvider(ILogger<ConsoleInputProvider> logger, IMovableObject robot, SurfaceConfig surfaceConfig)
        {
            _logger = logger;
            _robot = robot;
            _surfaceConfig = surfaceConfig;
        }

        public Task RunConsoleAsync(CancellationTokenSource cancellationTokenSource)
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    _exConsole.Menu(
                        new MenuDisplayArgs("Welcome to ToyRobot console. Please choose your next action:"),
                        ("Place Robot on Surface", PlaceRobot),
                        ("Move Forward", MoveRobotForward),
                        ("Turn Left", TurnRobotLeft),
                        ("Turn Right", TurnRobotRight),
                        ("Report", ReportRobotStatus),
                        ("Quit", () => cancellationTokenSource.Cancel())
                    );
                }
                catch (Exception e)
                {
                    var message = "Error reading user input";
                    _exConsole.WriteLine(message);
                    _logger.LogError(e, message);
                }
            }

            return Task.CompletedTask;
        }
        
        public void ReportRobotStatus()
        {
            var report = _robot.ReportPosition();
            if (string.IsNullOrEmpty(report))
                report = "Robot has not been placed on Surface yet.";
            _exConsole.WriteLine(report);
        }

        public void TurnRobotRight()
        {
            _robot.TurnRight();
        }

        public void TurnRobotLeft()
        {
            _robot.TurnLeft();
        }

        public virtual (Point position, DirectionEnum direction)? GetPlaceParameters()
        {
            var title = $"Please enter the position on the surface in format: X,Y. [Min: {_surfaceConfig.MinX},{_surfaceConfig.MinY} Max: {_surfaceConfig.MaxX},{_surfaceConfig.MaxY}]";
            var position = _exConsole.ReadUntilConverted(title, "Invalid position entered", ConvertToPoint);
            var directionOptions = Enum.GetNames<DirectionEnum>();
            var direction = _exConsole.Menu(
                new MenuDisplayArgs("Please choose a direction:"), directionOptions.Append("Cancel").ToArray());
            if (direction < directionOptions.Count())
                return (position, (DirectionEnum)direction);
            return null;
        }

        public void PlaceRobot()
        {
            var param = GetPlaceParameters();
            if (param != null)
            {
                _robot.Place(param.Value.position, param.Value.direction);
            }
        }

        public (bool Success, Point Value) ConvertToPoint(string arg)
        {
            if (Regex.IsMatch(arg, @"^\d+,\d+\z"))
            {
                var ss = arg.Split(',');
                var point = new Point(int.Parse(ss[0]), int.Parse(ss[1]));

                return new(true, point);
            }
            return new(false, new Point());
        }

        public void MoveRobotForward()
        {
            _robot.MoveForward();
        }
    }
}
