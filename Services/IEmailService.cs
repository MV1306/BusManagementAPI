﻿using System.Threading.Tasks;

namespace BusManagementAPI.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, byte[] qrImageBytes = null);
        string shortURLGeneration(string URL);
    }
}