#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 55343
EXPOSE 8081
EXPOSE 55344

ENV TREBLLE_API_KEY=
ENV TREBLLE_PROJECT_ID=

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ToDoAPI/ToDoAPI.csproj", "ToDoAPI/"]
RUN dotnet restore "./ToDoAPI/ToDoAPI.csproj"
COPY . .
WORKDIR "/src/ToDoAPI"
RUN dotnet build "./ToDoAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ToDoAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoAPI.dll"]