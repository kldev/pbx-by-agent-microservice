#!/bin/bash
dotnet ef migrations add "$1" \
  --startup-project ./AnswerRule.Api/AnswerRule.Api.csproj \
  --project ./AnswerRule.Data
