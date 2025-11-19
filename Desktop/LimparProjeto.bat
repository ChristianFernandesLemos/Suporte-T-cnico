@echo off
echo ========================================
echo   Limpeza do Projeto SistemaChamados
echo ========================================
echo.

cd /d "%~dp0"

echo [1/3] Removendo pasta bin...
if exist "bin\" (
    rmdir /s /q "bin"
    echo ✓ Pasta bin removida
) else (
    echo ⚠ Pasta bin nao encontrada
)

echo.
echo [2/3] Removendo pasta obj...
if exist "obj\" (
    rmdir /s /q "obj"
    echo ✓ Pasta obj removida
) else (
    echo ⚠ Pasta obj nao encontrada
)

echo.
echo [3/3] Removendo arquivos temporarios...
del /s /q "*.suo" >nul 2>&1
del /s /q "*.user" >nul 2>&1
del /s /q "*.cache" >nul 2>&1
echo ✓ Arquivos temporarios removidos

echo.
echo ========================================
echo   Limpeza concluida!
echo ========================================
echo.
echo Agora:
echo 1. Abra o Visual Studio
echo 2. Va em Build ^> Rebuild Solution
echo 3. Execute o projeto (F5)
echo.
pause
