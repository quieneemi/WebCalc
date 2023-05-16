using System.Runtime.InteropServices;

namespace SmartCalc.Core.Models;

[StructLayout(LayoutKind.Sequential)]
public struct Node
{
    public double Operand;
    public Operation Operation;
    public Function Function;
    public Bracket Bracket;
}