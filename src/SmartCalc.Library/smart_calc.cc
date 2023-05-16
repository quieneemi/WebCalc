#include "smart_calc.h"

#include <cmath>
#include <limits>

double Calc(Node nodes[], int size)
{
	std::stack<Node> stack;
	for (int i = 0; i < size; ++i)
		stack.push(nodes[i]);

	if (stack.size() > 1)
		return CalcReversePolishNotation(ReverseStack(stack));
	else
		return stack.top().operand;
}

std::stack<Node> ReverseStack(std::stack<Node> stack)
{
	std::stack<Node> result;

	for (auto i = stack.size(); i > 0; --i)
	{
		result.push(stack.top());
		stack.pop();
	}

	return result;
}

double CalcReversePolishNotation(std::stack<Node> nodes)
{
	std::stack<Node> temp, result;

	auto size = nodes.size();
	for (auto i = 0; i < size; ++i)
	{
		Node node = nodes.top();
		nodes.pop();

		Type node_type = GetType(node);
		if (node_type == Type::Operand)
		{
			result.push(node);
		}
		else if (node_type == Type::Operation || node_type == Type::Function)
		{
			if (temp.size() > 0)
			{
				Priority node_priority = GetPriority(node, node_type);
				Priority temp_node_priority = GetPriority(temp.top(), GetType(temp.top()));
				if (node_priority <= temp_node_priority)
					CalcTemp(&temp, &result);
			}

			temp.push(node);
		}
		else  // bracket
		{
			if (node.bracket == Bracket::OpenBracket)
			{
				temp.push(node);
			}
			else  // close bracket
			{
				while (temp.top().bracket != Bracket::OpenBracket)
					CalcTemp(&temp, &result);

				temp.pop();
			}
		}
	}

	while (temp.size() > 0)
		CalcTemp(&temp, &result);

	return result.top().operand;
}

void CalcTemp(std::stack<Node>* temp, std::stack<Node>* result)
{
	double a = result->top().operand;
	result->pop();

	Node op_node = temp->top();
	temp->pop();

	Node operand_node = { 0, Operation::None, Function::None, Bracket::None };

	if (GetPriority(op_node, GetType(op_node)) == Priority::Function)
	{
		operand_node.operand = CalcFunction(a, op_node.function);
	}
	else
	{
		double b = result->top().operand;
		result->pop();

		operand_node.operand = CalcOperation(b, a, op_node.operation);
	}

	result->push(operand_node);
}

double CalcOperation(double a, double b, Operation operation)
{
	switch (operation)
	{
	case Operation::Add:
		return a + b;
	case Operation::Sub:
		return a - b;
	case Operation::Mul:
		return a * b;
	case Operation::Div:
		return a / b;
	case Operation::Mod:
		return fmod(a, b);
	case Operation::Pow:
		return pow(a, b);
	default:
		return std::numeric_limits<double>::quiet_NaN();
	}
}

double CalcFunction(double x, Function function)
{
	switch (function)
	{
	case Function::Cos:
		return cos(x);
	case Function::Acos:
		return acos(x);
	case Function::Sin:
		return sin(x);
	case Function::Asin:
		return asin(x);
	case Function::Tan:
		return tan(x);
	case Function::Atan:
		return atan(x);
	case Function::Log:
		return log(x);
	case Function::Log10:
		return log10(x);
	case Function::Sqrt:
		return sqrt(x);
	default:
		return std::numeric_limits<double>::quiet_NaN();
	}
}

Type GetType(const Node& node)
{
	if (node.operation != Operation::None)
		return Type::Operation;

	if (node.function != Function::None)
		return Type::Function;

	if (node.bracket != Bracket::None)
		return Type::Bracket;

	return Type::Operand;
}

Priority GetPriority(const Node& node, const Type& type)
{
	if (type == Type::Operation)
	{
		if (node.operation == Operation::Add ||
			node.operation == Operation::Sub)
		{
            return Priority::AddSub;
		}

		if (node.operation == Operation::Mul ||
			node.operation == Operation::Div ||
			node.operation == Operation::Mod)
		{
            return Priority::MulDivMod;
        }

		return Priority::Pow;
	}

	if (type == Type::Function)
	{
		return Priority::Function;
	}

	return Priority::None;
}
