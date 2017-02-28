open System
open System.Diagnostics
open System.Threading
open Microsoft.ServiceFabric.Services.Runtime
open MBrace.Azure.ServiceFabric.Host.Worker

[<EntryPoint>]
let main argv =
    ServiceRuntime.RegisterServiceAsync("WorkerType", (fun ctx -> new Worker(ctx) :> StatelessService), TimeSpan.MaxValue, new CancellationToken()).GetAwaiter().GetResult()
    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof<Worker>.Name)
    Thread.Sleep(Timeout.Infinite)
    0
