First, disable MSIX Packaging in Lamina Properties in Visual Studio.

Export using this code :

dotnet publish -c Release -r win-x64 --self-contained true