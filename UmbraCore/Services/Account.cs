using UmbraCore.Services.Nn.Account.Baas;
using UmbraCore.Services.Nn.Account.Profile;

// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Account;

public partial class IAccountServiceForApplication {
    protected override IManagerForApplication GetBaasAccountManagerForApplication(byte[] _0) => new();
    protected override IProfile GetProfile(byte[] _0) => new();
    protected override void GetLastOpenedUser(out byte[] _0) {
        Console.WriteLine("IAccountServiceForApplication.GetLastOpenedUser called -- handing back 16 nulls");
        _0 = new byte[16];
    }

    protected override byte IsUserAccountSwitchLocked() => 0;
}
