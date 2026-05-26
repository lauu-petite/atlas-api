$baseUrl = "https://atlas-api-fznf.onrender.com/api/admin/eventos"
$jsonPath = "C:\Users\lauol\Desktop\atlas-api\AtlasAPI\Data\eventos.json"
$eventosNuevos = Get-Content -Raw -Encoding UTF8 $jsonPath | ConvertFrom-Json

$nuevo = $eventosNuevos[0]
$bodyObj = @{
    anio = $nuevo.Anio
    titulo = $nuevo.Nombre
    descripcion = $nuevo.Descripcion
    tipo = $nuevo.CategoriaNombre
    latitud = $nuevo.Lat
    longitud = $nuevo.Lon
    categoriaNombre = $nuevo.CategoriaNombre
    categoriaColor = $nuevo.CategoriaColor
    imagenEvento = $nuevo.ImagenEvento
    periodo = $nuevo.Periodo
    mapaId = 1
}

$bodyJson = $bodyObj | ConvertTo-Json -Depth 10

try {
    Invoke-RestMethod -Uri $baseUrl -Method Post -Body $bodyJson -ContentType "application/json"
    Write-Host "Insertado"
} catch {
    $stream = $_.Exception.Response.GetResponseStream()
    $reader = New-Object System.IO.StreamReader($stream)
    $responseBody = $reader.ReadToEnd()
    Write-Host "Error: $responseBody"
}
