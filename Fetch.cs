class Fetch
{
  public static async Task To(string downloadUrl, string destinationFilePath)
  {
    using (var client = new HttpClientDownloadWithProgress(downloadUrl, destinationFilePath))
    {
      var version = destinationFilePath.Split('/')[^3];
      client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage, rate) =>
      {
        Console.Write(
          $"patch:{version} sie:{totalFileSize}MB/{totalBytesDownloaded}MB speed:{rate}MB/s {progressPercentage}%\r"
          );
      };

      await client.StartDownload();
    }
  }
}