using System.Text.Json;
using SmartCalc.Core.Interfaces;
using SmartCalc.Core.Models;

namespace SmartCalc.Core;

public class HistoryService : IHistoryService
{
    public List<CalcOperation> Read()
    {
        var jsonText = File.ReadAllText("history.json");
        var items = JsonDocument.Parse(jsonText)
            .Deserialize<CalcOperation[]>();

        return items is null ? new List<CalcOperation>() : items.ToList();
    }

    public Task WriteAsync(IEnumerable<CalcOperation> items)
        => File.WriteAllTextAsync("history.json", JsonSerializer.Serialize(items));

    public void AddItem(CalcOperation item)
    {
        var items = Read();
        items.Add(item);
        WriteAsync(items);
    }
}