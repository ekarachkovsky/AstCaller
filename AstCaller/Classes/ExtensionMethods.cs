using AstCaller.Models.Domain;
using AstCaller.ViewModels;
using System;

namespace AstCaller.Classes
{
    public static class ExtensionMethods
    {
        public static string ToFileName(this FileType fileType, int campaignId)
        {
            return $"{campaignId}_{fileType.ToString().ToLower()}";
        }

        public static string GetFileName(this Campaign entity, FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Abonents:
                    return entity.AbonentsFileName;
                case FileType.Voice:
                    return entity.VoiceFileName;
            }

            throw new ArgumentException("Invalid file type");
        }
    }
}
