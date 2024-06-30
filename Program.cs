using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/Upload", async (IFormFile formFile) =>
{
    var connectionString = configuration.GetConnectionString("blobstorage");
    var containerName = "";

    try
    {
        // Converte o arquivo em um array de bytes
        using var memoryStream = new MemoryStream();
        // Lê a imagem do corpo da requisição        
        await formFile.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();
        // Cria um objeto CloudStorageAccount a partir da string de conexão
        var storageAccount = CloudStorageAccount.Parse(connectionString);

        // Cria um objeto CloudBlobClient a partir da conta de armazenamento
        var blobClient = storageAccount.CreateCloudBlobClient();

        // Cria o contêiner caso ele não exista
        var container = blobClient.GetContainerReference(containerName);
        await container.CreateIfNotExistsAsync();

        // Define o nome do blob a partir do nome do arquivo
        var blobName = Guid.NewGuid().ToString(); // + Path.GetExtension(formFile.FileName);

        // Faz upload do arquivo para o blob
        var blob = container.GetBlockBlobReference(blobName);
        await blob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length);

        // Obtém a URL de acesso ao blob
        var blobUrl = blob.Uri.ToString();
        return Results.Ok(blobName);
    }
    catch (Exception ex)
    {
        return Results.NotFound(ex);
    }
}).DisableAntiforgery().Accepts<FormFile>("multipart/form-data").WithTags("FileManagement");

app.MapGet("/GetSpecific", async (string blobName) =>
{
    string blobUrlWithSas = await GetBlobUrlWithSas(blobName, configuration);

    return Results.Ok(blobUrlWithSas);
}).WithName("GetSingle").WithTags("FileManagement");

app.MapGet("/GetMany", async (string stringBlobNameList) =>
{
    try
    {
        List<string> blobNameList = stringBlobNameList.Split(',').ToList();
        var result = new Dictionary<string, string>();

        foreach (string blobName in blobNameList)
        {
            string blobUrlWithSas = await GetBlobUrlWithSas(blobName, configuration);
            if (!string.IsNullOrEmpty(blobUrlWithSas))
            {
                result.Add(blobName, blobUrlWithSas);
            }
        }
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.NotFound(ex);
    }
}).WithName("GetMany").WithTags("FileManagement");



app.Run();

static async Task<string> GetBlobUrlWithSas(string blobName, IConfigurationRoot configuration)
{
    var containerName = "fotoschallenge";
    var connectionString = configuration.GetConnectionString("blobstorage");
    var storageAccount = CloudStorageAccount.Parse(connectionString);

    // Cria um objeto CloudBlobClient a partir da conta de armazenamento
    var blobClient = storageAccount.CreateCloudBlobClient();

    // Cria o contêiner caso ele não exista
    var container = blobClient.GetContainerReference(containerName);
    //await container.CreateIfNotExistsAsync();    

    // Cria a referencia do Blob
    var blob = container.GetBlobReference(blobName);

    var checkIfExists = await blob.ExistsAsync();
    if (!checkIfExists) return string.Empty;

    // Obtém o SAS Token para o blob
    var sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
    {
        SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(1), // Define o tempo de expiração do SAS Token
        Permissions = SharedAccessBlobPermissions.Read // Define as permissões do SAS Token (somente leitura neste caso)
    });
    //var sasToken = "sp=racwdl&st=2023-06-18T13:08:08Z&se=2023-06-23T21:08:08Z&spr=https&sv=2022-11-02&sr=c&sig=4njFIKcB7Ide2qD7voG%2ForlDZsF70qm51FYLOKIktGo%3D";

    // Constrói a URL do blob com o SAS Token
    var blobUrlWithSas = blob.Uri + sasToken;
    return blobUrlWithSas;
}
