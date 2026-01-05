using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Nfp.Detail;

public partial class IUserManager {
    protected override IUser CreateUserInterface() => new();
}