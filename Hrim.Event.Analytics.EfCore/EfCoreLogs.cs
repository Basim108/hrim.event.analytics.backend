namespace Hrim.Event.Analytics.EfCore;

public static class EfCoreLogs {
    public const string OPERATION_TIMEOUT = "Timeout error while executing Operation={Operation}, ErrorMessage={ErrorMessage}";
    public const string OPERATION_IS_FORBIDDEN_BY_USER_ID = "Operation={Operation} is forbidden as entity belongs to EntityOwnerId={EntityOwnerId}, EntityType={EntityType}.";
    public const string CANNOT_CREATE_IS_DELETED          = "Cannot create entity as it is existed and IsDeleted: EntityType={EntityType}";
    public const string CANNOT_CREATE_IS_ALREADY_EXISTED  = "Cannot create entity as it is already existed: EntityType={EntityType}, ExistedEntity={ExistedEnity}";
    public const string CANNOT_UPDATE_ENTITY_IS_DELETED   = "Cannot update entity as it is deleted: EntityConcurrentToken={EntityConcurrentToken}, EntityType={EntityType}.";
    public const string ENTITY_NOT_FOUND_BY_ID            = "Entity was not found by id EntityType={EntityType}";

    public const string CANNOT_SOFT_DELETE_ENTITY_IS_DELETED =
        "Cannot soft delete entity as it is already deleted: EntityConcurrentToken={EntityConcurrentToken}, EntityType={EntityType}.";

    public const string CANNOT_RESTORE_ENTITY_IS_NOT_DELETED =
        "Cannot restore entity as it is not deleted: EntityConcurrentToken={EntityConcurrentToken}, EntityType={EntityType}.";
}