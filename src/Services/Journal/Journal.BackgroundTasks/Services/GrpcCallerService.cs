using Grpc.Core;
using Grpc.Net.Client;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Journal.BackgroundTasks.Services
{
    public static class GrpcCallerService
    {
        public static async Task<TResponse> CallService<TResponse>(string urlGrpc, Func<GrpcChannel, Task<TResponse>> func)
        {
            try
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

                var httpClientHandler = new HttpClientHandler()
                {
                    // Return `true` to allow certificates that are untrusted/invalid
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                HttpClient httpClient = new HttpClient(httpClientHandler);

                var channel = GrpcChannel.ForAddress(urlGrpc,
                                    new GrpcChannelOptions() { HttpClient = httpClient });

                Log.Information("Creating grpc client base address urlGrpc ={@urlGrpc}, BaseAddress={@BaseAddress} ", urlGrpc, channel.Target);


                return await func(channel);
            }
            catch (RpcException e)
            {
                Log.Error("Error calling via grpc: {Status} - {Message}", e.Status, e.Message);
                return default;
            }
            finally
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
            }
        }
    }
}
