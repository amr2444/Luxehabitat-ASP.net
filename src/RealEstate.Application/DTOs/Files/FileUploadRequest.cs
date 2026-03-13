namespace RealEstate.Application.DTOs.Files;

public sealed record FileUploadRequest(
    string FileName,
    string ContentType,
    long Length,
    Stream Content);
