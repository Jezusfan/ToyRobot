using System.Drawing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ToyRobot.Core;
using ToyRobot.Interface.Movement;

namespace ToyRobot.Test
{
    [TestClass]
    public class RobotTest
    {
        private Mock<ILogger<Robot>> Logger { get; } 

        public RobotTest()
        {
            Logger = new Mock<ILogger<Robot>>(MockBehavior.Loose);
        }

        [TestMethod]
        public void TestRobotIgnoringCommandsWithoutBeingPlaced()
        {
            //Arrange
            var movementProcessorMock = new Mock<IMovementProcessor>(MockBehavior.Loose);
            var robot = new Robot(Logger.Object, movementProcessorMock.Object);

            //Act
            robot.MoveForward();
            robot.TurnLeft();
            robot.TurnRight();

            //Assert
            Assert.AreEqual(string.Empty, robot.ReportPosition());
            movementProcessorMock.Verify(x => x.MoveForward(It.IsAny<ILocationObject>()), Times.Never);
            movementProcessorMock.Verify(x => x.Place(It.IsAny<ILocationObject>(), It.IsAny<Point>(), It.IsAny<DirectionEnum>()), Times.Never);
            movementProcessorMock.Verify(x => x.TurnLeft(It.IsAny<ILocationObject>()), Times.Never);
            movementProcessorMock.Verify(x => x.TurnRight(It.IsAny<ILocationObject>()), Times.Never);
            
        }

        [TestMethod]
        public void TestRobotExecutingCommandsAfterBeingPlaced()
        {
            //Arrange
            var movementProcessorMock = new Mock<IMovementProcessor>(MockBehavior.Strict);
            movementProcessorMock.Setup(x => x.Place(It.IsAny<ILocationObject>(), It.IsAny<Point>(), It.IsAny<DirectionEnum>())).Returns(true);
            movementProcessorMock.Setup(x => x.MoveForward(It.IsAny<ILocationObject>())).Returns(true);
            movementProcessorMock.Setup(x => x.TurnLeft(It.IsAny<ILocationObject>())).Returns(true);
            movementProcessorMock.Setup(x => x.TurnRight(It.IsAny<ILocationObject>())).Returns(true);
            var robot = new Robot(Logger.Object, movementProcessorMock.Object);

            //Act
            robot.Place(Point.Empty, DirectionEnum.East);
            robot.MoveForward();
            robot.TurnLeft();
            robot.TurnRight();

            //Assert
            Assert.IsFalse(string.IsNullOrEmpty(robot.ReportPosition()));
            Assert.IsTrue(robot.IsPlaced);
            movementProcessorMock.Verify(x => x.Place(It.IsAny<ILocationObject>(), It.IsAny<Point>(), It.IsAny<DirectionEnum>()), Times.Once);
            movementProcessorMock.Verify(x => x.MoveForward(It.IsAny<ILocationObject>()), Times.Once);
            movementProcessorMock.Verify(x => x.TurnLeft(It.IsAny<ILocationObject>()), Times.Once);
            movementProcessorMock.Verify(x => x.TurnRight(It.IsAny<ILocationObject>()), Times.Once);
        }

        //TODO: test individual methods here...
    }
}