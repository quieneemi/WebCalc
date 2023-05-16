using System.Collections.Generic;
using System.Threading.Tasks;
using SmartCalc.Core.Models;

namespace SmartCalc.Core.Interfaces;

public interface IHistoryService
{
    public List<CalcOperation> Read();
    public Task WriteAsync(IEnumerable<CalcOperation> items);
    public void AddItem(CalcOperation item);
}