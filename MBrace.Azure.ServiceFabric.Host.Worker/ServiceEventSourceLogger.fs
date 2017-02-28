namespace MBrace.Azure.ServiceFabric.Host.Worker

open MBrace.Runtime
open System.Fabric

type ServiceEventSourceLogger(context : StatelessServiceContext) =
    interface ISystemLogger with 
        member __.LogEntry(entry) =
            let text = SystemLogEntry.Format(entry)
            ServiceEventSource.Current.ServiceMessage(context, text)