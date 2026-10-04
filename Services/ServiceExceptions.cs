namespace Nha_Gia_Kim.Services;

public sealed class ResourceNotFoundException(string message) : Exception(message);

public sealed class BusinessConflictException(string message) : Exception(message);

public sealed class InvalidOperationRequestException(string message) : Exception(message);
