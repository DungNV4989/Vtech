using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace VTECHERP.Attribute
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(params string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile)
            {
                var file = value as IFormFile;
                if (file != null)
                {
                    var extension = Path.GetExtension(file.FileName);
                    if (!_extensions.Contains(extension.ToLower()))
                    {
                        return new ValidationResult($"Loại file .{extension} không được phép.");
                    }
                }
            }

            else if (value is IEnumerable<IFormFile>)
            {
                var files = value as IEnumerable<IFormFile>;
                if (files != null && files.Any())
                {
                    foreach (var file in files)
                    {
                        var extension = Path.GetExtension(file.FileName);
                        if (!_extensions.Contains(extension.ToLower()))
                        {
                            return new ValidationResult($"Loại file .{extension} không được hỗ trợ.");
                        }
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
