using System.Drawing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ToyRobot.Core;
using ToyRobot.Interface.Movement;

namespace ToyRobot.Test
{
    [TestClass]
    public class MovementProcessorTest
    {
        private Mock<ILogger<MovementProcessor>> Logger { get; } 

        public MovementProcessorTest()
        {
            Logger = new Mock<ILogger<MovementProcessor>>(MockBehavior.Loose);
        }

        [TestMethod]
        public void TestPlaceMethod()
        {
            //Arrange
            var routePlannerMock = new Mock<IRoutePlanner>(MockBehavior.Strict);
            routePlannerMock.Setup(x => x.PositionIsReachable(It.IsAny<Point>())).Returns(true);
            var movementProcessor = new MovementProcessor(routePlannerMock.Object, Logger.Object);
            var movingObjectMock = new Mock<ILocationObject>(MockBehavior.Loose);
            var point = new Point(1, 1);

            //Act
            movementProcessor.Place(movingObjectMock.Object, point, DirectionEnum.East);

            //Assert
            movingObjectMock.VerifySet(x => x.Direction = DirectionEnum.East, Times.Once);
            movingObjectMock.VerifySet(x => x.Position = point, Times.Once);
            routePlannerMock.Verify(x=> x.PositionIsReachable(point), Times.Once);
        }

        //TODO: test other methods
    }
}
