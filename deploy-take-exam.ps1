# Quick Deploy Script - Student Take Exam

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Student Take Exam - Quick Deploy" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Backup old file
Write-Host "[1/4] Backing up old TakeExam.cshtml..." -ForegroundColor Yellow
$oldFile = "Views\Student\TakeExam.cshtml"
$backupFile = "Views\Student\TakeExam_Backup_$(Get-Date -Format 'yyyyMMdd_HHmmss').cshtml"

if (Test-Path $oldFile) {
    Copy-Item -Path $oldFile -Destination $backupFile
    Write-Host "✓ Backup created: $backupFile" -ForegroundColor Green
} else {
    Write-Host "⚠ Old file not found, skipping backup" -ForegroundColor Yellow
}

# Step 2: Deploy new file
Write-Host ""
Write-Host "[2/4] Deploying new TakeExam.cshtml..." -ForegroundColor Yellow
$newFile = "Views\Student\TakeExam_New.cshtml"

if (Test-Path $newFile) {
    Copy-Item -Path $newFile -Destination $oldFile -Force
    Write-Host "✓ New file deployed successfully" -ForegroundColor Green
} else {
    Write-Host "✗ ERROR: TakeExam_New.cshtml not found!" -ForegroundColor Red
    exit 1
}

# Step 3: Check MongoDB API
Write-Host ""
Write-Host "[3/4] Checking MongoDB API..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:3000/api/exams" -TimeoutSec 2 -ErrorAction Stop
    Write-Host "✓ MongoDB API is running" -ForegroundColor Green
} catch {
    Write-Host "✗ WARNING: MongoDB API not responding" -ForegroundColor Red
    Write-Host "  Please start it with: cd e-testhub; node src/server.js" -ForegroundColor Yellow
}

# Step 4: Build project
Write-Host ""
Write-Host "[4/4] Building project..." -ForegroundColor Yellow
dotnet build --no-restore
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Build successful" -ForegroundColor Green
} else {
    Write-Host "✗ Build failed" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Deployment Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Run: dotnet run" -ForegroundColor White
Write-Host "2. Navigate to: http://localhost:5230" -ForegroundColor White
Write-Host "3. Login as student and test Take Exam" -ForegroundColor White
Write-Host ""
Write-Host "See STUDENT_TAKE_EXAM_GUIDE.md for testing instructions" -ForegroundColor Cyan
