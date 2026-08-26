using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Lamina.Helpers;

public static class SettingsStorageExtensions
{
    private const string FileExtension = ".json";

    public static bool IsRoamingStorageAvailable(this ApplicationData appData)
    {
        return appData.RoamingStorageQuota == 0;
    }

    public static async Task SaveAsync<T>(this StorageFolder folder, string name, T content)
    {
        var file = await folder.CreateFileAsync(GetFileName(name), CreationCollisionOption.ReplaceExisting);
        var fileContent = JsonSerializer.Serialize(content);
        await FileIO.WriteTextAsync(file, fileContent);
    }

    public static async Task<T?> ReadAsync<T>(this StorageFolder folder, string name)
    {
        var fileName = GetFileName(name);
        var path = Path.Combine(folder.Path, fileName);

        if (!File.Exists(path))
        {
            return default;
        }

        var file = await folder.GetFileAsync(fileName);
        var fileContent = await FileIO.ReadTextAsync(file);

        return JsonSerializer.Deserialize<T>(fileContent);
    }

    public static async Task SaveAsync<T>(this ApplicationDataContainer settings, string key, T value)
    {
        settings.Values[key] = JsonSerializer.Serialize(value);
    }

    public static async Task<T?> ReadAsync<T>(this ApplicationDataContainer settings, string key)
    {
        if (settings.Values.TryGetValue(key, out var obj) && obj is string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        return default;
    }

    public static async Task<StorageFile> SaveFileAsync(this StorageFolder folder, byte[] content, string fileName, CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("Invalid file name", nameof(fileName));

        var storageFile = await folder.CreateFileAsync(fileName, options);
        await FileIO.WriteBytesAsync(storageFile, content);
        return storageFile;
    }

    public static async Task<byte[]?> ReadFileAsync(this StorageFolder folder, string fileName)
    {
        var item = await folder.TryGetItemAsync(fileName);

        if (item != null && item.IsOfType(StorageItemTypes.File))
        {
            var storageFile = await folder.GetFileAsync(fileName);
            return await storageFile.ReadBytesAsync();
        }

        return null;
    }

    public static async Task<byte[]?> ReadBytesAsync(this StorageFile file)
    {
        if (file != null)
        {
            using IRandomAccessStream stream = await file.OpenReadAsync();
            using var reader = new DataReader(stream.GetInputStreamAt(0));
            await reader.LoadAsync((uint)stream.Size);
            var bytes = new byte[stream.Size];
            reader.ReadBytes(bytes);
            return bytes;
        }

        return null;
    }

    private static string GetFileName(string name)
    {
        return name.EndsWith(FileExtension) ? name : string.Concat(name, FileExtension);
    }
}