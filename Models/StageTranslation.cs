using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BusManagementAPI.Models
{
    public class StageTranslation
    {
        [Key]
        public Guid TranslationId { get; set; }
        public string EnglishName { get; set; }
        public string TranslatedName { get; set; }
        public string TranslatedLanguage { get; set; }
        public bool IsActive { get; set; }
    }
}