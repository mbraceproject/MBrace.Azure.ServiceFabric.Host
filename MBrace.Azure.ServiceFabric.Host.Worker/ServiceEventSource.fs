namespace MBrace.Azure.ServiceFabric.Host.Worker

open System
open System.Diagnostics.Tracing
open System.Fabric
open System.Threading.Tasks

module Events =
    [<Literal>]
    let MessageEventId = 1

    [<Literal>]
    let ServiceMessageEventId = 2

    [<Literal>]
    let ServiceTypeRegisteredEventId = 3

    [<Literal>]
    let ServiceHostInitializationFailedEventId = 4

open Events

[<EventSource(Name = "MBrace.Azure.ServiceFabric.Host.Worker")>]
type ServiceEventSource private() =
    inherit EventSource()

    static let instance = new ServiceEventSource()
    static member Current = instance

    [<NonEvent>]
    member __.Message(message, [<ParamArray>] args : obj[]) =
        if __.IsEnabled() then            
            let finalMessage = String.Format(message, args)
            __.Message(finalMessage)

    [<Event(MessageEventId, Level = EventLevel.Informational, Message = "{0}")>]
    member __.Message(message : string) =
        if __.IsEnabled() then
            __.WriteEvent(MessageEventId, message)

    [<NonEvent>]
    member __.ServiceMessage(serviceContext : StatelessServiceContext, message, [<ParamArray>] args : obj[]) =
        if __.IsEnabled() then        
            let finalMessage = String.Format(message, args)
            __.ServiceMessage(
                serviceContext.ServiceName.ToString(),
                serviceContext.ServiceTypeName,
                serviceContext.InstanceId,
                serviceContext.PartitionId,
                serviceContext.CodePackageActivationContext.ApplicationName,
                serviceContext.CodePackageActivationContext.ApplicationTypeName,
                serviceContext.NodeContext.NodeName,
                finalMessage)

    [<Event(ServiceMessageEventId, Level = EventLevel.Informational, Message = "{7}")>]
    member __.ServiceMessage
        (
            serviceName,
            serviceTypeName,
            replicaOrInstanceId,
            partitionId,
            applicationName,
            applicationTypeName,
            nodeName,
            message
       ) =
        __.WriteEvent(ServiceMessageEventId, serviceName, serviceTypeName, replicaOrInstanceId, partitionId, applicationName, applicationTypeName, nodeName, message)

    [<Event(ServiceTypeRegisteredEventId, Level = EventLevel.Informational, Message = "Service host process {0} registered service type {1}")>]
    member __.ServiceTypeRegistered(hostProcessId : int, serviceType : string) =
        __.WriteEvent(ServiceTypeRegisteredEventId, hostProcessId, serviceType)
    
    [<Event(ServiceHostInitializationFailedEventId, Level = EventLevel.Error, Message = "Service host initialization failed")>]
    member __.ServiceHostInitializationFailed(exc : string) =
        __.WriteEvent(ServiceHostInitializationFailedEventId, exc)