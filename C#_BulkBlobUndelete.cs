using System;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Threading.Tasks;
using System.Linq;



namespace BulkBlobUndelete
{
class Program
{
static string connectionString = "";
static string containerName = "";
static async Task Main(string[] args)
{



Console.WriteLine("Please enter storage account Connection string");
connectionString = Console.ReadLine().Trim();




Console.WriteLine("Please enter storage container name");
containerName = Console.ReadLine().Trim();



if (connectionString != "" && containerName != "")
{



BlobContainerClient blobContainerClientw = new BlobContainerClient(connectionString, containerName);
await ListBlobsFromStorage(blobContainerClientw, 1000);



Console.WriteLine("Done");
}
else
{
Console.WriteLine("Please enter all the details");
}



}



private static async Task ListBlobsFromStorage(BlobContainerClient blobContainerClient,
int? segmentSize)
{
try
{



var resultSegment = blobContainerClient.GetBlobsAsync(BlobTraits.None, BlobStates.Version).AsPages(default, segmentSize);
int totalBlob = 0;
// Enumerate the blobs returned for each page.
BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);




await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
{



Console.WriteLine("Continuation Token is : " + blobPage.ContinuationToken);
foreach (BlobItem blobItem in blobPage.Values)
{
Console.WriteLine("Blob name: {0}", blobItem.Name);



totalBlob = totalBlob + 1;




Pageable<BlobItem> blobItems = blobContainerClient.GetBlobs(BlobTraits.None, BlobStates.Version);




BlobClient blockBlob = blobContainerClient.GetBlobClient(blobItem.Name);



// Get the URI for the most recent version.
BlobUriBuilder blobVersionUri = new BlobUriBuilder(blockBlob.Uri)
{
VersionId = blobItems.OrderByDescending(version => version.VersionId)
.ElementAtOrDefault(1)?.VersionId
};



Console.WriteLine(blobVersionUri.ToUri().ToString());



// Restore the most recently generated version by copying it to the base blob.
blockBlob.StartCopyFromUri(blobVersionUri.ToUri());



}
Console.WriteLine();
Console.WriteLine("Total Blob:" + totalBlob.ToString());




}





}
catch (RequestFailedException e)
{
Console.WriteLine(e.Message);
Console.ReadLine();
throw;
}
}
}
}