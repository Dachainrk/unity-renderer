
// AUTOGENERATED, DO NOT EDIT
// Type definitions for server implementations of ports.
// package: decentraland.renderer.kernel_services
// file: decentraland/renderer/kernel_services/analytics.proto
using Cysharp.Threading.Tasks;
using rpc_csharp;

namespace Decentraland.Renderer.KernelServices {
public class ClientAnalyticsKernelService
{
  private readonly RpcClientModule module;

  public ClientAnalyticsKernelService(RpcClientModule module)
  {
      this.module = module;
  }

  public UniTask<PerformanceReportResponse> PerformanceReport(PerformanceReportRequest request)
  {
      return module.CallUnaryProcedure<PerformanceReportResponse>("PerformanceReport", request);
  }

  public UniTask<SystemInfoReportResponse> SystemInfoReport(SystemInfoReportRequest request)
  {
      return module.CallUnaryProcedure<SystemInfoReportResponse>("SystemInfoReport", request);
  }

  public UniTask<AnalyticsEventResponse> AnalyticsEvent(AnalyticsEventRequest request)
  {
      return module.CallUnaryProcedure<AnalyticsEventResponse>("AnalyticsEvent", request);
  }

  public UniTask<DelightedSurveyResponse> SetDelightedSurveyEnabled(DelightedSurveyRequest request)
  {
      return module.CallUnaryProcedure<DelightedSurveyResponse>("SetDelightedSurveyEnabled", request);
  }
}
}