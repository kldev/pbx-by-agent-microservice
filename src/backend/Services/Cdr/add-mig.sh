dotnet ef migrations add "$1" \
  --startup-project ./Cdr.Api/Cdr.Api.csproj \
  --project ./Cdr.Data
