FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["InquisitorAI.Api/InquisitorAI.Api.csproj", "InquisitorAI.Api/"]
COPY ["InquisitorAI.Features/InquisitorAI.Features.csproj", "InquisitorAI.Features/"]
COPY ["InquisitorAI.Infrastructure/InquisitorAI.Infrastructure.csproj", "InquisitorAI.Infrastructure/"]
RUN dotnet restore "InquisitorAI.Api/InquisitorAI.Api.csproj"
COPY . .
RUN dotnet publish "InquisitorAI.Api/InquisitorAI.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "InquisitorAI.Api.dll"]
