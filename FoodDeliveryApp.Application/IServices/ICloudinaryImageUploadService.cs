using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDeliveryApp.Application.IServices
{
    public interface ICloudinaryImageUploadService
    {
        Task<ImageUploadResult> UploadImageAsync(IFormFile file);
        Task DeleteImageAsync(String publicId);
    }
}
