// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Apm;

public partial class IManager {
    protected override ISession OpenSession() => new();
}