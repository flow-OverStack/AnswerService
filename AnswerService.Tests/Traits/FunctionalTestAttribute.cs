using Xunit.Sdk;

namespace AnswerService.Tests.Traits;

[TraitDiscoverer("AnswerService.Tests.Traits.FunctionalTestDiscoverer", "AnswerService.Tests")]
[AttributeUsage(AttributeTargets.Class)]
public sealed class FunctionalTestAttribute : Attribute, ITraitAttribute;
