# Goal
It's very difficult to build an application that is able to update itself. Usually it takes a lot of effort to make an application self-updating in a reliable and secure way. The main goal of this project is to create a library that will support creating self-updating applications. It will make the process stright-forward.

# Assumptions
* The latest application in deployed to Azure Blob Storage
* Application stub
  * Will get the latest version of the real application from Azure Blob Storage
  * Will cache it on local drive (AppData)
  * Will use interface implemented in the real application to call it
  * Need to know Azure Blob Storage URL
  * Need to reference [AutoUpdate.Core](https://www.nuget.org/packages/AutoUpdate.Core/) NuGet package
* The real application
  * Need to implement interface (from [AutoUpdate.Common](https://github.com/csharp-today/AutoUpdate.Common))
  * Need to be deployed to Azure Blob Storage

# NuGet packages
## AutoUpdate.Core
It's the Core of auto-updating application. It has to be referenced by application stub and fed with Azure Blob Storage URL.
NuGet: https://www.nuget.org/packages/AutoUpdate.Core/
## AutpUpdate.Common
Set of interfaces. One have to be implemented so application stub will know how to call it.
NuGet: https://www.nuget.org/packages/AutoUpdate.Common/
