# Fashion Shop

## Author Nguyen Hoang Phong

### Basic command .NET

Add-Migration

```sh
dotnet ef migrations add InitialCreate --project fashion_shop.Infrastructure --startup-project fashion_shop.API
```

Remove-Migration

```sh
dotnet ef migrations remove  --project fashion_shop.Infrastructure --startup-project fashion_shop.API
```

Database update

```sh
dotnet ef database update --project fashion_shop.Infrastructure --startup-project fashion_shop.API
```

Drop Database

```sh
dotnet ef database drop --project fashion_shop.Infrastructure --startup-project fashion_shop.API
```

Seed User

```sh
dotnet run --project fashion_shop.API -- --seed-user
```

Seed Product

```sh
dotnet run --project fashion_shop.API -- --seed-product
```

## How to run  🎉 🎉 🎉

### Run local

1. Requirement apps

- [.NET 8.0]("https://dotnet.microsoft.com/en-us/download/dotnet/8.0")
- [Redis]("https://redis.io/downloads/")
- [Minio]("https://min.io/download?view=aistor")
- [PostgreSQL]("https://www.postgresql.org/download/")

2. Edit appsettings.json

- Change `DefaultConnection` with your host, port, account and database name
- Change `RedisConnection` with your redis settings

#### Run with Docker (Recommend) 🐳

1. Requirement app

- [Docker]("https://docs.docker.com/desktop/setup/install/mac-install/")

2. Create file `.env` from file `.env.example`

For mac & Linux

```sh
cp .env.example .env 
```

Your port, database name, accoutn(Postgresql) must be `the same` with your `appsetting.Development.json` `API` and `MVC` setting
For examople:
Database Postgresql will have below connection string

```
Host=database;port=5432;username=root;password=root;database=fashion_shop
```

👉 If you run Docker without `API` & `MVC` just comment it in `docker-compose` file
and connection string of localhost will be:

```sh
Host=localhost;port=5432;username=root;password=root;database=fashion_shop
```

3. Run docker - You must stay in root folder

- Run following CLI

```sh
docker compose up -d
```

### Run seed

1. Run local, you just run with above CLI in the new terminal

2. Run docker, access your container and run seed

### Your connection

### Port and database docker

|  No  | Name | Port | Type |
|--- |--- |--- |--- |
| 1   | api | 8123 | .NET API |
| 2   | mvc | 5123 | .NET MVC |
| 3   | database | 5432 | Postgres |
| 4   | redis | 6379 | Redis |
| 5   | minio | 9000,9001 | Minio |

### Config upload image

#### For Docker 🐳

After starting Minio with docker, you create a new WebHook event

- Endpoint : `http://host.docker.internal:5084//api/webhook/minio/upload-image`
