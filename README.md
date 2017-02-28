# MBrace.Azure.ServiceFabric.Host

Hosting MBrace cluster inside Azure Service Fabric.

## Installation
1. Provision Service Fabric Cluster on Azure or use existing.

    A0-size machines and 3-node cluster should be enough for tests.
    You can also use local multi-node cluster on your machine.

2. Provision Service Bus and Storage Account on Azure.

   You can also use emulated storage when running MBrace on your local Service Fabric cluster, 
   but you still need to have Service Bus on Azure.

3. Build MBrace service Fabric solution.

   Service Fabric SDK used: 5.4.164. Not sure how it will work if you have different SDK version installed.
   
   Note: after  opening solution Visual Studio asks to upgrade NuGet packages. Click "No" - Paket manager is used here.

4. Set correct values of your service bus and storage connection strings.

   Modify parameter files in `ApplicationParameters` folder inside `MBrace.Azure.ServiceFabric.Host` project.
   Use `Cloud.xml` or `Local.xNode.xml` depending on whether you have remote or local cluster.
   `MBrace_StorageConnectionString` is for storage and `MBrace_ServiceBusConnectionString` is for service bus connection string.

   Example:
   ```xml
   <Parameter Name="MBrace_StorageConnectionString" 
       Value="DefaultEndpointsProtocol=https;AccountName=<your storage account>;AccountKey=<your key>" />

   <Parameter Name="MBrace_ServiceBusConnectionString" 
       Value="Endpoint=sb://<your service bus name>.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=<your key>" />
   ```
   For emulated storage use 
   ```xml
   <Parameter Name="MBrace_StorageConnectionString" Value="UseDevelopmentStorage=true" />
   ```
5. Deploy MBrace solution to your cluster.
   
   Right click on "MBrace.Azure.ServiceFabric.Host" project and select "Publish".
   Enter your cluster "Connection Endpoint", select "Application Parameters File" and click "Publish".

6. MBrace cluster should be ready for use.

## Future work:

* Create ARM template to provision Service Fabric cluster using PowerShell
* Add service event source to log MBrace events there
* Make number of concurrent work items configurable