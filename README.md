# projecto antecedentes web

# Proyecto en .NET Core 7

Este es un proyecto de desarrollado en .NET Core 7.


## Clonación del repositorio

Primero, clona el repositorio a tu máquina local usando Git:

```bash
git clone https://gitlab.com/uti_anc/cade-anc/anc-antecedentes-web.git

```

# Configure la conexion a la Api en el archivo  appsetting.json dentro del proyecto ANC.Antecedentes-web
```bash

# dev "ConnectionStrings": {
    "WEBAPIERP_URL":"https://localhost:7182/api/",
    "WEBAPI_TOKEN": "400DAE57C072C15D1184332F4EA54AB5529E003B8B5CD7772B247DC427D98628"
  },
```

# Configure conexiones a BD en archivo appsetting.json dentro del proyecto ANC.Antecedentes-api

```bash
# dev ConnectionStrings

"ConnectionStrings": {
  "LINQ_SYBASE_USIS": "dsn=cn_desa_usis;uid=aqueque;pwd=123456",
  "LINQ_SYBASE_ZAV": "dsn=cn_desa_zav;uid=user_web_dj;pwd=123456",
  "LINQ_PG": "Host=172.19.100.64;Port=5432;Database=db_anc;Username=postgres;Password=9465296Se."
}

```
#C

# Run project

- Run in Development environment:

```bash
# dev -> Development (default)
# pre -> Staging
# prod -> Production

# dotnet [watch|run] --launch-profile [dev|pre|prod]
dotnet watch
```

# Build to production 🚀

```bash
dotnet publish -c Release
```

