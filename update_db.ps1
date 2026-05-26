$baseUrl = "https://atlas-api-fznf.onrender.com/api/admin/eventos"
Write-Host "Obteniendo eventos actuales..."
$eventos = Invoke-RestMethod -Uri $baseUrl -Method Get

if ($eventos -and $eventos.Count -gt 0) {
    Write-Host "Borrando $($eventos.Count) eventos..."
    foreach ($evt in $eventos) {
        Invoke-RestMethod -Uri "$baseUrl/$($evt.id)" -Method Delete
        Write-Host "Borrado evento $($evt.id)"
    }
} else {
    Write-Host "No hay eventos para borrar."
}

$jsonPath = "C:\Users\lauol\Desktop\atlas-api\AtlasAPI\Data\eventos.json"
$eventosNuevos = Get-Content -Raw -Encoding UTF8 $jsonPath | ConvertFrom-Json

Write-Host "Insertando $($eventosNuevos.Count) eventos nuevos..."
foreach ($nuevo in $eventosNuevos) {
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
        Write-Host "Insertado: $($nuevo.Nombre)"
    } catch {
        Write-Host "Error al insertar $($nuevo.Nombre): $_"
    }
}
Write-Host "Proceso completado!"
