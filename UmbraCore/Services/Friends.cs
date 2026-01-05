using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Friends.Detail.Ipc;

public partial class IServiceCreator {
    protected override IFriendService CreateFriendService() => new();
}