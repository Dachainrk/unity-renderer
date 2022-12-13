
// AUTOGENERATED, DO NOT EDIT
// Type definitions for server implementations of ports.
// package: 
// file: decentraland/renderer/kernel_services/friend_request_kernel.proto
using Cysharp.Threading.Tasks;
using rpc_csharp;

public class ClientFriendRequestKernelService
{
  private readonly RpcClientModule module;

  public ClientFriendRequestKernelService(RpcClientModule module)
  {
      this.module = module;
  }

  public UniTask<GetFriendRequestsReply> GetFriendRequests(GetFriendRequestsPayload request)
  {
      return module.CallUnaryProcedure<GetFriendRequestsReply>("GetFriendRequests", request);
  }

  public UniTask<SendFriendRequestReply> SendFriendRequest(SendFriendRequestPayload request)
  {
      return module.CallUnaryProcedure<SendFriendRequestReply>("SendFriendRequest", request);
  }

  public UniTask<CancelFriendRequestReply> CancelFriendRequest(CancelFriendRequestPayload request)
  {
      return module.CallUnaryProcedure<CancelFriendRequestReply>("CancelFriendRequest", request);
  }

  public UniTask<AcceptFriendRequestReply> AcceptFriendRequest(AcceptFriendRequestPayload request)
  {
      return module.CallUnaryProcedure<AcceptFriendRequestReply>("AcceptFriendRequest", request);
  }

  public UniTask<RejectFriendRequestReply> RejectFriendRequest(RejectFriendRequestPayload request)
  {
      return module.CallUnaryProcedure<RejectFriendRequestReply>("RejectFriendRequest", request);
  }
}