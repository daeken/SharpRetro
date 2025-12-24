using UmbraCore.Services.Nn.Account.Baas;
using UmbraCore.Services.Nn.Account.Profile;

// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account;

public partial class IAccountServiceForApplication {
    protected override IManagerForApplication GetBaasAccountManagerForApplication(byte[] _0) => new();
    protected override IProfile GetProfile(byte[] _0) => new();
}
