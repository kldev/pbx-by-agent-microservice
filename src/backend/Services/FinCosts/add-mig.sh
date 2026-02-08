dotnet ef migrations add "$1" \
  --startup-project ./FinCosts.Api/FinCosts.Api.csproj \
  --project ./FinCosts.Data
