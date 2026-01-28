dotnet ef migrations add "$1" \
  --startup-project ./Identity.Api/Identity.Api.csproj \
  --project ./Identity.Data
