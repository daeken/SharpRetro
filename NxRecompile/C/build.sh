#!/bin/bash

clang++ -std=c++2c -dynamiclib -o "$2" "$1" -O3 -DBUILD_LIB -ferror-limit=100000 -Wno-shift-count-overflow
