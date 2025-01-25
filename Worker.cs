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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly string _destinationFolder;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        // Retrieve the destination folder path from the environment variable
        _destinationFolder = Path.Combine(Environment.GetEnvironmentVariable("GITHUB_WORKSPACE"), "DestinationFolder6");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Ensure the destination folder exists
                if (!Directory.Exists(_destinationFolder))
                {
                    Directory.CreateDirectory(_destinationFolder);
                    _logger.LogInformation($"Created destination folder: {_destinationFolder}");
                }

                // Define the source file path
                string sourceFilePath = Path.Combine(Environment.GetEnvironmentVariable("GITHUB_WORKSPACE"), "SourceFolder6", "testfile.txt");

                // Check if the source file exists
                if (File.Exists(sourceFilePath))
                {
                    string destinationFilePath = Path.Combine(_destinationFolder, "testfile.txt");

                    // Move the file to the destination folder
                    File.Move(sourceFilePath, destinationFilePath);
                    _logger.LogInformation($"Moved file from {sourceFilePath} to {destinationFilePath}");
                }
                else
                {
                    _logger.LogWarning($"Source file not found: {sourceFilePath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
            }

            // Wait for a specified interval before checking again
            await Task.Delay(10000, stoppingToken); // 10 seconds
        }
    }
}
