namespace SmartCalc.Core.Models;

public struct Point
{
    public double? X { get; set; }
    public double? Y { get; set; }

    public Point(double? x = null, double? y = null) => (X, Y) = (x, y);
}