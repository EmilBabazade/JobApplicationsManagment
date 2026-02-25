namespace ApllicationsAPI.Commands.Common;

public class NotFoundException: Exception{}

public class BadRequestException(string message): Exception(message){}