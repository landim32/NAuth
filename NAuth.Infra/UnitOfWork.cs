using Microsoft.Extensions.Logging;
using NAuth.Infra.Context;
using NAuth.Infra.Interfaces;
using System;

namespace NAuth.Infra
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly NAuthContext _ccsContext;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(ILogger<UnitOfWork> logger, NAuthContext ccsContext)
        {
            this._ccsContext = ccsContext;
            _logger = logger;
        }

        public ITransaction BeginTransaction()
        {
            _logger.LogTrace("Iniciando bloco de transação.");
            // Corrigido: cria um logger específico para TransactionDisposable
            var transactionLogger = _logger is ILoggerFactory loggerFactory
                ? loggerFactory.CreateLogger<TransactionDisposable>()
                : (ILogger<TransactionDisposable>)Activator.CreateInstance(
                    typeof(Logger<TransactionDisposable>),
                    _logger
                );
            return new TransactionDisposable(transactionLogger, _ccsContext.Database.BeginTransaction());
        }
    }
}
