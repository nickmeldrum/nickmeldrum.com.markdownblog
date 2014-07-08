using System;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace MarkdownBlog.Net.Web.Models {
    public static class Azure {
        public static string StorageAccountName { get { return ConfigurationManager.AppSettings["azureStorageAccountName"]; } }
        public static string StorageKey { get { return ConfigurationManager.AppSettings["azureStorageKey"]; } }
        public static string StorageBlobEndPoint { get { return ConfigurationManager.AppSettings["azureStorageBlobEndPoint"]; } }

        public static CloudStorageAccount GetStorageAccount() {
            return new CloudStorageAccount(new StorageCredentials(StorageAccountName, StorageKey), new Uri(StorageBlobEndPoint), null, null);
        }
    }
}

