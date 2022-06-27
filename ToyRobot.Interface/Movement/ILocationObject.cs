using System.Drawing;

namespace ToyRobot.Interface.Movement
{
    /// <summary>
    /// Interface describing location properties of an object 
    /// </summary>
    public interface ILocationObject
    {
        string Name { get; }
        bool IsPlaced { get; }
        Point Position { get; set; }
        DirectionEnum Direction { get; set; }
    }
}