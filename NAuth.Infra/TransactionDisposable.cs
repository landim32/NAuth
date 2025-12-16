using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using NAuth.Infra.Interfaces;
using System;

namespace NAuth.Infra
{
    public class TransactionDisposable : ITransaction
    {
        private readonly ILogger<TransactionDisposable> _logger;
        private readonly IDbContextTransaction _transaction;

        public TransactionDisposable(ILogger<TransactionDisposable> logger, IDbContextTransaction transaction)
        {
            _logger = logger;
            _transaction = transaction;
        }

        public void Commit()
        {
            _logger.LogTrace("Finalizando bloco de transação.");
            _transaction.Commit();
        }

        public void Dispose()
        {
            _logger.LogTrace("Liberando transação da memória.");
            _transaction.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Rollback()
        {
            _logger.LogTrace("Rollback do bloco de transação.");
            _transaction.Rollback();

        }
    }
}
