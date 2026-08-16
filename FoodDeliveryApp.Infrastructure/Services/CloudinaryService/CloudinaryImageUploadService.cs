using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FoodDeliveryApp.Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Infrastructure.Services.CloudinaryService
{
    public class CloudinaryImageUploadService : ICloudinaryImageUploadService
    {
        private readonly CloudinarySettings _cloudinarySettings;
        private readonly Cloudinary _cloudinary;
        public CloudinaryImageUploadService(IOptions<CloudinarySettings> options)
        {
            _cloudinarySettings = options.Value;
            var account = new Account(
               _cloudinarySettings.CloudName,
               _cloudinarySettings.APIKey,
               _cloudinarySettings.APISecret
           );
            _cloudinary = new Cloudinary(account);
        }
        public async Task DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
            {
                return;
            }

            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Result != "ok" && result.Result != "not found")
            {
                throw new Exception("Failed to delete image");
            }
        }

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("File is empty");
            }

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "Tasty Hub"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }

            return uploadResult;
        }
    }
}
