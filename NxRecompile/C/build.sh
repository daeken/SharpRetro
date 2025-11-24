#!/bin/bash

clang -dynamiclib -o "$2" "$1" -DBUILD_LIB -Wno-shift-count-overflow -Wno-unused-value
