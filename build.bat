if not exist stable-diffusion.cpp-build (
    git clone https://github.com/DarthAffe/stable-diffusion.cpp-build
)

cd stable-diffusion.cpp-build
git fetch
git checkout b518ce72f1ba448f164e58961b1513ccacc95006

if not exist build (
    mkdir build
)

cd build

rem remove -DSD_CUBLAS=ON to disable cuda support
cmake .. -DCMAKE_BUILD_TYPE=Release -DSD_CUBLAS=ON
cmake --build . --config Release

cd ..\..

dotnet publish -c Release -o bin

copy .\stable-diffusion.cpp-build\build\bin\Release\sd-abi.dll .\bin\sd-abi.dll

pause