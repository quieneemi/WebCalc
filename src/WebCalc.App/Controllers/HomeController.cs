using Microsoft.AspNetCore.Mvc;
using SmartCalc.Core.Interfaces;
using SmartCalc.Core.Models;
using Point = SmartCalc.Core.Models.Point;

namespace WebCalc.App.Controllers;

public class HomeController : Controller
{
    private readonly ISmartCalcService _smartCalc;
    private readonly IHistoryService _historyService;

    public HomeController(ISmartCalcService smartCalc, IHistoryService historyService)
    {
        _smartCalc = smartCalc;
        _historyService = historyService;
    }

    public IActionResult Index(string? expression)
    {
        ViewBag.Expression = expression ?? string.Empty;
        return View();
    }

    [HttpPost]
    public IActionResult Calc(string expression, string xValue)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return Ok(string.Empty);

        if (!double.TryParse(xValue, out var x))
            return BadRequest("Incorrect data");

        var result = _smartCalc.Evaluate(expression, x);

        if (!double.IsNormal(result))
            return BadRequest("Something went wrong");

        var operation = new CalcOperation { Expression = expression, Result = result.ToString("G") };
        Task.Run(() => _historyService.AddItem(operation));

        return Ok(result.ToString("G"));
    }

    #region History

    public IActionResult History()
    {
        ViewBag.History = _historyService.Read();
        return View();
    }

    public IActionResult ClearHistory()
    {
        Task.Run(() => _historyService.WriteAsync(new List<CalcOperation>()));
        return RedirectToAction("Index");
    }

    #endregion

    #region Plot

    public IActionResult Plot() => View();

    [HttpPost]
    public Point[] Plot(string expression, int xMin, int xMax, int yMin, int yMax)
        => _smartCalc.CalcPlotData(expression, xMin, xMax, yMin, yMax);

    #endregion

    public IActionResult HelpTopics() => View();
}