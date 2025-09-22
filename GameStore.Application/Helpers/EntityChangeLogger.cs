using System.Text.Json;
using System.Text.Json.Serialization;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;

namespace GameStore.Application.Helpers;
public class EntityChangeLogger(ILogger<EntityChangeLogger> logger)
{
    public void LogNorthwindEntityChange<TOldEntity, TNewEntity>(
        string action,
        string entityType,
        TOldEntity oldVersion,
        TNewEntity newVersion)
    {
        var safeOldVersion = Sanitize(oldVersion);
        var safeNewVersion = Sanitize(newVersion);
        logger.LogInformation(
            "EntityChange: Action: {Action}, Entity from NorthwindDb" +
            " was copied to GameStoreDb EntityType: {EntityType}," +
            " OldVersion: {@OldVersion}, NewVersion: {@NewVersion}",
            action,
            entityType,
            safeOldVersion,
            safeNewVersion);
    }

    public void LogEntityChange<TOldEntity, TNewEntity>(
        string action,
        string entityType,
        TOldEntity oldVersion,
        TNewEntity newVersion)
    {
        var safeOldVersion = Sanitize(oldVersion);
        var safeNewVersion = Sanitize(newVersion);

        logger.LogInformation(
            "EntityChange: Action: {Action}, EntityType: {EntityType}," +
            " OldVersion: {@OldVersion}, NewVersion: {@NewVersion}",
            action,
            entityType,
            safeOldVersion,
            safeNewVersion);
    }

    public void LogEntityCreation<TNewEntity>(
        string entityType,
        TNewEntity newVersion)
    {
        var safeNewVersion = Sanitize(newVersion);
        var action = "CREATE";
        logger.LogInformation(
            "EntityChange: Action: {Action}, Entity was created EntityType: {EntityType}," +
            "NewVersion: {@NewVersion}",
            action,
            entityType,
            safeNewVersion);
    }

    public void LogEntityDeletion<TOldEntity>(
        string entityType,
        TOldEntity oldVersion)
    {
        var safeOldVersion = Sanitize(oldVersion);

        var action = "DELETE";
        logger.LogInformation(
            "EntityChange: Action: {Action}, Entity was deleted EntityType: {EntityType}," +
            "NewVersion: {@NewVersion}",
            action,
            entityType,
            safeOldVersion);
    }

    private static T Sanitize<T>(T entity)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };

        var json = JsonSerializer.Serialize(entity, options);
        return JsonSerializer.Deserialize<T>(json, options)!;
    }
}
