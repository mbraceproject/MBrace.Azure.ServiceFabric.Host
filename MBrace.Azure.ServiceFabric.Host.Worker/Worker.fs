namespace MBrace.Azure.ServiceFabric.Host.Worker

open System
open System.Collections.Generic
open System.Fabric
open System.Threading
open System.Threading.Tasks
open Microsoft.ServiceFabric.Services.Communication.Runtime
open Microsoft.ServiceFabric.Services.Runtime
open System.Diagnostics
open System.Net
open MBrace.Azure
open MBrace.Azure.Service
open MBrace.Runtime

type Worker(context : StatelessServiceContext) =
    inherit StatelessService(context)

    [<DefaultValue>]
    val mutable svc: WorkerService

    override __.CreateServiceInstanceListeners() = Seq.empty

    override __.RunAsync(cancellationToken) =
        try
            let fabricContext = FabricRuntime.GetActivationContext()
            let section = fabricContext.GetConfigurationPackageObject("Config").Settings.Sections.["MBrace"]
            let parameters = section.Parameters

            let storageConnectionString = parameters.["StorageConnectionString"].Value
            let serviceBusConnectionString = parameters.["ServiceBusConnectionString"].Value

            let config = new Configuration(storageConnectionString, serviceBusConnectionString)

            let proc = Process.GetCurrentProcess()
            let workerId = Dns.GetHostName() + "-p" + proc.Id.ToString()

            __.svc <- new WorkerService(config, workerId)
            __.svc.AttachLogger(new ServiceEventSourceLogger(context)) |> ignore

            __.svc.WorkingDirectory <- fabricContext.WorkDirectory

            __.svc.LogFile <- parameters.["LogFile"].Value
            __.svc.LogLevel <- Enum.Parse(typedefof<LogLevel>, parameters.["LogLevel"].Value) :?> LogLevel

            __.svc.HeartbeatInterval <- TimeSpan.Parse(parameters.["HeartbeatInterval"].Value)
            __.svc.HeartbeatThreshold <- TimeSpan.Parse(parameters.["HeartbeatThreshold"].Value)

            __.svc.MaxConcurrentWorkItems <- Environment.ProcessorCount * 8

            __.svc.StartAsTask() :> Task
        with
        | ex ->
            ServiceEventSource.Current.ServiceMessage(context, "MBrace.Azure.ServiceFabric.Host.Worker RunAsync unhandled exception: {0}", ex)
            reraise()

    override __.OnCloseAsync(cancellationToken) =
        try
            __.svc.Stop()
        with
        | ex ->
            ServiceEventSource.Current.ServiceMessage(context, "MBrace.Azure.ServiceFabric.Host.Worker OnCloseAsync unhandled exception: {0}", ex)
            reraise()

        base.OnCloseAsync(cancellationToken)