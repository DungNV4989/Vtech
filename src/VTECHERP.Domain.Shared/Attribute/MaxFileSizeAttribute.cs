using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace VTECHERP.Attribute
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile)
            {
                var file = value as IFormFile;
                if (file != null)
                {
                    if (file.Length > _maxFileSize)
                    {
                        return new ValidationResult($"Tổng dung lượng tối đa cho phép là {_maxFileSize} bytes.");
                    }
                }
            }
            else if (value is IEnumerable<IFormFile>)
            {
                var files = value as IEnumerable<IFormFile>;
                if (files != null && files.Any())
                {
                    long totalSize = 0;
                    foreach (var file in files)
                    {
                        totalSize += file.Length;
                    }
                    if (totalSize > _maxFileSize)
                    {
                        return new ValidationResult($"Tổng dung lượng tối đa cho phép là {_maxFileSize} bytes.");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
