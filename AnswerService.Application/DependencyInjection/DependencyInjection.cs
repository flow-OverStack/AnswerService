using System.Reflection;
using AnswerService.Application.Behaviours;
using AnswerService.Application.Mappings;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerService.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        services.AddAutoMapper(typeof(AnswerMapping));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScopedValidatorsForAssignableValidatedTypes(assembly);
        services.AddCacheHandlerDecorators(assembly);
    }

    private static void AddCacheHandlerDecorators(this IServiceCollection services, Assembly assembly)
    {
        services.AddHandlerDecoratorsFromNamespace(assembly, ".Decorators.Cache.");
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

    private static void AddHandlerDecoratorsFromNamespace(
        this IServiceCollection services, Assembly assembly, string namespaceSegment)
    {
        var handlerOpenType = typeof(IRequestHandler<,>);

        var allConcreteTypes = assembly.DefinedTypes
            .Select(x => x.AsType())
            .Where(x => x is { IsAbstract: false, IsInterface: false })
            .ToArray();

        var decoratorTypes = allConcreteTypes
            .Where(x => x.Namespace?.Contains(namespaceSegment) == true)
            .ToArray();

        var nonDecoratorTypes = allConcreteTypes
            .Where(x => x.Namespace?.Contains(".Decorators.") == false)
            .ToArray();

        foreach (var decoratorType in decoratorTypes)
        {
            var handlerInterfaces = decoratorType
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerOpenType)
                .ToArray();

            foreach (var handlerInterface in handlerInterfaces)
            {
                var hasCounterpart = nonDecoratorTypes.Any(t =>
                    t.GetInterfaces().Any(i => i == handlerInterface));

                if (!hasCounterpart) continue;

                services.Decorate(handlerInterface, decoratorType);
            }
        }
    }
}