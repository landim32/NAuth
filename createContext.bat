@echo off
cd .\Backend\NAuth\DB.Infra
dotnet ef dbcontext scaffold "Host=emagine-db-do-user-4436480-0.e.db.ondigitalocean.com;Port=25060;Database=nauth;Username=doadmin;Password=AVNS_akcvzXVnMkvNKaO10-O" Npgsql.EntityFrameworkCore.PostgreSQL --context NAuthContext --output-dir Context -f
pause