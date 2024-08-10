::  MIT license
::  Copyright (C) 2024 Intel Corporation
::  SPDX-License-Identifier: MIT


IF not exist build (mkdir build)
cd build
if %errorlevel% neq 0 goto ERROR

@call "C:\Program Files (x86)\Intel\oneAPI\setvars.bat" intel64 --force
if %errorlevel% neq 0 goto ERROR

cmake -G "Ninja" ..  -DGGML_SYCL=ON -DCMAKE_C_COMPILER=cl -DCMAKE_CXX_COMPILER=icx -DSD_BUILD_SHARED_LIBS=ON
if %errorlevel% neq 0 goto ERROR

cmake --build . -j --config Release
if %errorlevel% neq 0 goto ERROR

cd ..
exit /B 0

:ERROR
echo comomand error: %errorlevel%
exit /B %errorlevel%
