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
            return new TransactionDisposable(_logger, _ccsContext.Database.BeginTransaction());
        }
    }
}
