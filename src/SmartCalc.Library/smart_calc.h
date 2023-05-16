#ifndef SMART_CALC_H
#define SMART_CALC_H

#ifdef _WIN32
	#define EXPORT extern "C" __declspec(dllexport)
#else
	#define EXPORT extern "C"
#endif

#include <stack>

#include "node.h"

EXPORT double Calc(Node nodes[], int size);

std::stack<Node> ReverseStack(std::stack<Node> stack);
double CalcReversePolishNotation(std::stack<Node> nodes);
void CalcTemp(std::stack<Node>* temp, std::stack<Node>* result);
double CalcOperation(double a, double b, Operation);
double CalcFunction(double x, Function);
Type GetType(const Node& node);
Priority GetPriority(const Node& node, const Type& type);

#endif // !SMART_CALC_H
