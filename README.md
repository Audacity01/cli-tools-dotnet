# CLI Tools (.NET)

A collection of lightweight command-line tools built with .NET. Each tool is a standalone console app.

## Tools

| Tool | Description | Status |
|------|------------|--------|
| JsonFormatter | Pretty-print, minify, and validate JSON files | Ready |
| CsvConverter | Convert between CSV and JSON, view CSV stats | Ready |
| Base64Tool | Encode/decode text and files to/from base64 | Ready |

## Build & Run

```bash
cd tools/JsonFormatter
dotnet run -- format input.json
dotnet run -- minify input.json
dotnet run -- validate input.json
```

## Adding New Tools

Each tool lives in its own folder under `tools/`. Create a new console app:
```bash
dotnet new console -n ToolName -o tools/ToolName
```
