using System;
using dropkick.Configuration;
using dropkick.Configuration.Dsl;
using dropkick.Configuration.Dsl.Iis;
using dropkick.Configuration.Dsl.Msmq;
using dropkick.Configuration.Dsl.NetworkShare;
using dropkick.Configuration.Dsl.WinService;
using dropkick.Configuration.Dsl.MsSql;
using dropkick.Configuration.Dsl.Files;
using dropkick;
using dropkick.Tasks.Iis;
using dropkick.DeploymentModel;
using dropkick.Wmi;

namespace Spike.Deployment
{
    public class AppDeployment : Deployment<AppDeployment, AppDeploymentSettings>
    {
        public AppDeployment()
        {

            Define(settings => DeploymentStepsFor(Web, server =>
                                                           {
                                                               server.Iis7Site(settings.WebSiteName)
                                                                   .VirtualDirectory(
                                                                   settings.WebSiteVirtualDirectoryName)
                                                                   .SetAppPoolTo(settings.AppPoolName)
                                                                   .SetPathTo(settings.WebSitePath);

                                                               server.Iis7Site(settings.WebSiteName)
                                                                   .VirtualDirectory(
                                                                   settings.AppServiceVirtualDirectoryName)
                                                                   .SetAppPoolTo(settings.AppPoolName);

                                                               server
                                                                   .MapTo(TestServer);

                                                               server.Msmq()
                                                                   .PrivateQueueNamed(settings.MessageQueues_AppService);
                                                               server.Msmq()
                                                                   .PrivateQueueNamed(
                                                                   settings.MessageQueues_AppServiceToDataMart);
                                                               server.Msmq()
                                                                   .PrivateQueueNamed(
                                                                   settings.MessageQueues_AlertsService);
                                                               server.Msmq()
                                                                   .PrivateQueueNamed(
                                                                   settings.MessageQueues_ProductRecognition);
                                                               server.Msmq()
                                                                   .PrivateQueueNamed(
                                                                   settings.MessageQueues_PriceRecognition);
                                                           }));

            Define(settings => DeploymentStepsFor(AppWinService, server =>
                                                                     {
                                                                         server.CopyDirectory(".")
                                                                             .DeleteDestinationBeforeDeploying();

                                                                         server.WinService(
                                                                             settings.AppWinService_ServiceName)
                                                                             .Create()
                                                                             .WithDescription(
                                                                             settings.AppWinService_ServiceDescription)
                                                                             .WithServicePath(
                                                                             settings.AppWinService_ExePath)
                                                                             .WithStartMode(ServiceStartMode.Automatic);
                                                                             
                                                                     }));

            Define(settings => DeploymentStepsFor(AppDatabase, server =>
                                                               server.SqlInstance(settings.Database_OperationalDatabaseInstanceName)
                                                                   .Database(settings.Database_OperationalDatabaseName)
                                                                   .RunScript(settings.Database_OperationalDatabaseCreateScript)));
        }

        public static Role Web { get; set; }
        public static Role AppDatabase { get; set; }
        public static Role AppWinService { get; set; }

        public static DeploymentServer TestServer { get; set; }

    }

    public class AppDeploymentSettings
    {
        public string AppPoolName { get; set; }

        public string WebSiteName { get; set; }
        public string WebSiteVirtualDirectoryName { get; set; }
        public string WebSitePath { get; set; }

        public string AppServiceVirtualDirectoryName { get; set; }
        public string AppServicePhysicalPath { get; set; }

        public string AppWinService_ServiceName { get; set;}
        public string AppWinService_ServiceDescription { get; set; }
        public string AppWinService_ExePath { get; set; }

        public string MessageQueues_AppService { get; set; }
        public string MessageQueues_AppServiceToDataMart { get; set; }
        public string MessageQueues_AlertsService { get; set; }
        public string MessageQueues_ProductRecognition { get; set; }
        public string MessageQueues_PriceRecognition { get; set; }

        public string Database_OperationalDatabaseInstanceName { get; set; }
        public string Database_OperationalDatabaseName { get; set; }
        public string Database_OperationalDatabaseCreateScript { get; set; }
    }
}