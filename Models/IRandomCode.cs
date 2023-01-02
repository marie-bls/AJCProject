using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace AjcProject.Models
{

    public interface IRandomCode
    {
       string RndCode { get; }
    }
    public class RandomCode : IRandomCode
    {
        public string RndCode { get; private init; }

        public  RandomCode()
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
            string base64Guid = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(base64Guid);
            encodedBytes = md5.ComputeHash(originalBytes);
           
            this.RndCode = Regex.Replace(BitConverter.ToString(encodedBytes), "-", "").ToLower();

        }
    }
}
