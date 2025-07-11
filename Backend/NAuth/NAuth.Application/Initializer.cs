using Core.Domain;
using Core.Domain.Cloud;
using Core.Domain.Repository;
using DB.Infra;
using DB.Infra.Context;
using DB.Infra.Repository;
using NAuth.Domain.Impl.Core;
using NAuth.Domain.Impl.Factory;
using NAuth.Domain.Impl.Services;
using NAuth.Domain.Interfaces.Core;
using NAuth.Domain.Interfaces.Factory;
using NAuth.Domain.Interfaces.Models;
using NAuth.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using NAuth.Domain;
using NAuth.Client;
using AuthHandler = NAuth.Domain.AuthHandler;
//using AuthHandler = NAuth.Client.AuthHandler;

namespace NAuth.Application
{
    public static class Initializer
    {
        private static readonly string NAUTH_API_URL = "https://emagine.com.br/auth-api";

        private static void injectDependency(Type serviceType, Type implementationType, IServiceCollection services, bool scoped = true)
        {
            if(scoped)
                services.AddScoped(serviceType, implementationType);
            else
                services.AddTransient(serviceType, implementationType);
        }
        public static void Configure(IServiceCollection services, ConfigurationParam config, bool scoped = true)
        {
            if (scoped)
                services.AddDbContext<NAuthContext>(x => x.UseLazyLoadingProxies().UseNpgsql(config.ConnectionString));
            else
                services.AddDbContextFactory<NAuthContext>(x => x.UseLazyLoadingProxies().UseNpgsql(config.ConnectionString));

            #region Infra
            injectDependency(typeof(NAuthContext), typeof(NAuthContext), services, scoped);
            injectDependency(typeof(IUnitOfWork), typeof(UnitOfWork), services, scoped);
            injectDependency(typeof(ILogCore), typeof(LogCore), services, scoped);
            #endregion

            #region Repository
            injectDependency(typeof(IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory>), typeof(UserAddressRepository), services, scoped);
            injectDependency(typeof(IUserDocumentRepository<IUserDocumentModel, IUserDocumentDomainFactory>), typeof(UserDocumentRepository), services, scoped);
            injectDependency(typeof(IUserPhoneRepository<IUserPhoneModel, IUserPhoneDomainFactory>), typeof(UserPhoneRepository), services, scoped);
            injectDependency(typeof(IUserRepository<IUserModel, IUserDomainFactory>), typeof(UserRepository), services, scoped);
           
            #endregion

            #region Service
            injectDependency(typeof(IImageService), typeof(ImageService), services, scoped);
            injectDependency(typeof(IUserService), typeof(UserService), services, scoped);
            injectDependency(typeof(IMailerSendService), typeof(MailerSendService), services, scoped);
            #endregion

            if (scoped)
            {
                services.AddScoped<IUserClient, UserClient>(new Func<IServiceProvider, UserClient>(x => new UserClient(NAUTH_API_URL)));
            }
            else
            {
                services.AddTransient<IUserClient, UserClient>(new Func<IServiceProvider, UserClient>(x => new UserClient(NAUTH_API_URL)));
            }

            #region Factory
            injectDependency(typeof(IUserAddressDomainFactory), typeof(UserAddressDomainFactory), services, scoped);
            injectDependency(typeof(IUserDocumentDomainFactory), typeof(UserDocumentDomainFactory), services, scoped);
            injectDependency(typeof(IUserPhoneDomainFactory), typeof(UserPhoneDomainFactory), services, scoped);
            injectDependency(typeof(IUserDomainFactory), typeof(UserDomainFactory), services, scoped);
            #endregion


            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, AuthHandler>("BasicAuthentication", null);

        }
    }
}
