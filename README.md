# Personal Website Project 😌
This repository contains the source code for the site I'm creating as a side-project!
## Goals
- Create a fully responsive website (PC, mobile, tablet, ...) with pretty styles, animations and colors!
- Create and expand a wide repertoire of "**Applets**"[^1].
- Build and maintain a well-documented and modular code base that is **reusable** and **parameterized**.
- Constantly improve on UX and responsiveness.
- Document the code base with diagrams and pictures for easier understanding.

## Philosophy
My approach to developing this project is to first of all write code that scales and is reusable. 
Secondly, the code needs to be easily understood and well-documented for everyone's sake. 
Of course all this would be boring without having ideas for applets and designs! 

## Technologies
- [ASP.NET Core](https://dotnet.microsoft.com/en-us/learn/aspnet/what-is-aspnet-core) (using .NET 8)
- [Blazor Web App](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) (Project Structure)
- [PostgreSQL](https://www.postgresql.org/) (for DB and Data Access)
- [SignalR](https://dotnet.microsoft.com/en-us/apps/aspnet/signalr) (for Client-Server communication)
- [Azure](https://portal.azure.com/) (for publishing the site on the web)

## Caveats
- If you want to add styles to the application, you can use the /Styles/app.scss file. You will however need a SASS compiler that can convert the file in a pure .css file. Else your style additions will **not** be registered. Click [here](https://marketplace.visualstudio.com/items?itemName=Failwyn.WebCompiler64) to download the SASS compiler that I use.

[^1]: An **applet** is a small 'application' (not really) that runs inside the server. They are simply Razor Components that have the @page directive and are tagged with the attribute [Utilities/AppletAttribute.cs](https://github.com/Noveboi/Nove-Site/blob/38711cddaa8b58148c4c33dbcf667d1e75a0ce77/Utilities/AppletAttribute.cs)
