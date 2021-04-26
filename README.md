# MSWindowsService
C# Windows Service



#  Create a Windows service app

https://docs.microsoft.com/en-us/dotnet/framework/windows-services/walkthrough-creating-a-windows-service-application-in-the-component-designer


#  Install and uninstall Windows services

https://docs.microsoft.com/en-us/dotnet/framework/windows-services/how-to-install-and-uninstall-services

Example:
>cd C:\Windows\Microsoft.NET\Framework\v4.0.30319
>installutil C:\Users\jorgesys\source\repos\MSWindowsService\MSWindowsService\bin\Debug\MSWindowsService.exe

#Installing Service.
installutil <yourproject>.exe
  
#Uninstalling Service.
installutil /u <yourproject>.exe
