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
        private ExConsole _exConsole = new ExConsole(); //use nuget package: Extended.Console

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
                    ExecuteNextCommand(cancellationTokenSource);
                }
                catch (Exception e)
                {
                    HandleInvalidUserInput(e);
                }
            }

            return Task.CompletedTask;
        }

        #region Execute commands
        //public, to allow unit tests
        public void PlaceRobot()
        {
            var param = GetPlaceParameters();
            if (param != null)
            {
                _robot.Place(param.Value.position, param.Value.direction);
            }
        }
        
        //public, to allow unit tests
        public void MoveRobotForward()
        {
            _robot.MoveForward();
        }
        
        //public, to allow unit tests
        public void TurnRobotRight()
        {
            _robot.TurnRight();
        }
        
        //public, to allow unit tests
        public void TurnRobotLeft()
        {
            _robot.TurnLeft();
        }
        
        public void ReportRobotStatus()
        {
            var report = _robot.ReportPosition();
            if (string.IsNullOrEmpty(report))
                report = "Robot has not been placed on Surface yet.";
            _exConsole.WriteLine(report);
        }
        
        #endregion

        /// <summary>
        /// Virtual, so the user input can be unit-tested
        /// </summary>
        /// <returns></returns>
        public virtual (Point position, DirectionEnum direction)? GetPlaceParameters()
        {
            try
            {
                var title = $"Please enter the position on the surface in format: X,Y. [Min: {_surfaceConfig.MinX},{_surfaceConfig.MinY} Max: {_surfaceConfig.MaxX},{_surfaceConfig.MaxY}]";
                //first, get a valid position from the user. TODO: support cancelling this input
                var position = _exConsole.ReadUntilConverted(title, "Invalid position entered", ConvertToPoint);
                //iterate over the DirectionEnum and add these to the menu, as well as appending a Cancel=option.
                var directionOptions = Enum.GetNames<DirectionEnum>().Append("Cancel");
                var direction = _exConsole.Menu(new MenuDisplayArgs("Please choose a direction:"), directionOptions.ToArray());
            
                if (direction < directionOptions.Count())
                    return (position, (DirectionEnum)direction);
            }
            catch (Exception e)
            {
                HandleInvalidUserInput(e);
            }
            
            return null;
        }

        //public, to allow unit tests
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
        
        //TODO: unit-testing the console input that the user provides requires a slightly different structure, but we have time-constraints to work with...
        private void ExecuteNextCommand(CancellationTokenSource cancellationTokenSource)
        {
            //each menu-item is linked to a method/Action that will be executed
            _exConsole.Menu(new MenuDisplayArgs("Welcome to ToyRobot console. Please choose your next action:"),
                ("Place Robot on Surface", PlaceRobot),
                ("Move Forward", MoveRobotForward),
                ("Turn Left", TurnRobotLeft),
                ("Turn Right", TurnRobotRight),
                ("Report", ReportRobotStatus),
                ("Quit", () => cancellationTokenSource.Cancel())
            );
        }

        private void HandleInvalidUserInput(Exception e)
        {
            var message = "Error reading user input";
            _exConsole.WriteLine(message);
            _logger.LogError(e, message);
        }

    }
}
