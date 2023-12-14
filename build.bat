if not exist stable-diffusion.cpp-build (
    git clone https://github.com/seasonjs/stable-diffusion.cpp-build
)

cd stable-diffusion.cpp-build
git checkout 4b95d98404bbfe91698fd41b0f514656e358163a

if not exist build (
    mkdir build
)

cd build

rem remove -DSD_CUBLAS=ON to disable cuda support
cmake .. -DCMAKE_BUILD_TYPE=Release -DSD_CUBLAS=ON
cmake --build . --config Release

cd ..\..

dotnet publish -c Release -o bin

copy .\stable-diffusion.cpp-build\build\bin\Release\sd-abi.dll .\StableDiffusion.NET\bin\Debug\net8.0\sd-abi.dll
copy .\stable-diffusion.cpp-build\build\bin\Release\sd-abi.dll .\StableDiffusion.NET\bin\Debug\net8.0\sd-abi.dll
copy .\stable-diffusion.cpp-build\build\bin\Release\sd-abi.dll .\StableDiffusion.NET\bin\Debug\net8.0\sd-abi.dll

pause