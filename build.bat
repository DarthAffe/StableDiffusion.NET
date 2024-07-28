if not exist stable-diffusion.cpp (
    git clone --recursive https://github.com/leejet/stable-diffusion.cpp
)

cd stable-diffusion.cpp
git fetch
git checkout 73c2176648898b3a223c581ed138e5593f8fded5
git submodule init
git submodule update

if not exist build (
    mkdir build
)

cd build

Rem ----------------------------------------------------------------------------
rem Pick one of the builds below.

rem # cuda12 #
cmake .. -DSD_CUBLAS=ON -DSD_BUILD_SHARED_LIBS=ON -DSD_BUILD_EXAMPLES=OFF

rem # rocm5.5 #
rem cmake .. -G Ninja -DCMAKE_C_COMPILER=clang -DCMAKE_CXX_COMPILER=clang++ -DSD_HIPBLAS=ON -DCMAKE_BUILD_TYPE=Release -DAMDGPU_TARGETS="gfx1100;gfx1102;gfx1030" -DSD_BUILD_SHARED_LIBS=ON -DSD_BUILD_EXAMPLES=OFF

rem # avx512 #
rem cmake .. -DGGML_AVX512=ON -DSD_BUILD_SHARED_LIBS=ON -DSD_BUILD_EXAMPLES=OFF

rem # avx2 #
rem cmake .. -DGGML_AVX2=ON -DSD_BUILD_SHARED_LIBS=ON -DSD_BUILD_EXAMPLES=OFF

rem # avx #
rem cmake .. -DGGML_AVX2=OFF -DSD_BUILD_SHARED_LIBS=ON -DSD_BUILD_EXAMPLES=OFF

rem # noavx #
rem cmake .. -DGGML_AVX=OFF -DGGML_AVX2=OFF -DGGML_FMA=OFF -DSD_BUILD_SHARED_LIBS=ON -DSD_BUILD_EXAMPLES=OFF

Rem ----------------------------------------------------------------------------

cmake --build . --config Release

cd ..\..

dotnet publish -c Release -o bin

copy .\stable-diffusion.cpp\build\bin\Release\*.dll .\bin\

pause
