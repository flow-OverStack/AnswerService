using System.Reflection;
using AnswerService.Application.Behaviours;
using AnswerService.Application.Mappings;
using AnswerService.Domain.Results;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        services.AddAutoMapper(typeof(AnswerMapping));
        services.AddValidators();
        services.AddMediatR();
    }

    private static void AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.RegisterBaseResultValidationBehaviors(Assembly.GetExecutingAssembly());
        });
    }

    private static void AddValidators(this IServiceCollection services)
    {
        services.AddScopedValidatorsForAssignableValidatedTypes(Assembly.GetExecutingAssembly());
    }

    private static void AddScopedValidatorsForAssignableValidatedTypes(this IServiceCollection services,
        Assembly assembly)
    {
        var validatorOpenType = typeof(IValidator<>);

        var allConcreteTypes = assembly.DefinedTypes.Select(x => x.AsType())
            .Where(x => x is { IsAbstract: false, IsInterface: false })
            .ToArray();

        // 1) Find validator implementation classes (e.g., AnswerValidator)
        var validatorTypes = allConcreteTypes.Where(x =>
            x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorOpenType));

        foreach (var validatorType in validatorTypes)
        {
            // A validator class might implement multiple IValidator<T> interfaces; handle all.
            var validatedTypes = validatorType.GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == validatorOpenType)
                .Select(x => x.GetGenericArguments()[0])
                .Distinct();

            foreach (var validatedType in validatedTypes)
            {
                // 2) Find "target" types to register:
                //    - If validatedType is concrete: register IValidator<validatedType> -> validatorType
                //    - If validatedType is interface/abstract: register IValidator<TConcrete> for each TConcrete : validatedType
                var targetTypes = validatedType is { IsInterface: true } || validatedType.IsAbstract
                    ? allConcreteTypes.Where(t => validatedType.IsAssignableFrom(t))
                    : [validatedType];

                foreach (var targetType in targetTypes)
                {
                    var serviceType = validatorOpenType.MakeGenericType(targetType);

                    services.AddScoped(serviceType, validatorType);
                }
            }
        }
    }

    private static void RegisterBaseResultValidationBehaviors(this MediatRServiceConfiguration cfg, Assembly assembly)
    {
        var handlerOpenType = typeof(IRequestHandler<,>);
        var baseResultOpenType = typeof(BaseResult<>);
        var pipelineOpenType = typeof(IPipelineBehavior<,>);
        var validationBehaviorOpenType = typeof(ValidationBehavior<,>);

        var allTypes = assembly.DefinedTypes
            .Select(x => x.AsType())
            .Where(x => x is { IsAbstract: false, IsInterface: false });

        var handlerInterfaces = allTypes
            .SelectMany(x => x.GetInterfaces())
            .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == handlerOpenType);

        foreach (var handlerInterface in handlerInterfaces)
        {
            var genericArgs = handlerInterface.GetGenericArguments();
            var requestType = genericArgs[0];
            var responseType = genericArgs[1];

            if (!responseType.IsGenericType || responseType.GetGenericTypeDefinition() != baseResultOpenType) continue;

            var innerResponseType = responseType.GetGenericArguments()[0];

            // Register:
            // IPipelineBehavior<TRequest, BaseResult<TInner>>  ->  ValidationBehavior<TRequest, TInner>
            var serviceType = pipelineOpenType.MakeGenericType(requestType, responseType);
            var implementationType = validationBehaviorOpenType.MakeGenericType(requestType, innerResponseType);

            cfg.AddBehavior(serviceType, implementationType);
        }
    }
}