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
        private readonly ILogger<TransactionDisposable> _transactionLogger;

        public UnitOfWork(ILogger<UnitOfWork> logger, ILogger<TransactionDisposable> transactionLogger, NAuthContext ccsContext)
        {
            this._ccsContext = ccsContext;
            _logger = logger;
            _transactionLogger = transactionLogger;
        }

        public ITransaction BeginTransaction()
        {
            try
            {
                _logger.LogTrace("Iniciando bloco de transação.");
                return new TransactionDisposable(_transactionLogger, _ccsContext.Database.BeginTransaction());
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
