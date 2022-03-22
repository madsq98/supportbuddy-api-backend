FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
COPY ./bin/Release/net5.0 /app/publish 
CD /app/publish
ENTRYPOINT ["dotnet", "SB.WebAPI.dll"]