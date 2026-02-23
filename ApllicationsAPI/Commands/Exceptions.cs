namespace ApllicationsAPI.Commands;

public class NotFoundException: Exception{}

public class BadRequestException(string message): Exception(message){}