// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Pctl.Detail.Ipc;

public partial class IParentalControlServiceFactory {
    protected override IParentalControlService CreateService(ulong _0, ulong _1) => new();
    protected override IParentalControlService CreateServiceWithoutInitialize(ulong _0, ulong _1) => new();
}