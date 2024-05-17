![rskcnv Client Badge](https://img.shields.io/badge/rskcnv_CS<cli>-8A2BE2)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/risknu/rskcnvcs/dotnet.yml)
![GitHub License](https://img.shields.io/github/license/risknu/rskcnvcs)
![GitHub repo size](https://img.shields.io/github/repo-size/risknu/rskcnvcs)

# Universal Online Sandbox
This sandbox provides features such as chat and drawing capabilities, as well as custom clients that you can write yourself. It also includes the ability to create your own servers and much more. The project is open source with simple code.
> [!WARNING]  
> This project is still in the development stage, specifically in alpha testing. It is not stable and may crash, creating errors. Please reach out for assistance in the Issues section of the repository.

## Installation/Build
To install this sandbox, you will need .NET 8 installed on MacOS/Linux/Windows. Make sure to have `git` installed as well.
```sh
$ dotnet build # for build
$ dotnet run # for run
```
> [!IMPORTANT]  
> You must have `.NET` version 8 or higher, as well as a device running `MacOS`/`Linux`/`Windows`.

To start the server on your device, you need the `~/rskcnvcs/networking` folder in the main project directory where you should run the following commands:
```sh
$ source net_configure.sh && python3 server.py -ipvf 127.0.0.1 -p 2425 # creates necessary directories and files, and starts the server

# net_configure manually
$ mkdir ./logs/ && python3 --version # creates directories and checks the Python version (make sure it's above 3.11 or 3.11)
$ mkdir ./logs/; python --version # for Windows
```

By following these steps, you can set up everything locally and test or modify the code within the license terms.

## Contribution to the Project
I (the developer) welcome your contribution to the project. Any help is appreciated in creating and improving the project. However, I recommend familiarizing yourself with the `CONTRIBUTING.md` file first.

## License
This project is licensed under the GPL version 3.
