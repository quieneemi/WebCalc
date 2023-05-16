using SmartCalc.Core.Models;

namespace SmartCalc.Core.Interfaces;

public interface ISmartCalcService
{
    double Evaluate(string expression, double xValue = 1);
    Point[] CalcPlotData(string expression, int xMin, int xMax, int yMin, int yMax);
}