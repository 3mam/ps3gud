
public class HttpClientDownloadWithProgress : IDisposable
{
  readonly string _downloadUrl;
  readonly string _destinationFilePath;
  HttpClient? httpClient;
  DateTime _startedAt = DateTime.Now;

  public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage, double rate);
  public event ProgressChangedHandler? ProgressChanged;

  public HttpClientDownloadWithProgress(string downloadUrl, string destinationFilePath)
  {
    _downloadUrl = downloadUrl;
    _destinationFilePath = destinationFilePath;
  }

  public async Task StartDownload()
  {
    httpClient = new HttpClient { Timeout = TimeSpan.FromDays(1) };

    using (var response = await httpClient.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead))
      await DownloadFileFromHttpResponseMessage(response);
  }

  async Task DownloadFileFromHttpResponseMessage(HttpResponseMessage response)
  {
    response.EnsureSuccessStatusCode();

    var totalBytes = response.Content.Headers.ContentLength;

    using (var contentStream = await response.Content.ReadAsStreamAsync())
      await ProcessContentStream(totalBytes, contentStream);
  }

  async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream)
  {
    var totalBytesRead = 0L;
    var readCount = 0L;
    var buffer = new byte[8192];
    var isMoreToRead = true;

    using (var fileStream = new FileStream(_destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
    {
      do
      {
        var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
        if (bytesRead == 0)
        {
          isMoreToRead = false;
          TriggerProgressChanged(totalDownloadSize, totalBytesRead);
          continue;
        }

        await fileStream.WriteAsync(buffer, 0, bytesRead);

        totalBytesRead += bytesRead;
        readCount += 1;

        if (readCount % 100 == 0)
          TriggerProgressChanged(totalDownloadSize, totalBytesRead);
      }
      while (isMoreToRead);
    }
  }

  void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead)
  {
    var elapsed = (DateTime.Now - _startedAt) / 1000;
    var rate = Math.Round((totalBytesRead / elapsed.TotalMilliseconds)/1024/1024,1);
    if (ProgressChanged == null)
      return;

    double? progressPercentage = null;
    if (totalDownloadSize.HasValue)
      progressPercentage = Math.Round((double)totalBytesRead / totalDownloadSize.Value * 100, 0);

    ProgressChanged(totalDownloadSize/1024/1024, totalBytesRead/1024/1024, progressPercentage, rate);
  }

  public void Dispose()
  {
    httpClient?.Dispose();
  }
}