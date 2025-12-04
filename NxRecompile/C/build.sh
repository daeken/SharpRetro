#!/bin/bash

clang -dynamiclib -o "$2" "$1" -O3 -DBUILD_LIB -Wno-shift-count-overflow -Wno-unused-value -ferror-limit=100000
