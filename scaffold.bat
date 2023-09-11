set "CONNECTION_STRING=Database.SQL"

if NOT "%~1" == "" set "CONNECTION_STRING=%~1"

dotnet ef dbcontext scaffold "Name=%CONNECTION_STRING%" Pomelo.EntityFrameworkCore.MySql -o Models --context-dir Contexts -t EoullimPayments -t EoullimBalances -t EoullimBooths -t EoullimTransactions -t users -f --no-onconfiguring