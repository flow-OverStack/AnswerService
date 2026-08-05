using Xunit.Sdk;

namespace AnswerService.Tests.Traits;

[TraitDiscoverer("AnswerService.Tests.Traits.UnitTestDiscoverer", "AnswerService.Tests")]
[AttributeUsage(AttributeTargets.Class)]
public sealed class UnitTestAttribute : Attribute, ITraitAttribute;
