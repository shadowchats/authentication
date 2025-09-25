# Shadowchats — Copyright (C) 2025
# Dorovskoy Alexey Vasilievich (One290 / 0ne290) <lenya.dorovskoy@mail.ru>
# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU Affero General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version. See the LICENSE file for details.
# For full copyright and authorship information, see the COPYRIGHT file.

# -----------------------------
# Stage 1: Build
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем файл решения и файлы проектов
COPY Shadowchats.Authentication.sln ./
COPY Shadowchats.Authentication.Core/Shadowchats.Authentication.Core.csproj ./Shadowchats.Authentication.Core/
COPY Shadowchats.Authentication.Infrastructure/Shadowchats.Authentication.Infrastructure.csproj ./Shadowchats.Authentication.Infrastructure/
COPY Shadowchats.Authentication.Presentation/Shadowchats.Authentication.Presentation.csproj ./Shadowchats.Authentication.Presentation/

# Восстановление зависимостей по solution
RUN dotnet restore Shadowchats.Authentication.sln

# Копируем всё остальное
COPY . .

# Публикация конкретного проекта
RUN dotnet publish Shadowchats.Authentication.Presentation/Shadowchats.Authentication.Presentation.csproj -c Release -o /app/publish

# -----------------------------
# Stage 2: Runtime
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true

EXPOSE 5000

ENTRYPOINT ["dotnet", "Shadowchats.Authentication.Presentation.dll"]

