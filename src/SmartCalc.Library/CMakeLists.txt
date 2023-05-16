cmake_minimum_required(VERSION 2.22)

project(SmartCalc.Library)

add_library(smartcalc SHARED "smart_calc.cc")
set_property(TARGET smartcalc PROPERTY CXX_STANDARD 17)