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
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
 
public class Worker : BackgroundService
{ 
    private readonly ILogger<Worker> _logger;
private readonly string _sourceFolder = Path.Combine(Environment.CurrentDirectory, "SourceFolder6");
private readonly string _tempFolder = Path.Combine(Environment.CurrentDirectory, "DestinationFolder6");

      // Temporary folder
    private FileSystemWatcher _fileWatcher;
 
    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }
 
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Ensure the folders exist
        if (!Directory.Exists(_sourceFolder)) Directory.CreateDirectory(_sourceFolder);
        if (!Directory.Exists(_tempFolder)) Directory.CreateDirectory(_tempFolder);
 
        // Initialize FileSystemWatcher
        _fileWatcher = new FileSystemWatcher
        {
            Path = _sourceFolder,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            Filter = "*.*" // Monitor all file types
        };
 
        _fileWatcher.Created += async (sender, e) => await OnFileCreatedAsync(e);
        _fileWatcher.EnableRaisingEvents = true;
 
        _logger.LogInformation("Service started and monitoring folder: " + _sourceFolder);
 
        return Task.CompletedTask;
    }
 
    private async Task OnFileCreatedAsync(FileSystemEventArgs e)
    {
        try
        {
            // Wait until the file is fully written
            await WaitForFileAsync(e.FullPath);
 
            string tempFilePath = Path.Combine(_tempFolder, Path.GetFileName(e.FullPath));
 
            if (Path.GetExtension(e.FullPath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                // Handle ZIP files: Extract contents to the temp folder
                await ExtractZipFileAsync(e.FullPath, _tempFolder);
                _logger.LogInformation($"ZIP file extracted: {e.FullPath} -> {_tempFolder}");
            }
            else
            {
                // Move other files to the temporary folder
                File.Move(e.FullPath, tempFilePath);
                _logger.LogInformation($"File moved: {e.FullPath} -> {tempFilePath}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing file: {e.FullPath}. Exception: {ex.Message}");
        }
    }
 
    //private async Task ExtractZipFileAsync(string zipFilePath, string destinationFolder)
    //{
    //    try
    //    {
    //        await Task.Run(() =>
    //        {
    //            using (FileStream zipToOpen = new FileStream(zipFilePath, FileMode.Open))
    //            {
    //                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
    //                {
    //                    foreach (ZipArchiveEntry entry in archive.Entries)
    //                    {
    //                        string destinationPath = Path.Combine(destinationFolder, entry.FullName);
 
    //                        // Ensure the directory exists
    //                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
 
    //                        // Extract the file
    //                        entry.ExtractToFile(destinationPath, overwrite: true);
    //                        _logger.LogInformation($"Extracted: {entry.FullName} -> {destinationPath}");
    //                    }
    //                }
    //            }
 
    //            // Optionally delete the original zip file after extraction
    //            File.Delete(zipFilePath);
    //            _logger.LogInformation($"ZIP file deleted: {zipFilePath}");
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError($"Error extracting ZIP file: {zipFilePath}. Exception: {ex.Message}");
    //    }
    //}
 
    private async Task ExtractZipFileAsync(string zipFilePath, string destinationFolder)
    {
        try
        {
            await Task.Run(() =>
            {
                using (FileStream zipToOpen = new FileStream(zipFilePath, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            ;
                            string destinationPath = Path.Combine(destinationFolder, entry.FullName);
 
                            // Ensure that all directories in the path are created
                            string directoryPath = Path.GetDirectoryName(destinationPath);
                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath); // Create the directory if it doesn't exist
                            }
 
                            // Extract the file
                            entry.ExtractToFile(destinationPath, overwrite: true);
                            _logger.LogInformation($"Extracted: {entry.FullName} -> {destinationPath}");
                        }
                    }
                }
 
                // Optionally delete the original zip file after extraction
                File.Delete(zipFilePath);
                _logger.LogInformation($"ZIP file deleted: {zipFilePath}");
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error extracting ZIP file: {zipFilePath}. Exception: {ex.Message}");
        }
    }
 
 
    private async Task WaitForFileAsync(string filePath)
    {
        int retryCount = 10; // Retry up to 10 times
        while (retryCount > 0)
        {
            try
            {
                using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    fs.Close();
                }
                break;
            }
            catch (IOException)
            {
                await Task.Delay(500); // Wait for 500 ms
                retryCount--;
            }
        }
    }
}
