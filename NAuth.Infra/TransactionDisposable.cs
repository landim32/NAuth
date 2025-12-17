using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using NAuth.Infra.Interfaces;
using System;

namespace NAuth.Infra
{
    public class TransactionDisposable : ITransaction
    {
        private readonly ILogger<UnitOfWork> _logger;
        private readonly IDbContextTransaction _transaction;
        private bool _disposed;

        public TransactionDisposable(ILogger<UnitOfWork> logger, IDbContextTransaction transaction)
        {
            _logger = logger;
            _transaction = transaction;
            _disposed = false;
        }

        public void Commit()
        {
            ThrowIfDisposed();
            _logger.LogTrace("Finalizando bloco de transação.");
            _transaction.Commit();
        }

        public void Rollback()
        {
            ThrowIfDisposed();
            _logger.LogTrace("Rollback do bloco de transação.");
            _transaction.Rollback();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _logger.LogTrace("Liberando transação da memória.");
                    _transaction.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(TransactionDisposable));
        }
    }
}
