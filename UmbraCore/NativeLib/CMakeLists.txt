cmake_minimum_required(VERSION 4.1)
project(NativeLib)

set(CMAKE_CXX_STANDARD 20)

add_library(NativeLib SHARED library.cpp
        nv.cpp
        nv.h
        vi.cpp
        vi.h
        nvn.h)
