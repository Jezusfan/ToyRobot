using System.Drawing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ToyRobot.Configuration;
using ToyRobot.Console;
using ToyRobot.Core;
using ToyRobot.Interface.Movement;

namespace ToyRobot.Test
{
    
    [TestClass]
    public class ConsoleInputProviderTest
    {
        
        Mock<ILogger<ConsoleInputProvider>> _logger = new(MockBehavior.Loose);//don't consider this constructor notation very readable, tbh
        private SurfaceConfig _surfaceConfig;

        public ConsoleInputProviderTest()
        {
            //shared config between all test methods
            _surfaceConfig = new SurfaceConfig(){MaxX = 5,MaxY = 5,MinX = 0,MinY = 0};
        }

        [TestMethod]
        public void TestPlaceMethod()
        {
            //Arrange
            var robot = new Mock<IMovableObject>(MockBehavior.Loose);
            var console = new Mock<ConsoleInputProvider>(_logger.Object, robot.Object, _surfaceConfig);
            
            //NOTE: we mock user input here..!
            var point = new Point(1, 1);
            console.Setup(x => x.GetPlaceParameters()).Returns((point, DirectionEnum.South));
            
            //Act
            console.Object.PlaceRobot();

            //Assert
            robot.Verify(x => x.Place(point, DirectionEnum.South), Times.Once);
        }

        //TODO: test other input methods...
    }
}
