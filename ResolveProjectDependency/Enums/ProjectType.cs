enum ProjectType
{
    React,
    Angular,
    Vue,
    DotNet
}

enum BindingDirection
{
    Inbound,
    Outbound,
    Bidirectional
}


enum ApplicationType
{
    WebApp,
    Persistance,
    Messaging,
    SecretManagement,
    Monitoring,
    AuthZN
}

enum WebAppType
{
    FrontEnd,
    BackEnd
}

enum PersistanceType
{
    SQLServer,
    CosmosDB,
    Redis,
    BlobStorage,
    TableStorage,
    QueueStorage
}
enum MessagingType
{
    ServiceBus,
    EventHub,
    Kafka,
    RabbitMQ
}
enum SecretManagementType
{
    KeyVault,
    AzureAppConfig,
    AzureKeyVault
}
enum MonitoringType
{
    AppInsights,
    LogAnalytics,
    AzureMonitor
}
enum AuthZNType
{
    IdentityServer,
    AzureAD,
    AzureADAppReg,
    AzureADMSI
}