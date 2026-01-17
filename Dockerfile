FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Ae.Blog/Ae.Blog.csproj", "Ae.Blog/"]
RUN dotnet restore "Ae.Blog/Ae.Blog.csproj"
COPY src/ .
WORKDIR "/src/Ae.Blog"
RUN dotnet publish "Ae.Blog.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Ae.Blog.dll"]
