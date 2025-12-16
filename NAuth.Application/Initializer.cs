using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NAuth.Domain.Factory;
using NAuth.Domain.Factory.Interfaces;
using NAuth.Domain.Models.Models;
using NAuth.Domain.Services;
using NAuth.Domain.Services.Interfaces;
using NAuth.Infra;
using NAuth.Infra.Context;
using NAuth.Infra.Interfaces;
using NAuth.Infra.Interfaces.Repository;
using NAuth.Infra.Repository;
using NTools.ACL;
using NTools.ACL.Interfaces;
using System;
using LocalAuthHandler = NAuth.Domain.LocalAuthHandler;

namespace NAuth.Application
{
    public static class Initializer
    {
        private static void injectDependency(Type serviceType, Type implementationType, IServiceCollection services, bool scoped = true)
        {
            if (scoped)
                services.AddScoped(serviceType, implementationType);
            else
                services.AddTransient(serviceType, implementationType);
        }
        public static void Configure(IServiceCollection services, string connection, bool scoped = true)
        {
            if (scoped)
                services.AddDbContext<NAuthContext>(x => x.UseLazyLoadingProxies().UseNpgsql(connection));
            else
                services.AddDbContextFactory<NAuthContext>(x => x.UseLazyLoadingProxies().UseNpgsql(connection));

            #region Infra
            injectDependency(typeof(NAuthContext), typeof(NAuthContext), services, scoped);
            injectDependency(typeof(IUnitOfWork), typeof(UnitOfWork), services, scoped);
            #endregion

            #region Repository
            injectDependency(typeof(IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory>), typeof(UserAddressRepository), services, scoped);
            injectDependency(typeof(IUserDocumentRepository<IUserDocumentModel, IUserDocumentDomainFactory>), typeof(UserDocumentRepository), services, scoped);
            injectDependency(typeof(IUserPhoneRepository<IUserPhoneModel, IUserPhoneDomainFactory>), typeof(UserPhoneRepository), services, scoped);
            injectDependency(typeof(IUserTokenRepository<IUserTokenModel, IUserTokenDomainFactory>), typeof(UserTokenRepository), services, scoped);
            injectDependency(typeof(IUserRepository<IUserModel, IUserDomainFactory>), typeof(UserRepository), services, scoped);

            #endregion

            #region Factory
            injectDependency(typeof(IUserAddressDomainFactory), typeof(UserAddressDomainFactory), services, scoped);
            injectDependency(typeof(IUserDocumentDomainFactory), typeof(UserDocumentDomainFactory), services, scoped);
            injectDependency(typeof(IUserPhoneDomainFactory), typeof(UserPhoneDomainFactory), services, scoped);
            injectDependency(typeof(IUserTokenDomainFactory), typeof(UserTokenDomainFactory), services, scoped);
            injectDependency(typeof(IUserDomainFactory), typeof(UserDomainFactory), services, scoped);
            #endregion

            #region Clients
            injectDependency(typeof(IMailClient), typeof(MailClient), services, scoped);
            injectDependency(typeof(IFileClient), typeof(FileClient), services, scoped);
            injectDependency(typeof(IStringClient), typeof(StringClient), services, scoped);
            injectDependency(typeof(IDocumentClient), typeof(DocumentClient), services, scoped);
            #endregion

            #region Service Wrappers
            services.AddScoped<UserDomainFactories>(sp => new UserDomainFactories(
                sp.GetRequiredService<IUserDomainFactory>(),
                sp.GetRequiredService<IUserPhoneDomainFactory>(),
                sp.GetRequiredService<IUserAddressDomainFactory>(),
                sp.GetRequiredService<IUserTokenDomainFactory>()
            ));

            services.AddScoped<ExternalClients>(sp => new ExternalClients(
                sp.GetRequiredService<IMailClient>(),
                sp.GetRequiredService<IFileClient>(),
                sp.GetRequiredService<IStringClient>(),
                sp.GetRequiredService<IDocumentClient>()
            ));
            #endregion

            #region Service
            injectDependency(typeof(IUserService), typeof(UserService), services, scoped);
            #endregion


            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, LocalAuthHandler>("BasicAuthentication", null);

        }
    }
}
