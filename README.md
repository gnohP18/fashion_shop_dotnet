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

### Port and database docker

|  No  | Name | Port | Type |
|--- |--- |--- |--- |
| 1   | api | 8123 | .NET API |
| 2   | mvc | 5123 | .NET MVC |
| 3   | database | 5432 | Postgres |
| 4   | redis | 6379 | Redis |
| 5   | minio | 9000,9001 | Minio |
