using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Features.Variance;
using MediatR;
using MediatR.Pipeline;

namespace Identity.Api.Infrastructure.DI
{
  public class MediatorModule : Autofac.Module
  {
    private readonly IEnumerable<Assembly> _assemblies;
    private readonly Type[] _mediatrOpenTypes = {
            typeof(IRequestHandler<,>),
            typeof(IRequestHandler<>),
            typeof(INotificationHandler<>)
        };

    public MediatorModule(IEnumerable<Assembly> assemblies)
    {
      _assemblies = assemblies;
    }

    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterSource(new ContravariantRegistrationSource());
      builder.RegisterType<Mediator>()
          .As<IMediator>()
          .InstancePerLifetimeScope();

      // finally register our custom code (individually, or via assembly scanning)
      // - requests & handlers as transient, i.e. InstancePerDependency()
      // - pre/post-processors as scoped/per-request, i.e. InstancePerLifetimeScope()
      // - behaviors as transient, i.e. InstancePerDependency()
      foreach (var assembly in _assemblies)
      {
        foreach (var mediatrOpenType in _mediatrOpenTypes)
        {
          builder.RegisterAssemblyTypes(assembly)
              .AsClosedTypesOf(mediatrOpenType)
              .PropertiesAutowired()
              .AsImplementedInterfaces();
        }
      }

      builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
      builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

      builder.Register<ServiceFactory>(ctx =>
      {
        var c = ctx.Resolve<IComponentContext>();
        return t => c.TryResolve(t, out var o) ? o : null;
      }).InstancePerLifetimeScope();

      //builder.Register<ServiceFactory>(ctx =>
      //{
      //  var c = ctx.Resolve<IComponentContext>();
      //  return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
      //}).InstancePerLifetimeScope();

    }
  }
}
