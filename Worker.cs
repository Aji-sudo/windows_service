//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Npgsql;

//public class Worker : BackgroundService
//{
//    private readonly ILogger<Worker> _logger;
//    private const string ConnectionString = "Host=w3.training5.modak.com;Port=5432;Username=mt24023;Password=mt24023@m04y24;Database=postgres";
//    private readonly Dictionary<string, FileSystemWatcher> _watchers = new Dictionary<string, FileSystemWatcher>();

//    public Worker(ILogger<Worker> logger)
//    {
//        _logger = logger;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        _logger.LogInformation("Service started.");     

//        while (!stoppingToken.IsCancellationRequested)
//        {
//            await ConfigureWatchersAsync();
//            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Recheck the database every 5 minutes
//        }
//    }

//    private async Task ConfigureWatchersAsync()
//    {
//        try
//        {
//            using (var conn = new NpgsqlConnection(ConnectionString))
//            {
//                await conn.OpenAsync();

//                string query = "SELECT id, sourcefolder_path, destinationfolder_path FROM folder_activity WHERE activity_status = TRUE";
//                using (var cmd = new NpgsqlCommand(query, conn))
//                using (var reader = await cmd.ExecuteReaderAsync())
//                {
//                    while (await reader.ReadAsync())
//                    {
//                        string sourcePath = reader.GetString(1);
//                        string destinationPath = reader.GetString(2);

//                        if (!_watchers.ContainsKey(sourcePath))
//                        {
//                            CreateWatcher(sourcePath, destinationPath);
//                        }
//                    }
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError($"Error configuring watchers: {ex.Message}");
//        }
//    }

//    private void CreateWatcher(string sourcePath, string destinationPath)
//    {
//        if (!Directory.Exists(sourcePath) || !Directory.Exists(destinationPath))
//        {
//            _logger.LogWarning($"Source or destination folder does not exist: {sourcePath}, {destinationPath}");
//            return;
//        }

//        var watcher = new FileSystemWatcher(sourcePath)
//        {
//            NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size,
//            Filter = "*.*",
//            EnableRaisingEvents = true
//        };

//        watcher.Created += async (sender, args) =>
//        {
//            try
//            {
//                // Wait for the file to be fully written
//                await WaitForFileAsync(args.FullPath);

//                string destFile = Path.Combine(destinationPath, Path.GetFileName(args.FullPath));
//                if (File.Exists(destFile))
//                {
//                    // Delete the old file at the destination
//                    File.Delete(destFile);
//                }

//                File.Move(args.FullPath, destFile);
//                _logger.LogInformation($"Moved: {args.FullPath} -> {destFile}");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($"Error moving file {args.FullPath}: {ex.Message}");
//            }
//        };

//        _watchers[sourcePath] = watcher;
//        _logger.LogInformation($"Watcher created for: {sourcePath}");
//    }

//    private async Task WaitForFileAsync(string filePath)
//    {
//        int retryCount = 10; // Retry up to 10 times
//        while (retryCount > 0)
//        {
//            try
//            {
//                // Try to open the file exclusively, if it succeeds, the file is ready
//                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
//                {
//                    fs.Close();
//                }
//                break;
//            }
//            catch (IOException)
//            {
//                // File is still in use, wait before retrying
//                await Task.Delay(500); // Wait for 500 ms before retrying
//                retryCount--;
//            }
//        }

//        if (retryCount == 0)
//        {
//            _logger.LogWarning($"File is still being written after 10 retries: {filePath}");
//        }
//    }

//    public override async Task StopAsync(CancellationToken cancellationToken)
//    {
//        foreach (var watcher in _watchers.Values)
//        {
//            watcher.EnableRaisingEvents = false;
//            watcher.Dispose();
//        }
//        _watchers.Clear();
////        _logger.LogInformation("Watchers stopped.");

////        await base.StopAsync(cancellationToken);
////    }
////}


//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Compression;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Npgsql;

//public class Worker : BackgroundService
//{
//    private readonly ILogger<Worker> _logger;
//    private const string ConnectionString = "Host=w3.training5.modak.com;Port=5432;Username=mt24023;Password=mt24023@m04y24;Database=postgres";
//    private readonly Dictionary<string, FileSystemWatcher> _watchers = new Dictionary<string, FileSystemWatcher>();

//    public Worker(ILogger<Worker> logger)
//    {
//        _logger = logger;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        _logger.LogInformation("Service started.");

//        while (!stoppingToken.IsCancellationRequested)
//        {
//            await ConfigureWatchersAsync();
//            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Recheck the database every 5 minutes
//        }
//    }

//    private async Task ConfigureWatchersAsync()
//    {
//        try
//        {
//            using (var conn = new NpgsqlConnection(ConnectionString))
//            {
//                await conn.OpenAsync();

//                string query = "SELECT id, sourcefolder_path, destinationfolder_path FROM folder_activity WHERE activity_status = TRUE";
//                using (var cmd = new NpgsqlCommand(query, conn))
//                using (var reader = await cmd.ExecuteReaderAsync())
//                {
//                    while (await reader.ReadAsync())
//                    {
//                        string sourcePath = reader.GetString(1);
//                        string destinationPath = reader.GetString(2);

//                        if (!_watchers.ContainsKey(sourcePath))
//                        {
//                            CreateWatcher(sourcePath, destinationPath);
//                        }
//                    }
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError($"Error configuring watchers: {ex.Message}");
//        }
//    }

//    private void CreateWatcher(string sourcePath, string destinationPath)
//    {
//        if (!Directory.Exists(sourcePath) || !Directory.Exists(destinationPath))
//        {
//            _logger.LogWarning($"Source or destination folder does not exist: {sourcePath}, {destinationPath}");
//            return;
//        }

//        var watcher = new FileSystemWatcher(sourcePath)
//        {
//            NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size,
//            Filter = "*.*",
//            EnableRaisingEvents = true
//        };

//        watcher.Created += async (sender, args) =>
//        {
//            try
//            {
//                // Wait for the file to be fully written
//                await WaitForFileAsync(args.FullPath);

//                if (Path.GetExtension(args.FullPath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
//                {
//                    // Handle ZIP files: Extract contents to the destination folder
//                    await ExtractZipFileAsync(args.FullPath, destinationPath);
//                    _logger.LogInformation($"ZIP file extracted: {args.FullPath} -> {destinationPath}");
//                }
//                else
//                {
//                    string destFile = Path.Combine(destinationPath, Path.GetFileName(args.FullPath));
//                    if (File.Exists(destFile))
//                    {
//                        // Delete the old file at the destination
//                        File.Delete(destFile);
//                    }

//                    File.Move(args.FullPath, destFile);
//                    _logger.LogInformation($"Moved: {args.FullPath} -> {destFile}");
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($"Error processing file {args.FullPath}: {ex.Message}");
//            }
//        };

//        _watchers[sourcePath] = watcher;
//        _logger.LogInformation($"Watcher created for: {sourcePath}");
//    }

//    private async Task ExtractZipFileAsync(string zipFilePath, string destinationPath)
//    {
//        try
//        {
//            string extractPath = Path.Combine(destinationPath, Path.GetFileNameWithoutExtension(zipFilePath));

//            if (!Directory.Exists(extractPath))
//            {
//                Directory.CreateDirectory(extractPath);
//            }

//            await Task.Run(() => ZipFile.ExtractToDirectory(zipFilePath, extractPath, true));

//            File.Delete(zipFilePath); // Optionally delete the ZIP file after extraction
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError($"Error extracting ZIP file {zipFilePath}: {ex.Message}");
//        }
//    }

//    private async Task WaitForFileAsync(string filePath)
//    {
//        int retryCount = 10; // Retry up to 10 times
//        while (retryCount > 0)
//        {
//            try
//            {
//                // Try to open the file exclusively, if it succeeds, the file is ready
//                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
//                {
//                    fs.Close();
//                }
//                break;
//            }
//            catch (IOException)
//            {
//                // File is still in use, wait before retrying
//                await Task.Delay(500); // Wait for 500 ms before retrying
//                retryCount--;
//            }
//        }

//        if (retryCount == 0)
//        {
//            _logger.LogWarning($"File is still being written after 10 retries: {filePath}");
//        }
//    }

//    public override async Task StopAsync(CancellationToken cancellationToken)
//    {
//        foreach (var watcher in _watchers.Values)
//        {
//            watcher.EnableRaisingEvents = false;
//            watcher.Dispose();
//        }
//        _watchers.Clear();
//        _logger.LogInformation("Watchers stopped.");

//        await base.StopAsync(cancellationToken);
//    }
//}




using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private const string ConnectionString = "Host=w3.training5.modak.com;Port=5432;Username=mt24023;Password=mt24023@m04y24;Database=postgres";
    private readonly Dictionary<string, FileSystemWatcher> _watchers = new Dictionary<string, FileSystemWatcher>();
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await ConfigureWatchersAsync();
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Recheck the database every 5 minutes
        }
    }

    private async Task ConfigureWatchersAsync()
    {
        try
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                await conn.OpenAsync();

                string query = "SELECT id, sourcefolder_path, destinationfolder_path FROM folder_activity WHERE activity_status = TRUE";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var newWatchers = new Dictionary<string, FileSystemWatcher>();

                    while (await reader.ReadAsync())
                    {
                        string sourcePath = reader.GetString(1);
                        string destinationPath = reader.GetString(2);

                        if (!_watchers.ContainsKey(sourcePath))
                        {
                            CreateWatcher(sourcePath, destinationPath, newWatchers);
                        }
                    }

                    foreach (var watcher in newWatchers)
                    {
                        _watchers[watcher.Key] = watcher.Value;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error configuring watchers: {ex.Message}");
        }
    }

    private void CreateWatcher(string sourcePath, string destinationPath, Dictionary<string, FileSystemWatcher> newWatchers)
    {
        if (!Directory.Exists(sourcePath) || !Directory.Exists(destinationPath))
        {
            _logger.LogWarning($"Source or destination folder does not exist: {sourcePath}, {destinationPath}");
            return;
        }

        var watcher = new FileSystemWatcher(sourcePath)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size,
            Filter = "*.*",
            EnableRaisingEvents = true
        };

        watcher.Created += async (sender, args) =>
        {
            try
            {
                await WaitForFileAsync(args.FullPath);

                if (Path.GetExtension(args.FullPath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    await ExtractZipFileAsync(args.FullPath, destinationPath);
                    await LogActivityAsync(args.FullPath, destinationPath, "Moved");
                }
                else if (Path.GetExtension(args.FullPath).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    await AddHeadersToXmlAsync(args.FullPath, destinationPath);
                    await LogActivityAsync(args.FullPath, destinationPath, "Processed");
                }
                else
                {
                    string destFile = Path.Combine(destinationPath, Path.GetFileName(args.FullPath));
                    if (File.Exists(destFile)) File.Delete(destFile);

                    File.Move(args.FullPath, destFile);
                    _logger.LogInformation($"Moved: {args.FullPath} -> {destFile}");
                    await LogActivityAsync(args.FullPath, destinationPath, "Moved");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing file {args.FullPath}: {ex.Message}");
                await LogActivityAsync(args.FullPath, destinationPath, "Error");
            }
        };

        newWatchers[sourcePath] = watcher;
        _logger.LogInformation($"Watcher created for: {sourcePath}");
    }


    private async Task AddHeadersToXmlAsync(string filePath, string destinationPath)
    {
        try
        {
            // Load XML
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);

            // Add <Header> section
            var headerElement = xmlDocument.CreateElement("Header");

            var fileIdElement = xmlDocument.CreateElement("FileID");
            fileIdElement.InnerText = Guid.NewGuid().ToString();
            headerElement.AppendChild(fileIdElement);

            var timestampElement = xmlDocument.CreateElement("Timestamp");
            timestampElement.InnerText = DateTime.UtcNow.ToString("o"); // ISO 8601 format
            headerElement.AppendChild(timestampElement);

            var sourceSystemElement = xmlDocument.CreateElement("SourceSystem");
            sourceSystemElement.InnerText = filePath;
            headerElement.AppendChild(sourceSystemElement);

            var processedByElement = xmlDocument.CreateElement("ProcessedBy");
            processedByElement.InnerText = "FileWatcherService";
            headerElement.AppendChild(processedByElement);

            xmlDocument.DocumentElement?.PrependChild(headerElement);

            // Save updated XML to the destination path
            string destFile = Path.Combine(destinationPath, Path.GetFileName(filePath));
            xmlDocument.Save(destFile);

            _logger.LogInformation($"Headers added to XML: {filePath} -> {destFile}");

            // Optionally delete the original file
            File.Delete(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding headers to XML file {filePath}: {ex.Message}");
        }
    }
    private async Task ExtractZipFileAsync(string zipFilePath, string destinationPath)
    {
        try
        {
            string extractPath = Path.Combine(destinationPath, Path.GetFileNameWithoutExtension(zipFilePath));

            if (!Directory.Exists(extractPath))
            {
                Directory.CreateDirectory(extractPath);
            }

            // Use SemaphoreSlim to limit concurrency during extraction
            await _semaphore.WaitAsync();
            try
            {
                await Task.Run(() => ZipFile.ExtractToDirectory(zipFilePath, extractPath, true));
            }
            finally
            {
                _semaphore.Release();
            }

            File.Delete(zipFilePath); // Optionally delete the ZIP file after extraction
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error extracting ZIP file {zipFilePath}: {ex.Message}");
        }
    }

    private async Task WaitForFileAsync(string filePath)
    {
        int retryCount = 10; // Retry up to 10 times
        while (retryCount > 0)
        {
            try
            {
                // Try to open the file exclusively, if it succeeds, the file is ready
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    fs.Close();
                }
                break;
            }
            catch (IOException)
            {
                // File is still in use, wait before retrying
                await Task.Delay(500); // Wait for 500 ms before retrying
                retryCount--;
            }
        }

        if (retryCount == 0)
        {
            _logger.LogWarning($"File is still being written after 10 retries: {filePath}");
        }
    }



    private async Task LogActivityAsync(string filePath, string destinationPath, string status)
    {
        try
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                await conn.OpenAsync();

                string query = @"
                    INSERT INTO file_activity_log (file_path, destination_path, timestamp, status)
                    VALUES (@filePath, @destinationPath, @timestamp, @status)";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("filePath", filePath);
                    cmd.Parameters.AddWithValue("destinationPath", destinationPath);
                    cmd.Parameters.AddWithValue("timestamp", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("status", status);

                    await cmd.ExecuteNonQueryAsync();
                    _logger.LogInformation($"Logged activity: {filePath} -> {destinationPath}, Status: {status}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error logging activity for file {filePath}: {ex.Message}");
        }
    }

    //private async Task WaitForFileAsync(string filePath)
    //{
    //    int retryCount = 10;
    //    while (retryCount > 0)
    //    {
    //        try
    //        {
    //            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
    //            {
    //                fs.Close();
    //            }
    //            break;
    //        }
    //        catch (IOException)
    //        {
    //            await Task.Delay(500);
    //            retryCount--;
    //        }
    //    }

    //    if (retryCount == 0)
    //    {
    //        _logger.LogWarning($"File is still being written after 10 retries: {filePath}");
    //    }
    //}

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var watcher in _watchers.Values)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
        _watchers.Clear();
        _logger.LogInformation("Watchers stopped.");

        await base.StopAsync(cancellationToken);
    }
}
